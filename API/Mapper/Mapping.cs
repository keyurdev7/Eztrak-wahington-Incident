using AutoMapper;
using Models;
using ViewModels.Timesheet;
using ViewModels.Technician;
using ViewModels.Users;
using ViewModels;
using Pagination;
using ViewModels.Shared;
using ViewModels.WorkOrderTechnician;
using ViewModels.WorkOrder;
using Models.Models.Shared;

namespace API.Mapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TechnicianDetailViewModel, ApplicationUser>().ReverseMap();
            CreateMap<ApplicationUser, UserBriefViewModel>().ReverseMap();
            CreateMap<ApplicationUser, UserDetailViewModel>().ReverseMap();
            CreateMap<ApplicationUser, TechnicianBriefViewModel>().ReverseMap();

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
                .ForMember(src => src.ApproveStatus, opt => opt.MapFrom(dest => dest.ApproveStatus))
                .ForMember(src => src.TimesheetBreakdowns, opt => opt.MapFrom(dest => dest.TimesheetBreakdowns));
            CreateMap<Timesheet, TimeSheetWithBreakdownViewModel>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Approver, opt => opt.Ignore())
                .ForMember(x => x.WeekEnding, opt => opt.Ignore())
                .ForMember(src => src.ApproveStatus, opt => opt.MapFrom(dest => dest.ApproveStatus))
                .ForMember(src => src.TimesheetBreakdowns, opt => opt.MapFrom(dest => dest.TimesheetBreakdowns))
                 .AfterMap((src, dest) =>
                 {
                     dest.ProcessBreakdowns(dest);
                 })
                .ReverseMap();
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
            CreateMap<TimesheetBriefViewModel, Timesheet>().ReverseMap();

            //AssetTypeLevel1
            CreateMap<AssetTypeLevel1ModifyViewModel, AssetTypeLevel1>()
               .ForMember(s => s.AssetType, d => d.Ignore())
                .ReverseMap();
            CreateMap<AssetTypeLevel1DetailViewModel, AssetTypeLevel1>().ReverseMap();
            CreateMap<AssetTypeLevel1BriefViewModel, AssetTypeLevel1>().ReverseMap();
            CreateMap<AssetTypeLevel1ModifyViewModel, AssetTypeLevel1DetailViewModel>().ReverseMap();
            CreateMap<AssetTypeLevel1DetailAPIViewModel, AssetTypeLevel1DetailViewModel>().ReverseMap();
            CreateMap<BaseMinimalVM, AssetTypeLevel1DetailViewModel>().ReverseMap();
            CreateMap<AssetTypeLevel1DetailAPIViewModel, AssetTypeLevel1BriefViewModel>().ReverseMap();

            //AssetTypeLevel2
            CreateMap<AssetTypeLevel2ModifyViewModel, AssetTypeLevel2>()
               .ForMember(s => s.AssetTypeLevel1, d => d.Ignore())
               .ForMember(s => s.AssetType, d => d.Ignore())
               .ReverseMap();
            CreateMap<AssetTypeLevel2DetailViewModel, AssetTypeLevel2>().ReverseMap();
            CreateMap<AssetTypeLevel2BriefViewModel, AssetTypeLevel2>().ReverseMap();
            CreateMap<BaseMinimalVM, AssetTypeLevel2>().ReverseMap();
            CreateMap<BaseMinimalVM, AssetTypeLevel2DetailViewModel>().ReverseMap();
            CreateMap<AssetTypeLevel2ModifyViewModel, AssetTypeLevel2DetailViewModel>().ReverseMap();
            CreateMap<AssetTypeLevel2DetailAPIViewModel, AssetTypeLevel2DetailViewModel>().ReverseMap();

            //AssetAssociation
            CreateMap<AssetAssociationDetailViewModel, AssetAssociation>()
                .ForPath(s => s.Asset, d => d.Ignore())
                .ForPath(s => s.AssetType, d => d.Ignore())
                .ForPath(s => s.AssetTypeLevel1, d => d.Ignore())
                .ForPath(s => s.AssetTypeLevel2, d => d.Ignore())
                .ReverseMap();
            CreateMap<AssetAssociationDetailViewModel, MapAssetAssociationDetailViewModel>().ReverseMap();
            CreateMap<AssetAssociationAPIViewModel, AssetAssociationDetailViewModel>()
                .ForPath(s => s.Asset, d => d.Ignore())
                .ForPath(s => s.AssetType.Id, d => d.MapFrom(x => x.AssetTypeId))
                .ForPath(s => s.AssetType, d => d.Ignore())
                .ForPath(s => s.AssetTypeLevel1.Id, d => d.MapFrom(x => x.AssetTypeLevel1Id))
                .ForPath(s => s.AssetTypeLevel1, d => d.Ignore())
                .ForPath(s => s.AssetTypeLevel2.Id, d => d.MapFrom(x => x.AssetTypeLevel2Id))
                .ForPath(s => s.AssetTypeLevel2, d => d.Ignore())
                .ReverseMap();
            CreateMap<AssetAssociationDetailAPIViewModel, AssetAssociationDetailViewModel>()
               .ForPath(s => s.Asset.Id, d => d.MapFrom(x => x.AssetId))
               .ForPath(s => s.AssetType.Id, d => d.MapFrom(x => x.AssetTypeId))
               .ForPath(s => s.AssetTypeLevel2.Id, d => d.MapFrom(x => x.AssetTypeLevel1.SelectedAssetTypeLevel2Id))
               .ForPath(s => s.Asset, d => d.Ignore())
               .ForPath(s => s.AssetType, d => d.Ignore())
               .ForPath(s => s.AssetTypeLevel2, d => d.Ignore())
               .ForPath(s => s.Id, d => d.Ignore())
           .ReverseMap();


            //Asset
            CreateMap<AssetModifyAPIViewModel, AssetModifyViewModel>()
                .ForPath(s => s.Condition.Id, d => d.MapFrom(x => x.ConditionId))
                .ForPath(s => s.Condition, d => d.Ignore())
                .ForPath(s => s.Manufacturer.Id, d => d.MapFrom(x => x.ManufacturerId))
                .ForPath(s => s.Manufacturer, d => d.Ignore())
                .ForPath(s => s.AssetType.Id, d => d.MapFrom(x => x.AssetTypeId))
                .ForPath(s => s.AssetType, d => d.Ignore())
                .ForPath(s => s.MUTCD.Id, d => d.MapFrom(x => x.MUTCDId))
                .ForPath(s => s.MUTCD, d => d.Ignore())
                .ForPath(s => s.MountType, d => d.Ignore())
                .ReverseMap();
            CreateMap<AssetCreateAPIViewModel, AssetModifyViewModel>()
                .ForPath(s => s.Condition.Id, d => d.MapFrom(x => x.ConditionId))
                .ForPath(s => s.Condition, d => d.Ignore())
                .ForPath(s => s.Manufacturer.Id, d => d.MapFrom(x => x.ManufacturerId))
                .ForPath(s => s.Manufacturer, d => d.Ignore())
                .ForPath(s => s.AssetType.Id, d => d.MapFrom(x => x.AssetTypeId))
                .ForPath(s => s.AssetType, d => d.Ignore())
                .ForPath(s => s.MUTCD.Id, d => d.MapFrom(x => x.MUTCDId))
                .ForPath(s => s.MUTCD, d => d.Ignore())
                .ForPath(s => s.MountType, d => d.Ignore())
                .ReverseMap();
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
            CreateMap<AssetDetailViewModel, AssetDetailAPIViewModel>().ReverseMap();
            CreateMap<AssetDetailViewModel, MaintenanceReportViewModel>().ReverseMap();
            CreateMap<PaginatedResultModel<AssetDetailViewModel>, PaginatedResultModel<MaintenanceReportViewModel>>().ReverseMap();
            CreateMap<PaginatedResultModel<AssetDetailViewModel>, PaginatedResultModel<RepairingReportViewModel>>().ReverseMap();
            CreateMap<AssetDetailViewModel, RepairingReportViewModel>().ReverseMap();
            CreateMap<AssetBriefViewModel, Asset>().ReverseMap();
            CreateMap<AssetModifyViewModel, AssetDetailViewModel>().ReverseMap();
            CreateMap<BaseMinimalVM, AssetTypeBriefViewModel>().ReverseMap();

            CreateMap<AssetMapViewModel, AssetDetailViewModel>()
             .ForPath(d => d.Condition.Name, opt => opt.MapFrom(s => s.Condition))
             .ForPath(d => d.AssetType.Name, opt => opt.MapFrom(s => s.AssetType))
             .ForPath(d => d.SystemGeneratedId, opt => opt.MapFrom(s => s.AssetId))
             .ReverseMap();

            CreateMap<MapBriefVM, AssetBriefViewModel>().ReverseMap();
            CreateMap<MapBriefVM, AssetTypeBriefViewModel>().ReverseMap();
            CreateMap<MapBriefVM, AssetTypeLevel1BriefViewModel>().ReverseMap();
            CreateMap<MapBriefVM, AssetTypeLevel2BriefViewModel>().ReverseMap();
            CreateMap<AssetMapViewModel, AssetDetailViewModel>()
           .ForPath(d => d.Condition.Name, opt => opt.MapFrom(s => s.Condition))
           .ForPath(d => d.AssetType.Name, opt => opt.MapFrom(s => s.AssetType))
           .ReverseMap();

            //AssestType
            CreateMap<AssetTypeModifyViewModel, AssetType>().ReverseMap();
            CreateMap<AssetTypeBriefViewModel, AssetType>().ReverseMap();
            CreateMap<AssetTypeDetailViewModel, AssetType>().ReverseMap();
            CreateMap<AssetTypeBriefViewModel, AssetTypeDetailViewModel>().ReverseMap();
            CreateMap<BaseMinimalVM, AssetTypeDetailViewModel>().ReverseMap();
            CreateMap<BaseMinimalVM, AssetTypeBriefViewModel>().ReverseMap();

            //Condition
            CreateMap<ConditionModifyViewModel, Condition>().ReverseMap();
            CreateMap<ConditionDetailViewModel, Condition>().ReverseMap();
            CreateMap<ConditionBriefViewModel, Condition>().ReverseMap();
            CreateMap<ConditionBriefViewModel, ConditionDetailViewModel>().ReverseMap();
            CreateMap<BaseMinimalVM, ConditionDetailViewModel>().ReverseMap();
            CreateMap<BaseMinimalVM, ConditionBriefViewModel>().ReverseMap();

            //MUTCD
            CreateMap<MUTCDModifyViewModel, MUTCD>().ReverseMap();
            CreateMap<MUTCDDetailViewModel, MUTCD>().ReverseMap();
            CreateMap<MUTCDBriefViewModel, MUTCD>().ReverseMap();
            CreateMap<MUTCDBriefViewModel, MUTCDDetailViewModel>().ReverseMap();
            CreateMap<BaseMinimalVM, MUTCDDetailViewModel>()
                .ForMember(x => x.Description, s => s.MapFrom(x => x.Name))
                .ReverseMap();
            CreateMap<BaseMinimalVM, MUTCDBriefViewModel>().ReverseMap();

            //Manufacturer
            CreateMap<ManufacturerModifyViewModel, Manufacturer>().ReverseMap();
            CreateMap<ManufacturerDetailViewModel, Manufacturer>().ReverseMap();
            CreateMap<ManufacturerBriefViewModel, Manufacturer>().ReverseMap();
            CreateMap<ManufacturerBriefViewModel, ManufacturerDetailViewModel>().ReverseMap();
            CreateMap<BaseMinimalVM, ManufacturerDetailViewModel>().ReverseMap();

            //Condition
            CreateMap<ConditionModifyViewModel, Condition>().ReverseMap();
            CreateMap<ConditionDetailViewModel, Condition>().ReverseMap();
            CreateMap<ConditionBriefViewModel, Condition>().ReverseMap();
            CreateMap<ConditionBriefViewModel, ConditionDetailViewModel>().ReverseMap();
            CreateMap<BaseMinimalVM, ConditionDetailViewModel>().ReverseMap();

            //WorkOrder
            CreateMap<WorkOrderModifyViewModel, WorkOrder>()
                .ForMember(s => s.RepairId, d => d.MapFrom(x => x.Repair.Id))
                .ForMember(s => s.Repair, d => d.Ignore())
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
            CreateMap<WorkOrderModifyStatusAPIViewModel, WorkOrderModifyStatusViewModel>().ReverseMap();

            CreateMap<WorkOrderDetailAPIViewModel, WorkOrderDetailViewModel>()
                 .ForMember(s => s.CreatedOn, d => d.MapFrom(x => x.DueDate))
                .ReverseMap();
            CreateMap<WorkOrderBriefAPIViewModel, WorkOrderDetailViewModel>()
                 .ForMember(s => s.CreatedOn, d => d.MapFrom(x => x.DueDate))
                .ReverseMap();

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

            //WorkOrderAsset
            CreateMap<AssetWorkOrderAPIViewModel, WOAssetViewModel>()
                .ReverseMap();

            //WorkOrderTechnician
            CreateMap<WorkOrderTechnicianModifyViewModel, WorkOrderTechnician>()
                .ForMember(s => s.CraftSkillId, d => d.MapFrom(x => x.CraftSkill.Id))
                .ForMember(s => s.CraftSkill, d => d.Ignore())
                 .ForMember(s => s.TechnicianId, d => d.MapFrom(x => x.Technician.Id))
                .ForMember(s => s.Technician, d => d.Ignore())
                .ForMember(s => s.WorkOrder, d => d.Ignore())
                .ReverseMap();

            //TaskType
            CreateMap<TaskTypeBriefViewModel, BaseMinimalVM>()
                .ForMember(s => s.Name, d => d.MapFrom(x => x.Select2Text));
            CreateMap<TaskTypeBriefViewModel, TaskTypeDetailViewModel>().ReverseMap();

            //AssetNotes
            CreateMap<AssetNotesAPIViewModel, AssetNotesViewModel>().ReverseMap();
            CreateMap<AssetNotesViewModel, AssetNotes>().ReverseMap();

            //WorkOrderNotes
            CreateMap<WorkOrderNotesAPIViewModel, WorkOrderNotesViewModel>().ReverseMap();
            CreateMap<WorkOrderNotesViewModel, WorkOrderNotes>().ReverseMap();

            //Manufacturer
            CreateMap<BaseMinimalVM, ManufacturerBriefViewModel>().ReverseMap();

            //MountType
            CreateMap<BaseMinimalVM, MountTypeBriefViewModel>().ReverseMap();

            //Attachment
            CreateMap<AttachmentVM, Attachment>().ReverseMap();

            CreateMap<CraftSkill, CraftSkillBriefViewModel>().ReverseMap();

        }
    }
}
