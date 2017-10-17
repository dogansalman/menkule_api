using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using System.Security.Claims;
using rest_api.Context;
using rest_api.Models;
using System.Linq;
using rest_api.Libary.Bcrypt;

namespace rest_api.OAuth.Provider
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        // OAuthAuthorizationServerProvider sınıfının client erişimine izin verebilmek için ilgili ValidateClientAuthentication metotunu override ediyoruz.
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
           context.Validated();
        }

        DatabaseContext db = new DatabaseContext();

        // OAuthAuthorizationServerProvider sınıfının kaynak erişimine izin verebilmek için ilgili GrantResourceOwnerCredentials metotunu override ediyoruz.
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // CORS ayarlarını set ediyoruz.
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            // Kullanıcının access_token alabilmesi için gerekli validation işlemlerini yapıyoruz.
            string pass = Bcrypt.hash(context.Password);
            Users usr = db.users.Where(u => u.email == context.UserName && u.password == pass).FirstOrDefault();
            if (usr != null)
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("user_id", usr.id.ToString()));

                //Role eklemek için
                if(usr.ownershiping) identity.AddClaim(new Claim(ClaimTypes.Role, "owner"));
                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "Kullanıcı adı veya şifre yanlış.");
            }
        }
    }
}