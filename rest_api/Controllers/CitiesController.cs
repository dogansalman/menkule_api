using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using rest_api.Context;
using rest_api.Models;

namespace rest_api.Controllers
{
    [RoutePrefix("cities")]
    public class CitiesController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        [HttpGet]
        [Route("")]
        public List<Cities> cities()
        {
            return db.cities.ToList();
        }
        [HttpGet]
        [Route("{id}")]
        public List<Towns> towns(int id)
        {
            return db.towns.Where(t => t.city_id == id).ToList();
        }
    }
}
