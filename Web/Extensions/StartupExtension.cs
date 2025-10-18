using Authorization.Providers;

using BoilerPlate.Authorization;
using BoilerPlate.Authorization.Handlers;

using Centangle.Common.ResponseHelpers.Models;

using CorrelationId.DependencyInjection;

using DataLibrary;

using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

using Helpers.File;

using IdentityManager;

using IdentityProvider.Seed;

using IdentityStore;

using Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;    
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Models;
using Models.Mapper;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using NotificationWorkerService.Interface;
using NotificationWorkerService.Repository;

using Repositories;
using Repositories.Common;
using Repositories.Common.AdministratorService;
using Repositories.Common.AdministratorService.Interface;
using Repositories.Common.Example;
using Repositories.Common.Example.Interface;
using Repositories.Common.GuestService;
using Repositories.Common.GuestService.Interface;
using Repositories.Common.ManagerService;
using Repositories.Common.ManagerService.Interface;
using Repositories.Common.Permission;
using Repositories.Common.Permission.Interface;
using Repositories.Common.Role;
using Repositories.Common.Role.Interface;
using Repositories.Common.TechnicianService;
using Repositories.Common.TechnicianService.Interface;
using Repositories.Common.Users;
using Repositories.Common.Users.Interface;
using Repositories.Services.ArcGis;
using Repositories.Services.ArcGis.Interface;
using Repositories.Services.AttachmentService;
using Repositories.Services.AttachmentService.Interface;
using Repositories.Services.AuthenticationService;
using Repositories.Services.AuthenticationService.Interface;
using Repositories.Services.Dashboard;
using Repositories.Services.Dashboard.Interface;
using Repositories.Services.ExcelHelper;
using Repositories.Services.ExcelHelper.Interface;
using Repositories.Services.IncidentHistory.Interface;
using Repositories.Services.Report;
using Repositories.Services.Report.Common;
using Repositories.Services.Report.Common.interfaces;
using Repositories.Services.Report.Interface;
using Repositories.Shared.NotificationServices;
using Repositories.Shared.NotificationServices.Interface;
using Repositories.Shared.UserInfoServices;
using Repositories.Shared.UserInfoServices.Interface;

using Repository;

using ViewModels.Dashboard;
using ViewModels.Dashboard.interfaces;
using ViewModels.Report.Factory.interfaces;

namespace Web.Extensions
{
    public static class StartupExtension
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("StreetsDivisionContextConnection") ?? throw new InvalidOperationException("Connection string 'StreetsDivisionContextConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .UseSqlServer(connectionString));
            services.AddAutoMapper(typeof(Mapping));

            //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                    {
                        options.SignIn.RequireConfirmedAccount = true;
                        options.Password.RequireUppercase = false;
                    })
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddUserStore<ApplicationUserStore<ApplicationUser, ApplicationRole>>()
                    //.AddUserManager<UserManager<ApplicationUser>>()
                    .AddRoleManager<RoleManager<ApplicationRole>>()
                    .AddSignInManager<ApplicationSignInManager<ApplicationUser>>()
                    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
                    .AddDefaultTokenProviders();
            //services.AddAuthorization();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login"; // Customize the login path here
            });
            services.AddTransient<IAuthorizationHandler, PermissionHandler>();
            services.AddPermission();


            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
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
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        public static void ConfigureDependencies(this IServiceCollection services)
        {
            services.AddHostedService<SeedWorker>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IEmail, EmailService>();
            services.AddSingleton<ISms, SmsService>();
            services.AddSingleton<IPushNotification, PushNotificationService>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IRepositoryResponse, RepositoryResponse>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IUserStore<ApplicationUser>, UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, long>>();

            services.AddScoped<IFileHelper, FileHelper>();
            services.AddScoped<IExcelHelper, ExcelHelper>();
            services.AddScoped(typeof(IPermissionService<,,>), typeof(PermissionService<,,>));
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IUserInfoService, UserInfoService>();
            services.AddScoped<IAttachment, AttachmentService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped(typeof(IAssignedPermissionService<,,>), typeof(AssignedPermissionService<,,>));
            services.AddScoped(typeof(IUserService<,,>), typeof(UserService<,,>));
            services.AddScoped(typeof(IRoleService<,,>), typeof(RoleService<,,>));
            services.AddScoped(typeof(IUOMService<,,>), typeof(UOMService<,,>));
            services.AddScoped(typeof(IConditionService<,,>), typeof(ConditionService<,,>));
            services.AddScoped(typeof(ICategoryService<,,>), typeof(CategoryService<,,>));
            services.AddScoped(typeof(IAssetTypeService<,,>), typeof(AssetTypeService<,,>));
            services.AddScoped(typeof(ISourceService<,,>), typeof(SourceService<,,>));
            services.AddScoped(typeof(ILocationService<,,>), typeof(LocationService<,,>));
            services.AddScoped(typeof(ISupplierService<,,>), typeof(SupplierService<,,>));
            services.AddScoped(typeof(IContractorService<,,>), typeof(ContractorService<,,>));
            services.AddScoped(typeof(IManufacturerService<,,>), typeof(ManufacturerService<,,>));
            services.AddScoped(typeof(IAdministratorService<,,>), typeof(AdministratorService<,,>));
            services.AddScoped(typeof(IGuestService<,,>), typeof(GuestService<,,>));
            services.AddScoped(typeof(IAssetService<,,>), typeof(AssetService<,,>));
            services.AddScoped(typeof(ICraftSkillService<,,>), typeof(CraftSkillService<,,>));
            services.AddScoped(typeof(IInventoryService<,,>), typeof(InventoryService<,,>));
            services.AddScoped(typeof(ITransactionService<,,>), typeof(TransactionService<,,>));
            services.AddScoped(typeof(IShipmentService<,,>), typeof(ShipmentService<,,>));
            services.AddScoped(typeof(IRepairService<,,>), typeof(RepairService<,,>));
            services.AddScoped(typeof(IReplaceService<,,>), typeof(ReplaceService<,,>));
            services.AddScoped(typeof(IWorkOrderService<,,>), typeof(WorkOrderService<,,>));
            services.AddScoped(typeof(IManagerService<,,>), typeof(ManagerService<,,>));
            services.AddScoped(typeof(IOrderService<,,>), typeof(OrderService<,,>));
            services.AddScoped(typeof(IAssetTypeLevel1Service<,,>), typeof(AssetTypeLevel1Service<,,>));
            services.AddScoped(typeof(IAssetTypeLevel2Service<,,>), typeof(AssetTypeLevel2Service<,,>));
            services.AddScoped(typeof(IExecuteService), typeof(ExecuteService));
            services.AddScoped(typeof(IExecuteEquipmentService), typeof(ExecuteEquipmentService));
            services.AddScoped(typeof(IDashboardService), typeof(DashboardService));
            services.AddScoped(typeof(IReportService), typeof(ReportService));
            services.AddScoped(typeof(IReportServiceQueries), typeof(ReportServiceQueries));
            services.AddScoped(typeof(IDashboardFactory), typeof(DashboardFactory));
            services.AddScoped(typeof(IDashboardTableService), typeof(DashboardTableService));
            services.AddScoped(typeof(IDashboardCardService), typeof(DashboardCardService));
            services.AddScoped(typeof(ITechnicianService<,,>), typeof(TechnicianService<,,>));
            services.AddScoped(typeof(IMUTCDService<,,>), typeof(MUTCDService<,,>));
            services.AddScoped(typeof(IMountTypeService<,,>), typeof(MountTypeService<,,>));

            services.AddScoped(typeof(IEquipmentService<,,>), typeof(EquipmentService<,,>));
            services.AddScoped(typeof(IEquipmentTransactionService<,,>), typeof(EquipmentTransactionService<,,>));
            services.AddScoped(typeof(IStreetServiceRequestService<,,>), typeof(StreetServiceRequestService<,,>));
            services.AddScoped(typeof(ITaskTypeService<,,>), typeof(TaskTypeService<,,>));
            services.AddScoped(typeof(ITicketService<,,>), typeof(TicketService<,,>));
            services.AddScoped(typeof(IDynamicColumnService<,,>), typeof(DynamicColumnService<,,>));

            services.AddScoped<ITimesheetService, TimesheetService>();
            services.AddScoped<ITimesheetLimit, TimesheetLimitService>();
            services.AddScoped(typeof(IUsersinService<,,>), typeof(UsersinService<,,>));
            services.AddScoped(typeof(IUserSearchSettingService<,,>), typeof(UserSearchSettingService<,,>));
            services.AddScoped(typeof(IReportFactory), typeof(ReportFactory));
            services.AddScoped(typeof(IRelationshipService<,,>), typeof(RelationshipService<,,>));
            services.AddScoped(typeof(IEventTypeService<,,>), typeof(EventTypeService<,,>));
            services.AddScoped(typeof(ISeverityLevelService<,,>), typeof(SeverityLevelService<,,>));
            services.AddScoped(typeof(IAssetIdService<,,>), typeof(AssetIdService<,,>));
            services.AddScoped(typeof(IUserManagementService<,,>), typeof(UserManagementService<,,>));
            services.AddScoped(typeof(IStatusLegendService<,,>), typeof(StatusLegendService<,,>));
            services.AddScoped(typeof(IAssignResponseTeamsService<,,>),typeof(AssignResponseTeamsService<,,>));
            services.AddScoped(typeof(IProgressService<,,>), typeof(ProgressService<,,>));
            services.AddScoped(typeof(IEquipmentFieldsService<,,>), typeof(EquipmentFieldsService<,,>));
            services.AddScoped(typeof(ICompanyService<,,>), typeof(CompanyService<,,>));
            services.AddScoped<IIncidentService, IncidentService>();
            services.AddScoped<IIncidentNoteService, IncidentNoteService>();
            services.AddScoped<IIncidentHistoryService, IncidentHistoryService>();
            services.AddScoped<IIncidentDashboardService, IncidentDashboardService>();
            services.AddScoped<IIncidentValidationService, IncidentValidationService>();
            services.AddScoped(typeof(IPolicyService<,,>), typeof(PolicyService<,,>));
            services.AddScoped<IArcGisGeocodingService, ArcGisGeocodingService>();
            services.AddHttpClient<ArcGisGeocodingService>();
            services.AddScoped(typeof(IIncidentTeamService<,,>), typeof(IncidentTeamService<,,>));
            services.AddScoped(typeof(IAdditionalLocationsService), typeof(AdditionalLocationsService));
            services.AddScoped(typeof(IMaterialService<,,>), typeof(MaterialService<,,>));
            services.AddScoped(typeof(IIncidentRoleService<,,>), typeof(IncidentRoleService<,,>));
            services.AddScoped(typeof(IIncidentShiftService<,,>), typeof(IncidentShiftService<,,>));


            services.AddDefaultCorrelationId();
        }
    }
}
