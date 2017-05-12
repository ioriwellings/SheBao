using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace App
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            //自定义路由 wxj 20150818
           // config.Routes.MapHttpRoute(
           //    name: "DefaultAreaApi",
           //    routeTemplate: "api/{area}/{controller}/{action}/{id}",
           //    defaults: new { id = RouteParameter.Optional }
           //);

            //自定义路由 wp 20150720
            config.Routes.MapHttpRoute(
               name: "InsusApi",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
        }
    }
}
