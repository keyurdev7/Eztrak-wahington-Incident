using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

using Models;
using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Incident
{

    public class BaseIncidentValidationViewModel
    {
        public long Id { get; set; }
        public string IncidentId { get; set; } = default!;
    }

    public class ValidationWorkflowViewModel : BaseIncidentValidationViewModel
    {
        public IncidentValidationCountViewModel IVCount { get; set; } = new();
        public IncidentValidationDetailViewModel IVDetails { get; set; } = new();
        public IncidentValidationViewModel IVValidation { get; set; } = new();
        public List<IncidentValidationPendingViewModel> IVPendingList { get; set; } = new();
        public List<RecentlyIncidentValidationViewModel> IVRecentlyList { get; set; } = new();
        public List<IncidentResponseTeamViewModel> IVResponseTeamList { get; set; } = new();
        public List<IncidentPolicyViewModel> IVPolicyList { get; set; } = new();
        public IncidentCommunicationViewModel IVCommunication { get; set; } = new();
        public List<TeamWithUsersViewModel> IVIncidentTeamUser { get; set; } = new();

        public List<IncidentAdditionalLocationViewModel> IVAdditionalLocations { get; set; } = new();
    }

    public class IncidentValidationCountViewModel : BaseIncidentValidationViewModel
    {
        public long PendingValidationCount { get; set; } = default!;
        public long TodayValidationCount { get; set; } = default!;
        public long HighSeverityCount { get; set; } = default!;
    }

    public class IncidentValidationPendingViewModel : BaseIncidentValidationViewModel
    {
        public string EventType { get; set; } = default!;
        public string IncidentLocation { get; set; } = default!;
        public string Severity { get; set; } = default!;
        public string SeverityColor { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string IncidentDate { get; set; } = default!;
    }
    public class RecentlyIncidentValidationViewModel : BaseIncidentValidationViewModel
    {
        public string EventType { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string StatusColor { get; set; } = default!;
        public string IncidentDate { get; set; } = default!;
    }
    public class IncidentValidationDetailViewModel : BaseIncidentValidationViewModel
    {
        public string CallerName { get; set; } = default!;
        public string CallerContact { get; set; } = default!;
        public string CallerAddress { get; set; } = default!;
        public string CallerDateTime { get; set; } = default!;
        public string IncidentLocation { get; set; } = default!;
        public string NearestIntersection { get; set; } = default!;
        public string EventType { get; set; } = default!;
        public string Severity { get; set; } = default!;
        public string DescriptionIssue { get; set; } = default!;
        public string SeverityColor { get; set; } = default!;
        public string IncidentStatus { get; set; } = default!;
        public string IncidentStatusColor { get; set; } = default!;
        public double Lat { get; set; } = default!;
        public double Long { get; set; } = default!;
        public List<string> AffectedAssets { get; set; } = new();
        public string GasPresent { get; set; } = default!;
        public string HissingPresent { get; set; } = default!;
        public string VisibleDamagePresent { get; set; } = default!;
        public string WaterPresent { get; set; } = default!;
        public string PeopleInjured { get; set; } = default!;
        public string EvacuationRequired { get; set; } = default!;
    }

    public class SafetyAssessment
    {
        public string Name { get; set; } = default!;
        public string AssetStatus { get; set; } = default!;
    }

    public class IncidentValidationViewModel : BaseIncidentValidationViewModel
    {
        public List<SelectListItem> severityLevels { get; set; } = new();
        public List<SelectListItem> UserList { get; set; } = new();
        public List<SelectListItem> CompanyList { get; set; } = new();
        public List<SelectListItem> RoleList { get; set; } = new();
        public List<SelectListItem> ShiftsList { get; set; } = new();
        public List<SelectListItem> StatusList { get; set; } = new();
        public string severityLevel { get; set; } = default!;
        public double Lat { get; set; } = default!;
        public double Long { get; set; } = default!;
        public string ValidationNotes { get; set; } = default!;
        public long RadiusId { get; set; } = default!;
        public long severityLevelId { get; set; } = default!;
        public long UserId { get; set; } = default!;
        public IncidentValidationAssignedRoleViewModel assignedRole { get; set; } = new();
        public IncidentValidationValidationGatesViewModel validationGates { get; set; } = new();
        public IncidentValidationRepairViewModel validationRepair { get; set; } = new();

        public string IncidentLocation { get; set; } = default!;
        public string Source { get; set; } = default!;
    }

    public class IncidentResponseTeamViewModel
    {
        public long ReponseTeamId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Tag { get; set; } = default!;
        public string Contact { get; set; } = default!;
        public string Specializations { get; set; } = string.Empty;
    }

    public class IncidentPolicyViewModel
    {
        public long PolicyId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public List<SelectListItem> assignTeams { get; set; } = new();
        public List<string> PolicySteps { get; set; } = new();
    }
    public class IncidentCommunicationViewModel
    {
        public List<IFormFile>? File { get; set; }
        public string Notes { get; set; } = default!;
        public string? ImageUrl { get; set; } = default!;
    }
    public class IncidentSubmitViewModel : BaseIncidentValidationViewModel
    {
        public IncidentValidationAssignedRoleViewModel assignedRole { get; set; } = new();
        public IncidentValidationValidationGatesViewModel validationGates { get; set; } = new();
        public long ConfirmedSeverityLevelId { get; set; }
        
        public long DiscoveryPerimeterId { get; set; }
        public string ValidationNotes { get; set; } = default!;
        public string AssignResponseTeams { get; set; } = default!;
        public string listPolicyVM { get; set; } = default!;
        public string listValidationLocationVM { get; set; } = default!;
        public string listPersonalDataVM { get; set; } = default!;
        public string listTaskDataVM { get; set; } = default!;
        public string listCloseoutTaskDataVM { get; set; } = default!;
        public bool IsMarkFalseAlarm { get; set; } = false;
        public string incidentValidationAssessment { get; set; } = default!;
        public string Source { get; set; } = default!;
        public string IncidentLocation { get; set; } = default!;
        

        //public string listCommunicationVM { get; set; } = default!;
        public List<IncidentSubmitPolicyViewModel> listSubmitPolicyVM { get; set; } = new();
        public List<IncidentValidationLocationViewModel> listSubmitValidationLocationVM { get; set; } = new();
        public List<IncidentSubmitCommunicationViewModel> listSubmitCommunicationVM { get; set; } = new();
        public List<IncidentValidationPersonalViewModel> listSubmitPersonalDataVM { get; set; } = new();
        public IncidentValidationAssessment incidentSubmitValidationAssessment { get; set; } = new();
        public IncidentValidationRepairViewModel validationRepair { get; set; } = new();

        public List<IncidentValidationTaskViewModel> listSubmitTaskDataVM { get; set; } = new();
        public List<IncidentValidationCloseoutTaskViewModel> listSubmitCloseoutTaskDataVM { get; set; } = new();
    }
    public class IncidentSubmitPolicyViewModel
    {
        public long PolicyId { get; set; }
        public long Status { get; set; }
        public List<long> Teams { get; set; }= new();
    }
    public class IncidentSubmitCommunicationViewModel
    {
        public string UserName { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string TimeStamp { get; set; } = default!;
        public string RecipientNames { get; set; } = default!;
        public string RecipientsIds { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public long MessageType { get; set; } = 1;

        // Files bind from FormData, not JSON
        public List<FileMeta>? FileMeta { get; set; } = new();
        public List<IFormFile>? Files { get; set; }
    }

    public class FileMeta
    {
        public string FileName { get; set; }   // actual saved name
        public string OriginalName { get; set; } // original upload name
        public string TempPath { get; set; }   // path in temp folder
    }

    public class IncidentValidationLocationViewModel
    {
        public long? LocationId { get; set; }
        public long? SeverityID { get; set; }
        public long? DiscoveryPerimeter { get; set; }
        public string ICPLocation { get; set; } = default!;
        public string Source { get; set; } = default!;
        public string SeverityName { get; set; } = default!;
        public float Lat { get; set; } = default!;
        public float Lon { get; set; } = default!;
    }
    public class IncidentValidationAssignedRoleViewModel
    {
        public long? IncidentCommanderId { get; set; }
        public long? FieldEnvRepId { get; set; }
        public long? GECCoordinatorId { get; set; }
        public long? EngineeringLeadId { get; set; }
    }

    public class IncidentValidationValidationGatesViewModel 
    {
        public bool ContainmentAcknowledgement { get; set; }
        public bool Exception { get; set; }
        public bool IndependentInspection { get; set; }
        public string Regulatory { get; set; }
        public bool IsOtherEvent { get; set; }
        public string OtherEventDetail { get; set; }
    }

    public class IncidentValidationPersonalViewModel
    {
        public long? UserId { get; set; }
        public long? CompanyId { get; set; }
        public long? RoleId { get; set; }
        public long? ShiftId { get; set; }
    }
    public class IncidentValidationRepairViewModel
    {
        public string? SourceOfLeak { get; set; }
        public string? SourceOfLeakStatus { get; set; }
        public string? PreventFurtherOutage { get; set; }
        public string? PreventFurtherOutageStatus { get; set; }
        public string? VacuumTruckFitting { get; set; }
        public string? VacuumTruckFittingStatus { get; set; }
    }
    public class IncidentValidationTaskViewModel
    {
        public string RoleIds { get; set; }= string.Empty;
        public long? StatusId { get; set; }
        public string TaskDescription { get; set; } = string.Empty;
    }
    public class IncidentValidationCloseoutTaskViewModel
    {
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; }
        public string Description { get; set; } = string.Empty;
    }
    //public class AddIncidentValidationAssessment
    //{
    //    public long? IncidentId { get; set; }
    //    public long? IncidentValidationId { get; set; }

    //    public long? IC_MCR_AssignId { get; set; }
    //    public long? IC_MCR_StatusId { get; set; }

    //    public long? IC_Notify_AssignId { get; set; }
    //    public long? IC_Notify_StatusId { get; set; }

    //    public long? IC_EstablishICP_AssignId { get; set; }
    //    public long? IC_EstablishICP_StatusId { get; set; }

    //    public long? FER_PCA_AssignId { get; set; }
    //    public long? FER_PCA_StatusId { get; set; }

    //    public long? FER_LC_AssignId { get; set; }
    //    public long? FER_LC_StatusId { get; set; }
       

    //    public long? EGEC_RSM_AssignId { get; set; }
    //    public long? EGEC_RSM_StatusId { get; set; }
        
    //    public long? EGEC_MLP_AssignId { get; set; }
    //    public long? EGEC_MLP_StatusId { get; set; }
       

    //    public long? EGEC_ICT_AssignId { get; set; }
    //    public long? EGEC_ICT_StatusId { get; set; }

       
    //}
}
