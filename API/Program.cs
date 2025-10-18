using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Repositories;
using System.Text;
using Helpers.File;
using Centangle.Common.RequestHelpers.SwaggerFilters;
using Repositories.Services.AuthenticationService;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Models;
using IdentityStore;
using DataLibrary;
using IdentityManager;
using Repositories.VersionService;
using Repositories.Services.AuthenticationService.Interface;
using API.Mapper;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.EntityFrameworkCore;
using CorrelationId;
using Repositories.Services.ExcelHelper.Interface;
using Repositories.Services.ExcelHelper;
using Repositories.Common;
using Repositories.Services.AttachmentService.Interface;
using Repositories.Services.AttachmentService;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Repositories.Shared.NotificationServices.Interface;
using Repositories.Shared.NotificationServices;
using Repositories.Shared.UserInfoServices.Interface;
using Repositories.Shared.UserInfoServices;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config

// Add services to the container.

builder.Services.AddRazorPages();

var connectionString = configuration.GetConnectionString("StreetsDivisionContextConnection") ?? throw new InvalidOperationException("Connection string 'StreetsDivisionContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
    .UseSqlServer(connectionString));

builder.Services.AddAutoMapper(typeof(Mapping));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddUserStore<ApplicationUserStore<ApplicationUser, ApplicationRole>>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager<ApplicationSignInManager<ApplicationUser>>()
.AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer  
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    //options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});
builder.Services.AddAuthorization();

builder.Services.AddCors(confg =>
    confg.AddPolicy("AllowAll",
        p => p.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()));

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 5;
    options.Password.RequiredUniqueChars = 0;
    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
    //Sign in settings.
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
});


builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LAC API", Version = "v1" });
    c.OperationFilter<SwaggerFileOperationFilter>();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
        "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n " +
        "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
});
builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
//Dependance Injections for custom service configured
//builder.Services.AddScoped<IRepositoryResponse, RepositoryResponse>();

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<IRepositoryResponse, RepositoryResponse>();
builder.Services.AddScoped<IUserInfoService, UserInfoService>();
builder.Services.AddScoped<IExcelHelper, ExcelHelper>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ITimesheetService, TimesheetService>();
builder.Services.AddScoped<ITimesheetLimit, TimesheetLimitService>();
builder.Services.AddScoped<IFileHelper, FileHelper>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IVersionService, VersionService>();
builder.Services.AddScoped(typeof(IInventoryService<,,>), typeof(InventoryService<,,>));
builder.Services.AddScoped(typeof(IAssetService<,,>), typeof(AssetService<,,>));
builder.Services.AddScoped(typeof(IAssetTypeService<,,>), typeof(AssetTypeService<,,>));
builder.Services.AddScoped(typeof(IConditionService<,,>), typeof(ConditionService<,,>));
builder.Services.AddScoped(typeof(IConditionService<,,>), typeof(ConditionService<,,>));
builder.Services.AddScoped(typeof(IManufacturerService<,,>), typeof(ManufacturerService<,,>));
builder.Services.AddScoped(typeof(IManufacturerService<,,>), typeof(ManufacturerService<,,>));
builder.Services.AddScoped(typeof(IMUTCDService<,,>), typeof(MUTCDService<,,>));
builder.Services.AddScoped(typeof(IDynamicColumnService<,,>), typeof(DynamicColumnService<,,>));
builder.Services.AddScoped(typeof(IWorkOrderService<,,>), typeof(WorkOrderService<,,>));
builder.Services.AddScoped(typeof(ITaskTypeService<,,>), typeof(TaskTypeService<,,>));
builder.Services.AddScoped(typeof(ITransactionService<,,>), typeof(TransactionService<,,>));
builder.Services.AddScoped(typeof(ITransactionService<,,>), typeof(TransactionService<,,>));
builder.Services.AddScoped(typeof(IEquipmentTransactionService<,,>), typeof(EquipmentTransactionService<,,>));
builder.Services.AddScoped(typeof(IDynamicColumnService<,,>), typeof(DynamicColumnService<,,>));
builder.Services.AddScoped<IAttachment, AttachmentService>();
builder.Services.AddDefaultCorrelationId();


var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCorrelationId();

app.UseCors("AllowAll");


app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LAC v1"));
app.MapControllers();

app.Run();



//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//    endpoints.MapControllerRoute(
//       name: "default",
//       pattern: "{controller=Account}/{action=Login}/{id?}");
//    endpoints.MapRazorPages();
//});

