using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Net.Http;
using System.Net;
using rest_api.Context;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

namespace rest_api.OAuth.CustomAttributes.Owner
{
    public class Owner: ActionFilterAttribute
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            if (HttpContext.Current.User == null || HttpContext.Current.User.Identity == null)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Lütfen oturum açın.");
            }

            var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            using (var db = new DatabaseContext())
            {
                if (!db.users.Any(u => u.id == user_id && u.ownershiping == true))
                    actionContext.Response =  actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Ev sahipliğinizi onaylayın.");
            }
        }



    }
}