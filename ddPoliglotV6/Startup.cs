using ddPoliglotV6.BL.Constants;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using ddPoliglotV6.Infrastructure;
using ddPoliglotV6.Infrastructure.Route;
using ddPoliglotV6.Infrastructure.Services;
using ddPoliglotV6.Resources;
using ddPoliglotV6.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ddPoliglotV6
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var environment = services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>();

            services.AddDataProtection()
                    .SetApplicationName($"ddPoliglotV6")
                    .PersistKeysToFileSystem(new DirectoryInfo($@"{environment.WebRootPath}\keys"));

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                 {
                    new CultureInfo("en"),
                    new CultureInfo("sk"),
                    new CultureInfo("ru"),
                };
                options.DefaultRequestCulture = new RequestCulture("ru-RU");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Clear();
                options.SetDefaultCulture("ru");
                options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider
                {
                    Options = options
                });
            });

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = false;
                options.ConstraintMap.Add("values", typeof(ValuesConstraint));
                options.ConstraintMap.Add("LessonsFolderAliases", typeof(LessonsFolderAliasesConstraint));
                options.ConstraintMap.Add("LessonPageAliases", typeof(LessonPageAliasesConstraint));
            });

            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            if (environment.IsDevelopment())
            {
                services.AddCors(o => o.AddPolicy(name: "MyAllowSpecificOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                           // .WithOrigins("https://localhost:58279");
                }));
            }
            else
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(name: "MyAllowSpecificOrigins", builder =>
                    {
                        builder.WithOrigins(
                            "https://ddpoliglot.com",
                            "https://www.ddpoliglot.com",
                            "http://ddpoliglot.com",
                            "http://www.ddpoliglot.com"
                        ).AllowAnyHeader();
                    }
                    );
                });
            }

            //**
            services.AddDbContext<ddPoliglotDbContext>();

            //// First AddSession()
            //services.AddSession(options =>
            //{
            //    options.Cookie.Name = "MyDDPoliglotSession";
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.Expiration = TimeSpan.FromHours(3);
            //});

            //**
            services.AddDefaultIdentity<ApplicationUser>(
                    options => { 
                        options.SignIn.RequireConfirmedAccount = false;
                        options.Password.RequireDigit = false;
                        options.Password.RequiredLength = 4;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireLowercase = false;
                    }
                )
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ddPoliglotDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.EventsType = typeof(CustomCookieAuthenticationEvents);
                options.Cookie.Name = "MyDDPoliglotCookie";
                options.ExpireTimeSpan = TimeSpan.FromDays(27);
                options.SlidingExpiration = true;
            });

            services.AddSingleton<RoutersTree>();

            services.AddScoped<CustomCookieAuthenticationEvents>();
            services.AddSingleton<CommonLocalizationService>();

            var secret = Configuration.GetSection("JwtConfig").GetSection("secret").Value;
            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true
                };
            })
            .AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration.GetSection("FacebookOptions").GetSection("AppId").Value; 
                facebookOptions.AppSecret = Configuration.GetSection("FacebookOptions").GetSection("AppSecret").Value;
            })
            .AddGoogle(options =>
            {
                options.ClientId = Configuration.GetSection("GoogleOptions").GetSection("ClientId").Value; 
                options.ClientSecret = Configuration.GetSection("GoogleOptions").GetSection("ClientSecret").Value;
            });

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var assemblyName = new AssemblyName(typeof(CommonResources).GetTypeInfo().Assembly.FullName);
                    return factory.Create(nameof(CommonResources), assemblyName.Name);
                };
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // requires
            // using Microsoft.AspNetCore.Identity.UI.Services;
            // using WebPWrecover.Services;
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddRazorPages(options =>
            {
                options.Conventions.Add(new CultureTemplatePageRouteModelConvention());
            })
            .AddRazorRuntimeCompilation();

            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            IServiceProvider services,
            ddPoliglotDbContext dbContext,
            IMemoryCache memoryCache
            )
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/html";

                    await context.Response.WriteAsync("<html lang=\"en\"><body>");
                    await context.Response.WriteAsync("ERROR!<br><br>");

                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();

                    // save exeption to db
                    using (var ent = new ddPoliglotDbContext(Configuration))
                    {
                        var log = new Log()
                        {
                            Created = DateTime.Now,
                            Description = exceptionHandlerPathFeature.Error.StackTrace.ToString()
                            + exceptionHandlerPathFeature.Error.InnerException?.StackTrace?.ToString() ?? "",
                            Message = exceptionHandlerPathFeature.Error.Message.ToString() + exceptionHandlerPathFeature.Error.InnerException?.Message?.ToString() ?? "",
                            Name = "error",
                            Type = 0,
                            UserID = Guid.NewGuid() //Guid.Parse(context.User.f.Identity..FindFirst(ClaimTypes.id).Value)
                        };

                        ent.Add(log);
                        ent.SaveChanges();
                    }

                    await context.Response.WriteAsync($"{exceptionHandlerPathFeature.Error.Message.ToString()}<br><br>");
                    await context.Response.WriteAsync($"{exceptionHandlerPathFeature.Error.InnerException?.Message?.ToString() ?? ""}<br><br>");
                    await context.Response.WriteAsync($"{exceptionHandlerPathFeature.Error.StackTrace.ToString()}<br><br>");

                    await context.Response.WriteAsync("<a href=\"/\">Home</a><br>");
                    await context.Response.WriteAsync("</body></html>");
                    await context.Response.WriteAsync(new string(' ', 512)); // IE padding
                });
            });

            //app.UseReferrerPolicy(opts => opts.NoReferrer());

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            var localizationOptions = app.ApplicationServices
                .GetService<IOptions<RequestLocalizationOptions>>().Value;

            app.UseRequestLocalization(localizationOptions);

            app.UseCors("MyAllowSpecificOrigins");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

            });

            app.Map("/xx/lessons-fabric", app1 =>
            {
                app1.UseSpa(spa =>
                {
                    if (env.IsDevelopment())
                    {
                        spa.Options.SourcePath = "../ClientAppV61";
                        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");

                        // spa.UseAngularCliServer(npmScript: "start");
                    }
                    else
                    {
                        spa.Options.SourcePath = "wwwroot/xx/lessons-fabric";
                        spa.Options.DefaultPage = "/xx/lessons-fabric/index.html";
                    }
                });
            });

            dbContext.Database.Migrate();
            CreateLanguages(dbContext).Wait();
            CreateRoles(services).Wait();
        }

        private async Task CreateLanguages(ddPoliglotDbContext dbContext)
        {
            if (!dbContext.Languages.AsNoTracking().Any(x=> x.LanguageID == 1 && x.CodeFull == "en-US"))
            {
                await dbContext.Languages.AddAsync(new Language()
                {
                    Code = "en",
                    CodeFull = "en-US",
                    Name = "English(US, GB)",
                    ShortName = "en"
                });

                await dbContext.SaveChangesAsync();
                await dbContext.Languages.AddAsync(new Language()
                {
                    //LanguageID = 2,
                    Code = "ru",
                    CodeFull = "ru-RU",
                    Name = "Russian",
                    ShortName = "ru"
                });
                await dbContext.SaveChangesAsync();
                await dbContext.Languages.AddAsync(new Language()
                {
                    //LanguageID = 3,
                    Code = "de",
                    CodeFull = "de-DE",
                    Name = "Deutsch(Deutschland)",
                    ShortName = "de"
                });
                await dbContext.SaveChangesAsync();
                await dbContext.Languages.AddAsync(new Language()
                {
                    //LanguageID = 4,
                    Code = "sk",
                    CodeFull = "sk-SK",
                    Name = "Slovenčina(Slovensko)",
                    ShortName = "sk"
                });
                await dbContext.SaveChangesAsync();
            }
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await RoleManager.RoleExistsAsync(RolesConstants.SuperAdminRoleName))
            {
                await RoleManager.CreateAsync(new IdentityRole(RolesConstants.SuperAdminRoleName));
            }

            if (!await RoleManager.RoleExistsAsync(RolesConstants.AdminRoleName))
            {
                await RoleManager.CreateAsync(new IdentityRole(RolesConstants.AdminRoleName));
            }

            if (!await RoleManager.RoleExistsAsync(RolesConstants.LessonsMakerRoleName))
            {
                await RoleManager.CreateAsync(new IdentityRole(RolesConstants.LessonsMakerRoleName));
            }

            if (!await RoleManager.RoleExistsAsync(RolesConstants.UserRoleName))
            {
                await RoleManager.CreateAsync(new IdentityRole(RolesConstants.UserRoleName));
            }

            ApplicationUser user = await UserManager.FindByEmailAsync("drytest1@gmail.com");
            if (user != null)
            {
                if (!(await UserManager.IsInRoleAsync(user, RolesConstants.SuperAdminRoleName)))
                {
                    await UserManager.AddToRoleAsync(user, RolesConstants.SuperAdminRoleName);
                }
            }

            user = await UserManager.FindByEmailAsync("DimaDry_22@gmail.com");
            if (user != null)
            {
                if (!(await UserManager.IsInRoleAsync(user, RolesConstants.SuperAdminRoleName)))
                {
                    await UserManager.AddToRoleAsync(user, RolesConstants.SuperAdminRoleName);
                }
            }
        }
    }
}
