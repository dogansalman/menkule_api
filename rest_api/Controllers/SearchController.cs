using System.Linq;
using System.Web.Http;
using rest_api.Context;
using rest_api.ModelViews;
using System;

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
            

            double lat_max = Convert.ToDouble(cordinates.lat.ToString().Split('-')[0].Replace('.',','));
            double lat_min = Convert.ToDouble(cordinates.lat.ToString().Split('-')[1].Replace('.', ','));
            double lng_max = Convert.ToDouble(cordinates.lng.ToString().Split('-')[0].Replace('.', ','));
            double lng_min = Convert.ToDouble(cordinates.lng.ToString().Split('-')[1].Replace('.', ','));

            return (from a in db.advert
                    where (a.latitude <= lat_max & a.longitude <= lng_max) & (a.latitude >= lat_min & a.longitude >= lng_min)
                    join c in db.cities on a.city_id equals c.id
                    join t in db.towns on a.town_id equals t.id
                    join p in db.advert_properties on a.id equals p.advert_id
                    join po in db.advert_possibilities on a.id equals po.advert_id
                    join at in db.advert_types on a.advert_type_id equals at.id
                    join u in db.users on a.user_id equals u.id
                    join uimg in db.images on u.image_id equals uimg.id into j1
                    from j2 in j1.DefaultIfEmpty()
                    where a.state == true
                    select new
                    {
                        id = a.id,
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
                        images = (
                            from ad in db.advert
                            where ad.id == a.id
                            from aimg in db.advert_images
                            where aimg.is_default == true && aimg.advert_id == ad.id
                            join img in db.images on aimg.image_id equals img.id
                            select new { image = img }
                            ).FirstOrDefault(),
                        city = c,
                        town = t,
                        possibility = po,
                        properties = p,
                        advert_type = at,
                        available_date = (
                            from ad in db.advert_avaiable_dates
                            where ad.advert_id == a.id
                            group ad by ad.uniq into g
                            select new
                            {
                                from_date = g.Min(e => e.fulldate),
                                to_date = g.Max(e => e.fulldate),
                                uniq = g.Select(a => a.uniq).FirstOrDefault()
                            }
                            ).ToList(),
                        unavaiable_date = (db.advert_unavaiable_dates.Where(aud => aud.advert_id == a.id)).ToList(),
                        state = a.state,
                        created_date = a.created_date,
                        user = new { id = u.id, fullname = u.name + " " + u.lastname, photo = j2.url }
                    }).ToList();
        }
    }
}
