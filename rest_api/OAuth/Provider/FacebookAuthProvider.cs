using Microsoft.Owin.Security.Facebook;
using System.Security.Claims;
using System.Threading.Tasks;

namespace rest_api.OAuth.Provider
{
    public class FacebookAuthProvider: FacebookAuthenticationProvider
    {
        public override Task Authenticated(FacebookAuthenticatedContext context)
        {
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            foreach (var claim in context.User)
            {
                var claimType = string.Format("urn:facebook:{0}", claim.Key);
                string claimValue = claim.Value.ToString();
                if (!context.Identity.HasClaim(claimType, claimValue))
                    context.Identity.AddClaim(new Claim(claimType, claimValue, "XmlSchemaString", "Facebook"));
            }
            return Task.FromResult<object>(null);
        }
    }
}