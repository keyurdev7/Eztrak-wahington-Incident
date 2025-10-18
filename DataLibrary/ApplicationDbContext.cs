using CorrelationId.Abstractions;
using Enums;
using Helpers.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Common.Interfaces;
using Models.Models.Shared;
using Newtonsoft.Json;
using System.Security.Claims;
using ViewModels.Shared;

namespace DataLibrary;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICorrelationContextAccessor _correlationContext;
    private long _userId
    {
        get
        {
            var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userIdParsed = !string.IsNullOrEmpty(userId) ? long.Parse(userId) : 0;
            return userIdParsed;
        }
    }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor, ICorrelationContextAccessor correlationContext)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _correlationContext = correlationContext;
    }

    public DbSet<AssignedPermission> AssignedPermissions { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<LogData> LogDatas { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AssetType> AssetTypes { get; set; }
    public DbSet<UOM> UOMs { get; set; }
    public DbSet<Condition> Conditions { get; set; }
    public DbSet<Source> Sources { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Contractor> Contractors { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetNotes> AssetNotes { get; set; }
    public DbSet<StreetServiceRequestNotes> StreetServiceRequestNotes { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<InventoryNotes> InventoryNotes { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<EquipmentTransaction> EquipmentTransactions { get; set; }
    public DbSet<RemovedInventory> RemovedInventories { get; set; }
    public DbSet<CraftSkill> CraftSkills { get; set; }
    public DbSet<Repair> Repairs { get; set; }
    public DbSet<Replace> Replaces { get; set; }
    public DbSet<WorkOrderMaterial> WorkOrderMaterials { get; set; }
    public DbSet<WorkOrderLabor> WorkOrderLabors { get; set; }
    public DbSet<WorkOrder> WorkOrder { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<AssetTypeLevel1> AssetTypeLevels1 { get; set; }
    public DbSet<AssetTypeLevel2> AssetTypeLevels2 { get; set; }
    public DbSet<WorkOrderNotes> WorkOrderNotes { get; set; }
    public DbSet<WorkOrderComment> WorkOrderComments { get; set; }
    public DbSet<AssetAssociation> AssetAssociations { get; set; }
    public DbSet<WorkOrderTechnician> WorkOrderTechnicians { get; set; }
    public DbSet<WorkOrderEquipment> WorkOrderEquipments { get; set; }
    public DbSet<WorkOrderTasks> WorkOrderTasks { get; set; }
    public DbSet<MUTCD> MUTCDs { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<EquipmentNotes> EquipmentNotes { get; set; }
    public DbSet<MountType> MountTypes { get; set; }
    public DbSet<Timesheet> Timesheets { get; set; }
    public DbSet<TimesheetBreakdown> TimesheetBreakdowns { get; set; }
    public DbSet<StreetServiceRequest> StreetServiceRequests { get; set; }
    public DbSet<TaskType> TaskTypes { get; set; }
    public DbSet<TaskEquipment> TaskEquipments { get; set; }
    public DbSet<TaskWorkStep> TaskWorkSteps { get; set; }
    public DbSet<TaskMaterial> TaskMaterials { get; set; }
    public DbSet<TaskLabor> TaskLabors { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketHistory> TicketHistories { get; set; }
    public DbSet<UserSearchSetting> UserSearchSettings { get; set; }
    public DbSet<TaskTypeStep> TaskTypeSteps { get; set; }
    public DbSet<DynamicColumn> DynamicColumns { get; set; }
    public DbSet<DynamicColumnOption> DynamicColumnOptions { get; set; }
    public DbSet<DynamicColumnValue> DynamicColumnValues { get; set; }
    public DbSet<Relationship> Relationships { get; set; }
    public DbSet<EventType> EventTypes { get; set; }
    public DbSet<StatusLegend> StatusLegends { get; set; }
    public DbSet<SeverityLevel> SeverityLevels { get; set; }
    public DbSet<AssetIncident> AssetIncidents { get; set; }
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<IncidentNote> IncidentNotes { get; set; }
    public DbSet<AssignResponseTeams> AssignResponseTeams { get; set; }
    public DbSet<IncidentHistory> IncidentHistories { get; set; }
    public DbSet<Policy> Policies { get; set; }
    public DbSet<IncidentTeam> IncidentTeams { get; set; }
    public DbSet<IncidentUser> IncidentUsers { get; set; }
    public DbSet<IncidentValidation> IncidentValidations { get; set; }
    public DbSet<IncidentValidationCommunicationHistory> IncidentValidationCommunicationHistories { get; set; }
    public DbSet<IncidentValidationPolicy> IncidentValidationPolicies { get; set; }
    public DbSet<Progress> Progress { get; set; }
    public DbSet<AdditionalLocations> AdditionalLocations { get; set; }
    public DbSet<EquipmentFields> EquipmentFields { get; set; }
    public DbSet<IncidentValidationAssignedRole> IncidentValidationAssignedRoles { get; set; }
    public DbSet<IncidentValidationGate> IncidentValidationGates { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<IncidentValidationLocation> IncidentValidationLocations { get; set; }
    public DbSet<Company> Company { get; set; }
    public DbSet<IncidentRole> IncidentRoles { get; set; }
    public DbSet<IncidentShift> IncidentShifts { get; set; }
    public DbSet<IncidentValidationPersonnel> IncidentValidationPersonnels { get; set; }
    public DbSet<IncidentValidationRepair> IncidentValidationRepairs { get; set; }
    public DbSet<IncidentValidationNotes> IncidentValidationNotes { get; set; }
    public DbSet<ValidationCloseout> ValidationCloseouts { get; set; }
    public DbSet<IncidentValidationAssessment> IncidentValidationAssessments { get; set; }
    public DbSet<IncidentValidationTask> IncidentValidationTasks { get; set; }
    public DbSet<IncidentMapChat> IncidentMapChats { get; set; }
    
    public DbSet<IncidentPostDetail> IncidentPostDetails { get; set; }

    [DbFunction("GetWeekNumber", "dbo")]
    public int GetWeekNumber(DateTime dateToCheck)
    {
        throw new NotSupportedException();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyGlobalFilters<IIsDeleted>(x => x.IsDeleted == false);
        modelBuilder.HasDbFunction(typeof(ApplicationDbContext).GetMethod(nameof(ApplicationDbContext.GetWeekNumber)));
        base.OnModelCreating(modelBuilder);
    }
    public override int SaveChanges()
    {
        InitializeGlobalProperties();
        return base.SaveChanges();
    }
    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        InitializeGlobalProperties();
        return await base.SaveChangesAsync(true, cancellationToken);
    }

    public async virtual Task<int> SaveChangesAsync(IBaseCrudViewModel viewModel, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            InitializeGlobalProperties();
            await OnBeforeSaveChanges(viewModel);
            return await base.SaveChangesAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            return 0;
        }
    }
    private void InitializeGlobalProperties()
    {
        //To be fixed once we get user Id from Identity
        var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userIdParsed = !string.IsNullOrEmpty(userId) ? long.Parse(userId) : 0;

        var addedList = ChangeTracker.Entries().Where(m => (m.Entity is BaseDBModel || m.Entity is ApplicationUser) && m.State == EntityState.Added);
        foreach (var item in addedList)
        {
            if (item.Entity is IBaseModel)
            {
                ((IBaseModel)item.Entity).IsDeleted = false;
                //((BaseDBModel)item.Entity).IsActive = true;
                ((IBaseModel)item.Entity).CreatedBy = userIdParsed;
                ((IBaseModel)item.Entity).UpdatedBy = userIdParsed;
                ((IBaseModel)item.Entity).CreatedOn = DateTime.Now;
                ((IBaseModel)item.Entity).UpdatedOn = DateTime.Now;
            }
        }
        var updatedList = ChangeTracker.Entries().Where(m => (m.Entity is BaseDBModel || m.Entity is ApplicationUser) && m.State == EntityState.Modified);
        foreach (var item in updatedList)
        {
            if (item.Entity is BaseDBModel)
            {
                ((BaseDBModel)item.Entity).UpdatedBy = userIdParsed;
                ((BaseDBModel)item.Entity).UpdatedOn = DateTime.Now;
            }
        }
    }

    public async Task AddManualLogData<T, VM>(T dbModel, VM model) where T : class, IBaseModel
    {
        var serializedViewModel = JsonConvert.SerializeObject(model, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        var serializedDbModel = JsonConvert.SerializeObject(dbModel, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        var audit = new LogData
        {
            UserId = _userId,
            Type = LogDataAction.Create.ToString(),
            Action = LogDataAction.Create,
            TableName = typeof(T).Name,
            CorrelationId = Guid.NewGuid().ToString(),
            PrimaryKey = long.MinValue,
            OldValues = "",
            NewValues = serializedViewModel,
            AffectedColumns = null,
            JsonDBModelData = serializedDbModel,
            JsonViewModelData = serializedViewModel,
            CreatedOn = DateTime.Now,
            UpdatedOn = DateTime.Now,
            CreatedBy = _userId,
            UpdatedBy = _userId
        };
        LogDatas.Add(audit);
        await base.SaveChangesAsync(true);
    }

    public async Task OnBeforeSaveChanges(IBaseCrudViewModel viewModel)
    {
        try
        {
            var correlationId = _correlationContext.CorrelationContext?.CorrelationId;
            ChangeTracker.DetectChanges();
            var auditEntries = new List<LogDataViewModel>();
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is LogData || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                var auditEntry = new LogDataViewModel(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.UserId = _userId;
                auditEntry.CorrelationId = correlationId;
                auditEntry.JsonDBModelData = JsonConvert.SerializeObject(entry.Entity, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                auditEntry.JsonViewModelData = JsonConvert.SerializeObject(viewModel, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                var currentEntryState = entry.State;
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }
                    switch (currentEntryState)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = LogDataAction.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = LogDataAction.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = LogDataAction.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }
            foreach (var auditEntry in auditEntries)
            {
                LogDatas.Add(auditEntry.ToAudit());
            }
        }
        catch (Exception ex)
        {

        }

    }
}
