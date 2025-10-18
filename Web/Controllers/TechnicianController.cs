using AutoMapper;
using ViewModels.DataTable;
using ViewModels.Technician;
using Enums;
using Microsoft.AspNetCore.Identity;
using Models;
using Repositories.Common.TechnicianService.Interface;
using Web.Controllers.Shared;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using ViewModels.CRUD;
using Select2.Model;
using ViewModels;
using ViewModels.Users;
using ViewModels.Authentication;
using Centangle.Common.ResponseHelpers.Error;

namespace Web.Controllers
{
    public class TechnicianController : UserBaseController<TechnicianUpdateViewModel, TechnicianUpdateViewModel, TechnicianDetailViewModel, TechnicianDetailViewModel, TechnicianSelect2ViewModel, TechnicianSearchViewModel>
    {
        private readonly ITechnicianService<TechnicianUpdateViewModel, TechnicianUpdateViewModel, TechnicianDetailViewModel> _service;
        private readonly ILogger<UserController> _logger;
        private readonly bool _hasAdditionalInfo;

        public TechnicianController(
                   ITechnicianService<TechnicianUpdateViewModel, TechnicianUpdateViewModel, TechnicianDetailViewModel> service,
                   ILogger<UserController> logger,
                   IMapper mapper,
                   UserManager<ApplicationUser> userTechnician,
                   bool hasAdditionalInfo = false) :
                   base(service, logger, mapper, userTechnician, "Technician", "Technicians", RolesCatalog.Technician, hasAdditionalInfo)
        {
            _service = service;
            _logger = logger;
            _hasAdditionalInfo = hasAdditionalInfo;
        }


        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "First Name",data = "FirstName"},
                new DataTableViewModel{title = "Last Name",data = "LastName"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }

        protected override void SetDatatableActions<T>(DatatablePaginatedResultModel<T> result)
        {
            result.ActionsList = new List<DataTableActionViewModel>()
            {
                   // new DataTableActionViewModel() {Action="Detail",Title="Detail",Href=$"/Technician/Detail/{{Id}}"},
                    new DataTableActionViewModel() {Action="Update",Title="Update",Href=$"/Technician/Update/{{Id}}"},
                    new DataTableActionViewModel() {Action="Delete",Title="Delete",Href=$"/Technician/Delete/{{Id}}",DatatableHrefType=DatatableHrefType.Link},
                };
            if (User.IsInRole("SystemAdministrator"))
            {
                result.ActionsList.Add(new DataTableActionViewModel() { Action = "ResetPinCode", Title = "ResetPinCode", Href = $"/Technician/ResetPinCode/{{Id}}" });
            }
        }


        public async override Task<IRepositoryResponse> GetResponse(TechnicianSearchViewModel svm)
        {
            return await _service.GetDataForSelect2<TechnicianSelect2ViewModel>(svm);
        }
    }

}

