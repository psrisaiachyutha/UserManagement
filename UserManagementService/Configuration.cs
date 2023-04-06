#region References
using AutoMapper;
using Business.Implementations;
using Business.Interfaces;
using Business.Mappers;
using Common.Models.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.Implementations;
using Repository.Interfaces;
using System.Text;
using UserManagementService.Validators;
#endregion References

namespace UserManagementService
{
    public static class Configuration
    {
        /// <summary>
        /// An extention method for adding dependency objects
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMyDependencyGroup(
             this IServiceCollection services)
        {
            services.AddScoped<IUserBusinessHandler, UserBusinessHandler>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleBusinessHandler, RoleBusinessHandler>();

            // Adding validators
            services.AddScoped<IValidator<LoginRequestDTO>, LoginRequestDTOValidator>();
            services.AddScoped<IValidator<CreateUserRequestDTO>, CreateUserRequestDTOValidator>();
            services.AddScoped<IValidator<AssignRoleRequestDTO>, AssignRoleRequestDTOValidator>();
            services.AddScoped<IValidator<CreateRoleRequestDTO>, CreateRoleRequestDTOValidator>();

            // Adding automapper
            services.AddSingleton(new MapperConfiguration(mc => {
                mc.AddProfile(new UserProfile());
            }).CreateMapper());

            return services;
        }

        /// <summary>
        /// Thsi function will initialize the swagger generation
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomSwaggerGeneration(
             this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "UserManagement APIs",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            return services;
        }

        /// <summary>
        /// This function will iniitalize the authentication
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomAuthentication(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["ApplicationSettings:SignatureKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}
