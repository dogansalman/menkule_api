using System.Web.Http;
using rest_api.Context;
using rest_api.Models;
using System.Linq;


namespace rest_api.Controllers
{
    
    [RoutePrefix("scores")]
    public class AdvertScoresController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] AdvertScores advertScore)
        {

           
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            if (!db.advert.Any(a => a.id == advertScore.advert_id)) return NotFound();
            if (db.advert_scores.Any(adscr => adscr.advert_id == advertScore.advert_id && adscr.user_id == 0)) {
                ResponserController.Response(System.Net.HttpStatusCode.Forbidden, "Daha önce puan eklenmiş.");
            } 

            db.advert_scores.Add(advertScore);
            try
            {
                db.SaveChanges();
            }
            catch (System.Exception ex)
            {
                ExceptionController.Handle(ex);
            }
            return Ok(advertScore);
        }
    }
}
