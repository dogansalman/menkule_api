using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace rest_api.OAuth.Unauthorized
{
    public class AuthorizeAttr : AuthorizeAttribute
    {
        
       protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
                Content = new StringContent("You are unauthorized to access this resource")
            };
        }
     

    }
}