using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Google.Cloud.Firestore;
using FirebaseAdmin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using GrooverAdm.Data;
using GrooverAdm.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using GrooverAdm.Mappers.Interface;
using GrooverAdm.Business.Services.Places;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.DataAccess.Firestore.PlacesDao;
using GrooverAdm.Mappers.Firestore;
using GrooverAdm.Business.Services.Playlist;
using GrooverAdm.DataAccess.Firestore.Dao;
using GrooverAdm.Business.Services.Song;
using GrooverAdm.Business.Services;
using System.IO;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using NSwag;
using NSwag.Generation.Processors.Security;
using GrooverAdm.Business.Services.Rating;

namespace GrooverAdm
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            //
            //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            //
            //services.AddIdentityServer()
            //    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
            //
            //services.AddAuthentication()
            //    .AddIdentityServerJwt();


            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.IncludeErrorDetails = true;
                    option.Authority = "https://securetoken.google.com/groover-3b82a";
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "https://securetoken.google.com/groover-3b82a",
                        ValidateAudience = true,
                        ValidAudience = "groover-3b82a",
                        ValidateLifetime = true,
                    };
                });

            services.AddControllersWithViews();
            services.AddRazorPages();


            services.AddSingleton(typeof(FirebaseApp), FirebaseApp.Create());

            services.AddSingleton(typeof(FirestoreDb), FirestoreDb.Create("groover-3b82a"));
            AddPlaceServices(services);
            AddPlaylistServices(services);
            AddSongServices(services);
            AddUserServices(services);
            AddRatingServices(services);
            services.AddScoped<SpotifyService>();
            services.AddScoped<LastFmService>();
            services.AddScoped<RecommendationService>();

            services.AddOpenApiDocument(c =>
            {
                c.PostProcess = d =>
                {
                    d.Info.Version = "v1";
                    d.Info.Title = "Groover API";
                };
                c.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("JWT",
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Type into the textbox: Bearer {your JWT token}."
                    }));
                c.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));

            });
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddRouting(opt => opt.LowercaseUrls = true);
        }

        private static void AddUserServices(IServiceCollection services)
        {
            services.AddScoped<GrooverAdm.Business.Services.User.IUserService, GrooverAdm.Business.Services.User.UserService>();
            services.AddScoped<IUserDao<GrooverAdm.DataAccess.Firestore.Model.User>, UserFirestoreDao>();
            services.AddScoped<IUserMapper<GrooverAdm.DataAccess.Firestore.Model.User>, UserMapper>();
        }

        private static void AddPlaceServices(IServiceCollection services)
        {
            services.AddScoped<IPlacesService, PlacesService>();
            services.AddScoped<IPlacesDao<GrooverAdm.DataAccess.Firestore.Model.Place>, PlacesFirestoreDao>();
            services.AddScoped<IPlaceMapper<GrooverAdm.DataAccess.Firestore.Model.Place>, PlaceMapper>();
        }

        private static void AddPlaylistServices(IServiceCollection services)
        {
            services.AddScoped<IPlaylistService, PlaylistService>();
            services.AddScoped<IPlaylistDao<GrooverAdm.DataAccess.Firestore.Model.Playlist>, PlaylistFirestoreDao>();
            services.AddScoped<IPlaylistMapper<GrooverAdm.DataAccess.Firestore.Model.Playlist>, PlaylistMapper>();
        }

        private static void AddSongServices(IServiceCollection services)
        {
            services.AddScoped<ISongService, SongService>();
            services.AddScoped<ISongDao<GrooverAdm.DataAccess.Firestore.Model.Song>, SongFirestoreDao>();
            services.AddScoped<ISongMapper<GrooverAdm.DataAccess.Firestore.Model.Song>, SongMapper>();
        }

        private static void AddRatingServices(IServiceCollection services)
        {
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IRatingDao<GrooverAdm.DataAccess.Firestore.Model.Rating>, RatingFirestoreDao>();
            services.AddScoped<IRatingMapper<GrooverAdm.DataAccess.Firestore.Model.Rating>, RatingMapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseOpenApi();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUi3(c =>
            {
            });


            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
