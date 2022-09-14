using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddPoliglotV6.Infrastructure.Route
{
    public class ValuesConstraint : IRouteConstraint
    {
        private readonly string[] validOptions;
        public ValuesConstraint(string options)
        {
            validOptions = options.Split('|');
            System.Diagnostics.Debug.Print("options {0}", options);
        }

        public bool Match(HttpContext httpContext, IRouter route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object value;
            if (values.TryGetValue(parameterName, out value) && value != null)
            {
                System.Diagnostics.Debug.Print("check value {0}", value);
                return validOptions.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
