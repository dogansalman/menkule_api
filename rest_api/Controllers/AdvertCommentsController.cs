using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using rest_api.Models;
using rest_api.Context;


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

        [HttpGet]
        [Route("user")]
        public List<AdvertComments> getUserComment()
        {
            return db.advert_comments.Where(a => a.user_id == 1).ToList();
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] AdvertComments advertComments)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!db.advert.Any(a => a.id == advertComments.advert_id)) return NotFound();
            advertComments.fullname = "Doğan SALMAN";
            advertComments.user_id = 1;
            advertComments.photo = "user_photo";
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
