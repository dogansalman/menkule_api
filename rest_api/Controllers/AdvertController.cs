using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using rest_api.Context;
using rest_api.Models;
using System.Data.Entity.Migrations;
using rest_api.ModelViews;
using System;

namespace rest_api.Controllers
{
    [RoutePrefix("adverts")]
    public class AdvertController : ApiController
    {
        //Get
        DatabaseContext db = new DatabaseContext();

        [HttpGet]
        [Route("")]
        public List<Advert> get()
        {
            return db.advert.OrderByDescending(a => a.id).ToList();
        }

        //Detail
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult detail(int id)
        {
            Advert advert = db.advert.Find(id);
            if (advert == null) return NotFound();

            advert.properties = db.advert_properties.FirstOrDefault(ap => ap.advert_id == advert.id);
            advert.possibility = db.advert_possibilities.FirstOrDefault(ap => ap.advert_id == advert.id);
            advert.unavaiable_date = db.advert_unavaiable_dates.Where(aud => aud.advert_id == advert.id).ToList();
            advert.available_date = db.advert_avaiable_dates.Where(aad => aad.advert_id == advert.id).ToList();
            
            return Ok(advert);
        }

        //Add
        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] Advert advert)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            db.advert.Add(advert);
            db.SaveChanges();

            // Images
            advert.images.ToList().ForEach(i => {
                if (i.is_new)
                {
                    AdvertImages ai = new AdvertImages()
                    {
                        advert_id = advert.id,
                        image_id = i.image_id,
                        is_default = i.is_default
                    };
                    db.advert_images.Add(ai);
                }
            });

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

            //Avaiable Dates
            advert.available_date.ToList().ForEach(ad =>
            {
                for (DateTime date = ad.from; date.Date <= ad.to.Date; date = date.AddDays(1))
                {
                    AdvertAvailableDate avaiableDate = new AdvertAvailableDate()
                    {
                        day = date.Day,
                        month = date.Month,
                        year = date.Year,
                        fulldate = date,
                        uniq = String.Format("{0:MMddyyyy}", ad.from) + String.Format("{0:MMddyyyy}", ad.to),
                        advert_id = advert.id
                    };
                    db.advert_avaiable_dates.Add(avaiableDate);
                }
            });
        
            
            try
            {
                db.SaveChanges();
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

            //Possibility
            AdvertPossibilities ap = db.advert_possibilities.FirstOrDefault(sm => sm.advert_id == id) ;
            if (ap != null) db.advert_possibilities.Remove(ap);

            //Properties
            AdvertProperties apro = db.advert_properties.FirstOrDefault(apr => apr.advert_id == id);
            if (apro != null) db.advert_properties.Remove(apro);

            //Unavaiable Dates
            db.advert_unavaiable_dates.RemoveRange(db.advert_unavaiable_dates.Where(uad => uad.advert_id == id));

            //Avaiable Dates
            db.advert_avaiable_dates.RemoveRange(db.advert_avaiable_dates.Where(ad => ad.advert_id == id));

            //Images
            db.advert_images.RemoveRange(db.advert_images.Where(ai => ai.advert_id == id));
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

        //Update
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult update([FromBody] Advert advert, int id)
        {
            //TODO update properties images possibility avaiable_date unaviabla_date

            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (db.advert.Find(id) == null) return NotFound();

            using (var dbContext = new DatabaseContext())
            {
                advert.id = id;
                dbContext.Entry(advert).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    ExceptionController.Handle(ex);
                }
            }
            return Ok(advert);

        }
    }
}
