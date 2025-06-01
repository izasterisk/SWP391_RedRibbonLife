using System.Text;
using BLL.Interfaces;
using BLL.Services;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;
using DAL.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DAL.Models.SWP391_RedRibbonLifeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Auto-mapper 
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

//Dependency Injection
builder.Services.AddScoped<IUserUtils, UserUtils>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IUserRepository<User>, UserRepository<User>>();
builder.Services.AddScoped<IUserRepository<Doctor>, UserRepository<Doctor>>();

//CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
    //options.AddPolicy("AllowOnlyLocalHost", policy =>
    //{
    //    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
    //});
    //options.AddPolicy("AllowOnlyGoogle", policy =>
    //{
    //    policy.WithOrigins("http://google.com", "http://gmail.com").AllowAnyHeader().AllowAnyMethod();
    //});
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//78. Swagger Configuration for Authorization
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Enter your JWT token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

//JWT Authentication Configuration
string LocalAudience = builder.Configuration.GetValue<string>("LocalAudience");
string LocalIssuer = builder.Configuration.GetValue<string>("LocalIssuer");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
//    .AddJwtBearer("LoginforGoogleuser", options =>
//{
//    //options.RequireHttpsMetadata = false;
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
//            .GetBytes(builder.Configuration.GetValue<string>("JWTSecretforGoogle"))),
//        ValidateIssuer = true,
//        ValidIssuer = GoogleIssuer,
//        ValidateAudience = true,
//        ValidAudience = GoogleAudience,
//    };
//})
    .AddJwtBearer("LoginforLocaluser", options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
            .GetBytes(builder.Configuration.GetValue<string>("JWTSecretforLocaluser"))),
        ValidateIssuer = true,
        ValidIssuer = LocalIssuer,
        ValidateAudience = true,
        ValidAudience = LocalAudience,
    };
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
