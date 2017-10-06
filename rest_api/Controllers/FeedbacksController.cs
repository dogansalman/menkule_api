using System;
using System.Web.Http;
using System.Linq;
using System.Security.Claims;
using rest_api.Context;
using rest_api.Models;
using rest_api.Libary.Exceptions;

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
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!db.advert.Any(a => a.id == advertFeedbacks.advert_id)) return NotFound();

            advertFeedbacks.user_id = user_id;
            db.advert_feedbakcs.Add(advertFeedbacks);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
            return Ok(advertFeedbacks);
        }
    }
}
