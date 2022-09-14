using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddPoliglotV6.Infrastructure.Route
{
    public class LessonsFolderAliasesConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object value;
            if (values.TryGetValue(parameterName, out value) && value != null)
            {
                return RoutersTree.GetLessonsFolderPagesNames().Contains(value.ToString(), StringComparer.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
