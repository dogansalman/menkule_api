using System;
using System.Web.Http;
using rest_api.Context;
using rest_api.Models;
using System.Linq;

namespace rest_api.Controllers
{
    [RoutePrefix("feedbacks")]
    public class FeedbacksController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] AdvertFeedbacks advertFeedbacks)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (db.advert.Find(advertFeedbacks.advert_id) == null) return NotFound();
            if (db.advert_feedbakcs.Where(af => af.advert_id == advertFeedbacks.advert_id && af.user_id == 0).FirstOrDefault() != null)
            {
                ResponserController.Response(System.Net.HttpStatusCode.Forbidden, "Geri bildirim kaydınız bulunmaktadır.");
            }
            db.advert_feedbakcs.Add(advertFeedbacks);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionController.Handle(ex);
            }
            return Ok(advertFeedbacks);
        }
    }
}
