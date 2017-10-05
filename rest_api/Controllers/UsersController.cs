using System.Linq;
using System.Net;
using System.Web.Http;
using rest_api.Models;
using rest_api.Context;
using rest_api.ModelViews;
using System.Security.Claims;

namespace rest_api.Controllers
{
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
       
        // Add
        [HttpPost]
        [Route("")]
        public IHttpActionResult add([FromBody] Users user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (db.users.Any(u => u.email == user.email)) ResponserController.Response(HttpStatusCode.Forbidden, "e-posta adresi kullanılmaktadır.");
            if (db.users.Any(u => u.gsm == user.gsm)) ResponserController.Response(HttpStatusCode.Forbidden, "gsm no kullanılmaktadır.");
            
            db.users.Add(user);
            db.SaveChanges();
            return Ok(new UsersMV
            {
                name = user.name,
                lastname = user.lastname,
                email = user.email,
                gsm = user.gsm,
                gender = user.gender,
                photo = null,
                ownershiping = user.ownershiping,
                state = user.state,
                email_state = user.email_state,
                gsm_state = user.gsm_state,
                created_date = user.created_date
            });
        }

        // Get
        [HttpGet]
        [Authorize]
        [Route("")]
        public IHttpActionResult get()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var user = db.users.GroupJoin(
                 db.images,
                 u => u.image_id,
                 i => i.id,
                 (u, i) => new { user = u, image = i }
                 )
                 .Where(u => u.user.id == int.Parse(claimsIdentity.FindFirst("user_id").Value))
                 .SelectMany(userWithImage =>
                 userWithImage.image.DefaultIfEmpty(),
                 (u, i) => new UsersMV
                 {
                     name = u.user.name,
                     lastname = u.user.lastname,
                     email = u.user.email,
                     gsm = u.user.gsm,
                     gender = u.user.gender,
                     photo = i.url,
                     ownershiping = u.user.ownershiping,
                     state = u.user.state,
                     email_state = u.user.email_state,
                     gsm_state = u.user.gsm_state,
                     created_date = u.user.created_date
                 }
                 ).FirstOrDefault();
            if (user == null) return NotFound();
            return Ok(user);
        }

    }
}
