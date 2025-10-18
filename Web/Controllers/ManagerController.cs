using AutoMapper;
using ViewModels.DataTable;
using ViewModels.Manager;
using Enums;
using Microsoft.AspNetCore.Identity;
using Models;
using Repositories.Common.ManagerService.Interface;
using Web.Controllers.Shared;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using ViewModels.CRUD;
using Select2.Model;
using ViewModels;

namespace Web.Controllers
{
    public class ManagerController : UserBaseController<ManagerUpdateViewModel, ManagerUpdateViewModel, ManagerDetailViewModel, ManagerDetailViewModel, ManagerSelect2ViewModel, ManagerSearchViewModel>
    {
        private readonly IManagerService<ManagerUpdateViewModel, ManagerUpdateViewModel, ManagerDetailViewModel> _service;
        private readonly bool _hasAdditionalInfo;

        public ManagerController(
                   IManagerService<ManagerUpdateViewModel, ManagerUpdateViewModel, ManagerDetailViewModel> service,
                   ILogger<UserController> logger,
                   IMapper mapper,
                   UserManager<ApplicationUser> userManager,
                   bool hasAdditionalInfo = false) :
                   base(service, logger, mapper, userManager, "Manager", "Managers", RolesCatalog.Manager, hasAdditionalInfo)
        {
            _service = service;
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
                   // new DataTableActionViewModel() {Action="Detail",Title="Detail",Href=$"/Manager/Detail/{{Id}}"},
                    new DataTableActionViewModel() {Action="Update",Title="Update",Href=$"/Manager/Update/{{Id}}"},
                    new DataTableActionViewModel() {Action="Delete",Title="Delete",Href=$"/Manager/Delete/{{Id}}",DatatableHrefType=DatatableHrefType.Link},
                };
            if (User.IsInRole("SystemAdministrator"))
            {
                result.ActionsList.Add(new DataTableActionViewModel() { Action = "ResetPassword", Title = "ResetPassword", Href = $"/Manager/ResetPassword/{{Id}}" });
            }
        }

        public async override Task<IRepositoryResponse> GetResponse(ManagerSearchViewModel svm)
        {
            return await _service.GetDataForSelect2<ManagerSelect2ViewModel>(svm);
        }
    }

}

