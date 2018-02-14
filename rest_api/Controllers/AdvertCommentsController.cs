using System;
using System.Linq;
using System.Web.Http;
using rest_api.Models;
using rest_api.Context;
using rest_api.Libary.Exceptions.ExceptionThrow;


namespace rest_api.Controllers
{
    [RoutePrefix("comments")]
    public class AdvertCommentsController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
    
        // Get Comment
        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult read(int id)
        {
            int user_id = Users.GetUserId(User);
            AdvertComments comment = db.advert_comments.Where(ac => ac.id == id).FirstOrDefault();
            if (comment == null) return NotFound();
            return Ok(comment);

        }

        // Get Comments
        [Authorize]
        [HttpGet]
        [Route("")]
        public object gets()
        {
            
            int user_id = Users.GetUserId(User);
            return from c in db.advert_comments
                   from aimg in db.advert_images
                   where aimg.is_default == true && aimg.advert_id == c.advert_id
                   join i in db.images on aimg.image_id equals i.id
                   into userscomments
                   where c.user_id == user_id
                   from img in userscomments.DefaultIfEmpty()
                   select new { comment = c, advert_photo = img.url, advert_id = c.advert_id };
        }

        // Add
        [Authorize]
        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] AdvertComments advertComments)
        {
            int user_id = Users.GetUserId(User);
            Users user = db.users.Find(user_id);

            
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!db.advert.Any(a => a.id == advertComments.advert_id)) return NotFound();

            advertComments.user_id = user_id;
            db.advert_comments.Add(advertComments);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionThrow.Throw(ex);
            }
            return Ok(advertComments);
        }

        //Update
        [Authorize]
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult update([FromBody] AdvertComments advertComments, int id)
        {
            int user_id = Users.GetUserId(User);
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
                    ExceptionThrow.Throw(ex);
                }
            }
            return Ok(advertComments);
        }

        // Delete
        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult delete(int id)
        {
            int user_id = Users.GetUserId(User);
            AdvertComments comment = db.advert_comments.Where(ac => ac.id == id).FirstOrDefault();
            if (comment == null) return NotFound();
            db.advert_comments.Remove(comment);
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

    }
}
