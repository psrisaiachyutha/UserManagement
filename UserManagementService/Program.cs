using AutoMapper;
using Business.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Repository;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Configurations;
using FluentValidation;
using Common.Models.Requests;
using UserManagementService.Validators;
using Business.Interfaces;
using Business.Implementations;
using Repository.Interfaces;
using Repository.Implementations;
using UserManagementService.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Api versioning
builder.Services.AddApiVersioning(x =>
{
    x.DefaultApiVersion = new ApiVersion(1, 0);
    x.AssumeDefaultVersionWhenUnspecified = true;
    x.ReportApiVersions = true;
});


builder.Services.AddEndpointsApiExplorer();

// Added the swagger gen
builder.Services.AddSwaggerGen(c => {
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
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

     
    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);
});

// Adding database
builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseInMemoryDatabase("UserManagementDatabase"));

// Adding authentication with Jwt bearer
builder.Services.AddAuthentication(options =>
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["ApplicationSettings:SignatureKey"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

{
    // configure strongly typed settings object
    builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

    builder.Services.AddScoped<IUserBusinessHandler, UserBusinessHandler>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IRoleBusinessHandler, RoleBusinessHandler>();

    // Adding validators
    builder.Services.AddScoped<IValidator<LoginRequestDTO>, LoginRequestDTOValidator>();
    builder.Services.AddScoped<IValidator<CreateUserRequestDTO>, CreateUserRequestDTOValidator>();
    builder.Services.AddScoped<IValidator<AssignRoleRequestDTO>, AssignRoleRequestDTOValidator>();
    builder.Services.AddScoped<IValidator<CreateRoleRequestDTO>, CreateRoleRequestDTOValidator>();

    // Adding automapper
    builder.Services.AddSingleton(new MapperConfiguration(mc => {
        mc.AddProfile(new UserProfile());
    }).CreateMapper());
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// global error handler
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
