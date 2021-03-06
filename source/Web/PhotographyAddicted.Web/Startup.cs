﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhotographyAddicted.Web.Models;
using PhotographyAddicted.Web.Areas.Identity.Data;
using PhotographyAddicted.Data.Common;
using PhotographyAddicted.Data;
using Microsoft.AspNetCore.ResponseCompression;
using PhotographyAddicted.Services.DataServices;
using PhotographyAddicted.Services.Models.Themes;
using PhotographyAddicted.Services.Models.Users;
using Microsoft.AspNetCore.Authentication.Facebook;
using PhotographyAddicted.Services.DataServices.PhotoStoryService;
using PhotographyAddicted.Services.DataServices.PhotoStoryFragmentService;
using PhotographyAddicted.Services.DataServices.PhotoStoryCommentService;
using PhotographyAddicted.Services.DataServices.MessageService;
using PhotographyAddicted.Services.DataServices.ConversationService;
using PhotographyAddicted.Services.DataServices.CommonService;
using PhotographyAddicted.Services.DataServices.ImageService;

namespace PhotographyAddicted.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {                     
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
                        
            services.AddDbContext<PhotographyAddictedContext>(options =>
                     options.UseSqlServer(
                         this.Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<PhotographyAddictedUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<PhotographyAddictedContext>()
                .AddRoles<IdentityRole>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //Add IRepository

            services.AddScoped(typeof(IRepository<>),typeof(DbRepository<>));
            services.AddScoped<IUserService,UserService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IThemeService, ThemeService>();
            services.AddScoped<IImageCommentService, ImageCommentService>();
            services.AddScoped<IThemeCommentService, ThemeCommentService>();
            services.AddScoped<INewService, NewService>();
            services.AddScoped<INewCommentService, NewCommentService>();
            services.AddScoped<IPhotoStoryService, PhotoStoryService>();
            services.AddScoped<IPhotoStoryFragmentService, PhotoStoryFragmentService>();
            services.AddScoped<IPhotoStoryCommentService, PhotoStoryCommentService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication().AddFacebook(facebookOptions => 
            {
                facebookOptions.AppId = Configuration["AppId"];
                facebookOptions.AppSecret = Configuration["AppSecret"];
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
