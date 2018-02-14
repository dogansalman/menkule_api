using System;
using System.Linq;
using System.Web.Http;
using rest_api.Models;
using rest_api.Context;
using rest_api.Libary.Exceptions.ExceptionThrow;


namespace rest_api.Controllers
{
    [RoutePrefix("suggestions")]
    public class SuggestionsController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] Suggetions suggestions)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (db.suggestions.Any(s => s.email == suggestions.email)) return BadRequest();
            db.suggestions.Add(suggestions);
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
