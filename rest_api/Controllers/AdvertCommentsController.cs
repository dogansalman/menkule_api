using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using rest_api.Models;
using rest_api.Context;
using System.Security.Claims;

namespace rest_api.Controllers
{
    [RoutePrefix("comments")]
    public class AdvertCommentsController : ApiController
    {
        DatabaseContext db = new DatabaseContext();

        [HttpGet]
        [Route("advert/{id}")]
        public List<AdvertComments> get(int id)
        {
            return db.advert_comments.Where(a => a.advert_id == id).ToList();
        }

        [Authorize]
        [HttpGet]
        [Route("user")]
        public List<AdvertComments> getUserComment()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            return db.advert_comments.Where(a => a.user_id == int.Parse(claimsIdentity.FindFirst("user_id").Value)).ToList();
        }
        [Authorize]
        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] AdvertComments advertComments)
        {
            var claimsIdendity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdendity.FindFirst("user_id").Value);
            Users user = db.users.Find(user_id);

            
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!db.advert.Any(a => a.id == advertComments.advert_id)) return NotFound();

            advertComments.user_id = user_id;
            advertComments.comment = advertComments.comment;
            advertComments.advert_id = advertComments.advert_id;
            db.advert_comments.Add(advertComments);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionController.Handle(ex);
            }
            return Ok(advertComments);
        }
    }
}
