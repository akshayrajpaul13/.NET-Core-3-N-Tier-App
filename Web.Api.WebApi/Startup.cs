using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Web.Api.Base.Extensions;
using Web.Api.Data.DbContexts;
using Web.Api.Data.Enums;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Logic.Services;
using Web.Api.Logic.Validations;
using Web.Api.Web.Shared.Helpers.Security;
using Web.Api.WebApi.Helpers;
using Web.Api.WebApi.Helpers.Security;

namespace Web.Api.WebApi
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
            var key = Encoding.ASCII.GetBytes(appSettings.JwtSecret);
            services.AddAuthentication()
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
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

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Akshay - WebApi",
                    Version = "v1"
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

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

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("../swagger/v1/swagger.json", "My API v1");
                    c.RoutePrefix = "";
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
