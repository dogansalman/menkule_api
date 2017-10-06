using System;
using System.Linq;
using System.Web.Http;
using System.Security.Claims;
using rest_api.Context;
using rest_api.Models;
using rest_api.Libary.Exceptions;

namespace rest_api.Controllers
{
    [RoutePrefix("adverts")]
    public class AdvertController : ApiController
    {
        //Get
        DatabaseContext db = new DatabaseContext();
        [Authorize]
        [HttpGet]
        [Route("")]
        public object get()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            return (from a in db.advert
                    from aimg in db.advert_images
                    where aimg.is_default == true && aimg.advert_id == a.id
                    join c in db.cities on a.city_id equals c.id
                    join t in db.towns on a.town_id equals t.id
                    join at in db.advert_types on a.advert_type_id equals at.id
                    join img in db.images on aimg.image_id equals img.id
                select new { advert = a, city = c.name, town = t.name, advert_type = at.name, image = img.url }).ToList();
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
        [Authorize]
        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] Advert advert)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            advert.user_id = user_id;

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

                ExceptionHandler.Handle(ex);
            }
            return Ok(advert);
        }


        //Delete
        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public IHttpActionResult delete(int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            Advert advert = db.advert.Where(a => a.id == id && a.user_id == user_id).FirstOrDefault();
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
                ExceptionHandler.Handle(ex);
            }

            return Ok();
        }

        //Update
        [HttpPut]
        [Authorize]
        [Route("{id}")]
        public IHttpActionResult update([FromBody] Advert advert, int id)
        {
            //TODO update properties images possibility avaiable_date unaviabla_date
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (db.advert.Where(a => a.id == id && a.user_id == user_id).FirstOrDefault() == null) return NotFound();

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
                    ExceptionHandler.Handle(ex);
                }
            }
            return Ok(advert);

        }
    }
}
