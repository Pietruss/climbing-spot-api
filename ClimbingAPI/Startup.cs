using System.Text;
using ClimbingAPI.Authorization;
using ClimbingAPI.Entities;
using ClimbingAPI.Middleware;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Models.User;
using ClimbingAPI.Models.UserClimbingSpot;
using ClimbingAPI.Models.Validator;
using ClimbingAPI.Seeders;
using ClimbingAPI.Services;
using ClimbingAPI.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using CreateBoulderModelDto = ClimbingAPI.Models.Boulder.CreateBoulderModelDto;

namespace ClimbingAPI
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
            #region loginConfig
            var authenticationSettings = new AuthenticationSettings();
            Configuration.GetSection("Authentication").Bind(authenticationSettings);

            services.AddSingleton(authenticationSettings);

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });
            #endregion config

            services.AddControllers().AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddControllers().AddFluentValidation();
            services.AddControllers();

            #region entitiesConfig
            services.AddScoped<IClimbingSpotService, ClimbingSpotService>();
            services.AddScoped<IBoulderService, BoulderService>();
            services.AddScoped<IAccountService, AccountService>();
            #endregion

            #region Validators
            services.AddScoped<IValidator<CreateUserDto>, CreateUserDtoValidator>();
            services.AddScoped<IValidator<CreateBoulderModelDto>, CreateBoulderModelDtoValidator>();
            services.AddScoped<IValidator<CreateClimbingSpotDto>, CreateClimbingSpotDtoValidator>();
            services.AddScoped<IValidator<LoginUserDto>, LoginUserDtoValidator>();
            services.AddScoped<IValidator<UpdateUserClimbingSpotDto>, UpdateUserClimbingSpotDtoValidator>();
            services.AddScoped<IValidator<UpdateClimbingSpotDto>, UpdateClimbingSpotDtoValidator>();
            #endregion

            #region handlers

            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementClimbingSpotHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementBoulderHandler>();

            #endregion

            #region others
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddDbContext<ClimbingDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));

            services.AddScoped<UserRoleSeeder>();
            services.AddScoped<ClimbingSpotSeeder>();
            services.AddScoped<UserClimbingSpotSeeder>();

            services.AddScoped<IUserContextService, UserContextService>();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen();

            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserRoleSeeder seeder, ClimbingSpotSeeder climbingSpotSeeder, UserClimbingSpotSeeder userClimbingSpot)
        {
            seeder.Seed();
            climbingSpotSeeder.Seed();
            userClimbingSpot.Seed();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Climbing API");
            });


            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
