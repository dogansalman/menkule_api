using System.Web.Http;
using System.Linq;
using rest_api.Context;
using rest_api.Models;
using rest_api.Libary.Exceptions.ExceptionThrow;

namespace rest_api.Controllers
{
    
    [RoutePrefix("scores")]
    public class AdvertScoresController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        [HttpPost]
        [Authorize]
        [Route("")]
        public IHttpActionResult add([FromBody] AdvertScores advertScore)
        {

            int user_id = Users.GetUserId(User);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            if (!db.advert.Any(a => a.id == advertScore.advert_id)) return NotFound();
           
            db.advert_scores.Add(advertScore);
            try
            {
                db.SaveChanges();
            }
            catch (System.Exception ex)
            {
                ExceptionThrow.Throw(ex);
            }
            return Ok(advertScore);
        }
    }
}
