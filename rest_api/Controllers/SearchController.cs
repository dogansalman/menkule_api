using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using rest_api.Context;
using rest_api.Models;
using rest_api.ModelViews;

namespace rest_api.Controllers
{


    [RoutePrefix("search")]
    public class SearchController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        [HttpPost]
        [Route("")]
        public object find([FromBody] _LatitudeLongitude cordinates)
        {
            return (from a in db.advert
                    from aimg in db.advert_images
                    where a.latitude <= cordinates.lat && a.longitude <= cordinates.lng
                    where aimg.is_default == true && aimg.advert_id == a.id
                    join c in db.cities on a.city_id equals c.id
                    join t in db.towns on a.town_id equals t.id
                    join p in db.advert_properties on a.id equals p.advert_id
                    join po in db.advert_possibilities on a.id equals po.advert_id
                    join at in db.advert_types on a.advert_type_id equals at.id
                    join img in db.images on aimg.image_id equals img.id
                    join u in db.users on a.user_id equals u.id
                    join uimg in db.images on u.image_id equals uimg.id into j1
                    from j2 in j1.DefaultIfEmpty()
   
                    select new _AdvertSearch {
                        adress = a.adress,
                        title = a.title,
                        latitude = a.latitude,
                        longitude = a.longitude,
                        comment_size = (db.advert_comments.Where(cmt => cmt.advert_id == a.id).Count()),
                        cancel_time = a.cancel_time,
                        min_layover = a.min_layover,
                        entry_time = a.entry_time,
                        exit_time = a.exit_time,
                        score = a.score,
                        views = a.views,
                        price = a.price,
                        city = c,
                        town = t,
                        possibility = po,
                        properties = p,
                        advert_type = at,
                        available_date = (db.advert_avaiable_dates.Where(ad => ad.advert_id == a.id)).ToList(),
                        unavaiable_date = (db.advert_unavaiable_dates.Where(aud => aud.advert_id == a.id)).ToList(),
                        state = a.state,
                        created_date = a.created_date,
                        user = new { id = u.id, fullname = u.name + " " + u.lastname, photo = j2.url }
                    }).ToList();
        }
    }
}
