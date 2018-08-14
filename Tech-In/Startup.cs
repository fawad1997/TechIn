using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tech_In.Data;
using Tech_In.Extensions;
using Tech_In.Models;
using Tech_In.Services;

namespace Tech_In
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                //facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                //facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                facebookOptions.AppId = Configuration.GetConnectionString("FacebookAppId");
                facebookOptions.AppSecret = Configuration.GetConnectionString("FacebookAppSecret");
                //facebookOptions.Scope.Add("public_profile");
                //facebookOptions.Scope.Add("user_birthday");
                //facebookOptions.Scope.Add("user_location");
                //facebookOptions.Scope.Add("user_gender");
                //facebookOptions.Scope.Add("user_hometown");
                //facebookOptions.Fields.Add("name");
                //facebookOptions.Fields.Add("birthday");
                //facebookOptions.Fields.Add("hometown");
                //facebookOptions.Fields.Add("gender");
            });

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                //googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                //googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                googleOptions.ClientId = Configuration.GetConnectionString("GoogleClientId");
                googleOptions.ClientSecret = Configuration.GetConnectionString("GoogleClientSecret");
            });

            //services.AddAuthentication().AddLinkedIn(options =>
            //{
            //    options.ClientId = Configuration["Authentication:LinkedIn:ClientId"];
            //    options.ClientSecret = Configuration["Authentication:LinkedIn:ClientSecret"];
            //});

            //Model Mappings
            var configMap = new AutoMapper.MapperConfiguration(c =>
            {
                c.AddProfile(new ModelMapping());
            });
            var mapping = configMap.CreateMapper();
            services.AddSingleton(mapping);

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            
            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
                config.Lockout.AllowedForNewUsers = true;
                config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                config.Lockout.MaxFailedAccessAttempts = 5;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession();
           
            //Configure AutthMessageSenderOptions to get the sedGrid key
            services.Configure<AuthMessageSenderOptions>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
