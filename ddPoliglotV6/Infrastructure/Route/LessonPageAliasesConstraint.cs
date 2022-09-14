using ddPoliglotV6.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddPoliglotV6.Infrastructure.Route
{
    public class LessonPageAliasesConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object value;
            if (values.TryGetValue(parameterName, out value) && value != null)
            {
                var cache = (IMemoryCache)httpContext.RequestServices.GetService(typeof(IMemoryCache));
                var key = "LessonPageNames";
                List<string> result;
                if (!cache.TryGetValue<List<string>>(key, out result))
                {
                    var dbContext = (ddPoliglotDbContext)httpContext.RequestServices.GetService(typeof(ddPoliglotDbContext));
                    result = dbContext.Lessons.Select(x => x.PageName).Where(x => x != null && x != "").Distinct().ToList();
                    cache.Set(key, result, TimeSpan.FromMinutes(3));
                }

                return result.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
