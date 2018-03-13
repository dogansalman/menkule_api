using System;
using System.Web.Http;
using System.Linq;
using rest_api.Context;
using rest_api.Models;
using rest_api.Libary.Exceptions.ExceptionThrow;

namespace rest_api.Controllers
{
    [RoutePrefix("feedbacks")]
    public class FeedbacksController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        [HttpPost]
        [Route("")]
        [Authorize]
        public IHttpActionResult add([FromBody] AdvertFeedbacks advertFeedbacks)
        {
            int user_id = Users.GetUserId(User);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!db.advert.Any(a => a.id == advertFeedbacks.advert_id)) return NotFound();

            advertFeedbacks.user_id = user_id;

            if (db.advert_feedbakcs.Where(af => af.advert_id == advertFeedbacks.advert_id & af.user_id == user_id).ToList().Count == 5) ExceptionThrow.Throw("Bir ilan için en fazla 5 kere geri bildirim hakkınız bulunmaktadır.", System.Net.HttpStatusCode.Forbidden);

            db.advert_feedbakcs.Add(advertFeedbacks);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionThrow.Throw(ex);
            }
            return Ok(advertFeedbacks);
        }
    }
}
