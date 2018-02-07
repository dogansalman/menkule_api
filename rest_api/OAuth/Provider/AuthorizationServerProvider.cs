using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using System.Security.Claims;
using rest_api.Context;
using rest_api.Models;
using System.Linq;
using rest_api.Libary.Bcrypt;
using Microsoft.Owin;
using System;

namespace rest_api.OAuth.Provider
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        // OAuthAuthorizationServerProvider sınıfının client erişimine izin verebilmek için ilgili ValidateClientAuthentication metotunu override ediyoruz.
        public override  Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
           context.Validated();
           return Task.FromResult<object>(null);
        }

        DatabaseContext db = new DatabaseContext();
 
        // OAuthAuthorizationServerProvider sınıfının kaynak erişimine izin verebilmek için ilgili GrantResourceOwnerCredentials metotunu override ediyoruz.
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            // Kullanıcının access_token alabilmesi için gerekli validation işlemlerini yapıyoruz.
            string pass = Bcrypt.hash(context.Password);
            Users usr = db.users.Where(u => u.email == context.UserName && u.password == pass).FirstOrDefault();
            if (usr == null)
            {
                context.SetError("invalid_grant", "Kullanıcı adı veya şifre yanlış.");
                return;
            }
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("user_id", usr.id.ToString()));

            //Role eklemek için
            if (usr.ownershiping) identity.AddClaim(new Claim(ClaimTypes.Role, "owner"));
            context.Validated(identity);
        }

        public override Task MatchEndpoint(OAuthMatchEndpointContext context)
        {
            SetCORSPolicy(context.OwinContext);
            if (context.Request.Method == "OPTIONS")
            {
                
                context.RequestCompleted();
                return Task.FromResult(0);
            }
            return base.MatchEndpoint(context);
        }
  
        private void SetCORSPolicy(IOwinContext context)
        {
            string allowedUrls = "http://localhost:9000, http://www.menkule.com.tr, https://www.menkule.com.tr";

            if (!String.IsNullOrWhiteSpace(allowedUrls))
            {
                var list = allowedUrls.Split(',');
                if (list.Length > 0)
                {
                    string origin = context.Request.Headers.Get("Origin");
                    var found = list.Where(item => item == origin).Any();
                    //Allow All for hybrid app
                    context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "*" });
                    //if (found) context.Response.Headers.Add("Access-Control-Allow-Origin",new string[] { found });
                }
            }
            context.Response.Headers.Add("Access-Control-Allow-Headers",
                                   new string[] { "Authorization", "Content-Type" });
            context.Response.Headers.Add("Access-Control-Allow-Methods",
                                   new string[] { "OPTIONS", "POST", "PUT", "DELETE", "GET" });

        }
    }
}