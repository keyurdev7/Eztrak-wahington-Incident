using CorrelationId;

using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Web.UI;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using NotificationWorkerService;

using Repositories.Services.ArcGis;

using Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);
//builder.Services.AddAuthorization(options =>
//{
//    // By default, all incoming requests will be authorized according to the default policy.
//    options.FallbackPolicy = options.DefaultPolicy;
//});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();
//builder.Services.AddRazorPages();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});

builder.Services.AddSession(options =>
{
    // Set session timeout value
    //options.IdleTimeout = TimeSpan.FromMinutes(120);
    // Make the session cookie essential
    options.Cookie.IsEssential = true;
});

builder.Services.ConfigureDependencies();
builder.Services.AddHostedService<NotificationWorker>();


Microsoft.Extensions.Configuration.ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
var directoryPath = configuration.GetValue<string>("DirectoryPath");
var uploadBasePath = configuration.GetValue<string>("UploadBasePath");
//app.UseHttpsRedirection(); //ENABLE IN PRODUCTION
app.UseStaticFiles(); // For the wwwroot folder  
app.UseStaticFiles(new StaticFileOptions
{

    FileProvider = new PhysicalFileProvider(Path.Combine(directoryPath, uploadBasePath)),
    RequestPath = "/Storage"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
                    Path.Combine(builder.Environment.ContentRootPath, "static")),
    RequestPath = "/static"
});

app.UseSession();
app.UseCorrelationId();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllerRoute(
        name: "default",
        //pattern: "{controller=Account}/{action=Login}/{id?}");
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );

});

//app.MapRazorPages();
//app.MapControllers();

app.Run();
