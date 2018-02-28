using System;
using System.Web;
using System.Linq;
using System.Web.Http;
using System.Web.Helpers;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.Http.Description;
using rest_api.Context;
using rest_api.Models;
using rest_api.ModelViews;
using rest_api.Libary.Exceptions.ExceptionThrow;
using rest_api.Libary.Cloudinary;
using rest_api.OAuth.CustomAttributes.Owner;

namespace rest_api.Controllers
{
    [RoutePrefix("adverts")]
    public class AdvertController : ApiController
    {
        //Get
        DatabaseContext db = new DatabaseContext();
        [Authorize]
        [Owner]
        [HttpGet]
        [Route("")]
        public object get()
        {
            int user_id = Users.GetUserId(User);
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
                            user_id = a.user_id,
                            entry_time = a.entry_time,
                            exit_time = a.exit_time,
                            state = a.state,
                            views = a.views,
                            score = a.score,
                            price = a.price,
                            min_layover = a.min_layover,
                            cancel_time = a.cancel_time,
                            zoom = a.zoom,
                            latitude = a.latitude,
                            longitude = a.longitude,
                            title = a.title,
                            created_date = a.created_date,
                            updated_date = a.updated_date,
                            
                        },
                        city = c, town = t, advert_type = at, image = img.url })
                        .ToList();
        }

        //Detail
        [HttpGet]
        [Authorize]
        [Owner]
        [Route("{id}")]
        public object detail(int id)
        {
            int user_id = Users.GetUserId(User);

            return (from a in db.advert
                    where a.user_id == user_id && a.id == id
                    join p in db.advert_properties on a.id equals p.advert_id
                    join pos in db.advert_possibilities on a.id equals pos.advert_id
                    select new
                    {
                        
                            id = a.id,
                            adress = a.adress,
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
                        ,
                        possibilities = pos,
                        properties = p,
                        city = (db.cities.Where(c => c.id == a.city_id)).FirstOrDefault(),
                        town = (db.towns.Where(t => t.id == a.town_id)).FirstOrDefault(),
                        advert_type = (db.advert_types.Where(at => at.id == a.advert_type_id)).FirstOrDefault(),
                        available_date = (
                            from ad in db.advert_avaiable_dates
                            where ad.advert_id == id
                            group ad by ad.uniq into g
                                select new
                                {
                                    from_date = g.Min(e => e.fulldate),
                                    to_date = g.Max(e => e.fulldate),
                                    uniq = g.Select(a => a.uniq).FirstOrDefault()
                                }
                            ).ToList(),
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
        [Authorize]
        [Owner]
        [Route("")]
        public IHttpActionResult add([FromBody] Advert advert)
        {
            int user_id = Users.GetUserId(User);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            advert.user_id = user_id;
            advert.score = 0;
            advert.state = false;
            advert.views = 0;
            advert.created_date = DateTime.Now;
            advert.updated_date = null;

            db.advert.Add(advert);
            db.SaveChanges();

            // Advert Images Validation
            var imagesList = advert.images.ToList();
            if (imagesList.FindAll(img => img.is_default && !img.deleted).Count == 0) ExceptionThrow.Throw("Varsayılan fotoğraf seçin.", System.Net.HttpStatusCode.BadRequest);
            if (imagesList.FindAll(img => !img.deleted).Count < 3) ExceptionThrow.Throw("En az 3 fotoğraf yüklemelisiniz.", System.Net.HttpStatusCode.BadRequest);
            if (imagesList.FindAll(img => img.is_default).Count > 1) ExceptionThrow.Throw("En fazla 1 fotoğraf varsayılan olarak seçilebilir.", System.Net.HttpStatusCode.BadRequest);

            List <_AdvertImages> selectedImages = advert.images.ToList().FindAll(i => i.is_default == true);
            if (selectedImages.Count == 0) advert.images.ToList()[0].is_default = true;
            
            if(selectedImages.Count > 1)
            {
                advert.images.ToList().ForEach(i => i.is_default = false);
                advert.images.ToList()[0].is_default = true;
            }
            
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
                    for (DateTime date = ad.from_fulldate; date.Date <= ad.to_fulldate.Date; date = date.AddDays(1))
                    {
                        AdvertAvailableDate avaiableDate = new AdvertAvailableDate()
                        {
                            day = date.Day,
                            month = date.Month,
                            year = date.Year,
                            fulldate = date,
                            uniq = String.Format("{0:MMddyyyy}", ad.from_fulldate) + String.Format("{0:MMddyyyy}", ad.to_fulldate),
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

                ExceptionThrow.Throw(ex);
            }
            return Ok(advert);
        }


        //Delete
        [HttpDelete]
        [Authorize]
        [Owner]
        [Route("{id}")]
        public IHttpActionResult delete(int id)
        {
            int user_id = Users.GetUserId(User);
            Advert advert = db.advert.Where(a => a.id == id && a.user_id == user_id).FirstOrDefault();
            if (advert == null) return NotFound();
            db.advert.Remove(advert);

            //Unavaiable Dates
            db.advert_unavaiable_dates.RemoveRange(db.advert_unavaiable_dates.Where(uad => uad.advert_id == id));

            //Avaiable Dates
            db.advert_avaiable_dates.RemoveRange(db.advert_avaiable_dates.Where(ad => ad.advert_id == id));

            try
            {
                db.SaveChanges();
            }
            catch (System.Exception ex)
            {
                ExceptionThrow.Throw(ex);
            }

            return Ok();
        }

        //Update
        [HttpPut]
        [Authorize]
        [Owner]
        [Route("{id}")]
        public IHttpActionResult update([FromBody] Advert advert, int id)
        {
            
            int user_id = Users.GetUserId(User);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (db.advert.Where(a => a.id == id && a.user_id == user_id).FirstOrDefault() == null) return NotFound();
            db.Entry(advert).State = System.Data.Entity.EntityState.Detached;

            AdvertPossibilities apos = db.advert_possibilities.Where(aposs => aposs.advert_id == id).First();
            db.Entry(advert.possibility).State = System.Data.Entity.EntityState.Detached;

            AdvertProperties ap = db.advert_properties.Where(app => app.advert_id == id).First();
            db.Entry(advert.properties).State = System.Data.Entity.EntityState.Detached;

            db.advert_avaiable_dates.RemoveRange(db.advert_avaiable_dates.Where(ad => ad.advert_id == id));
            db.advert_unavaiable_dates.RemoveRange(db.advert_unavaiable_dates.Where(ud => ud.advert_id == id));


            // Advert Images Validation
            var imagesList = advert.images.ToList();
            if (imagesList.FindAll(img => img.is_default && !img.deleted).Count == 0) ExceptionThrow.Throw("Varsayılan fotoğraf seçin.", System.Net.HttpStatusCode.BadRequest);
            if (imagesList.FindAll(img => !img.deleted).Count < 3) ExceptionThrow.Throw("En az 3 fotoğraf yüklemelisiniz.", System.Net.HttpStatusCode.BadRequest);
            if (imagesList.FindAll(img => img.is_default).Count > 1) ExceptionThrow.Throw("En fazla 1 fotoğraf varsayılan olarak seçilebilir.", System.Net.HttpStatusCode.BadRequest);

            //Images
            imagesList.ForEach(i => {
                if (i.is_new && !db.advert_images.Any(ai => ai.advert_id == id && ai.image_id == i.id))
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
                        for (DateTime date = ad.from_fulldate; date.Date <= ad.to_fulldate.Date; date = date.AddDays(1))
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
        [Authorize]
        [Owner]
        [Route("photo")]
        [ResponseType(typeof(FileUpload))]
        public IHttpActionResult upload()
        {
            try
            {
 
                int user_id = Users.GetUserId(User);
                Users user = db.users.Find(user_id);

                var httpRequest = HttpContext.Current.Request;
                List<string> imageExt = new List<string> { { "jpg" }, { "png" }, { "jpeg" } };
                var image = new WebImage(httpRequest.InputStream);
                if (!imageExt.Contains(image.ImageFormat.ToString().ToLower())) new BadImageFormatException();

                image.AddImageWatermark(HttpContext.Current.Server.MapPath("~/App_Data/watermark/logo.png"), 150, 56, "Right", "Bottom", 40, 10);

                Images advertImage = Cloudinary.upload(image, "advert/" + user.name + "-" + user.lastname + "-" + user.id);

                db.images.Add(advertImage);
                db.SaveChanges();

                if (advertImage == null) return BadRequest();

                return Ok(advertImage);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message.ToString());
            }
        }

        //Search Find
        [HttpGet]
        [Route("find/{id}")]
        public object find(int id)
        {
        

            // Update views
            using (var _db = new DatabaseContext())
            {
                var _advert = _db.advert.Find(id);
                if (_advert == null) return NotFound();

               string clientIp = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
               HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim().Replace(".", "");

                if (_db.advert_views.Where(av => av.advert_id == id && av.ip == clientIp).FirstOrDefault() == null)
                {
                    AdvertViews _av = new AdvertViews { advert_id = id, ip = clientIp };
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _advert.views = _advert.views + 1;
                    _db.advert_views.Add(_av);
                    _db.SaveChanges();
                    _db.Configuration.ValidateOnSaveEnabled = true;
                }   
            }


            return (from a in db.advert
                        where a.state == true && a.id == id
                        join p in db.advert_properties on a.id equals p.advert_id
                        join pos in db.advert_possibilities on a.id equals pos.advert_id
                        join u in db.users on a.user_id equals u.id
                        join uimg in db.images on u.image_id equals uimg.id into j1
                        from j2 in j1.DefaultIfEmpty()
                        select new
                        {
                            advert = new
                            {
                                id = a.id,
                                adress = a.adress,
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
                            city = (db.cities.Where(c => c.id == a.city_id)).FirstOrDefault(),
                            town = (db.towns.Where(t => t.id == a.town_id)).FirstOrDefault(),
                            advert_type = (db.advert_types.Where(at => at.id == a.advert_type_id)).FirstOrDefault(),
                            available_date = (
                                from ad in db.advert_avaiable_dates
                                where ad.advert_id == id
                                group ad by ad.uniq into g
                                select new
                                {
                                    from_date = g.Min(e => e.fulldate),
                                    to_date = g.Max(e => e.fulldate),
                                    uniq = g.Select(a => a.uniq).FirstOrDefault()
                                }
                                ).ToList(),
                            unavailable_date = (db.advert_unavaiable_dates.Where(uad => uad.advert_id == id)).ToList(),
                            images = (from ai in db.advert_images
                                      where ai.advert_id == a.id
                                      join i in db.images on ai.image_id equals i.id
                                      select new { url = i.url, id = i.id, is_default = ai.is_default }
                                  ).ToList(),
                            comments = (
                            from com in db.advert_comments
                            where com.advert_id == id && com.state == true
                            select new { comment = com, user = new { id = u.id, fullname = u.name + " " + u.lastname, photo = j2.url } }
                                        ).Take(10).ToList(),
                            user = new { id = u.id, fullname = u.name + " " + u.lastname, photo = j2.url, created_date = u.created_date, state = u.state }
                        }).FirstOrDefault();
        }
    }
}