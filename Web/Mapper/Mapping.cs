using AutoMapper;
using ViewModels;
using ViewModels.Example;
using ViewModels.Permission;
using ViewModels.AssignedPermissions;
using ViewModels.Users;
using ViewModels.Role;
using ViewModels.Authentication;
using ViewModels.Employee;
using ViewModels.Shared.Notification;
using ViewModels.Guest;
using ViewModels.Administrator;
using ViewModels.Shared;
using Models.Models.Shared;
using ViewModels.Manager;
using ViewModels.Technician;
using ViewModels.WorkOrderTechnician;
using ViewModels.WorkOrderTasks;
using Pagination;
using ViewModels.Timesheet;

namespace Models.Mapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            //Example
            CreateMap<ExampleUpdateViewModel, ExampleDbModel>().ReverseMap();
            CreateMap<ExampleDbModel, ExampleDetailViewModel>().ReverseMap();
            CreateMap<ExampleUpdateViewModel, ExampleDetailViewModel>().ReverseMap();
            CreateMap<ExampleDbModel, ExampleBriefViewModel>().ReverseMap();
            CreateMap<BaseSelect2VM, ExampleBriefViewModel>().ReverseMap();

            CreateMap<AssignedPermissionUpdateViewModel, AssignedPermission>()
                .ForMember(s => s.PermissionId, d => d.MapFrom(x => x.Permission.Id))
                .ForMember(s => s.Permission, d => d.Ignore())
                .ReverseMap();
            CreateMap<AssignedPermissionDetailViewModel, AssignedPermission>()
                .ForMember(s => s.PermissionId, d => d.MapFrom(x => x.Permission.Id))
                .ForMember(s => s.Permission, d => d.Ignore())
                .ReverseMap();

            //Permission
            CreateMap<PermissionUpdateViewModel, Permission>().ReverseMap();
            CreateMap<Permission, PermissionDetailViewModel>().ReverseMap();
            CreateMap<PermissionUpdateViewModel, PermissionDetailViewModel>().ReverseMap();
            CreateMap<Permission, PermissionBriefViewModel>().ReverseMap();
            CreateMap<BaseSelect2VM, PermissionBriefViewModel>().ReverseMap();

            //User
            CreateMap<UserCreateViewModel, ApplicationUser>().ReverseMap();
            CreateMap<UserUpdateViewModel, ApplicationUser>().ReverseMap();
            CreateMap<UserDetailViewModel, ApplicationUser>().ReverseMap();
            CreateMap<UserBriefViewModel, ApplicationUser>().ReverseMap();
            CreateMap<UserUpdateViewModel, UserDetailViewModel>().ReverseMap();

            //Employee
            CreateMap<EmployeeUpdateViewModel, Employee>().ReverseMap();
            CreateMap<EmployeeUpdateViewModel, ApplicationUser>()
                .ForMember(s => s.Id, d => d.Ignore())
                .ReverseMap();
            CreateMap<Employee, EmployeeDetailViewModel>().ReverseMap();
            CreateMap<EmployeeUpdateViewModel, EmployeeDetailViewModel>().ReverseMap();
            CreateMap<Employee, EmployeeBriefViewModel>().ReverseMap();
            CreateMap<BaseSelect2VM, EmployeeBriefViewModel>().ReverseMap();
            CreateMap<EmployeeDetailViewModel, ApplicationUser>().ReverseMap();


            CreateMap<SignUpModel, ApplicationUser>().ReverseMap();

            CreateMap<UserCreateViewModel, SignUpModel>().ReverseMap();

            //Role
            CreateMap<RoleCreateViewModel, ApplicationRole>().ReverseMap();
            CreateMap<RoleUpdateViewModel, ApplicationRole>().ReverseMap();
            CreateMap<RoleDetailViewModel, ApplicationRole>().ReverseMap();
            CreateMap<RoleBriefViewModel, ApplicationRole>().ReverseMap();
            CreateMap<RoleUpdateViewModel, RoleDetailViewModel>().ReverseMap();

            //Notification
            CreateMap<NotificationViewModel, Notification>().ReverseMap();

            //UOM
            CreateMap<UOMModifyViewModel, UOM>().ReverseMap();
            CreateMap<UOMDetailViewModel, UOM>().ReverseMap();
            CreateMap<UOMBriefViewModel, UOM>().ReverseMap();
            CreateMap<UOMModifyViewModel, UOMDetailViewModel>().ReverseMap();

            //Repair
            CreateMap<RepairModifyViewModel, Repair>().ReverseMap();
            CreateMap<RepairDetailViewModel, Repair>().ReverseMap();
            CreateMap<RepairBriefViewModel, Repair>().ReverseMap();
            CreateMap<RepairModifyViewModel, RepairDetailViewModel>().ReverseMap();

            //Repalce
            CreateMap<ReplaceModifyViewModel, Replace>().ReverseMap();
            CreateMap<ReplaceDetailViewModel, Replace>().ReverseMap();
            CreateMap<ReplaceBriefViewModel, Replace>().ReverseMap();
            CreateMap<ReplaceModifyViewModel, ReplaceDetailViewModel>().ReverseMap();


            //AssestType
            CreateMap<AssetTypeModifyViewModel, AssetType>().ReverseMap();
            CreateMap<AssetTypeDetailViewModel, AssetType>().ReverseMap();
            CreateMap<AssetTypeBriefViewModel, AssetType>().ReverseMap();
            CreateMap<AssetTypeModifyViewModel, AssetTypeDetailViewModel>().ReverseMap();

            //Condition
            CreateMap<ConditionModifyViewModel, Condition>().ReverseMap();
            CreateMap<ConditionDetailViewModel, Condition>().ReverseMap();
            CreateMap<ConditionBriefViewModel, Condition>().ReverseMap();
            CreateMap<ConditionModifyViewModel, ConditionDetailViewModel>().ReverseMap();

            //Category
            CreateMap<CategoryModifyViewModel, Category>().ReverseMap();
            CreateMap<CategoryDetailViewModel, Category>().ReverseMap();
            CreateMap<CategoryBriefViewModel, Category>().ReverseMap();
            CreateMap<CategoryModifyViewModel, CategoryDetailViewModel>().ReverseMap();

            //Source
            CreateMap<SourceModifyViewModel, Source>().ReverseMap();
            CreateMap<SourceDetailViewModel, Source>().ReverseMap();
            CreateMap<SourceBriefViewModel, Source>().ReverseMap();
            CreateMap<SourceModifyViewModel, SourceDetailViewModel>().ReverseMap();

            //Location
            CreateMap<LocationModifyViewModel, Location>().ReverseMap();
            CreateMap<LocationDetailViewModel, Location>().ReverseMap();
            CreateMap<LocationBriefViewModel, Location>().ReverseMap();
            CreateMap<LocationModifyViewModel, LocationDetailViewModel>().ReverseMap();

            //Contactor
            CreateMap<ContractorModifyViewModel, Contractor>().ReverseMap();
            CreateMap<ContractorDetailViewModel, Contractor>().ReverseMap();
            CreateMap<ContractorBriefViewModel, Contractor>().ReverseMap();
            CreateMap<ContractorModifyViewModel, ContractorDetailViewModel>().ReverseMap();

            //Supplier
            CreateMap<SupplierModifyViewModel, Supplier>().ReverseMap();
            CreateMap<SupplierDetailViewModel, Supplier>().ReverseMap();
            CreateMap<SupplierBriefViewModel, Supplier>().ReverseMap();
            CreateMap<SupplierModifyViewModel, SupplierDetailViewModel>().ReverseMap();

            //Relationship
            CreateMap<RelationshipModifyViewModel, Relationship>().ReverseMap();
            CreateMap<RelationshipDetailViewModel, Relationship>().ReverseMap();
            CreateMap<RelationshipBriefViewModel, Relationship>().ReverseMap();
            CreateMap<RelationshipModifyViewModel, RelationshipDetailViewModel>().ReverseMap();

            // EventType
            CreateMap<EventTypeModifyViewModel, EventType>().ReverseMap();
            CreateMap<EventTypeDetailViewModel, EventType>().ReverseMap();
            CreateMap<EventTypeBriefViewModel, EventType>().ReverseMap();
            CreateMap<EventTypeModifyViewModel, EventTypeDetailViewModel>().ReverseMap();

            // SeverityLevel
            CreateMap<SeverityLevelModifyViewModel, SeverityLevel>().ReverseMap();
            CreateMap<SeverityLevelDetailViewModel, SeverityLevel>().ReverseMap();
            CreateMap<SeverityLevelBriefViewModel, SeverityLevel>().ReverseMap();
            CreateMap<SeverityLevelModifyViewModel, SeverityLevelDetailViewModel>().ReverseMap();
            //AssetIds
            CreateMap<AssetIdModifyViewModel, AssetIncident>().ReverseMap();
            CreateMap<AssetIdDetailViewModel, AssetIncident>().ReverseMap();
            CreateMap<AssetIdBriefViewModel, AssetIncident>().ReverseMap();
            CreateMap<AssetIdModifyViewModel, AssetIdDetailViewModel>().ReverseMap();


            //Manufacturer
            CreateMap<ManufacturerModifyViewModel, Manufacturer>().ReverseMap();
            CreateMap<ManufacturerDetailViewModel, Manufacturer>().ReverseMap();
            CreateMap<ManufacturerBriefViewModel, Manufacturer>().ReverseMap();
            CreateMap<ManufacturerModifyViewModel, ManufacturerDetailViewModel>().ReverseMap();

            //CraftSkill
            CreateMap<CraftSkillModifyViewModel, CraftSkill>().ReverseMap();
            CreateMap<CraftSkillDetailViewModel, CraftSkill>().ReverseMap();
            CreateMap<CraftSkillBriefViewModel, CraftSkill>().ReverseMap();
            CreateMap<CraftSkillModifyViewModel, CraftSkillDetailViewModel>().ReverseMap();

            //Guest
            CreateMap<GuestUpdateViewModel, ApplicationUser>()
                .ForMember(s => s.Id, d => d.Ignore())
                .ReverseMap();
            CreateMap<GuestUpdateViewModel, GuestDetailViewModel>().ReverseMap();
            CreateMap<BaseSelect2VM, GuestBriefViewModel>().ReverseMap();
            CreateMap<GuestDetailViewModel, ApplicationUser>().ReverseMap();

            //Administrator
            CreateMap<AdministratorUpdateViewModel, ApplicationUser>()
                .ForMember(s => s.Id, d => d.Ignore())
                .ReverseMap();
            CreateMap<AdministratorUpdateViewModel, AdministratorDetailViewModel>().ReverseMap();
            CreateMap<BaseSelect2VM, AdministratorBriefViewModel>().ReverseMap();
            CreateMap<AdministratorDetailViewModel, ApplicationUser>().ReverseMap();


            CreateMap<MapBriefVM, AssetBriefViewModel>().ReverseMap();
            CreateMap<MapBriefVM, AssetTypeBriefViewModel>().ReverseMap();
            CreateMap<MapBriefVM, AssetTypeLevel1BriefViewModel>().ReverseMap();
            CreateMap<MapBriefVM, AssetTypeLevel2BriefViewModel>().ReverseMap();

            //Asset
            CreateMap<AssetModifyViewModel, Asset>()
                .ForMember(s => s.ConditionId, d => d.MapFrom(x => x.Condition.Id))
                .ForMember(s => s.Condition, d => d.Ignore())
                .ForMember(s => s.ManufacturerId, d => d.MapFrom(x => x.Manufacturer.Id))
                .ForMember(s => s.Manufacturer, d => d.Ignore())
                .ForMember(s => s.AssetTypeId, d => d.MapFrom(x => x.AssetType.Id))
                .ForMember(s => s.AssetType, d => d.Ignore())
                .ForMember(s => s.MUTCDId, d => d.MapFrom(x => x.MUTCD.Id))
                .ForMember(s => s.MUTCD, d => d.Ignore())
                .ForMember(s => s.MountTypeId, d => d.MapFrom(x => x.MountType.Id))
                .ForMember(s => s.MountType, d => d.Ignore())
                .ReverseMap();
            CreateMap<AssetDetailViewModel, Asset>().ReverseMap();
            CreateMap<AssetDetailViewModel, MaintenanceReportViewModel>().ReverseMap();
            CreateMap<PaginatedResultModel<AssetDetailViewModel>, PaginatedResultModel<MaintenanceReportViewModel>>().ReverseMap();
            CreateMap<PaginatedResultModel<AssetDetailViewModel>, PaginatedResultModel<RepairingReportViewModel>>().ReverseMap();
            CreateMap<AssetDetailViewModel, RepairingReportViewModel>().ReverseMap();
            CreateMap<AssetBriefViewModel, Asset>().ReverseMap();
            CreateMap<AssetModifyViewModel, AssetDetailViewModel>().ReverseMap();

            CreateMap<AssetMapViewModel, AssetDetailViewModel>()
             .ForPath(d => d.Condition.Name, opt => opt.MapFrom(s => s.Condition))
             .ForPath(d => d.AssetType.Name, opt => opt.MapFrom(s => s.AssetType))
             .ReverseMap();
            CreateMap<AssetAssociationDetailViewModel, MapAssetAssociationDetailViewModel>().ReverseMap();



            //Inventory
            CreateMap<InventoryModifyViewModel, Inventory>()
                .ForMember(s => s.CategoryId, d => d.MapFrom(x => x.Category.Id))
                .ForMember(s => s.Category, d => d.Ignore())
                .ForMember(s => s.ManufacturerId, d => d.MapFrom(x => x.Manufacturer.Id))
                .ForMember(s => s.Manufacturer, d => d.Ignore())
                .ForMember(s => s.UOMId, d => d.MapFrom(x => x.UOM.Id))
                .ForMember(s => s.UOM, d => d.Ignore())
                .ForMember(s => s.MUTCDId, d => d.MapFrom(x => x.MUTCD.Id))
                .ForMember(s => s.MUTCD, d => d.Ignore())
                .ReverseMap();
            CreateMap<InventoryDetailViewModel, Inventory>().ReverseMap();
            CreateMap<InventoryBriefViewModel, Inventory>().ReverseMap();
            CreateMap<InventoryModifyViewModel, InventoryDetailViewModel>().ReverseMap();
            CreateMap<InventoryBriefViewModel, InventoryDetailViewModel>().ReverseMap();

            //Attachment
            CreateMap<AttachmentVM, Attachment>().ReverseMap();

            //Notes
            CreateMap<AssetNotesViewModel, AssetNotes>()
              .ReverseMap();
            CreateMap<InventoryNotesViewModel, InventoryNotes>()
               .ReverseMap();
            CreateMap<WorkOrderNotesViewModel, WorkOrderNotes>()
               .ReverseMap();
            CreateMap<SSRNotesViewModel, StreetServiceRequestNotes>()
            .ReverseMap();

            //Transaction
            CreateMap<TransactionModifyViewModel, Transaction>()
                .ForMember(s => s.SourceId, d => d.MapFrom(x => x.Source.Id))
                .ForMember(s => s.Source, d => d.Ignore())
                .ForMember(s => s.SupplierId, d => d.MapFrom(x => x.Supplier.Id))
                .ForMember(s => s.Supplier, d => d.Ignore())
                .ForMember(s => s.LocationId, d => d.MapFrom(x => x.Location.Id))
                .ForMember(s => s.Location, d => d.Ignore())
                .ForMember(s => s.Inventory, d => d.Ignore())
                .ForMember(s => s.Reference, d => d.Ignore())
                .ReverseMap();

            CreateMap<TransactionIssueViewModel, Transaction>()
                .ForMember(s => s.SourceId, d => d.MapFrom(x => x.Source.Id))
                .ForMember(s => s.Source, d => d.Ignore())
                .ForMember(s => s.SupplierId, d => d.MapFrom(x => x.Supplier.Id))
                .ForMember(s => s.Supplier, d => d.Ignore())
                .ForMember(s => s.LocationId, d => d.MapFrom(x => x.Location.Id))
                .ForMember(s => s.Location, d => d.Ignore())
                .ForMember(s => s.InventoryId, d => d.MapFrom(x => x.Inventory.Id))
                .ForMember(s => s.Inventory, d => d.Ignore())
                .ForMember(s => s.Reference, d => d.Ignore())
            .ReverseMap();

            CreateMap<TransactionDetailViewModel, Transaction>()
                .ForMember(s => s.SourceId, d => d.MapFrom(x => x.Source.Id))
                .ForMember(s => s.Source, d => d.Ignore())
                .ForMember(s => s.SupplierId, d => d.MapFrom(x => x.Supplier.Id))
                .ForMember(s => s.Supplier, d => d.Ignore())
                .ForMember(s => s.LocationId, d => d.MapFrom(x => x.Location.Id))
                .ForMember(s => s.Location, d => d.Ignore())
                .ForMember(s => s.InventoryId, d => d.MapFrom(x => x.Inventory.Id))
                .ForMember(s => s.Inventory, d => d.Ignore())
                .ForMember(s => s.Reference, d => d.Ignore())
                .ReverseMap();

            CreateMap<TransactionModifyViewModel, ShipmentDetailViewModel>().ReverseMap();

            CreateMap<TransactionIssueViewModel, ReStageListViewModel>().ReverseMap();
            CreateMap<TransactionIssueViewModel, ReturnInventoryItemsListViewModel>().ReverseMap();

            CreateMap<TransactionIssueViewModel, RemoveInventoryItemsListViewModel>().ReverseMap();
            CreateMap<EquipmentTransactionIssueViewModel, ReturnEquipmentListViewModel>().ReverseMap();

            //Shipment
            CreateMap<ShipmentModifyViewModel, Transaction>()
                .ForMember(s => s.SourceId, d => d.MapFrom(x => x.Source.Id))
                .ForMember(s => s.Source, d => d.Ignore())
                .ForMember(s => s.SupplierId, d => d.MapFrom(x => x.Supplier.Id))
                .ForMember(s => s.Supplier, d => d.Ignore())
                .ForMember(s => s.LocationId, d => d.MapFrom(x => x.Location.Id))
                .ForMember(s => s.Location, d => d.Ignore())
                .ForMember(s => s.Inventory, d => d.Ignore())
                .ForMember(s => s.Reference, d => d.Ignore())
                .ReverseMap();
            CreateMap<ShipmentDetailViewModel, Transaction>()
                .ForMember(s => s.SourceId, d => d.MapFrom(x => x.Source.Id))
                .ForMember(s => s.Source, d => d.Ignore())
                .ForMember(s => s.SupplierId, d => d.MapFrom(x => x.Supplier.Id))
                .ForMember(s => s.Supplier, d => d.Ignore())
                .ForMember(s => s.LocationId, d => d.MapFrom(x => x.Location.Id))
                .ForMember(s => s.Location, d => d.Ignore())
                .ForMember(s => s.InventoryId, d => d.MapFrom(x => x.Inventory.Id))
                .ForMember(s => s.Inventory, d => d.Ignore())
                .ForMember(s => s.Reference, d => d.Ignore())
                .ReverseMap();
            CreateMap<ShipmentBriefViewModel, Transaction>().ReverseMap();
            CreateMap<ShipmentModifyViewModel, ShipmentDetailViewModel>().ReverseMap();

            //Manager
            CreateMap<ManagerUpdateViewModel, ApplicationUser>()
                .ForMember(s => s.Id, d => d.Ignore())
                .ReverseMap();
            CreateMap<ManagerUpdateViewModel, ManagerDetailViewModel>().ReverseMap();
            CreateMap<BaseSelect2VM, ManagerBriefViewModel>().ReverseMap();
            CreateMap<ApplicationUser, ManagerBriefViewModel>().ReverseMap();
            CreateMap<ManagerDetailViewModel, ApplicationUser>().ReverseMap();

            CreateMap<ManagerBriefViewModel, ManagerBriefViewModel>().ReverseMap();

            //WorkOrder
            CreateMap<WorkOrderModifyViewModel, WorkOrder>()
                .ForMember(s => s.RepairId, d => d.MapFrom(x => x.Repair.Id))
                .ForMember(s => s.Repair, d => d.Ignore())
                .ForMember(s => s.AssetTypeId, d => d.MapFrom(x => x.AssetType.Id))
                .ForMember(s => s.AssetType, d => d.Ignore())
                .ForMember(s => s.ReplaceId, d => d.MapFrom(x => x.Replace.Id))
                .ForMember(s => s.Replace, d => d.Ignore())
                .ForMember(s => s.ManagerId, d => d.MapFrom(x => x.Manager.Id))
                .ForMember(s => s.Manager, d => d.Ignore())
                .ForMember(s => s.AssetId, d => d.MapFrom(x => x.Asset.Id))
                .ForMember(s => s.Asset, d => d.Ignore())
                .ForMember(s => s.TaskTypeId, d => d.MapFrom(x => x.TaskType.Id))
                .ForMember(s => s.TaskType, d => d.Ignore())
                .ForMember(s => s.StreetServiceRequest, d => d.Ignore())
                .ReverseMap();
            CreateMap<WorkOrderDetailViewModel, WorkOrder>().ReverseMap();
            CreateMap<WorkOrderBriefViewModel, WorkOrder>().ReverseMap();
            CreateMap<WorkOrderModifyViewModel, WorkOrderDetailViewModel>().ReverseMap();
            CreateMap<WorkOrderBriefViewModel, WorkOrderDetailViewModel>().ReverseMap();
            CreateMap<WorkOrderForIssueBriefViewModel, WorkOrderDetailViewModel>().ReverseMap();
            CreateMap<WorkOrderModifyStatusViewModel, WorkOrderDetailViewModel>().ReverseMap();

            //WorkOrderLabor
            CreateMap<WorkOrderLaborModifyViewModel, WorkOrderLabor>()
                .ForMember(s => s.CraftId, d => d.MapFrom(x => x.Craft.Id))
                .ForMember(s => s.Rate, d => d.MapFrom(x => x.LaborRate))
                .ForMember(s => s.Estimate, d => d.MapFrom(x => x.LaborEstimate))
                .ForMember(s => s.CraftSkill, d => d.Ignore())
                .ForMember(s => s.WorkOrder, d => d.Ignore())
                .ReverseMap();

            //WorkOrderMaterial
            CreateMap<WorkOrderMaterialModifyViewModel, WorkOrderMaterial>()
                .ForMember(s => s.InventoryId, d => d.MapFrom(x => x.Inventory.Id))
                .ForMember(s => s.Inventory, d => d.Ignore())
                .ForMember(s => s.WorkOrder, d => d.Ignore())
                .ReverseMap();

            //WorkOrderEquipment
            CreateMap<WorkOrderEquipmentModifyViewModel, WorkOrderEquipment>()
                .ForMember(s => s.EquipmentId, d => d.MapFrom(x => x.Equipment.Id))
                .ForMember(s => s.Equipment, d => d.Ignore())
                .ForMember(s => s.WorkOrder, d => d.Ignore())
                .ReverseMap();

            //WorkOrderTechnician
            CreateMap<WorkOrderTechnicianModifyViewModel, WorkOrderTechnician>()
                .ForMember(s => s.CraftSkillId, d => d.MapFrom(x => x.CraftSkill.Id))
                .ForMember(s => s.CraftSkill, d => d.Ignore())
                 .ForMember(s => s.TechnicianId, d => d.MapFrom(x => x.Technician.Id))
                .ForMember(s => s.Technician, d => d.Ignore())
                .ForMember(s => s.WorkOrder, d => d.Ignore())
                .ReverseMap();

            //Order
            CreateMap<OrderModifyViewModel, Order>()
                .ForMember(s => s.WorkOrder, d => d.MapFrom(x => x.WorkOrder.Id))
                .ForMember(s => s.WorkOrder, d => d.Ignore())
                .ForMember(s => s.OrderItems, d => d.Ignore())
                .ReverseMap();
            CreateMap<OrderDetailViewModel, Order>().ReverseMap();
            CreateMap<OrderModifyViewModel, OrderDetailViewModel>().ReverseMap();

            //OrderItems
            CreateMap<OrderItemModifyViewModel, OrderItem>()
                .ForMember(s => s.InventoryId, d => d.MapFrom(x => x.Equipment.Id > 0 ? (long?)null : x.Inventory.Id))
                .ForMember(s => s.Inventory, d => d.Ignore())
                .ForMember(s => s.EquipmentId, d => d.MapFrom(x => x.Inventory.Id > 0 ? (long?)null : x.Equipment.Id))
                .ForMember(s => s.Equipment, d => d.Ignore())
                .ForMember(s => s.Order, d => d.Ignore())
                .ReverseMap();
            CreateMap<OrderItemDetailViewModel, OrderItem>().ReverseMap();
            CreateMap<OrderItemModifyViewModel, OrderItemDetailViewModel>().ReverseMap();

            //IssueInventoryItem
            CreateMap<IssueInventoryItemListViewModel, TransactionDetailViewModel>().ReverseMap();

            //AssetTypeLevel1
            CreateMap<AssetTypeLevel1ModifyViewModel, AssetTypeLevel1>()
               .ForMember(s => s.AssetType, d => d.Ignore())
                .ReverseMap();
            CreateMap<AssetTypeLevel1DetailViewModel, AssetTypeLevel1>().ReverseMap();
            CreateMap<AssetTypeLevel1BriefViewModel, AssetTypeLevel1>().ReverseMap();
            CreateMap<AssetTypeLevel1ModifyViewModel, AssetTypeLevel1DetailViewModel>().ReverseMap();

            //AssetTypeLevel2
            CreateMap<AssetTypeLevel2ModifyViewModel, AssetTypeLevel2>()
               .ForMember(s => s.AssetTypeLevel1, d => d.Ignore())
               .ForMember(s => s.AssetType, d => d.Ignore())
               .ReverseMap();
            CreateMap<AssetTypeLevel2DetailViewModel, AssetTypeLevel2>().ReverseMap();
            CreateMap<AssetTypeLevel2BriefViewModel, AssetTypeLevel2>().ReverseMap();
            CreateMap<AssetTypeLevel2ModifyViewModel, AssetTypeLevel2DetailViewModel>().ReverseMap();

            //AssetAssociation
            CreateMap<AssetAssociationDetailViewModel, AssetAssociation>()
                .ForMember(s => s.AssetId, d => d.MapFrom(x => x.Asset.Id))
                .ForMember(s => s.Asset, d => d.Ignore())
                .ForMember(s => s.AssetTypeId, d => d.MapFrom(x => x.AssetType.Id))
                .ForMember(s => s.AssetType, d => d.Ignore())
                .ForMember(s => s.AssetTypeLevel1Id, d => d.MapFrom(x => x.AssetTypeLevel1.Id))
                .ForMember(s => s.AssetTypeLevel1, d => d.Ignore())
                .ForMember(s => s.AssetTypeLevel2Id, d => d.MapFrom(x => x.AssetTypeLevel2.Id))
                .ForMember(s => s.AssetTypeLevel2, d => d.Ignore())
                .ReverseMap();

            //Techinician
            CreateMap<TechnicianUpdateViewModel, ApplicationUser>()
                .ForMember(s => s.Id, d => d.Ignore())
                .ForMember(s => s.PinCode, d => d.Ignore())
                .ReverseMap();
            CreateMap<TechnicianUpdateViewModel, TechnicianDetailViewModel>().ReverseMap();
            CreateMap<BaseSelect2VM, TechnicianBriefViewModel>().ReverseMap();
            CreateMap<ApplicationUser, TechnicianBriefViewModel>().ReverseMap();
            CreateMap<TechnicianDetailViewModel, ApplicationUser>().ReverseMap();
            CreateMap<TechnicianBriefViewModel, TechnicianBriefViewModel>().ReverseMap();

            //MUTCD
            CreateMap<MUTCDModifyViewModel, MUTCD>().ReverseMap();
            CreateMap<MUTCDDetailViewModel, MUTCD>().ReverseMap();
            CreateMap<MUTCDBriefViewModel, MUTCD>().ReverseMap();
            CreateMap<MUTCDModifyViewModel, MUTCDDetailViewModel>().ReverseMap();

            //Equipment
            CreateMap<EquipmentModifyViewModel, Equipment>()
                .ForMember(s => s.CategoryId, d => d.MapFrom(x => x.Category.Id))
                .ForMember(s => s.Category, d => d.Ignore())
                .ForMember(s => s.ManufacturerId, d => d.MapFrom(x => x.Manufacturer.Id))
                .ForMember(s => s.Manufacturer, d => d.Ignore())
                .ForMember(s => s.UOM, d => d.MapFrom(x => x.UOM.Id))
                .ForMember(s => s.UOM, d => d.Ignore())
                .ReverseMap();
            CreateMap<EquipmentDetailViewModel, Equipment>()
                .ForMember(s => s.CategoryId, d => d.MapFrom(x => x.Category.Id))
                .ForMember(s => s.Category, d => d.Ignore())
                .ForMember(s => s.ManufacturerId, d => d.MapFrom(x => x.Manufacturer.Id))
                .ForMember(s => s.Manufacturer, d => d.Ignore())
                .ForMember(s => s.UOMId, d => d.MapFrom(x => x.UOM.Id))
                .ForMember(s => s.UOM, d => d.Ignore()).ReverseMap();
            CreateMap<EquipmentBriefViewModel, Equipment>().ReverseMap();
            CreateMap<EquipmentModifyViewModel, EquipmentDetailViewModel>().ReverseMap();
            CreateMap<EquipmentBriefViewModel, EquipmentDetailViewModel>().ReverseMap();

            //EquipmentNotes
            CreateMap<EquipmentNotesViewModel, EquipmentNotes>()
               .ReverseMap();

            //EquipmentTransaction
            CreateMap<EquipmentTransactionModifyViewModel, EquipmentTransaction>()
                .ForMember(s => s.SupplierId, d => d.MapFrom(x => x.Supplier.Id))
                .ForMember(s => s.Supplier, d => d.Ignore())
                .ForMember(s => s.LocationId, d => d.MapFrom(x => x.Location.Id))
                .ForMember(s => s.Location, d => d.Ignore())
                .ForMember(s => s.ConditionId, d => d.MapFrom(x => x.Condition.Id))
                .ForMember(s => s.Condition, d => d.Ignore())
                .ForMember(s => s.Equipment, d => d.Ignore())
                .ReverseMap();

            CreateMap<EquipmentTransactionIssueViewModel, EquipmentTransaction>()
                .ForMember(s => s.SupplierId, d => d.MapFrom(x => x.Supplier.Id))
                .ForMember(s => s.Supplier, d => d.Ignore())
                .ForMember(s => s.LocationId, d => d.MapFrom(x => x.Location.Id))
                .ForMember(s => s.Location, d => d.Ignore())
                .ForMember(s => s.ConditionId, d => d.MapFrom(x => x.Condition.Id))
                .ForMember(s => s.Condition, d => d.Ignore())
                .ForMember(s => s.Equipment, d => d.Ignore())
            .ReverseMap();

            CreateMap<EquipmentTransactionDetailViewModel, EquipmentTransaction>()
                .ForMember(s => s.SupplierId, d => d.MapFrom(x => x.Supplier.Id))
                .ForMember(s => s.Supplier, d => d.Ignore())
                .ForMember(s => s.LocationId, d => d.MapFrom(x => x.Location.Id))
                .ForMember(s => s.Location, d => d.Ignore())
                .ForMember(s => s.ConditionId, d => d.MapFrom(x => x.Condition.Id))
                .ForMember(s => s.Condition, d => d.Ignore())
                .ForMember(s => s.Equipment, d => d.Ignore())
                .ReverseMap();

            CreateMap<IssueEquipmentItemListViewModel, EquipmentTransaction>()
                .ForMember(s => s.SupplierId, d => d.MapFrom(x => x.Supplier.Id))
                .ForMember(s => s.Supplier, d => d.Ignore())
                .ForMember(s => s.LocationId, d => d.MapFrom(x => x.Location.Id))
                .ForMember(s => s.Location, d => d.Ignore())
                .ForMember(s => s.ConditionId, d => d.MapFrom(x => x.Condition.Id))
                .ForMember(s => s.Condition, d => d.Ignore())
                .ForMember(s => s.Equipment, d => d.Ignore())
                .ReverseMap();

            CreateMap<EquipmentTransactionDetailViewModel, IssueEquipmentItemListViewModel>().ReverseMap();

            CreateMap<EquipmentTransactionModifyViewModel, ShipmentDetailViewModel>().ReverseMap();

            CreateMap<EquipmentTransactionIssueViewModel, ReStageListViewModel>().ReverseMap();

            CreateMap<EquipmentTransactionIssueViewModel, RemoveInventoryItemsListViewModel>().ReverseMap();

            CreateMap<EquipmentShipmentModifyViewModel, EquipmentTransaction>()
                .ForMember(s => s.SupplierId, d => d.MapFrom(x => x.Supplier.Id))
                .ForMember(s => s.Supplier, d => d.Ignore())
                .ForMember(s => s.LocationId, d => d.MapFrom(x => x.Location.Id))
                .ForMember(s => s.Location, d => d.Ignore())
                .ForMember(s => s.ConditionId, d => d.MapFrom(x => x.Condition.Id))
                .ForMember(s => s.Condition, d => d.Ignore())
                .ForMember(s => s.Equipment, d => d.Ignore())
                .ReverseMap();
            CreateMap<EquipmentShipmentDetailViewModel, EquipmentTransaction>()
                .ForMember(s => s.SupplierId, d => d.MapFrom(x => x.Supplier.Id))
                .ForMember(s => s.Supplier, d => d.Ignore())
                .ForMember(s => s.LocationId, d => d.MapFrom(x => x.Location.Id))
                .ForMember(s => s.Location, d => d.Ignore())
                .ForMember(s => s.ConditionId, d => d.MapFrom(x => x.Condition.Id))
                .ForMember(s => s.Condition, d => d.Ignore())
                .ForMember(s => s.Equipment, d => d.Ignore())
                .ReverseMap();
            CreateMap<EquipmentShipmentBriefViewModel, EquipmentTransaction>().ReverseMap();
            CreateMap<EquipmentShipmentModifyViewModel, EquipmentShipmentDetailViewModel>().ReverseMap();

            //WorkOrderTasks 
            CreateMap<WorkOrderTasksModifyViewModel, WorkOrderTasks>().ReverseMap();
            CreateMap<WorkOrderTasksDetailViewModel, WorkOrderTasks>().ReverseMap();

            //MountType
            CreateMap<MountTypeModifyViewModel, MountType>().ReverseMap();
            CreateMap<MountTypeDetailViewModel, MountType>().ReverseMap();
            CreateMap<MountTypeBriefViewModel, MountType>().ReverseMap();
            CreateMap<MountTypeModifyViewModel, MountTypeDetailViewModel>().ReverseMap();

            //Workorder 
            //  CreateMap<WorkOrderBriefViewModel, Asset>().ReverseMap();
            CreateMap<AssetExcelSheetViewModel, Asset>()
              .ForMember(s => s.AssetType, d => d.Ignore())
              .ForMember(s => s.MUTCD, d => d.Ignore())
              .ForMember(s => s.MountType, d => d.Ignore())
              .ForMember(s => s.ConditionId, d => d.Ignore())
              .ForMember(s => s.ManufacturerId, d => d.Ignore())
              .ReverseMap();

            //StreetServiceRequest
            CreateMap<StreetServiceRequestModifyViewModel, StreetServiceRequest>().ReverseMap();
            CreateMap<StreetServiceRequestDetailViewModel, StreetServiceRequest>().ReverseMap();
            CreateMap<StreetServiceRequestBriefViewModel, StreetServiceRequest>().ReverseMap();
            CreateMap<StreetServiceRequestModifyViewModel, StreetServiceRequestDetailViewModel>().ReverseMap();

            //TaskType
            CreateMap<TaskTypeModifyViewModel, TaskType>()
                .ForMember(s => s.TaskEquipment, d => d.Ignore())
                .ForMember(s => s.TaskLabors, d => d.Ignore())
                .ForMember(s => s.TaskMaterials, d => d.Ignore())
                .ForMember(s => s.TaskWorkSteps, d => d.Ignore())
                .ReverseMap();
            CreateMap<TaskTypeDetailViewModel, TaskType>().ReverseMap();
            CreateMap<TaskTypeBriefViewModel, TaskType>()
                .ForMember(s => s.Code, d => d.MapFrom(x => x.Select2Text))
                .ReverseMap();
            CreateMap<TaskTypeBriefViewModel, TaskTypeDetailViewModel>().ReverseMap();
            CreateMap<TaskTypeModifyViewModel, TaskTypeDetailViewModel>().ReverseMap();

            //WorkSteps mappings
            CreateMap<TaskWorkStepViewModel, TaskWorkStep>().ReverseMap();
            CreateMap<TaskLaborViewModel, TaskLabor>()
                .ForMember(s => s.CraftSkillId, d => d.MapFrom(x => x.CraftSkill.Id))
                .ForMember(s => s.CraftSkill, d => d.Ignore())
                .ReverseMap();
            CreateMap<TaskMaterialViewModel, TaskMaterial>()
                .ForMember(s => s.MaterialId, d => d.MapFrom(x => x.Material.Id))
                .ForMember(s => s.Material, d => d.Ignore())
                .ReverseMap();
            CreateMap<TaskEquipmentViewModel, TaskEquipment>()
                .ForMember(s => s.EquipmentId, d => d.MapFrom(x => x.Equipment.Id))
                .ForMember(s => s.Equipment, d => d.Ignore())
                .ReverseMap();


            //Ticket
            CreateMap<TicketModifyViewModel, Ticket>().ReverseMap();
            CreateMap<TicketDetailViewModel, Ticket>().ReverseMap();
            CreateMap<TicketBriefViewModel, Ticket>().ReverseMap();
            CreateMap<TicketStatusUpdateViewModel, Ticket>().ReverseMap();
            CreateMap<TicketStatusUpdateViewModel, TicketModifyViewModel>().ReverseMap();
            CreateMap<TicketStatusUpdateViewModel, TicketDetailViewModel>().ReverseMap();
            CreateMap<TicketDetailViewModel, TicketStatusUpdateViewModel>().ReverseMap();
            CreateMap<TicketModifyViewModel, TicketDetailViewModel>().ReverseMap();

            CreateMap<Ticket, TicketHistory>()
                .ForMember(s => s.TicketId, d => d.MapFrom(x => x.Id))
                .ForMember(s => s.ShopperId, d => d.MapFrom(x => x.CreatedBy))
                .ForMember(s => s.Id, d => d.Ignore())
                .ForMember(s => s.Shopper, d => d.Ignore())
                .ReverseMap();

            //UserSearchSetting
            CreateMap<UserSearchSettingModifyViewModel, UserSearchSetting>().ReverseMap();
            CreateMap<UserSearchSettingDetailViewModel, UserSearchSetting>().ReverseMap();
            CreateMap<UserSearchSettingBriefViewModel, UserSearchSetting>().ReverseMap();
            CreateMap<UserSearchSettingBriefViewModel, UserSearchSettingDetailViewModel>().ReverseMap();

            //DynamicColumn
            CreateMap<DynamicColumnModifyViewModel, DynamicColumn>()
                .ForMember(s => s.Options, d => d.Ignore())
                .ForMember(s => s.Values, d => d.Ignore())
                .ReverseMap();
            CreateMap<DynamicColumnDetailViewModel, DynamicColumn>()
                .ForMember(s => s.Options, d => d.Ignore())
                .ForMember(s => s.Values, d => d.Ignore())
                .ReverseMap();
            CreateMap<DynamicColumnBriefViewModel, DynamicColumn>().ReverseMap();
            CreateMap<DynamicColumnBriefViewModel, DynamicColumnDetailViewModel>().ReverseMap();
            CreateMap<DynamicColumnModifyViewModel, DynamicColumnDetailViewModel>().ReverseMap();

            //DynamicColumnOption
            CreateMap<DynamicColumnOptionModifyViewModel, DynamicColumnOption>()
                .ForMember(s => s.DynamicColumnId, d => d.MapFrom(x => x.DynamicColumn.Id))
                .ForMember(s => s.DynamicColumn, d => d.Ignore())
                .ReverseMap();
            CreateMap<DynamicColumnOptionDetailViewModel, DynamicColumnOption>()
                .ForMember(s => s.DynamicColumnId, d => d.MapFrom(x => x.DynamicColumn.Id))
                .ForMember(s => s.DynamicColumn, d => d.Ignore())
                .ReverseMap();
            CreateMap<DynamicColumnOptionBriefViewModel, DynamicColumnOption>().ReverseMap();
            CreateMap<DynamicColumnOptionBriefViewModel, DynamicColumnOptionDetailViewModel>().ReverseMap();

            //DynamicColumnValue
            CreateMap<DynamicColumnValueModifyViewModel, DynamicColumnValue>()
                .ForMember(s => s.Value, d => d.MapFrom(x => x.Value ?? ""))
                .ForMember(s => s.DynamicColumnId, d => d.MapFrom(x => x.DynamicColumn.Id))
                .ForMember(s => s.DynamicColumn, d => d.Ignore())
                .ReverseMap();
            CreateMap<DynamicColumnValueDetailViewModel, DynamicColumnValue>()
                .ForMember(s => s.Value, d => d.MapFrom(x => x.Value ?? ""))
                .ForMember(s => s.DynamicColumnId, d => d.MapFrom(x => x.DynamicColumn.Id))
                .ForMember(s => s.DynamicColumn, d => d.Ignore())
                .ReverseMap();

            //Timesheet
            CreateMap<TimesheetModifyViewModel, Timesheet>()
                 .ForMember(src => src.ApproverId, opt => opt.MapFrom(dest => dest.Approver.Id))
                 .ForMember(x => x.Approver, opt => opt.Ignore())
                 .ReverseMap();
            CreateMap<Timesheet, TimesheetDetailViewModel>()
                .ForMember(src => src.TimesheetBreakdowns, opt => opt.MapFrom(dest => dest.TimesheetBreakdowns))
                .AfterMap((src, dest) =>
                {
                    dest.ProcessBreakdowns(dest);
                })
                .ReverseMap();
            CreateMap<Timesheet, TimesheetDetailForBreakdownViewModel>()
                .ForMember(src => src.TimesheetBreakdowns, opt => opt.MapFrom(dest => dest.TimesheetBreakdowns))
                .AfterMap((src, dest) =>
                {
                    dest.ProcessBreakdowns(dest);
                })
                .ReverseMap();
            CreateMap<TimeSheetWithBreakdownViewModel, Timesheet>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.ApproverId, opt => opt.Ignore())
                .ForMember(x => x.Approver, opt => opt.Ignore())
                .ForMember(x => x.WeekEnding, opt => opt.Ignore())
                .ForMember(src => src.TimesheetBreakdowns, opt => opt.MapFrom(dest => dest.TimesheetBreakdowns))
                ;
            CreateMap<Timesheet, TimeSheetWithBreakdownViewModel>()
                .ForMember(x => x.Approver, opt => opt.Ignore())
                .ForMember(src => src.TimesheetBreakdowns, opt => opt.MapFrom(dest => dest.TimesheetBreakdowns))
                .AfterMap((src, dest) =>
                {
                    dest.ProcessBreakdowns(dest);
                });
            CreateMap<Timesheet, TimesheetReportViewModel>().ReverseMap();
            CreateMap<TimesheetModifyViewModel, TimesheetDetailViewModel>()
                .AfterMap((src, dest) =>
                {
                    dest.ProcessBreakdowns(dest);
                })
                .ReverseMap();
            CreateMap<TimesheetBreakdown, TimesheetBreakdownDetailForReportViewModel>()
                .ForMember(src => src.Timesheet, opt => opt.MapFrom(dest => dest.Timesheet))
                .ReverseMap();
            CreateMap<TimesheetBreakdown, TimesheetBreakdownDetailViewModel>().ReverseMap();
            CreateMap<TimesheetBriefViewModel, TimesheetWebUpdateViewModel>().ReverseMap();
            CreateMap<TimesheetBriefViewModel, Timesheet>().ReverseMap();
            CreateMap<TimesheetBreakdownUpdateViewModel, TimesheetWebUpdateViewModel>().ReverseMap();

            // IncidentTeam
            CreateMap<IncidentTeamModifyViewModel, IncidentTeam>().ReverseMap();
            CreateMap<IncidentTeamDetailViewModel, IncidentTeam>().ReverseMap();
            CreateMap<IncidentTeamBriefViewModel, IncidentTeam>().ReverseMap();
            CreateMap<IncidentTeamModifyViewModel, IncidentTeamDetailViewModel>().ReverseMap();
            //StatusLegend
            CreateMap<StatusLegendModifyViewModel, StatusLegend>().ReverseMap();
            CreateMap<StatusLegendDetailViewModel, StatusLegend>().ReverseMap();
            CreateMap<StatusLegendBriefViewModel, StatusLegend>().ReverseMap();
            CreateMap<StatusLegendModifyViewModel, StatusLegendDetailViewModel>().ReverseMap();

            //AssignResponseTeams
            CreateMap<AssignResponseTeamsModifyViewModel, AssignResponseTeams>().ReverseMap();
            CreateMap<AssignResponseTeamsDetailViewModel, AssignResponseTeams>().ReverseMap();
            CreateMap<AssignResponseTeamsBriefViewModel, AssignResponseTeams>().ReverseMap();
            CreateMap<AssignResponseTeamsModifyViewModel, AssignResponseTeamsDetailViewModel>().ReverseMap();

            // Policy
            CreateMap<PolicyModifyViewModel, Policy>().ReverseMap();
            CreateMap<PolicyDetailViewModel, Policy>().ReverseMap();
            CreateMap<PolicyBriefViewModel, Policy>().ReverseMap();
            CreateMap<PolicyModifyViewModel, PolicyDetailViewModel>().ReverseMap();

            // UsersTeamSettings
            CreateMap<UserModifyViewModel, IncidentUser>().ReverseMap();
            CreateMap<UserDetailViewModel, IncidentUser>().ReverseMap();
            CreateMap<UserBriefViewModel, IncidentUser>().ReverseMap();
            CreateMap<UserModifyViewModel, UserDetailViewModel>().ReverseMap();
            //IgnoreGlobalProperties();

            // Progress
            CreateMap<ProgressModifyViewModel, Progress>().ReverseMap();
            CreateMap<ProgressDetailViewModel, Progress>().ReverseMap();
            CreateMap<ProgressBriefViewModel, Progress>().ReverseMap();
            CreateMap<ProgressModifyViewModel, ProgressDetailViewModel>().ReverseMap();

            // Material
            CreateMap<MaterialModifyViewModel, Material>().ReverseMap();
            CreateMap<MaterialDetailViewModel, Material>().ReverseMap();
            CreateMap<MaterialBriefViewModel, Material>().ReverseMap();
            CreateMap<MaterialModifyViewModel, MaterialDetailViewModel>().ReverseMap();


            //EquipmentFields
            CreateMap<EquipmentFieldsModifyViewModel, EquipmentFields>().ReverseMap();
            CreateMap<EquipmentFieldsDetailViewModel, EquipmentFields>().ReverseMap();
            CreateMap<EquipmentFieldsBriefViewModel, EquipmentFields>().ReverseMap();
            CreateMap<EquipmentFieldsModifyViewModel, EquipmentFieldsDetailViewModel>().ReverseMap();

            // IncidentRole
            CreateMap<IncidentRoleModifyViewModel, IncidentRole>().ReverseMap();
            CreateMap<IncidentRoleDetailViewModel, IncidentRole>().ReverseMap();
            CreateMap<IncidentRoleBriefViewModel, IncidentRole>().ReverseMap();
            CreateMap<IncidentRoleModifyViewModel, IncidentRoleDetailViewModel>().ReverseMap();

            // IncidentShift
            CreateMap<IncidentShiftModifyViewModel, IncidentShift>().ReverseMap();
            CreateMap<IncidentShiftDetailViewModel, IncidentShift>().ReverseMap();
            CreateMap<IncidentShiftBriefViewModel, IncidentShift>().ReverseMap();
            CreateMap<IncidentShiftModifyViewModel, IncidentShiftDetailViewModel>().ReverseMap();



        }
        //private void IgnoreGlobalProperties()
        //{
        //    var properties = typeof(BaseVM).GetProperties();
        //    foreach (var property in properties.Select(x => x.Name).ToList())
        //    {
        //        if (property != "Id")
        //            AddGlobalIgnore(property);
        //    }
        //}

    }
}
