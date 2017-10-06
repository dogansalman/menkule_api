using System.Web.Http;
using System.Linq;
using System.Security.Claims;
using rest_api.Context;
using rest_api.Models;
using rest_api.Libary.Exceptions;

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

            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            if (!db.advert.Any(a => a.id == advertScore.advert_id)) return NotFound();
           
            db.advert_scores.Add(advertScore);
            try
            {
                db.SaveChanges();
            }
            catch (System.Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
            return Ok(advertScore);
        }
    }
}
