using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using rest_api.Context;
using rest_api.Models;
using rest_api.ModelViews;

namespace rest_api.Controllers
{
    [RoutePrefix("adverts")]
    public class AdvertController : ApiController
    {
        // Get
        DatabaseContext db = new DatabaseContext();
        [HttpGet]
        [Route("")]
        public List<Advert> get()
        {
            return db.advert.OrderByDescending(a => a.id).ToList();
        }

        // Add
        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] Advert advert)
        {
            if (!ModelState.IsValid) BadRequest(ModelState);
            db.advert.Add(advert);
            db.SaveChanges();

            if(advert.id > 0)
            {
                advert.images.ToList().ForEach(i => {
                    if (i.is_new)
                    {
                        AdvertImages ai = new AdvertImages()
                        {
                            advert_id = advert.id,
                            image_id = i.image_id
                        };
                        db.advert_images.Add(ai);
                    }
                });

            }
            advert.possibility.advert_id = advert.id;
            db.advert_possibilities.Add(advert.possibility);

            advert.properties.advert_id = advert.id;
            db.advert_properties.Add(advert.properties);
          

            db.SaveChanges();
            try
            {
               
            }
            catch (System.Exception ex)
            {

                ExceptionController.Handle(ex);
            }
            return Ok(advert);
        }
    }
}
