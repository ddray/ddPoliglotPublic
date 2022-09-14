using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ddPoliglotV6.Infrastructure
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> ctx)
        {
            if (ctx.Request.Path.StartsWithSegments("/api") &&
                ctx.Response.StatusCode == (int)HttpStatusCode.OK)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                ctx.Response.Redirect(ctx.RedirectUri);
            }

            return Task.CompletedTask;
        }

        public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> ctx)
        {
            if (ctx.Request.Path.StartsWithSegments("/api") &&
                ctx.Response.StatusCode == (int)HttpStatusCode.OK)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            else
            {
                ctx.Response.Redirect(ctx.RedirectUri);
            }

            return Task.CompletedTask;
        }

        
        //public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        //{
        //    var userPrincipal = context.Principal;

        //    // Look for the LastChanged claim.
        //    var lastChanged = (from c in userPrincipal.Claims
        //                       where c.Type == "LastChanged"
        //                       select c.Value).FirstOrDefault();

        //}
    }
}
