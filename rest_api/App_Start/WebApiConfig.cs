using System.Web.Http;
using System.Web.Http.Cors;

namespace rest_api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Enable Cors
            var corsConfig = new EnableCorsAttribute("*", "*", "*") { SupportsCredentials = true };
            config.EnableCors(corsConfig);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApia",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
