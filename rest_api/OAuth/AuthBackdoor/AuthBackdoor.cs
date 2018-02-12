using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using rest_api.Models;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Web;

namespace rest_api.OAuth.AuthBackdoor
{
    public static class AuthBackdoor
    {
      public static AuthenticationProperties auth(Users user, HttpRequestMessage req)
        {
            if (user == null || req == null) return null;

            // set token expiration time
            var tokenExpiration = TimeSpan.FromHours(12);

            // create new claims identity
            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.name));
            identity.AddClaim(new Claim("user_id", user.id.ToString()));

            // set auth properties
            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            // new ticket
            var ticket = new AuthenticationTicket(identity, props);
            
            // create auth context
            var context = new Microsoft.Owin.Security.Infrastructure.AuthenticationTokenCreateContext(
                    req.GetOwinContext(),
                    Startup.OAuthBearerOptions.AccessTokenFormat, ticket);

            // get refresh token
            var refresh_token = Startup.oAuthAuthorizationServerOptions.RefreshTokenFormat.Protect(ticket);
            props.Dictionary.Add("refresh_token", refresh_token);
            
            // get access token
            var access_token =  Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            props.Dictionary.Add("access_token", access_token);

            return props;
        }
    }
}