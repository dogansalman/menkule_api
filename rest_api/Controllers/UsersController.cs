using System.Linq;
using System.Net;
using System.Web.Http;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using rest_api.Models;
using rest_api.Context;
using rest_api.ModelViews;
using rest_api.Libary.Exceptions;
using rest_api.Libary.Responser;
using rest_api.Libary.Bcrypt;
using rest_api.Libary.Mailgun;
using rest_api.Libary.NetGsm;

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
            if (db.users.Any(u => u.email == user.email)) Responser.Response(HttpStatusCode.BadRequest, "e-posta adresi kullanılmaktadır.");
            if (db.users.Any(u => u.gsm == user.gsm)) Responser.Response(HttpStatusCode.BadRequest, "gsm no kullanılmaktadır.");

            //generate activation code
            Random rnd = new Random();
            string gsm_code = rnd.Next(9999, 999999).ToString();
            string email_code = rnd.Next(9999, 999999).ToString();

            //create user 
             Users userData = new Users {
                name = user.name,
                lastname = user.lastname,
                email = user.email,
                gender = user.gender,
                gsm = user.gsm,
                description = user.description,
                password = Bcrypt.hash(user.password),
                source = "web",
                email_activation_code = email_code,
                gsm_activation_code = gsm_code
            };
            

            //insert user
            db.users.Add(userData);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
            
            //Send Gsm Activation Code
            NetGsm.Send(user.gsm, "menkule.com.tr uyeliginiz ile ilgili onay kodunuz: " + gsm_code);

            //Send Email Notification
            Mailgun.Send("register", new Dictionary<string, object>() { { "fullname", user.name + " " + user.lastname } }, user.email, "Üyeliğiniz için teşekkürler");

            return Ok(new UsersMV
            {
                name = user.name,
                lastname = user.lastname,
                email = user.email,
                gsm = user.gsm,
                gender = user.gender,
                photo = null,
                ownershiping = user.ownershiping,
                state = user.state,
                email_state = user.email_state,
                gsm_state = user.gsm_state,
                created_date = user.created_date
            });
        }

        // Get
        [HttpGet]
        [Authorize]
        [Route("")]
        public IHttpActionResult get()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            var user = db.users.GroupJoin(
                 db.images,
                 u => u.image_id,
                 i => i.id,
                 (u, i) => new { user = u, image = i }
                 )
                 .Where(u => u.user.id == user_id)
                 .SelectMany(userWithImage =>
                 userWithImage.image.DefaultIfEmpty(),
                 (u, i) => new UsersMV
                 {
                     name = u.user.name,
                     lastname = u.user.lastname,
                     email = u.user.email,
                     gsm = u.user.gsm,
                     gender = u.user.gender,
                     photo = i.url,
                     ownershiping = u.user.ownershiping,
                     state = u.user.state,
                     email_state = u.user.email_state,
                     gsm_state = u.user.gsm_state,
                     created_date = u.user.created_date
                 }
                 ).FirstOrDefault();
            if (user == null) return NotFound();
            return Ok(user);
        }

        /*
         User password  update forgot functions
         */

        //Update Password
        [HttpPut]
        [Authorize]
        [Route("password/reset")]
        public IHttpActionResult changepas([FromBody] UserPasswordMV password)
        {
            if (password.password != password.reply) return BadRequest();

            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            string pas = Bcrypt.hash(password.currentpassword);
          
            Users user = db.users.Where(u => u.id == user_id && u.password == pas).FirstOrDefault();
            if (user == null) return NotFound();
            
            user.password = Bcrypt.hash(password.password);
            try
            {
                db.SaveChanges();
            }
            catch (System.Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
            return Ok();
        }

        //Forgot Password
        [HttpPost]
        [Route("password/forgot")]
        public IHttpActionResult forgotpas([FromBody] UserMailMV mail)
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
                ExceptionHandler.Handle(ex);
            }
            
            Mailgun.Send("forgot_password", new Dictionary<string, object>() { { "fullname", user.name + " " + user.lastname}, {"token", token } }, user.email, "Menkule Şifre Yenileme Talebiniz");

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
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
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
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);

            if (!db.users.Any(u => u.gsm_state == true && u.id == user_id)) Responser.Response(HttpStatusCode.Forbidden, "Gsm not approved");
            return Ok();
        }

        // Email
        [HttpGet]
        [Authorize]
        [Route("validate/mail")]
        public IHttpActionResult mail()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);

            if (!db.users.Any(u => u.email_state == true && u.id == user_id)) Responser.Response(HttpStatusCode.Forbidden, "E-mail not approved");
            return Ok();
        }

        // Resend Gsm Activation Code
        [HttpGet]
        [Authorize]
        [Route("validate/gsm/send")]
        public IHttpActionResult resendGsmCode()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);

            Users user = db.users.Where(u => u.id == user_id).FirstOrDefault();
            if (user == null) return NotFound();

            if(user.gsm_last_update != null)
            {
                TimeSpan diff = DateTime.Now - Convert.ToDateTime(user.gsm_last_update);
                if (diff.TotalHours >= 1) return BadRequest();
            }
           

            //generate activation code
            Random rnd = new Random();
            string gsm_code = rnd.Next(9999, 999999).ToString();

            user.gsm_activation_code = gsm_code;

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
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
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);

            Users user = db.users.Find(user_id);
            if (user == null) return NotFound();
            user.ownershiping = true;
            try
            {
                db.SaveChanges();
            }
            catch (System.Exception e)
            {
                ExceptionHandler.Handle(e);
            }
            return Ok();
        }

        // Gsm
        [HttpPost]
        [Authorize]
        [Route("approve/gsm")]
        public IHttpActionResult approveGsm([FromBody] UserApproveMV userApproved)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);

            Users user = db.users.Where(u => u.id == user_id && u.gsm_activation_code == userApproved.code).FirstOrDefault();
            if (user == null) return NotFound();
            user.gsm_state = true;
            user.state = true;
            try
            {
                db.SaveChanges();
            }
            catch (System.Exception e)
            {
                ExceptionHandler.Handle(e);
            }
            return Ok();
        }
    
        //Note: this function is not using.
        // Mail
        [HttpPost]
        [Authorize]
        [Route("approve/mail")]
        public IHttpActionResult approveMail([FromBody] UserApproveMV userApproved)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);

            Users user = db.users.Where(u => u.id == user_id && u.email_activation_code == userApproved.code).FirstOrDefault();
            if (user == null) return NotFound();
            user.email_state = true;
            try
            {
                //db.SaveChanges();
            }
            catch (System.Exception e)
            {
                ExceptionHandler.Handle(e);
            }
            return Ok();
        }

      
    }
}
