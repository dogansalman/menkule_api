using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using rest_api.Context;
using rest_api.Models;

namespace rest_api.Controllers
{
    [RoutePrefix("advert/types")]
    public class AdvertTypesController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        [HttpGet]
        [Route("")]
        public List<AdvertTypes> get() {
            
            return db.advert_types.ToList();
        }
    }
}
