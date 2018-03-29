using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Collections.Generic;
using rest_api.ModelViews;
using rest_api.Context;
using rest_api.Models;
using rest_api.Libary.NetGsm;
using rest_api.Libary.Mailgun;
using rest_api.Libary.Exceptions.ExceptionThrow;
using rest_api.OAuth.CustomAttributes.Owner;
using rest_api.OAuth.CustomAttributes.Activated;

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
            int user_id = Users.GetUserId(User);

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

            int user_id = Users.GetUserId(User);
            return (
                 from r in db.rezervations
                 where r.owner == user_id
                 from aimg in db.advert_images
                 where aimg.is_default == true && aimg.advert_id == r.advert_id
                 join img in db.images on aimg.image_id equals img.id
                 join ra in db.rezervation_adverts on r.id equals ra.rezervation_id
                 join c in db.cities on ra.city_id equals c.id
                 join t in db.towns on ra.town_id equals t.id
                 join ru in db.users on r.user_id equals ru.id
                 join uimg in db.images on ru.image_id equals uimg.id into j1
                 from j2 in j1.DefaultIfEmpty()
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
                     user = new {
                         name = ru.name,
                         lastname = ru.lastname,
                         identity_no = ru.identity_no,
                         photo = j2.url,
                         gender = ru.gender

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
            try
            {
                int user_id = Users.GetUserId(User);

                var rezervation = (
                               from r in db.rezervations
                               where r.owner == user_id || r.user_id == user_id
                               where r.id == id
                               join ra in db.rezervation_adverts on r.id equals ra.rezervation_id
                               join c in db.cities on ra.city_id equals c.id
                               join t in db.towns on ra.town_id equals t.id
                               join p in db.advert_properties on r.advert_id equals p.advert_id
                               join pos in db.advert_possibilities on r.advert_id equals pos.advert_id
                               join at in db.advert_types on ra.advert_type_id equals at.id

                               select new _RezervationDetails
                               {
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
                                   rezervation_advert = new _RezervationAdvert
                                   {
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
                int _user_id = user_id == rezervation.rezervation_advert.advert.user_id ? rezervation.user_id : rezervation.rezervation_advert.advert.user_id;
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
                if (rezervation.state == false)
                {
                    rezervation.user_information.gsm = rezervation.user_information.gsm.Substring(0, rezervation.user_information.gsm.Length - 4) + "****";
                }

                // rezervation is cancalable
                if (!rezervation.advert_owner)
                {
                    DateTime lastCanceleableDate = rezervation.checkin.AddDays(-rezervation.rezervation_advert.advert.cancel_time);
                    DateTime EndDate = DateTime.Now;
                    int dateDiff = Convert.ToInt32(lastCanceleableDate.Subtract(EndDate).TotalDays) + 1;
                    rezervation.is_cancelable = dateDiff <= 0 || rezervation.is_cancel ? false : true;
                }

                // set state notification
                Notifications notifiy = db.notifications.Where(n => n.user_id == user_id & n.rezervation_id == id).FirstOrDefault();
                if(notifiy != null)
                {
                    notifiy.state = false;
                    db.SaveChanges();
                }
                
                return rezervation;
            }
            catch (Exception ex)
            {
                ExceptionThrow.Throw(ex);
            }

            return NotFound();
        }

        // Create
        [HttpPost]
        [Authorize]
        [Route("")]
        [Activated]
        public IHttpActionResult add([FromBody] _Rezervation _rezervation)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // get user
            int user_id = Users.GetUserId(User);
            Users user = db.users.Where(u => u.id == user_id && u.state == true).FirstOrDefault();
            if (user == null) ExceptionThrow.Throw("Lütfen hesabınızı doğrulayın.", HttpStatusCode.Forbidden);
            
            // existence
            Advert advert = db.advert.Where(a => a.state == true && a.id == _rezervation.advert_id).FirstOrDefault();
            if (advert == null) return NotFound();
            if (db.rezervations.Any(rez => rez.user_id == user_id && rez.advert_id == _rezervation.advert_id && rez.checkin == _rezervation.checkin && rez.checkout == _rezervation.checkout)) ExceptionThrow.Throw("Zaten aynı tarih için bir rezervasyon talebiniz bulunmakta.", HttpStatusCode.Forbidden);

            if(user_id == advert.user_id) ExceptionThrow.Throw("Lütfen farklı bir hesap ile deneyin.", HttpStatusCode.Forbidden);

            // visitor validation
            AdvertProperties properties = db.advert_properties.Where(ap => ap.advert_id == advert.id).FirstOrDefault();
            if (properties == null) return NotFound();

            if(properties.visitor < _rezervation.visitors.Count) ExceptionThrow.Throw("Bu ilan için en fazla. " + properties.visitor + " misafir kabul edilebilmektedir.", HttpStatusCode.Forbidden);

            // get owner
            Users owner = db.users.Where(u => u.id == advert.user_id).FirstOrDefault();
            if (owner == null) return NotFound();

            // rezervation dates list
            var RezervationDates = new List<DateTime>();
            for (DateTime date = _rezervation.checkin; date.Date < _rezervation.checkout.Date; date = date.AddDays(1))
            {
                RezervationDates.Add(date);
            }

            // available dates validation
            List<AdvertAvailableDate> avaiableDates = db.advert_avaiable_dates.Where(aad => aad.advert_id == _rezervation.advert_id).ToList();
            if(avaiableDates.Count > 0)
            {
                RezervationDates.ForEach(rd =>
                {
                    if (avaiableDates.Find(a => a.fulldate == rd) == null) ExceptionThrow.Throw("İlan belirtilen tarih için müsait değil.", HttpStatusCode.Forbidden);
                });
            }
            
            // unavailable dates validation
            if (db.advert_unavaiable_dates.Where(i => i.advert_id == _rezervation.advert_id && RezervationDates.Contains(i.fulldate)).Count() > 0) ExceptionThrow.Throw("İlan belirtilen tarih için müsait değil.", HttpStatusCode.Forbidden);
            
            // min layover date validation
            if ((_rezervation.checkout - _rezervation.checkin).TotalDays  < advert.min_layover ) ExceptionThrow.Throw("Bu ilan için en az " + advert.min_layover + " günlük rezervasyon oluşturulabilir.", HttpStatusCode.Forbidden);

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
                owner = advert.user_id,
                note = _rezervation.note

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
            notify.add(advert.user_id, "#" + advert.id + " nolu ilanınz için " + rezervation.days  + " günlük rezervasyon talebi!", rezervation.id);

            // send sms
            NetGsm.Send(owner.gsm, "#" + advert.id + " nolu ilaniniz icin toplam " + rezervation.days + " günlük (" + rezervation.total_price + " TL) rezervasyon talebi oluşturuldu. - Menkule.com.tr");

            //send mail
            Mailgun.Send("rezervation", new Dictionary<string, object>() { { "fullname", System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.name) + " " + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.lastname) }, { "advert_id", advert.id }, { "checkin", Convert.ToDateTime(rezervation.checkin).ToShortDateString() }, { "checkout", Convert.ToDateTime(rezervation.checkout).ToShortDateString() }, { "days", rezervation.days }, { "price", rezervation.total_price + " TL." } }, owner.email, "Yeni rezervasyon talebi");

            return Ok();

        }

        // Approved
        [HttpPut]
        [Owner]
        [Route("approve/{id}")]
        public object approve(int id)
        {
            int user_id = Users.GetUserId(User);

            Rezervations rezervation = db.rezervations.Find(id);
            if (rezervation == null) return NotFound();


            if(rezervation.updated_date != null)  ExceptionThrow.Throw("Yetkisiz işlem gerçekleştirildi!", HttpStatusCode.Forbidden);

            RezervationAdverts advert = db.rezervation_adverts.Where(ra => ra.advert_id == rezervation.advert_id).FirstOrDefault();
            if (advert == null) return NotFound();

            if (advert.user_id != user_id) ExceptionThrow.Throw("Yetkisiz işlem gerçekleştirildi!", HttpStatusCode.Forbidden);

            Users user = db.users.Find(rezervation.user_id);
            if (user == null) return NotFound();

            // exist rezervation validations
            var exist_rezervations = db.rezervations.Where(r => r.advert_id == advert.advert_id & r.id != rezervation.id && r.state == false & r.is_cancel == false & r.checkin >= rezervation.checkin & r.checkin <= rezervation.checkout).ToList().FirstOrDefault();
            if (exist_rezervations != null) ExceptionThrow.Throw(exist_rezervations, HttpStatusCode.NotImplemented);

            rezervation.state = true;
            rezervation.is_cancel = false;
            rezervation.updated_date = DateTime.Now;

            // available date validation
            var dateList = new List<DateTime>();
            for (DateTime date = rezervation.checkin; date.Date < rezervation.checkout.Date; date = date.AddDays(1))
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

            // send sms
            NetGsm.Send(user.gsm, "#" + rezervation.id + " nolu " + "(" + rezervation.days + " gün - " + rezervation.total_price + " TL) rezervasyonunuz onaylandı. - Menkule.com.tr");

            // send notifications
            Notifications notify = new Notifications();
            notify.add(user.id, "#" + rezervation.id + " nolu rezervasyon talebiniz onaylandı.", rezervation.id);

            // Send email
            Mailgun.Send("approve", new Dictionary<string, object>() { { "fullname", System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.name) + " " + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.lastname) }, { "rezervation_id", rezervation.id }, { "checkin", Convert.ToDateTime(rezervation.checkin).ToShortDateString() }, { "checkout", Convert.ToDateTime(rezervation.checkout).ToShortDateString() }, { "days", rezervation.days }, { "price", rezervation.total_price + " TL." } }, user.email, "Rezervasyon talebi onaylandı.");

            return Ok();
        }
        
        // Force Approved
        [HttpPut]
        [Owner]
        [Route("force/approve/{id}")]
        public object forceApprove([FromBody] _ExistRezervation rezervations, int id)
        {
            int user_id = Users.GetUserId(User);

            // get exist rezervation id
            List<int> rezervations_id = new List<int>();
            rezervations.rezervations.ToList().ForEach(r =>
            {
                if(!db.rezervations.Any(rr => rr.owner == user_id)) ExceptionThrow.Throw("Yetkisiz işlem gerçekleştirildi!", HttpStatusCode.Forbidden);
                rezervations_id.Add(r.id);
            });

            // rezervation validation
            Rezervations rezervation = db.rezervations.Find(id);
            if (rezervation == null) return NotFound();

            // rezervation validation
            if (rezervation.updated_date != null) ExceptionThrow.Throw("Yetkisiz işlem gerçekleştirildi!", HttpStatusCode.Forbidden);

            // rezervation advert validation
            RezervationAdverts advert = db.rezervation_adverts.Where(ra => ra.advert_id == rezervation.advert_id).FirstOrDefault();
            if (advert == null) return NotFound();

            // rezervation owner validation
            if (advert.user_id != user_id) ExceptionThrow.Throw("Yetkisiz işlem gerçekleştirildi!", HttpStatusCode.Forbidden);

            // rezervation user validation
            Users user = db.users.Find(rezervation.user_id);
            if (user == null) return NotFound();

            rezervation.state = true;
            rezervation.is_cancel = false;
            rezervation.updated_date = DateTime.Now;

            // available date validation
            var dateList = new List<DateTime>();
            for (DateTime date = rezervation.checkin; date.Date < rezervation.checkout.Date; date = date.AddDays(1))
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

            db.rezervations.Where(r => r.owner == user_id && rezervations_id.Contains(r.id)).ToList().ForEach(rez =>
            {
                rez.state = false;
                rez.is_cancel = true;
            });

            db.SaveChanges();

            // send sms
            NetGsm.Send(user.gsm, "#" + rezervation.id + " nolu " + "(" + rezervation.days + " gün - " + rezervation.total_price + " TL) rezervasyonunuz onaylandı. - Menkule.com.tr");

            // send notifications
            Notifications notify = new Notifications();
            notify.add(user.id, "#" + rezervation.id + " nolu rezervasyon talebiniz onaylandı.", rezervation.id);

            // Send email
            Mailgun.Send("approve", new Dictionary<string, object>() { { "fullname", System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.name) + " " + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.lastname) }, { "rezervation_id", rezervation.id }, { "checkin", Convert.ToDateTime(rezervation.checkin).ToShortDateString() }, { "checkout", Convert.ToDateTime(rezervation.checkout).ToShortDateString() }, { "days", rezervation.days }, { "price", rezervation.total_price + " TL." } }, user.email, "Rezervasyon talebi onaylandı.");

            return Ok();
        }

        // cancel
        [HttpGet]
        [Route("cancel/{id}")]
        [Authorize]
        public IHttpActionResult cancel(int id)
        {
            int user_id = Users.GetUserId(User);

            Rezervations rezervation = db.rezervations.Find(id);
            if (rezervation == null) return NotFound();
            if (rezervation.is_cancel) ExceptionThrow.Throw("Rezervasyon daha önce iptal edilmiş.", HttpStatusCode.Forbidden);

            RezervationAdverts advert = db.rezervation_adverts.Where(ra => ra.advert_id == rezervation.advert_id).FirstOrDefault();
            if (advert == null) return NotFound();

            bool is_cancel = rezervation.is_cancel;

            rezervation.state = false;
            rezervation.is_cancel = true;
            rezervation.updated_date = DateTime.Now;

            
            if (advert.user_id == user_id) {
                Users user = db.users.Find(rezervation.user_id);
                if (user == null) return NotFound();
                db.SaveChanges();

                // Add notify
                Notifications notify = new Notifications();
                notify.add(user.id, "#" + rezervation.id + " nolu " + rezervation.days + " günlük rezervasyon talebi iptal edildi!", rezervation.id);

                // Send sms
                NetGsm.Send(user.gsm, "#" + rezervation.id + " nolu " + "(" + rezervation.days + " gün - " + rezervation.total_price + " TL) rezervasyonunuz iptal edildi. - Menkule.com.tr");

                // Send email
                Mailgun.Send("cancel", new Dictionary<string, object>() { { "fullname", System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.name) + " " + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.lastname) }, { "rezervation_id", rezervation.id }, { "checkin", Convert.ToDateTime(rezervation.checkin).ToShortDateString() }, { "checkout", Convert.ToDateTime(rezervation.checkout).ToShortDateString() }, { "days", rezervation.days }, { "price", rezervation.total_price + " TL." } }, user.email, "Rezervasyon talebi iptal edildi.");
            }
            

            // Delete unavaiable dates
            db.advert_unavaiable_dates.RemoveRange(db.advert_unavaiable_dates.Where(uad => uad.advert_id == rezervation.advert_id && uad.rezervation_id == id));

            if (user_id == rezervation.user_id)
            {
                DateTime lastCanceleableDate = rezervation.checkin.AddDays(-advert.cancel_time);
                DateTime EndDate = DateTime.Now;
                int dateDiff = Convert.ToInt32(lastCanceleableDate.Subtract(EndDate).TotalDays) + 1;
                if (!(dateDiff <= 0 || is_cancel ? false : true)) ExceptionThrow.Throw("Bu rezervasyon iptal süresi dışındadır.", HttpStatusCode.Forbidden);
                db.SaveChanges();

                Users advert_owner = db.users.Find(rezervation.owner);
                if(advert_owner != null)
                {
                    // Send sms
                    NetGsm.Send(advert_owner.gsm, "#" + rezervation.id + " nolu " + "(" + rezervation.days + " gün - " + rezervation.total_price + " TL) rezervasyon talebi iptal edildi. - Menkule.com.tr");

                    // Add Notify
                    Notifications notify = new Notifications();
                    notify.add(advert_owner.id, "#" + rezervation.id + " nolu " + rezervation.days + " günlük rezervasyon talebi iptal edildi!", rezervation.id);

                    // Send email
                    Mailgun.Send("cancel", new Dictionary<string, object>() { { "fullname", System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(advert_owner.name) + " " + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(advert_owner.lastname) }, { "rezervation_id", rezervation.id }, { "checkin", Convert.ToDateTime(rezervation.checkin).ToShortDateString() }, { "checkout", Convert.ToDateTime(rezervation.checkout).ToShortDateString() }, { "days", rezervation.days }, { "price", rezervation.total_price + " TL." } }, advert_owner.email, "Rezervasyon talebi iptal edildi.");

                }
                
            }

            return Ok();
        }

    }
}
