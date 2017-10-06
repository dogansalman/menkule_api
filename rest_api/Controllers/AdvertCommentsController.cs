using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Security.Claims;
using rest_api.Models;
using rest_api.Context;
using rest_api.Libary.Exceptions;
using rest_api.ModelViews;

namespace rest_api.Controllers
{
    [RoutePrefix("comments")]
    public class AdvertCommentsController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        // Get Advert Comments
        [HttpGet]
        [Route("advert/{id}")]
        public object get(int id)
        {
            return from c in db.advert_comments
                   join u in db.users on c.user_id equals u.id
                   join i in db.images on u.image_id equals i.id
                   into users
                   where c.advert_id == id && c.state == true
                   from img in users.DefaultIfEmpty()
                   select new { comment = c, user = new UserCommentMV { id = u.id, fullname = u.name + " " + u.lastname, photo = img.url } };

        }

        // Get User Comments
        [Authorize]
        [HttpGet]
        [Route("user")]
        public List<AdvertComments> getUserComment()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            return db.advert_comments.Where(a => a.user_id == user_id).ToList();
        }

        // Add
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
                ExceptionHandler.Handle(ex);
            }
            return Ok(advertComments);
        }

        //Update
        [Authorize]
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult update([FromBody] AdvertComments advertComments, int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!db.advert_comments.Any(ac => ac.advert_id == advertComments.advert_id && ac.id == id && ac.user_id == user_id)) return NotFound();

            using (var dbContext = new DatabaseContext())
            {
                advertComments.id = id;
                advertComments.user_id = user_id;
                advertComments.updated_date = DateTime.Now;
                dbContext.Entry(advertComments).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex);
                }
            }
            return Ok(advertComments);
        }
    }
}
