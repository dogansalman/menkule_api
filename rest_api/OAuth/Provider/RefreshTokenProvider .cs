using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rest_api.OAuth.Provider
{
    public class RefreshTokenProvider: IAuthenticationTokenProvider
    {
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            Create(context);
        }
        public void Create(AuthenticationTokenCreateContext context)
        {   
            object owinCollection;
            context.OwinContext.Environment.TryGetValue("Microsoft.Owin.Form#collection", out owinCollection);

            var grantType = ((FormCollection)owinCollection)?.GetValues("grant_type").FirstOrDefault();

            if (grantType == null || grantType.Equals("refresh_token")) return;

            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(2);
           
            context.SetToken(context.SerializeTicket());
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            Receive(context);
        }
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);

            if (context.Ticket == null)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                context.Response.ReasonPhrase = "invalid token";
                return;
            }

            context.SetTicket(context.Ticket);
        }

    }
}