using System.Web.Http;

namespace rest_api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Enable Cors
            // Cors setting enabled in Authorization Servers
            
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
