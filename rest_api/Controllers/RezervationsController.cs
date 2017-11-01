﻿using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Security.Claims;
using System.Collections.Generic;
using rest_api.Libary.Responser;
using rest_api.ModelViews;
using rest_api.Context;
using rest_api.Models;
using rest_api.Libary.NetGsm;
namespace rest_api.Controllers
{

    [RoutePrefix("rezervations")]
    public class RezervationsController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        //Gets Outbound
        [HttpGet]
        [Authorize]
        [Route("out")]
        public object getsOut()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);

             return (
                 from r in db.rezervations
                 where r.user_id == user_id
                 from aimg in db.advert_images
                 where aimg.is_default == true && aimg.advert_id == r.advert_id
                 join img in db.images on aimg.image_id equals img.id
                 join ra in db.rezervation_adverts on r.id equals ra.rezervation_id
                 join c in db.cities on ra.city_id equals c.id
                 join t in db.towns on ra.town_id equals t.id
                 join at in db.advert_types on ra.advert_type_id equals at.id
                 select new
                 {
                     rezervation = r,
                     advert = new {
                         adress = ra.adress,
                         title = ra.title,
                         city = c.name,
                         town = t.name,
                         photo = img.url,
                         type = at.name

                     },
                     visitors = (from v in db.rezervation_visitors where v.rezervation_id == r.id select new { visitor = v}).ToList()
                 }
                 ).ToList();
        }

        //Gets Inbound
        [HttpGet]
        [Authorize]
        [Route("in")]
        public object getsIn()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            return (
                 from r in db.rezervations
                 where r.owner == user_id
                 from aimg in db.advert_images
                 where aimg.is_default == true && aimg.advert_id == r.advert_id
                 join img in db.images on aimg.image_id equals img.id
                 join ra in db.rezervation_adverts on r.id equals ra.rezervation_id
                 join c in db.cities on ra.city_id equals c.id
                 join t in db.towns on ra.town_id equals t.id
                 join at in db.advert_types on ra.advert_type_id equals at.id
                 select new
                 {
                     rezervation = r,
                     advert = new
                     {
                         adress = ra.adress,
                         title = ra.title,
                         city = c.name,
                         town = t.name,
                         photo = img.url,
                         type = at.name

                     },
                     visitors = (from v in db.rezervation_visitors where v.rezervation_id == r.id select new { visitor = v }).ToList()
                 }
                 ).ToList();

        }

        // Get
        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public object get(int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            
            var rezervation = (
                           from r in db.rezervations
                           where r.owner == user_id || r.user_id == user_id
                           join ra in db.rezervation_adverts on r.id equals ra.rezervation_id
                           join c in db.cities on ra.city_id equals c.id
                           join t in db.towns on ra.town_id equals t.id
                           join p in db.advert_properties on r.advert_id equals p.advert_id
                           join pos in db.advert_possibilities on r.advert_id equals pos.advert_id
                           join at in db.advert_types on ra.advert_type_id equals at.id

                           select new _RezervationDetails {
                               id = r.id,
                               checkin = r.checkin,
                               checkout = r.checkout,
                               advert_id = r.advert_id,
                               days = r.days,
                               day_price = r.day_price,
                               total_price = r.total_price,
                               user_id = r.user_id,
                               visitor = r.visitor,
                               description_state = r.description_state,
                               note = r.note,
                               is_cancel = r.is_cancel,
                               state = r.state,
                               created_date = r.created_date,
                               updated_date = r.updated_date,
                               rezervation_advert = new _RezervationAdvert {
                                   advert = ra,
                                   images = (db.advert_images.GroupJoin(
                                               db.images,
                                               aimg => aimg.image_id,
                                               i => i.id,
                                               (ai, i) => new { advertimg = ai, image = i }
                                               )
                                               .Where(aimg => aimg.advertimg.advert_id == ra.advert_id)
                                               .SelectMany(AdvertWithImage =>
                                               AdvertWithImage.image).ToList()),
                                   advert_type = at,
                                   cities = c,
                                   possibilities = pos,
                                   properties = p,
                                   towns = t
                               },
                               visitors = (db.rezervation_visitors.Where(v => v.rezervation_id == r.id)).ToList()
                           }
                           ).FirstOrDefault();

            // user informations validation
            int _user_id = user_id == rezervation.rezervation_advert.advert.user_id ? rezervation.user_id: rezervation.rezervation_advert.advert.user_id;
            rezervation.advert_owner = rezervation.rezervation_advert.advert.user_id == user_id ? true : false;
            var user = (
                    from u in db.users
                    where u.id == _user_id
                    join uimg in db.images on u.image_id equals uimg.id into j1
                    from j2 in j1.DefaultIfEmpty()
                    select new _RezervationUserInfo
                    {
                        id = u.id,
                        fullname = u.name + " " + u.lastname,
                        gsm = u.gsm,
                        photo = j2.url
                    }).FirstOrDefault();

            rezervation.user_information = user;
            if(rezervation.state == false)
            {
                rezervation.user_information.gsm = rezervation.user_information.gsm.Substring(0, rezervation.user_information.gsm.Length - 4) + "****";
            }
            return rezervation;
        }

        // Create
        [HttpPost]
        [Authorize]
        [Route("")]
        public IHttpActionResult add([FromBody] _Rezervation _rezervation)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // get user
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            Users user = db.users.Where(u => u.id == user_id && u.state == true).FirstOrDefault();
            if (user == null) Responser.Response(HttpStatusCode.Forbidden, "Lütfen hesabınızı doğrulayın.");


            // existence
            Advert advert = db.advert.Where(a => a.state == true && a.id == _rezervation.advert_id).FirstOrDefault();
            if (advert == null) return NotFound();
            if (db.rezervations.Any(rez => rez.user_id == user_id && rez.advert_id == _rezervation.advert_id && rez.checkin == _rezervation.checkin && rez.checkout == _rezervation.checkout)) Responser.Response(HttpStatusCode.Forbidden, "Zaten aynı tarih için bir rezervasyon talebiniz bulunmakta.");

            //get owner
            Users owner = db.users.Where(u => u.id == advert.user_id).FirstOrDefault();
            if (owner == null) return NotFound();

            // available date validation
            var dateList = new List<DateTime>();
            for (DateTime date = _rezervation.checkin; date.Date <= _rezervation.checkout.Date; date = date.AddDays(1)) {
                dateList.Add(date);
            }
            if(db.advert_unavaiable_dates.Where(i => dateList.Contains(i.fulldate)).Count() > 0) Responser.Response(HttpStatusCode.Forbidden, "İlan belirtilen tarih için müsait değil.");

            // create rezervation
            Rezervations rezervation = new Rezervations
            {
                advert_id = _rezervation.advert_id,
                checkin = _rezervation.checkin,
                checkout = _rezervation.checkout,
                created_date = DateTime.Now,
                gsm = user.gsm,
                name = user.name,
                lastname = user.lastname,
                visitor = _rezervation.visitors.Count,
                user_id = user.id,
                day_price = advert.price,
                owner = advert.user_id
                
            };

            db.rezervations.Add(rezervation);
            db.SaveChanges();

            // create rezervations advert
            RezervationAdverts rezervation_advert = new RezervationAdverts
            {
                adress = advert.adress,
                advert_id = advert.id,
                cancel_time = advert.cancel_time,
                city_id = advert.city_id,
                town_id = advert.town_id,
                title = advert.title,
                description = advert.description,
                entry_time = advert.entry_time,
                exit_time = advert.exit_time,
                latitude = advert.latitude,
                longitude = advert.longitude,
                rezervation_id = rezervation.id,
                created_date = advert.created_date,
                user_id = advert.user_id,
                advert_type_id = advert.advert_type_id
            };
            db.rezervation_adverts.Add(rezervation_advert);

            // create visitors
            _rezervation.visitors.ToList().ForEach(v =>
            {
                RezervationVisitors visitor = new RezervationVisitors
                {
                    created_date = DateTime.Now,
                    fullname = v.fullname,
                    gender = v.gender,
                    rezervation_id = rezervation.id,
                    tc = v.tc
                };
                db.rezervation_visitors.Add(visitor);
            });

            db.SaveChanges();

            // send notifications
            Notifications notify = new Notifications();
            notify.add(advert.user_id, "#" + advert.id + " nolu ilanınz için " + rezervation.days + " günlük rezervasyon talebi!");

            // send sms
            NetGsm.Send(owner.gsm, "#" + advert.id + " nolu ilaniniz icin toplam " + rezervation.days + " günlük (" + rezervation.total_price + " TL) rezervasyon talebi oluşturuldu. - Menkule.com.tr");

            return Ok();
  
        }

        //Approved
        [HttpGet]
        [Authorize(Roles = "owner")]
        [Route("approve/{id}")]
        public IHttpActionResult approve(int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);

            Rezervations rezervation = db.rezervations.Find(id);
            if (rezervation == null) return NotFound();
            if(rezervation.updated_date != null) Responser.Response(HttpStatusCode.Forbidden, "Yetkisiz işlem gerçekleştirildi!");
            RezervationAdverts advert = db.rezervation_adverts.Where(ra => ra.advert_id == rezervation.advert_id).FirstOrDefault();
            if (advert == null) return NotFound();
            if (advert.user_id != user_id) Responser.Response(HttpStatusCode.Forbidden, "Yetkisiz işlem gerçekleştirildi!");

            
            rezervation.state = true;
            rezervation.is_cancel = false;
            rezervation.updated_date = DateTime.Now;

            // available date validation
            var dateList = new List<DateTime>();
            for (DateTime date = rezervation.checkin; date.Date <= rezervation.checkout.Date; date = date.AddDays(1))
            {
                dateList.Add(date);
            }

            // set unavaiable date
                dateList.ForEach(date =>
                {
                    AdvertUnavailableDate advertUnavaiableDate = new AdvertUnavailableDate
                    {
                        advert_id = rezervation.advert_id,
                        day = date.Day,
                        month = date.Month,
                        year = date.Year,
                        fulldate = date,
                        created_date = DateTime.Now,
                        rezervation_id = rezervation.id
                    };
                    db.advert_unavaiable_dates.Add(advertUnavaiableDate);
                });

            db.SaveChanges();

            return Ok();
        }
        //Cancel
        [HttpGet]
        [Authorize(Roles = "owner")]
        [Route("cancel/{id}")]
        public IHttpActionResult cancel(int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);

            Rezervations rezervation = db.rezervations.Find(id);
            if (rezervation == null) return NotFound();
            RezervationAdverts advert = db.rezervation_adverts.Where(ra => ra.advert_id == rezervation.advert_id).FirstOrDefault();
            if (advert == null) return NotFound();
            if (advert.user_id != user_id) Responser.Response(HttpStatusCode.Forbidden, "Yetkisiz işlem gerçekleştirildi!");

            rezervation.state = false;
            rezervation.is_cancel = true;
            rezervation.updated_date = DateTime.Now;

            db.SaveChanges();

            return Ok();
        }

    }
}
