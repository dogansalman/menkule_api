using System.Web.Http;
using System.Web.Http.Cors;

namespace rest_api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var corsConfig = new EnableCorsAttribute("*", "*", "*");
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
