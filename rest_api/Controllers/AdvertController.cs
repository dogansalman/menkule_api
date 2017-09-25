using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using rest_api.Context;
using rest_api.Models;

namespace rest_api.Controllers
{
    [RoutePrefix("adverts")]
    public class AdvertController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        [HttpGet]
        [Route("")]
        public List<Advert> get()
        {
            return db.advert.OrderByDescending(a => a.id).ToList();
        }
    }
}
