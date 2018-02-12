using rest_api.OAuth.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Web.Http;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Facebook;
using Microsoft.AspNet.Identity;
using System.Configuration;

[assembly: OwinStartup(typeof(rest_api.OAuth.Startup))]
namespace rest_api.OAuth
{
    // Servis çalışmaya başlarken Owin pipeline'ını ayağa kaldırabilmek için Startup'u hazırlıyoruz.
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static FacebookAuthenticationOptions facebookAuthOptions { get; private set; }
        public static OAuthAuthorizationServerOptions oAuthAuthorizationServerOptions = new OAuthAuthorizationServerOptions()
        {
            AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            TokenEndpointPath = new PathString("/auth"), // token alacağımız path'i belirtiyoruz
            AccessTokenExpireTimeSpan = TimeSpan.FromHours(12),
            AllowInsecureHttp = true,
            Provider = new AuthorizationServerProvider(),
            RefreshTokenProvider = new RefreshTokenProvider()
        };

        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration httpConfiguration = new HttpConfiguration();
            ConfigureOAuth(appBuilder);
            WebApiConfig.Register(httpConfiguration);
            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.UseWebApi(httpConfiguration);
        }

        private void ConfigureOAuth(IAppBuilder appBuilder)
        {
            //use a cookie to temporarily store information about a user logging in with a third party login provider
            appBuilder.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            // AppBuilder'a token üretimini gerçekleştirebilmek için ilgili authorization ayarlarımızı veriyoruz.
            appBuilder.UseOAuthAuthorizationServer(oAuthAuthorizationServerOptions);


            // Facebook auth
            facebookAuthOptions = new FacebookAuthenticationOptions()
            {
                UserInformationEndpoint = "https://graph.facebook.com/v2.8/me?fields=id,name,email,first_name,last_name,picture.width(800).height(800),gender",
                AppId = ConfigurationManager.AppSettings["facebook:clientID"],
                AppSecret = ConfigurationManager.AppSettings["facebook:clientSecret"],
                Provider = new FacebookAuthProvider()
            };
            facebookAuthOptions.Scope.Add("email");
            facebookAuthOptions.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            appBuilder.UseFacebookAuthentication(facebookAuthOptions);



            // Authentication type olarak ise Bearer Authentication'ı kullanacağımızı belirtiyoruz.
            // Bearer token OAuth 2.0 ile gelen standartlaşmış token türüdür.
            // Herhangi kriptolu bir veriye ihtiyaç duymadan client tarafından token isteğinde bulunulur ve server belirli bir expire date'e sahip bir access_token üretir.
            // Bearer token üzerinde güvenlik SSL'e dayanır.
            // Bir diğer tip ise MAC token'dır. OAuth 1.0 versiyonunda kullanılıyor, hem client'a, hemde server tarafına implementasyonlardan dolayı ek maliyet çıkartmaktadır. Bu maliyetin yanı sıra ise Bearer token'a göre kaynak alış verişinin biraz daha güvenli olduğu söyleniyor çünkü client her request'inde veriyi hmac ile imzalayıp verileri kriptolu bir şekilde göndermeleri gerektiği için.
            appBuilder.UseOAuthBearerAuthentication(OAuthBearerOptions);
            



        }

    }
}