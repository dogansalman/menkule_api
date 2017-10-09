﻿using System.Linq;
using System.Net;
using System.Web.Http;
using System.Security.Claims;
using rest_api.Models;
using rest_api.Context;
using rest_api.ModelViews;
using rest_api.Libary.Exceptions;
using rest_api.Libary.Responser;

namespace rest_api.Controllers
{
  
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
     
        // Add
        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] Users user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (db.users.Any(u => u.email == user.email)) Responser.Response(HttpStatusCode.Forbidden, "e-posta adresi kullanılmaktadır.");
            if (db.users.Any(u => u.gsm == user.gsm)) Responser.Response(HttpStatusCode.Forbidden, "gsm no kullanılmaktadır.");
            
            db.users.Add(user);
            try
            {
                db.SaveChanges();
            }
            catch (System.Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
           
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

        // Validate GSM
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

        // Validate Email
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

        //Ownershiping
        [HttpPost]
        [Authorize]
        [Route("ownership/approve")]
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

    }
}
