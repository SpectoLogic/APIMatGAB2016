using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Calc
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            // apollak: Added action routing for multiple gets
            config.Routes.MapHttpRoute(
                name: "CalcApi",
                routeTemplate: "api/Calc/{action}",
                defaults: new { id = RouteParameter.Optional,
                                Controller = "Calc" }
                // Changed to a fix route because otherwise Swagger picks up the GET action in the Values controller which creates
                // duplicated operations
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
