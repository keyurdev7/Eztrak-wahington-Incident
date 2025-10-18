using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;

using Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ViewModels.Dashboard;

namespace ViewModels.Incident
{
    public class IncidentViewModel
    {
        public IncidentCellerInformationViewModel incidentCellerInformation { get; set; } = new();
        public IncidentiLocationViewModel incidentiLocation { get; set; } = new();
        public IncidentDetailsViewModel incidentDetails { get; set; } = new();
        public IncidentEnvironmentalViewModel incidentEnvironmentalViewModel { get; set; } = new();
        public IncidentSupportingInfoViewModel incidentSupportingInfoViewModel { get; set; } = new();
        public IncidentDetailByIdViewModel incidentDetailByIdViewModel { get; set; } = new();
        public List<IncidentGridViewModel> incidentGridViewModel { get; set; } = new();
        public IncidentValidationsDetailsViewModel incidentValidationsDetailsViewModel { get; set; } = new();
        public List<WorkStepViewModel> workSteps { get; set; } = new();
        public List<List<WorkStepViewModel>> workStepsByPolicy { get; set; } = new();

        public List<SelectListItem> statusLegends { get; set; } = new();
        public List<SelectListItem> severityLevels { get; set; } = new();
        public List<SelectListItem> progressLevels { get; set; } = new();
        public long? severityLevelId { get; set; } = default!;
        public long? Id { get; set; } = default!;
        public string DescriptionIssue { get; set; } = default!;

        public List<IncidentLocationMapViewModel> ListIncidentLocationMapViewModel { get; set; } = new();
        public AdditionalLocationViewModel additionalLocation { get; set; } = new();
        public List<AdditionalLocationViewModel> additionalLocations { get; set; } = new();

        public IncidentValidationAssignedRolesViewModel incidentValidationAssignedRolesViewModel { get; set; } = new();
        public IncidentValidationGatesViewModel incidentValidationGatesViewModel { get; set; } = new();
        public IncidentValidationLocationViewModel IncidentValidationLocations { get; set; } = new();
        public List<IncidentMapChat> listIncidentMapChats { get; set; } = new();
        public IncidentAssessmentDetailViewModel IncidentAssessmentDetails { get; set; } = new();
        public IncidentViewAssessmentAttachmentViewModel incidentViewAssessmentAttachmentView { get; set; } = new();
        public IncidentViewAssessmentAttachmentViewModel incidentViewRestorationAttachmentView { get; set; } = new();
        public IncidentViewAssessmentAttachmentViewModel incidentViewCloseOutAttachmentView { get; set; } = new();
        public IncidentViewRestorationListViewModel IncidentViewRestorationViewModel { get; set; } = new();

        public IncidentValidationDetailViewModel IVDetails { get; set; } = new();

        #region Personnel
        public long? CompanyId { get; set; }
        public long? IncidentRoleId { get; set; }
        public class CompanyViewModel
        {
            public long CompanyId { get; set; }
            public string CompanyName { get; set; } = string.Empty;
        }
        public class IncidentRoleViewModel
        {
            public long IncidentRoleId { get; set; }
            public string IncidentRoleName { get; set; } = string.Empty;
        }
        public class ProgressStatusViewModel
        {
            public long StatusId { get; set; }
            public string StatusName { get; set; }
        }
        public class UsersViewModel
        {
            public long UsersId { get; set; }
            public string UsersName { get; set; } = string.Empty;
        }
        public List<IncidentValidationPersonnelsViewModel> incidentValidationPersonnelsViewModel { get; set; } = new();
        public IncidentValidationPersonnelsCountViewModel incidentValidationPersonnelsCountViewModel { get; set; } = new();
        public List<IncidentValidationPersonnelsTopContributorsViewModel> incidentValidationPersonnelsTopContributorsViewModel { get; set; } = new();
        #endregion

        public long IncidentViewPostType { get; set; }
        public IncidentViewRepairViewModel IncidentViewRepairViewModel { get; set; } = new();
        public IncidentViewCloseoutViewModel IncidentViewCloseoutViewModel { get; set; } = new();

        public List<SelectListItem> UserList { get; set; } = new();
        public List<SelectListItem> CompanyList { get; set; } = new();
        public List<SelectListItem> RoleList { get; set; } = new();
        public List<SelectListItem> StatusList { get; set; } = new();
        public List<SelectListItem> ShiftsList { get; set; } = new();
        public IncidentViewTaskViewModel IncidentViewTaskViewModel { get; set; } = new();
        public IncidentValidationViewModel IVValidation { get; set; } = new();
        public long TotalCompletedCloseOut { get; set; }
    }

    public class IncidentCellerInformationViewModel
    {
        public string CallerName { get; set; } = default!;
        public string CallerPhoneNumber { get; set; } = default!;
        public string CallerAddress { get; set; } = default!;
        public List<SelectListItem> Relationships { get; set; } = new();
        public long? RelationshipId { get; set; } = default!;
        public DateTime CallTime { get; set; } = default!;
        public string RelationshipName { get; set; } = string.Empty;
        public string CallDateInFormat { get; set; } = string.Empty;
        public string CallTimeInFormat { get; set; } = string.Empty;
    }
    public class IncidentiLocationViewModel
    {
        public string Address { get; set; } = default!;
        public bool IsSameCallerAddress { get; set; } = default!;
        public string Landmark { get; set; } = default!;
        public string? ServiceAccount { get; set; } = default!;
        public string AssetIDs { get; set; } = default!;
        public List<SelectListItem> AssetsIncidentList { get; set; } = new();
        public List<string> AssetNames { get; set; } = new();
    }
    public class IncidentDetailsViewModel
    {
        public string EventTypeIds { get; set; } = default!;
        public bool IsOtherEvent { get; set; } = default!;
        public string OtherEventDetail { get; set; } = default!;
        public List<SelectListItem> EventTypes { get; set; } = new();
        public List<string> EventTypeNames { get; set; } = new();
    }
    public class IncidentEnvironmentalViewModel
    {
        public long? GasodorpresentID { get; set; } = default!;
        public long? WaterPresentID { get; set; } = default!;
        public long? HissingSoundPresentID { get; set; } = default!;
        public long? VisibleDamageID { get; set; } = default!;
        public long? PeopleInjuredID { get; set; } = default!;
        public long? EvacuationRequiredID { get; set; } = default!;
        public long? EmergencyResponseNotifiedID { get; set; } = default!;
        // ✅ Friendly
        public string GasOdorText { get; set; } = string.Empty;
        public string WaterPresentText { get; set; } = string.Empty;
        public string HissingSoundText { get; set; } = string.Empty;
        public string VisibleDamageText { get; set; } = string.Empty;
        public string PeopleInjuredText { get; set; } = string.Empty;
        public string EvacuationRequiredText { get; set; } = string.Empty;
        public string EmergencyResponseNotifiedText { get; set; } = string.Empty;
    }
    public class IncidentSupportingInfoViewModel
    {
        public List<IFormFile>? File { get; set; }
        public string Notes { get; set; } = default!;
        public string? ImageUrl { get; set; } = default!;
        public List<string> ImageUrls { get; set; } = new();
    }

    public class IncidentDetailByIdViewModel
    {
        public long StatusLegendId { get; set; }
        public string SeverityName { get; set; } = string.Empty;
        public string StatusLegendName { get; set; } = string.Empty;
        public string StatusLegendColor { get; set; } = string.Empty;
        public string SeverityColor { get; set; } = string.Empty;
        public string IncidentNumber { get; set; } = string.Empty;
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string CreatedOnDate { get; set; } = string.Empty;
        public string CreatedOnTime { get; set; } = string.Empty;
    }

    public class IncidentGridViewModel
    {
        public long Id { get; set; }
        public long? StatusLegendId { get; set; }
        public string StatusLegend { get; set; }
        public string StatusLegendColor { get; set; }
        public string CallDate { get; set; }
        public string CallTime { get; set; }
        public string LocationAddress { get; set; }
        public string Intersection { get; set; }
        public string AssetId { get; set; }
        public string EventType { get; set; }
        public string EventTypeId { get; set; }
        public long? SeverityId { get; set; }
        public string Severity { get; set; }

        public long? RelationShipId { get; set; }
        public string RelationShipName { get; set; }
        public string DescriptionIssue { get; set; }
        public string GasESIndicator { get; set; }
        public int AdditionalLocationCount { get; set; } = 0;
    }
    public class ChangeStatusRequest
    {
        public long IncidentId { get; set; }
        public string Status { get; set; }
    }
    public class FilterRequest
    {
        public long severityId { get; set; }
        public long statusId { get; set; }
        public string description { get; set; }
    }

    public class GeocodeResult
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class SaveCommunicationRequest
    {
        public long IncidentId { get; set; }
        public long IncidentValidationId { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public long MessageType { get; set; }
        public List<IFormFile> File { get; set; }
    }

    public class IncidentValidationsDetailsViewModel
    {
        public long IncidentValidationId { get; set; }
        public long ConfirmedSeverityLevelId { get; set; }
        public long DiscoveryPerimeterId { get; set; }
        public string DiscoveryPerimeterName { get; set; }
        public string AssignResponseTeams { get; set; }
        public string ValidationNotes { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedDateInFormat { get; set; }
        public string CreatedTimeInFormat { get; set; }
        public string SeverityLevelName { get; set; }
        public string SeverityLevelColor { get; set; }
        public List<IncidentValidationCommunicationHistoriesViewModel> IncidentValidationCommunicationHistoriesViewModelList { get; set; }
        public List<IncidentValidationNoteViewModel> IncidentValidationNotesList { get; set; } = new();
        //public List<IncidentValidationNoteViewModel> IncidentValidationNotesList { get; set; } = new List<IncidentValidationNoteViewModel>();
    }

    public class IncidentValidationCommunicationHistoriesViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TimeStamp { get; set; } = string.Empty;
        public string ReceipientsIds { get; set; } = string.Empty;
        private string _imageUrl = string.Empty;
        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                _imageUrl = value ?? string.Empty;
                Image = string.IsNullOrWhiteSpace(_imageUrl)
                    ? string.Empty
                    : Path.GetFileName(_imageUrl);
            }
        }

        // new property that auto-extracts from ImageUrl
        public string Image { get; private set; } = string.Empty;

        public long MessageType { get; set; }
    }

    public class WorkStepViewModel
    {
        public long PolicyId { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public long TeamId { get; set; }

        // existing comma separated string
        public string PolicySteps { get; set; } = string.Empty;

        // new property: auto-splits PolicySteps into list
        public List<string> Steps
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PolicySteps))
                    return new List<string>();

                return PolicySteps
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();
            }
        }

        public string TeamsByPolicy { get; set; } = string.Empty;
    }

    public class AdditionalLocationViewModel
    {

        public long? Id { get; set; } = default!;
        public long? IncidentId { get; set; } = default!;
        public string LocationAddress { get; set; } = default!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? NearestIntersection { get; set; }
        public string? ServiceAccount { get; set; } = default!;
        public bool PerimeterType { get; set; }
        public bool IsPrimaryLocation { get; set; }
        public long? PerimeterTypeDigit { get; set; }
        public string AssetIDs { get; set; } = default!;
        public List<SelectListItem> AssetsIncidentList { get; set; } = new();
        public List<string> AssetNames { get; set; } = new();
    }

    public class IncidentAdditionalLocationViewModel
    {
        public long Id { get; set; } = default!;
        public long? IncidentId { get; set; } = default!;
        public string AdditionalLocation { get; set; } = default!;
        public double Lat { get; set; } = default!;
        public double Long { get; set; } = default!;
        public bool IsPrimaryLocation { get; set; } = default!;
    }

    public class IncidentLocationValidationViewModel
    {
        public long IncidentId { get; set; }
        public long IncidentValidationId { get; set; }
        public long IncidentLocationId { get; set; }
        public long SeverityLevelId { get; set; }
        public long DiscoveryPerimeterId { get; set; }
        public long ResponseTeamId { get; set; }
        public string TeamMemberIds { get; set; } = default!;
        public string LocationSpecificNotes { get; set; } = default!;
        public List<IncidentWorkStepViewModel> WorkSteps { get; set; } = new();
    }

    public class IncidentWorkStepViewModel
    {
        public string WorkStepName { get; set; } = default!;
        public string WorkStepDescription { get; set; } = default!;
        public string WorkStepSpecificPersion { get; set; } = default!;
    }

    public class IncidentValidationAssignedRolesViewModel
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long IncidentValidationId { get; set; }
        public long? IncidentCommanderId { get; set; }
        public long? FieldEnvRepId { get; set; }
        public long? GEC_CoordinatorId { get; set; }
        public long? EngineeringLeadId { get; set; }

        public string? IncidentCommanderName { get; set; }
        public string? FieldEnvRepName { get; set; }
        public string? GEC_CoordinatorName { get; set; }
        public string? EngineeringLeadName { get; set; }
    }

    public class IncidentValidationGatesViewModel
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long IncidentValidationId { get; set; }
        public string ContainmentAcknowledgement { get; set; } = string.Empty;
        public string Exception { get; set; } = string.Empty;
        public string IndependentInspection { get; set; } = string.Empty;
        public string Regulatory { get; set; } = string.Empty;
    }
    public class IncidentValidationNoteViewModel
    {
        public long? Id { get; set; }
        public long? IncidentId { get; set; }
        public long? IncidentValidationId { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
        public int ActiveStatus { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set; }
        public long UpdatedBy { get; set; }
    }
    #region
    public class IncidentValidationPersonnelsViewModel
    {
        public long IncidentValidationPersonnelsId { get; set; }
        public long? UserId { get; set; }
        public long? CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Shift { get; set; } = string.Empty;
        public DateTime? TimeIn { get; set; }
        public DateTime? HoursSoFar { get; set; }
        public string Supervisor { get; set; } = string.Empty;
    }
    public class IncidentValidationPersonnelsCountViewModel
    {
        public long OnsiteNowCount { get; set; }
        public long CheckedOutTodayCount { get; set; }
        public double? TotalHoursToday { get; set; }
        public double? AvgHoursWorker { get; set; }
        public long TotalDayShift { get; set; }
        public long TotalNightShift { get; set; }
        public long TotalEmployees { get; set; }
        public long TotalContractors { get; set; }
        public long? IncidentId { get; set; }
        public long? IncidentValidationId { get; set; }
    }
    public class IncidentValidationPersonnelsTopContributorsViewModel
    {
        public string Name { get; set; } = string.Empty;
        public double? TotalHoursToday { get; set; }
    }
    #endregion

    public class IncidentAssessmentDetailViewModel
    {
        public long Id { get; set; }
        public long? IncidentId { get; set; }
        public long? OpenTaskCount { get; set; } = 0;
        public long? ICPLocationCount { get; set; } = 0;
        public long? PrimaryLocationCount { get; set; } = 0;
        public long? AdditionalLocationCount { get; set; } = 0;
        public long? CompletedTaskCount { get; set; } = 0;
        public long? IncidentValidationId { get; set; }
        public List<SelectListItem> OwenerTypes { get; set; } = new();
        public List<SelectListItem> Status { get; set; } = new();
        public List<IncidentCommanderDetailViewModel> incidentCommanderDetailViewslist { get; set; } = new();
        public Dictionary<string, string> MainStepOwners { get; set; } = new();
        public List<IncidentViewPostViewModel> listIncidentViewPostViewModel { get; set; } = new();
    }
    public class IncidentAssessmentEditViewModel
    {
        public long Id { get; set; }
        public long? IncidentId { get; set; }
        public long? IncidentValidationId { get; set; }
        public long? StatusId { get; set; }
        public long? AssigneeId { get; set; }

        public long? MainStepId { get; set; }
        public long? SubStepId { get; set; }
        public List<SelectListItem> Assignees { get; set; } = new();
        public List<SelectListItem> Status { get; set; } = new();
        public string StartedTime { get; set; } = string.Empty;
        public string CompletedTime { get; set; } = string.Empty;
        public string MainStep { get; set; } = string.Empty;
        public string SubStep { get; set; } = string.Empty;
        public List<IFormFile> File { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
    public class IncidentCommanderDetailViewModel
    {
        public long? MainstepId { get; set; }
        public string Mainstep { get; set; } = string.Empty;
        public string Substep { get; set; } = string.Empty;
        public long? SubstepId { get; set; }
        public long? StatusId { get; set; }
        public string Status { get; set; } = string.Empty;
        public long? AssigneeId { get; set; }
        public string Assignee { get; set; } = string.Empty;
        public string Evidence { get; set; } = string.Empty;
        public bool IsOwner { get; set; }
        public string ClockIn { get; set; } = string.Empty;
        public string ClockOut { get; set; } = string.Empty;
        public string ImagesUrl { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public long? ImageCount { get; set; }
    }
    public class IncidentAssessmentReadViewModel
    {
        public long Id { get; set; }
        public long? IncidentId { get; set; }
        public long? IncidentValidationId { get; set; }
        public long? StatusId { get; set; }
        public long? AssigneeId { get; set; }
        public long? MainStepId { get; set; }
        public long? SubStepId { get; set; }
        public string StartedTime { get; set; } = string.Empty;
        public string CompletedTime { get; set; } = string.Empty;
        public string MainStep { get; set; } = string.Empty;
        public string SubStep { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Assignee { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class IncidentViewAssessmentAttachmentViewModel
    {
        public List<string> Image { get; set; } = new();
    }

    #region IncidentRepair
    public class IncidentViewRepairViewModel
    {
        public List<IncidentViewRepairListViewModel> listIncidentViewRepairViewModel { get; set; }
        public List<IncidentViewPostViewModel> listIncidentViewPostViewModel { get; set; }
    }

    public class IncidentViewPostViewModel
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public string? TimeforMessage { get; set; }
        public string? Message { get; set; }
        public long IncidentViewType { get; set; }

    }

    public class IncidentViewRestorationListViewModel
    {
        public List<IncidentViewPostViewModel> listIncidentViewPostViewModel { get; set; }
    }
    #endregion

    #region IncidentCloseOut
    public class IncidentViewCloseoutViewModel
    {
        public long PersonnelInvolved { get; set; }
        public long TotalCloseOut { get; set; }

        public List<IncidentViewPostViewModel> listIncidentViewPostViewModel { get; set; }
        public List<IncidentViewCloseoutListViewModel> listIncidentViewCloseoutViewModel { get; set; } = new();
    }
    public class IncidentViewCloseoutListViewModel
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long IncidentValidationId { get; set; }
        public string? Task { get; set; }
        public string? FieldValue { get; set; }
        public string? Status { get; set; }
        public string? Complted { get; set; }
        public List<IncidentViewPostViewModel> listIncidentViewPostViewModel { get; set; }
    }
    #endregion
    #region IncidentValidationTask

    public class IncidentViewTaskViewModel
    {
        public List<IncidentViewTaskListViewModel> listIncidentViewTaskViewModel { get; set; } = new();
        public List<IncidentViewPostViewModel> listIncidentViewPostViewModel { get; set; } = new();
        public List<IncidentViewTaskListViewModel> listIncidentViewTaskCloseOutViewModel { get; set; } = new();
    }

    public class IncidentViewTaskListViewModel
    {
        public long Id { get; set; }
        public long? IncidentId { get; set; }
        public long? IncidentValidationId { get; set; }
        public string? Task { get; set; }            // TaskDescription
        public string? FieldValue { get; set; }      // Responsible Party (resolved RoleIds)
        public string Status { get; set; }          // resolved from StatusId
        public string? Started { get; set; }         // placeholder (no DB column in screenshot)
        public string? Completed { get; set; }       // placeholder (no DB column in screenshot)
        public string? Attachment { get; set; }      // placeholder (no DB column in screenshot)
        public string? RoleIds { get; set; }     // receives "1,2" (if you join them first)
        public int? StatusId { get; set; }
        public string ImagesUrl { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public long? ImageCount { get; set; }
    }
    public class IncidentEditTaskListViewModel
    {
        public long Id { get; set; }
        public long? IncidentId { get; set; }
        public long? IncidentValidationId { get; set; }
        public string? Task { get; set; }            // TaskDescription
        public List<SelectListItem> RoleList { get; set; } = new();
        public List<SelectListItem> StatusList { get; set; } = new();
        public string? Started { get; set; }         // placeholder (no DB column in screenshot)
        public string? Completed { get; set; }       // placeholder (no DB column in screenshot)
        public string? Attachment { get; set; }      // placeholder (no DB column in screenshot)
        public string? RoleIds { get; set; }     // receives "1,2" (if you join them first)
        public long? StatusId { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<IFormFile> File { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
    #endregion
    public class AddIncidentTaskRequest
    {
        public long? IncidentId { get; set; }
        public long? IncidentValidationId { get; set; }
        public string? TaskDescription { get; set; }

        /// <summary>
        /// Comma-separated role ids like "1,2,3" OR you can pass as "1" when single.
        /// You can change this to List&lt;long&gt; if you prefer binding arrays.
        /// </summary>
        public string? RoleIds { get; set; }

        /// <summary>
        /// Optional status id (maps to your Progress table).
        /// </summary>
        public int? StatusId { get; set; }
    }
    public class IncidentAssessmentAddViewModel
    {
        public long Id { get; set; }
        public long? IncidentId { get; set; }
        public long? IncidentValidationId { get; set; }
        public List<SelectListItem> StatusList { get; set; } = new();

        public List<SelectListItem> UserList { get; set; } = new();
    }
    public class IncidentAssessmentSubmitRequest
    {
        public string incidentValidationAssessment { get; set; }
        public long IncidentId { get; set; }
    }
    public class IncidentViewRepairListViewModel
    {
        public long Id { get; set; }
        public long? IncidentId { get; set; }
        public long? IncidentValidationId { get; set; }
        public long? FieldTypeId { get; set; }
        public string? FieldType { get; set; }
        public string? FieldValue { get; set; }
        public string? FieldStatus { get; set; }
        public string? SOL_Path { get; set; }
        public string? SOL_Remark { get; set; }
        public string? PFO_Path { get; set; }
        public string? PFO_Remark { get; set; }
        public string? VTF_Path { get; set; }
        public string? VTF_Remark { get; set; }
    }



    public class IncidentRepairEditViewModel
    {
        public long Id { get; set; }
        public long? IncidentId { get; set; }
        public long? IncidentValidationId { get; set; }
        public string? SourceOfLeak { get; set; }
        public string? SourceOfLeakStatus { get; set; }
        public string? PreventFurtherOutage { get; set; }
        public string? PreventFurtherOutageStatus { get; set; }
        public string? VacuumTruckFitting { get; set; }
        public string? VacuumTruckFittingStatus { get; set; }
        public string? FieldType { get; set; }
        public long? FieldTypeId { get; set; }
        public List<SelectListItem> Status { get; set; } = new();
        public List<SelectListItem> RoleList { get; set; } = new();
        public List<IFormFile> File { get; set; }
        public string? SOL_Path { get; set; }
        public string? SOL_Remark { get; set; }
        public string? PFO_Path { get; set; }
        public string? PFO_Remark { get; set; }
        public string? VTF_Path { get; set; }
        public string? VTF_Remark { get; set; }
    }
}

