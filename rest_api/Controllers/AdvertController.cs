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

        // Detail
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult detail(int id)
        {
            Advert advert = db.advert.Find(id);
            if (advert == null) return NotFound();

            advert.properties = db.advert_properties.FirstOrDefault(ap => ap.advert_id == advert.id);
            advert.possibility = db.advert_possibilities.FirstOrDefault(ap => ap.advert_id == advert.id);
            advert.unavaiable_date = db.advert_unavaiable_dates.Where(aud => aud.advert_id == advert.id).ToList();
            advert.avaiable_dates = db.advert_avaiable_dates.Where(aad => aad.advert_id == advert.id).ToList();
            
            return Ok(advert);
        }

        // Add
        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] Advert advert)
        {
            if (!ModelState.IsValid) BadRequest(ModelState);
            db.advert.Add(advert);
            db.SaveChanges();

            // Images
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

            //Possibility
            advert.possibility.advert_id = advert.id;
            db.advert_possibilities.Add(advert.possibility);

            //Properties
            advert.properties.advert_id = advert.id;
            db.advert_properties.Add(advert.properties);

            //Unavaiable Dates
            advert.unavaiable_date.ToList().ForEach(i => {
                i.advert_id = advert.id;
                db.advert_unavaiable_dates.Add(i);
            });
            
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

        //Delete
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult delete(int id)
        {
            Advert advert = db.advert.Find(id);
            if (advert == null) return NotFound();
            db.advert.Remove(advert);

            AdvertPossibilities ap = db.advert_possibilities.FirstOrDefault(sm => sm.advert_id == id) ;
            if (ap != null) db.advert_possibilities.Remove(ap);

            AdvertProperties apro = db.advert_properties.FirstOrDefault(apr => apr.advert_id == id);
            if (apro != null) db.advert_properties.Remove(apro);

            
            

            try
            {
                db.SaveChanges();
            }
            catch (System.Exception ex)
            {
                ExceptionController.Handle(ex);
            }

            return Ok();
        }
    }
}
