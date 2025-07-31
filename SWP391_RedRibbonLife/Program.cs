using System.Text;
using BLL.Interfaces;
using BLL.Services;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;
using DAL.Repository;
using Hangfire;
using Hangfire.SqlServer;
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
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();
builder.Services.AddScoped<IDoctorScheduleUtils, DoctorScheduleUtils>();
builder.Services.AddScoped<IARVRegimensService, ARVRegimensService>();
builder.Services.AddScoped<ITestTypeService, TestTypeService>();
builder.Services.AddScoped<ITestResultService, TestResultService>();
builder.Services.AddScoped<IARVComponentService, ARVComponentService>();
builder.Services.AddScoped<IARVRegimenUtils, ARVRegimenUtils>();
builder.Services.AddScoped<ITreatmentService, TreatmentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationUtils, NotificationUtils>();
builder.Services.AddScoped<IHangfireBackgroundJobService, HangfireBackgroundJobService>();
builder.Services.AddScoped<IDoctorCertificateService, DoctorCertificateService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddScoped<IRepository<Doctor>, Repository<Doctor>>();
builder.Services.AddScoped<IRepository<Article>, Repository<Article>>();
builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
builder.Services.AddScoped<IRepository<Patient>, Repository<Patient>>();
builder.Services.AddScoped<IRepository<Appointment>, Repository<Appointment>>();
builder.Services.AddScoped<IRepository<DAL.Models.DoctorSchedule>, Repository<DAL.Models.DoctorSchedule>>();
builder.Services.AddScoped<IRepository<Arvregimen>, Repository<Arvregimen>>();
builder.Services.AddScoped<IRepository<TestType>, Repository<TestType>>();
builder.Services.AddScoped<IRepository<TestResult>, Repository<TestResult>>();
builder.Services.AddScoped<IRepository<Arvcomponent>, Repository<Arvcomponent>>();
builder.Services.AddScoped<IRepository<Treatment>, Repository<Treatment>>();
builder.Services.AddScoped<IRepository<Notification>, Repository<Notification>>();
builder.Services.AddScoped<IRepository<DoctorCertificate>, Repository<DoctorCertificate>>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IARVRegimensRepository, ARVRegimensRepository>();
builder.Services.AddScoped<ITreatmentRepository, TreatmentRepository>();
builder.Services.AddScoped<IARVComponentRepository, ARVComponentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

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

// Hangfire Configuration
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

// Đăng ký background job service
builder.Services.AddScoped<IHangfireBackgroundJobService, HangfireBackgroundJobService>();
builder.Services.AddSingleton<SendGridEmailUtil>();

builder.Services.AddSwaggerGen();

// Configure URLs for deployment
builder.WebHost.UseUrls($"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}");

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
// Hangfire Dashboard (chỉ trong development)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard();
}

// Configure recurring jobs
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    
    // Morning job - 6:30 AM daily
    recurringJobManager.AddOrUpdate<IHangfireBackgroundJobService>(
        "morning-notifications",
        service => service.ExecuteMorningJobAsync(),
        "30 6 * * *", // Cron expression for 6:30 AM daily
        TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time") // UTC+7
    );
    
    // Evening job - 8:00 PM daily
    recurringJobManager.AddOrUpdate<IHangfireBackgroundJobService>(
        "evening-notifications", 
        service => service.ExecuteEveningJobAsync(),
        "0 20 * * *", // Cron expression for 8:00 PM daily
        TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time") // UTC+7
    );
}
app.MapControllers();

app.Run();
