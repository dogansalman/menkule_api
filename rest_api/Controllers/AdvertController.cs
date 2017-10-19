﻿using System;
using System.Web;
using System.Linq;
using System.Web.Http;
using System.Web.Helpers;
using System.Security.Claims;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.Http.Description;
using rest_api.Context;
using rest_api.Models;
using rest_api.Libary.Exceptions;
using rest_api.Libary.Cloudinary;

namespace rest_api.Controllers
{
    [RoutePrefix("adverts")]
    public class AdvertController : ApiController
    {
        //Get
        DatabaseContext db = new DatabaseContext();
        [Authorize(Roles = "owner")]
        [HttpGet]
        [Route("")]
        public object get()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            return (from a in db.advert
                    where a.user_id == user_id
                    from aimg in db.advert_images
                    where aimg.is_default == true && aimg.advert_id == a.id
                    join c in db.cities on a.city_id equals c.id
                    join t in db.towns on a.town_id equals t.id
                    join at in db.advert_types on a.advert_type_id equals at.id
                    join img in db.images on aimg.image_id equals img.id
                    select new
                    {
                        advert = new
                        {
                            id = a.id,
                            adress = a.adress,
                            advert_type_id = a.advert_type_id,
                            city_id = a.city_id,
                            town_id = a.town_id,
                            user_id = a.user_id,
                            entry_time = a.entry_time,
                            exit_time = a.exit_time,
                            state = a.state,
                            views = a.views,
                            score = a.score,
                            price = a.price,
                            min_layover = a.min_layover,
                            cancel_time = a.cancel_time,
                            description = a.description,
                            zoom = a.zoom,
                            latitude = a.latitude,
                            longitude = a.longitude,
                            title = a.title,
                            created_date = a.created_date,
                            updated_date = a.updated_date,
                            
                        }, city = c.name, town = t.name, advert_type = at.name, image = img.url })
                        .ToList();
        }

        //Detail
        [HttpGet]
        [Authorize(Roles = "owner")]
        [Route("{id}")]
        public object detail(int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);

            return (from a in db.advert
                    where a.user_id == user_id && a.id == id
                    join p in db.advert_properties on a.id equals p.advert_id
                    join pos in db.advert_possibilities on a.id equals pos.advert_id
                    select new
                    {
                        advert = new
                        {
                            id = a.id,
                            adress = a.adress,
                            advert_type_id = a.advert_type_id,
                            city_id = a.city_id,
                            town_id = a.town_id,
                            user_id = a.user_id,
                            entry_time = a.entry_time,
                            exit_time = a.exit_time,
                            state = a.state,
                            views = a.views,
                            score = a.score,
                            price = a.price,
                            min_layover = a.min_layover,
                            cancel_time = a.cancel_time,
                            description = a.description,
                            zoom = a.zoom,
                            latitude = a.latitude,
                            longitude = a.longitude,
                            title = a.title,
                            created_date = a.created_date,
                            updated_date = a.updated_date
                        },
                        possibilities = pos,
                        properties = p,
                        available_date = (db.advert_avaiable_dates.Where(ad => ad.advert_id == id)).ToList(),
                        unavailable_date = (db.advert_unavaiable_dates.Where(uad => uad.advert_id == id)).ToList(),
                        images = (from ai in db.advert_images
                                  where ai.advert_id == a.id
                                  join i in db.images on ai.image_id equals i.id
                                  select new { url = i.url, id = i.id, is_default = ai.is_default }
                              ).ToList()
                    }).FirstOrDefault();
        }

        //Add
        [HttpPost]
        [Authorize(Roles = "owner")]
        [Route("")]
        public IHttpActionResult add([FromBody] Advert advert)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            
            if (!ModelState.IsValid) return BadRequest(ModelState);

            advert.user_id = user_id;
            advert.score = 0;
            advert.state = false;
            advert.views = 0;
            advert.created_date = DateTime.Now;
            advert.updated_date = null;

            db.advert.Add(advert);
            db.SaveChanges();

            // Images
            advert.images.ToList().ForEach(i => {
                if (i.is_new)
                {
                    AdvertImages ai = new AdvertImages()
                    {
                        advert_id = advert.id,
                        image_id = i.id,
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
            if(advert.unavaiable_date != null)
            {
                advert.unavaiable_date.ToList().ForEach(i => {
                    i.advert_id = advert.id;
                    db.advert_unavaiable_dates.Add(i);
                });
            }
           

            //Avaiable Dates
            if(advert.available_date != null)
            {
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
            }
            
        
            
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
        [Authorize(Roles = "owner")]
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
        [Authorize(Roles = "owner")]
        [Route("{id}")]
        public IHttpActionResult update([FromBody] Advert advert, int id)
        {
            
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (db.advert.Where(a => a.id == id && a.user_id == user_id).FirstOrDefault() == null) return NotFound();
            db.Entry(advert).State = System.Data.Entity.EntityState.Detached;

            AdvertPossibilities apos = db.advert_possibilities.Where(aposs => aposs.advert_id == id).First();
            db.Entry(advert.possibility).State = System.Data.Entity.EntityState.Detached;

            AdvertProperties ap = db.advert_properties.Where(app => app.advert_id == id).First();
            db.Entry(advert.properties).State = System.Data.Entity.EntityState.Detached;

            db.advert_avaiable_dates.RemoveRange(db.advert_avaiable_dates.Where(ad => ad.advert_id == id));
            db.advert_unavaiable_dates.RemoveRange(db.advert_unavaiable_dates.Where(ud => ud.advert_id == id));

            //Images
            advert.images.ToList().ForEach(i => {
                if (i.is_new)
                {
                    AdvertImages ai = new AdvertImages()
                    {
                        advert_id = id,
                        image_id = i.id,
                        is_default = i.is_default
                    };
                    db.advert_images.Add(ai);
                }
                if (i.is_default)
                {
                    AdvertImages defaultImage = db.advert_images.Where(img => img.image_id == i.id && img.advert_id == id).FirstOrDefault();
                    if (defaultImage != null) {

                        db.advert_images.Where(ai => ai.advert_id == id).ToList().ForEach(ai => ai.is_default = false);
                        defaultImage.is_default = true;
                    } 
                }
                if (i.deleted)
                {
                    AdvertImages deletedImage = db.advert_images.Where(img => img.image_id == i.id && img.advert_id == id).FirstOrDefault();
                    if (deletedImage != null) db.advert_images.Remove(deletedImage);
                }
            });
            

            db.SaveChanges();

            using (var dbContext = new DatabaseContext())
            {
                //Advert
                advert.id = id;
                advert.user_id = user_id;
                advert.updated_date = DateTime.Now;
                advert.state = false;
                dbContext.Entry(advert).State = System.Data.Entity.EntityState.Modified;
                dbContext.Entry(advert).Property("score").IsModified = false;
                dbContext.Entry(advert).Property("views").IsModified = false;
                dbContext.Entry(advert).Property("user_id").IsModified = false;
                dbContext.Entry(advert).Property("state").IsModified = false;

                //Possibilities
                advert.possibility.advert_id = advert.id;
                advert.possibility.id = apos.id;
                advert.possibility.updated_date = DateTime.Now;
                dbContext.Entry(advert.possibility).State = System.Data.Entity.EntityState.Modified;

                //Properties
                advert.properties.advert_id = advert.id;
                advert.properties.id = ap.id;
                advert.properties.updated_date = DateTime.Now;
                dbContext.Entry(advert.properties).State = System.Data.Entity.EntityState.Modified;

                //Unavaiable Dates
                if(advert.unavaiable_date != null)
                {
                    advert.unavaiable_date.ToList().ForEach(i => {
                        i.advert_id = advert.id;
                        dbContext.advert_unavaiable_dates.Add(i);
                    });
                }

                //Avaiable Dates
                if(advert.available_date != null)
                {
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
                            dbContext.advert_avaiable_dates.Add(avaiableDate);
                        }
                    });
                }
                
                dbContext.SaveChanges();
            }

            return Ok(advert);

        }

        //Photos
        [HttpPost]
        [Authorize(Roles = "owner")]
        [Route("photo")]
        [ResponseType(typeof(FileUpload))]
        public IHttpActionResult upload()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            Users user = db.users.Find(user_id);

            var httpRequest = HttpContext.Current.Request;
            List<string> imageExt = new List<string> { { "jpg" }, { "png" }, { "jpeg" } };
            var image = new WebImage(httpRequest.InputStream);
            if (!imageExt.Contains(image.ImageFormat.ToString().ToLower())) new BadImageFormatException();

            image.AddImageWatermark(HttpContext.Current.Server.MapPath("~/App_Data/watermark/logo.png"), 253, 93, "Right", "Bottom", 40, 10);

            Images userImage = Cloudinary.upload(image, "advert/" + user.name + "-" + user.lastname + "-" + user.id);
            if (userImage == null) return BadRequest();

            user.image_id = userImage.id;
            db.SaveChanges();

            return Ok(userImage);
        }
    }
}
