using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using rest_api.Context;

namespace rest_api.OAuth.CustomAttributes.Activated
{
    public class Activated: ActionFilterAttribute
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
                if (!db.users.Any(u => u.id == user_id && u.state == true))
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Lütfen hesabınızı onaylayın.");
            }
        }
    }
}