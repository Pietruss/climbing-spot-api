using System.Text;
using ClimbingAPI.Authorization;
using ClimbingAPI.Entities;
using ClimbingAPI.Middleware;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Models.User;
using ClimbingAPI.Models.UserClimbingSpot;
using ClimbingAPI.Models.Validator;
using ClimbingAPI.Services;
using ClimbingAPI.Services.Helpers;
using ClimbingAPI.Services.Helpers.AccountServiceHelpers;
using ClimbingAPI.Services.Helpers.AccountServiceHelpers.Interfaces;
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
            services.AddScoped<IImageService, ImageService>();

            services.AddScoped<IAccountServiceVerifier, AccountServiceVerifier>();
            services.AddScoped<IAccountServiceJwtHelper, AccountServiceJwtHelper>();
            services.AddScoped<IAccountServiceUpdateHelper, AccountServiceUpdateHelper>();
            services.AddScoped<IAccountServiceGetDataHelper, AccountServiceGetDataHelper>();
            services.AddScoped<IAccountServiceCreateAndRemoveHelper, AccountServiceCreateAndRemoveHelper>();
            #endregion

            #region Validators
            services.AddScoped<IValidator<CreateUserDto>, CreateUserDtoValidator>();
            services.AddScoped<IValidator<CreateBoulderModelDto>, CreateBoulderModelDtoValidator>();
            services.AddScoped<IValidator<CreateClimbingSpotDto>, CreateClimbingSpotDtoValidator>();
            services.AddScoped<IValidator<LoginUserDto>, LoginUserDtoValidator>();
            services.AddScoped<IValidator<UpdateUserClimbingSpotDto>, UpdateUserClimbingSpotDtoValidator>();
            services.AddScoped<IValidator<UpdateClimbingSpotDto>, UpdateClimbingSpotDtoValidator>();
            services.AddScoped<IValidator<UpdateUserDto>, UpdateUserDtoValidator>();
            services.AddScoped<IValidator<UpdateUserPasswordDto>, UpdateUserPasswordDtoValidator>();
            #endregion

            #region handlers

            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementClimbingSpotHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementBoulderHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementAccountHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementImageHandler>();

            #endregion

            #region others
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddDbContext<ClimbingDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));

            services.AddScoped<UserRoleSeeder>();

            services.AddScoped<IUserContextService, UserContextService>();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen();
            services.AddCors(options => 
            {
                options.AddPolicy("FrontEndClient", ApplicationBuilder =>

                 ApplicationBuilder.AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowAnyOrigin()
                );
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserRoleSeeder seeder)
        {
            app.UseResponseCaching();
            app.UseCors("FrontEndClient");
            seeder.Seed();

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
