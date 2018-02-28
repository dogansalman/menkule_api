using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web;
using System.Web.Http.Description;
using System.Web.UI.WebControls;
using System.Web.Helpers;
using System.Collections.Generic;
using System.Net.Http;
using rest_api.Models;
using rest_api.Context;
using rest_api.ModelViews;
using rest_api.Libary.Exceptions.ExceptionThrow;
using rest_api.Libary.Bcrypt;
using rest_api.Libary.Mailgun;
using rest_api.Libary.NetGsm;
using rest_api.Libary.Cloudinary;


namespace rest_api.Controllers
{

    [RoutePrefix("users")]
    public class UsersController : ApiController
    {

        
        DatabaseContext db = new DatabaseContext();
        /*
         User create read update functions
             */
        // Add
        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] Users user)
        {
            //validation
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (db.users.Any(u => u.email == user.email)) ExceptionThrow.Throw("e-posta adresi kullanılmaktadır.", HttpStatusCode.BadRequest);
            if (db.users.Any(u => u.gsm == user.gsm)) ExceptionThrow.Throw("gsm no kullanılmaktadır.", HttpStatusCode.BadRequest);

            //generate activation code
            Random rnd = new Random();
            string gsm_code = rnd.Next(9999, 999999).ToString();
            string email_code = rnd.Next(9999, 999999).ToString();

            //set password 
            bool no_password = user.password == null || user.password.Trim() == "";
            string password = no_password ? Users.generatePassword(5, 3) : user.password;

            //create user 
            Users userData = new Users
            {
                name = user.name,
                lastname = user.lastname,
                email = user.email,
                gender = user.gender,
                gsm = user.gsm,
                description = user.description,
                password = Bcrypt.hash(password),
                source = "web",
                email_activation_code = email_code,
                gsm_activation_code = gsm_code
            };
          

            if (user.identity_no != null) {
                userData.identity_no = user.identity_no;
            }

            //insert user
            db.users.Add(userData);

            try
            {
                db.SaveChanges();

                //If password is random generated
                if(no_password) NetGsm.Send(user.gsm, "Menkule.com.tr üyelik şifreniz " + password + " Şifrenizi değiştirmeyi unutmayınız.");
            }
            catch (Exception ex)
            {
                ExceptionThrow.Throw(ex);
            }

            //Send Gsm Activation Code
            NetGsm.Send(user.gsm, "menkule.com.tr uyeliginiz ile ilgili onay kodunuz: " + gsm_code);

            //Send Email Notification
            Mailgun.Send("register", new Dictionary<string, object>() { { "fullname", user.name + " " + user.lastname } }, user.email, "Üyeliğiniz için teşekkürler");

            object token = no_password ? Users.LoginOnBackDoor(user.email, password) : null;


            return Ok(new
            {
                name = user.name,
                lastname = user.lastname,
                email = user.email,
                gsm = user.gsm,
                gender = user.gender,
                photo = "",
                ownershiping = user.ownershiping,
                state = user.state,
                email_state = user.email_state,
                gsm_state = user.gsm_state,
                created_date = user.created_date,
                token = token
            });
            
        }

        // Get
        [HttpGet]
        [Route("")]
        [Authorize]
        public IHttpActionResult get()
        {

            int user_id = Users.GetUserId(User);
            var user = db.users.GroupJoin(
                 db.images,
                 u => u.image_id,
                 i => i.id,
                 (u, i) => new { user = u, image = i }
                 )
                 .Where(u => u.user.id == user_id)
                 .SelectMany(userWithImage =>
                 userWithImage.image.DefaultIfEmpty(),
                 (u, i) => new {
                     id = u.user.id,
                     name = u.user.name,
                     lastname = u.user.lastname,
                     email = u.user.email,
                     gsm = u.user.gsm,
                     gender = u.user.gender,
                     is_external_confirm = u.user.is_external_confirm,
                     photo = i.url,
                     identity_no = u.user.identity_no,
                     ownershiping = u.user.ownershiping,
                     advert_size = (
                     from ad in db.advert
                     where ad.user_id == u.user.id
                     select(ad)
                     ).Count(),
                     notification_size = (
                     from ntf in db.notifications
                     where ntf.user_id == u.user.id && ntf.state == true
                     select (ntf)
                     ).Count(),
                     state = u.user.state,
                     email_state = u.user.email_state,
                     gsm_state = u.user.gsm_state,
                     created_date = u.user.created_date
                 }
                 ).FirstOrDefault();
            if (user == null) return NotFound();
            return Ok(user);
        }

        // Put
        [HttpPut]
        [Route("")]
        [Authorize]
        public IHttpActionResult update([FromBody] Users user)
        {
            int user_id = Users.GetUserId(User);
            Users dbUser = db.users.Find(user_id);
            if (dbUser == null) return NotFound();
            if (dbUser.email != user.email && db.users.Any(u => u.email == user.email)) ExceptionThrow.Throw("e-posta adresi kullanılmaktadır.", HttpStatusCode.BadRequest);
            if (dbUser.gsm != user.gsm && db.users.Any(u => u.gsm == user.gsm)) ExceptionThrow.Throw("gsm no kullanılmaktadır.", HttpStatusCode.BadRequest);

            if (dbUser.gsm != user.gsm)
            {
                //generate activation code
                Random rnd = new Random();
                string gsm_code = rnd.Next(9999, 999999).ToString();

                dbUser.state = false;
                dbUser.gsm_state = false;
                dbUser.gsm_activation_code = gsm_code;

                //send gsm activation code
                NetGsm.Send(user.gsm, "menkule.com.tr uyeliginiz ile ilgili onay kodunuz: " + gsm_code);
            }

            dbUser.gsm = user.gsm;
            dbUser.email = user.email;
            dbUser.name = user.name;
            dbUser.lastname = user.lastname;
            dbUser.updated_date = DateTime.Now;
            dbUser.identity_no = user.identity_no;

            db.SaveChanges();

            try
            {

            }
            catch (Exception ex)
            {
                ExceptionThrow.Throw(ex);
            }
            return Ok(new {
                name = user.name,
                lastname = user.lastname,
                email = user.email,
                gsm = user.gsm,
                gender = user.gender,
                photo = "",
                ownershiping = user.ownershiping,
                state = user.state,
                email_state = user.email_state,
                gsm_state = user.gsm_state,
                created_date = user.created_date
            });
        }

        /*
         User password  update forgot functions
         */

        //Update Password
        [HttpPut]
        [Authorize]
        [Route("password/reset")]
        public IHttpActionResult changepas([FromBody] _UserPassword password)
        {
            if (password.password != password.reply) return BadRequest();

            int user_id = Users.GetUserId(User);
            string pas = Bcrypt.hash(password.currentpassword);

            Users user = db.users.Where(u => u.id == user_id && u.password == pas).FirstOrDefault();
            if (user == null) return NotFound();

            user.password = Bcrypt.hash(password.password);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionThrow.Throw(ex);
            }
            return Ok();
        }

        //Forgot Password
        [HttpPost]
        [Route("password/forgot")]
        public IHttpActionResult forgotpas([FromBody] _Mail mail)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Users user = db.users.Where(u => u.email == mail.email).FirstOrDefault();

            if (user == null) return NotFound();

            //generate password reset token
            Random rnd = new Random();
            string token = Bcrypt.hash(user.email + DateTime.Now.Hour + DateTime.Now.Millisecond + rnd.Next(999999, 999999));

            try
            {
                user.forgot_last_date = DateTime.Now;
                user.password_token = token;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionThrow.Throw(ex);
            }

            Mailgun.Send("forgot_password", new Dictionary<string, object>() { { "fullname", user.name + " " + user.lastname }, { "token", token } }, user.email, "Menkule Şifre Yenileme Talebiniz");

            return Ok();
        }

        //Update Password with Token
        [HttpPost]
        [Route("password/reset/token")]
        public IHttpActionResult resetpas([FromBody] _TokenResetPassword _token)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (_token.password != _token.reply) return BadRequest();

            Users user = db.users.Where(u => u.password_token == _token.token).FirstOrDefault();
            if (user == null) return NotFound();

            if (user.forgot_last_date != null)
            {
                TimeSpan diff = DateTime.Now - Convert.ToDateTime(user.forgot_last_date);
                if (diff.TotalHours >= 2) return BadRequest();
            }

            try
            {
                user.password = Bcrypt.hash(_token.password);
                user.forgot_last_date = null;
                user.password_token = null;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionThrow.Throw(ex);
            }

            return Ok();
        }

        [HttpGet]
        [Route("password/validate")]
        public IHttpActionResult checktoken()
        {
            var allUrlKeyValues = ControllerContext.Request.GetQueryNameValuePairs();
            string token = allUrlKeyValues.LastOrDefault(x => x.Key == "token").Value;
            Users user = db.users.Where(u => u.password_token == token).FirstOrDefault();
            if (user == null) return NotFound();

            if (user.forgot_last_date != null)
            {
                TimeSpan diff = DateTime.Now - Convert.ToDateTime(user.forgot_last_date);
                if (diff.TotalHours >= 2) return NotFound();
            }

            return Ok();
        }

        //External Auth Confirm Password & Gsm
        [HttpPut]
        [Authorize]
        [Route("external/confirm")]
        public IHttpActionResult externalConfirm([FromBody] _ExternalConfirm externalConfirmData)
        {
            int user_id = Users.GetUserId(User);

            if (externalConfirmData.password != externalConfirmData.reply) ExceptionThrow.Throw("Şifre tekrarı hatalı.", HttpStatusCode.BadRequest);

            if (db.users.Any(u => u.gsm == externalConfirmData.gsm)) ExceptionThrow.Throw("gsm no kullanılmaktadır.", HttpStatusCode.BadRequest);

            Users user = db.users.Where(u => u.id == user_id && u.is_external_confirm == false).FirstOrDefault();

            if (user == null) ExceptionThrow.Throw("Zaten şifre güncellenmiş", HttpStatusCode.Forbidden);

            user.gsm = externalConfirmData.gsm;
            user.updated_date = DateTime.Now;
            user.password = Bcrypt.hash(externalConfirmData.password);
            user.is_external_confirm = true;

            try
            {
                db.SaveChanges();

                //Send Gsm Activation Code
                NetGsm.Send(externalConfirmData.gsm, "menkule.com.tr uyeliginiz ile ilgili onay kodunuz: " + user.gsm_activation_code);
            }
            catch (Exception ex)
            {
                ExceptionThrow.Throw(ex);
            }
            return Ok();
        }


        /*
         User Gsm Email Validations Function
             */

        // Gsm
        [HttpGet]
        [Authorize]
        [Route("validate/gsm")]
        public IHttpActionResult gsm()
        {
            int user_id = Users.GetUserId(User);

            if (!db.users.Any(u => u.gsm_state == true && u.id == user_id)) ExceptionThrow.Throw("Gsm no onaylanmadı.", HttpStatusCode.Forbidden);
            return Ok();
        }

        // Email
        [HttpGet]
        [Authorize]
        [Route("validate/mail")]
        public IHttpActionResult mail()
        {
            int user_id = Users.GetUserId(User);

            if (!db.users.Any(u => u.email_state == true && u.id == user_id)) ExceptionThrow.Throw("E-posta adresi onaylanmadı.", HttpStatusCode.Forbidden);
            return Ok();
        }

        // Resend Gsm Activation Code
        [HttpGet]
        [Authorize]
        [Route("validate/gsm/send")]
        public IHttpActionResult resendGsmCode()
        {
            int user_id = Users.GetUserId(User);

            Users user = db.users.Where(u => u.id == user_id).FirstOrDefault();
            if (user == null) return NotFound();

            if (user.gsm_last_update != null)
            {
                TimeSpan diff = DateTime.Now - Convert.ToDateTime(user.gsm_last_update);
                if (diff.TotalMinutes <= 4) ExceptionThrow.Throw("Yeni aktivasyon kodu için 4 dakika beklemeniz gerekmektedir.", HttpStatusCode.Forbidden);
            }

            //generate activation code
            Random rnd = new Random();
            string gsm_code = rnd.Next(9999, 999999).ToString();

            try
            {
                user.gsm_activation_code = gsm_code;
                user.gsm_last_update = DateTime.Now;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionThrow.Throw(ex);
            }

            //Send Gsm Activation Code
            NetGsm.Send(user.gsm, "menkule.com.tr uyeliginiz ile ilgili onay kodunuz: " + user.gsm_activation_code);

            return Ok();
        }

        /*
         User Approve Ownership & Gsm & Email Functions 
         */

        // Ownershiping
        [HttpPost]
        [Authorize]
        [Route("approve/ownership")]
        public IHttpActionResult ownerApprove()
        {
            int user_id = Users.GetUserId(User);

            Users user = db.users.Find(user_id);
            if (user == null) return NotFound();
            user.ownershiping = true;
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ExceptionThrow.Throw(e);
            }

            return Ok();
        }

        // Gsm
        [HttpPost]
        [Authorize]
        [Route("approve/gsm")]
        public IHttpActionResult approveGsm([FromBody] _Code userApproved)
        {
            int user_id = Users.GetUserId(User);

            Users user = db.users.Where(u => u.id == user_id && u.gsm_activation_code == userApproved.code).FirstOrDefault();
            if (user == null) return NotFound();
            user.gsm_state = true;
            user.state = true;
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ExceptionThrow.Throw(e);
            }
            return Ok();
        }

        //Note: this function is not using.
        // Mail
        [HttpPost]
        [Authorize]
        [Route("approve/mail")]
        public IHttpActionResult approveMail([FromBody] _Code userApproved)
        {
            int user_id = Users.GetUserId(User);

            Users user = db.users.Where(u => u.id == user_id && u.email_activation_code == userApproved.code).FirstOrDefault();
            if (user == null) return NotFound();
            user.email_state = true;
            try
            {
                //db.SaveChanges();
            }
            catch (Exception e)
            {
                ExceptionThrow.Throw(e);
            }
            return Ok();
        }


        /*
         User Photos
         */
        [HttpPost]
        [Authorize]
        [Route("photo")]
        [ResponseType(typeof(FileUpload))]
        public  IHttpActionResult upload()
        {
            int user_id = Users.GetUserId(User);
            Users user = db.users.Find(user_id);

            var httpRequest = HttpContext.Current.Request;
            List<string> imageExt = new List<string>{ { "jpg" }, { "png" }, { "jpeg" } };
            var image = new WebImage(httpRequest.InputStream);
            if (!imageExt.Contains(image.ImageFormat.ToString().ToLower())) new BadImageFormatException();

            Images userImage = Cloudinary.upload(image, "users/" + user.name + "-" + user.lastname + "-" + user.id);
            if (userImage == null) return BadRequest();

            db.images.Add(userImage);
            db.SaveChanges();

            user.image_id = userImage.id;
            db.SaveChanges();

            return Ok(userImage);
        }
    }
}
