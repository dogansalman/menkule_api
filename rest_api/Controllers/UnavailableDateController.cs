using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using rest_api.Models;
using rest_api.Context;
using System.Security.Claims;


namespace rest_api.Controllers
{
    [RoutePrefix("dates")]
    public class UnavailableDateController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        [Route("unavailable/{id}")]
        [HttpPut]
        [Authorize(Roles = "owner")]
        public IHttpActionResult setDates([FromBody] ICollection<AdvertUnavailableDate> dates, int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            if (!db.advert.Any(a => a.user_id == user_id && a.id == id)) return NotFound();

            db.advert_unavaiable_dates.RemoveRange(db.advert_unavaiable_dates.Where(uad => uad.advert_id == id));

            if ( dates != null)
            {
                dates.ToList().ForEach(date =>
                {
                    AdvertUnavailableDate aud = new AdvertUnavailableDate
                    {
                        advert_id = id,
                        created_date = DateTime.Now,
                        day = date.day,
                        month = date.month,
                        year = date.year,
                        fulldate = date.fulldate,
                        rezervation_id = date.rezervation_id
                    };
                    db.advert_unavaiable_dates.Add(aud);
                });
            }
            db.SaveChanges();
            return Ok(dates);
        }
    }
}
