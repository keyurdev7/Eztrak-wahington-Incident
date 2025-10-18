using System.Net;
using System.Reflection;
using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pagination;
using Repositories.Common;
using ViewModels;
using ViewModels.CRUD;
using ViewModels.CRUD.Interfaces;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class UserSearchSettingController : CrudBaseController<UserSearchSettingModifyViewModel, UserSearchSettingModifyViewModel, UserSearchSettingDetailViewModel, UserSearchSettingDetailViewModel, UserSearchSettingBriefViewModel, UserSearchSettingSearchViewModel>
    {
        private readonly IUserSearchSettingService<UserSearchSettingModifyViewModel, UserSearchSettingModifyViewModel, UserSearchSettingDetailViewModel> _userSearchSettingService;

        public UserSearchSettingController(
            IUserSearchSettingService<UserSearchSettingModifyViewModel, UserSearchSettingModifyViewModel, UserSearchSettingDetailViewModel> service
            , ILogger<UserSearchSettingController> logger
            , IMapper mapper
            , IUserSearchSettingService<UserSearchSettingModifyViewModel, UserSearchSettingModifyViewModel, UserSearchSettingDetailViewModel> userSearchSettingService
            ) : base(service, logger, mapper, "UserSearchSetting", "UserSearchSetting")
        {
            this._userSearchSettingService = userSearchSettingService;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}
            };
        }

        protected override Task<CrudUpdateViewModel> OverrideUpdateViewModel(CrudUpdateViewModel model)
        {
            model.SubmitOnClick = "SubmitUserSearchSetting(this)";
            return base.OverrideUpdateViewModel(model);
        }

        public override Task<ActionResult> Create(UserSearchSettingModifyViewModel model)
        {
            return base.Update(model);
        }

        public async Task<IActionResult> LoadSearchProfile(long id)
        {
            var searchProfileResponse = await _userSearchSettingService.GetById(id);
            var searchProfile = new UserSearchSettingDetailViewModel();
            if (searchProfileResponse.Status == HttpStatusCode.OK)
            {
                searchProfile = (searchProfileResponse as RepositoryResponseWithModel<UserSearchSettingDetailViewModel>).ReturnModel;
                var className = searchProfile?.ClassName;

                if (!string.IsNullOrEmpty(className))
                {
                    Type targetType = null;
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        targetType = assembly.GetTypes().FirstOrDefault(t => t.FullName == className);
                        if (targetType != null)
                        {
                            break;
                        }
                    }

                    if (targetType != null)
                    {
                        var jsonData = searchProfile.FilterJson;
                        var deserializedObject = JsonConvert.DeserializeObject(jsonData, targetType);
                        if (deserializedObject is ISaveSearch)
                        {
                            (deserializedObject as ISaveSearch).UserSearchSetting = new UserSearchSettingBriefViewModel
                            {
                                Id = searchProfile.Id,
                                Name = searchProfile.Name
                            };
                        }
                        return View(searchProfile.ViewPath, deserializedObject);
                    }
                }
            }
            return View(searchProfile.ViewPath);
        }
    }
}

