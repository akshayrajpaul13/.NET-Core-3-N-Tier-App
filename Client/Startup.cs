using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Text;
using Web.Api.Base.Extensions;
using Web.Api.Client.Helpers.Security;
using Web.Api.Data.DbContexts;
using Web.Api.Data.Enums;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Logic.Services;
using Web.Api.Logic.Validations;
using Web.Api.Web.Shared.Helpers.Security;

namespace Web.Api.Client
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
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection"));
            });
            services.AddUnitOfWork<AppDbContext>();

            // configure authentication
            services.AddAuthentication()
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });

            services.AddHttpContextAccessor();

            // Add all application permissions as policies for authorization
            services.AddAuthorization(options =>
            {
                var permissionTypesInCode = EnumExtensions.GetValues<PermissionTypes>();
                foreach (var permissionType in permissionTypesInCode.Where(x => x != PermissionTypes.Undefined))
                {
                    options.AddPolicy(permissionType.ToString(), policy => policy.Requirements.Add(new PermissionRequirement(permissionType)));
                }
            });

            services.AddScoped<IAuthorizationHandler, AuthorizationHandler>();

            services.AddControllersWithViews();

            // Client Services
            services.AddTransient<IAccountManager, AccountManager>();
            // Validation Services
            services.AddTransient<IUserValidation, UserValidation>();

            // Logic Services
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IPermissionUpdaterService, PermissionUpdaterService>();
            services.AddTransient<ISecurityService, SecurityService>();
            services.AddTransient<ISessionService, SessionService>();
            services.AddTransient<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
