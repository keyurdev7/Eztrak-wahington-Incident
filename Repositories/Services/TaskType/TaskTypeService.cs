using Models.Common.Interfaces;
using ViewModels.Shared;
using Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Pagination;
using System.Linq.Expressions;
using ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2010.Excel;
using Helpers.Extensions;
using Centangle.Common.ResponseHelpers;
using Enums;

namespace Repositories.Common
{
    public class TaskTypeService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<TaskType, CreateViewModel, UpdateViewModel, DetailViewModel>, ITaskTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : TaskTypeModifyViewModel, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<TaskTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;

        public TaskTypeService(ApplicationDbContext db, ILogger<TaskTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IActionContextAccessor actionContext) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
        }

        public override async Task<Expression<Func<TaskType, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as TaskTypeSearchViewModel;

            return x =>
                        (
                            (
                                string.IsNullOrEmpty(searchFilters.Search.value)
                                ||
                                x.Code.ToLower().Contains(searchFilters.Search.value.ToLower())
                            )
                        )
                        &&
                        (searchFilters.Category == null || searchFilters.Category == x.Category)
                        &&
                        (string.IsNullOrEmpty(searchFilters.Code) || x.Code.ToLower().Contains(searchFilters.Code.ToLower()))
                        &&
                        (string.IsNullOrEmpty(searchFilters.Title) || x.Title.ToLower().Contains(searchFilters.Title.ToLower()))
                        &&
                        (searchFilters.Category == null || x.Category == searchFilters.Category)
                        ;
        }

        public async override Task<IRepositoryResponse> Create(CreateViewModel model)
        {
            var viewModel = model as TaskTypeModifyViewModel;
            SetParentFields(viewModel);
            var totalAssetCount = await _db.TaskTypes.IgnoreQueryFilters().CountAsync();
            model.Code = "WS-" + (totalAssetCount + 1).ToString("D4");
            var response = await base.Create(model);
            long id = 0;
            if (response.Status == System.Net.HttpStatusCode.OK)
            {
                var parsedResponse = response as RepositoryResponseWithModel<long>;
                id = parsedResponse?.ReturnModel ?? 0;
                if (viewModel.TaskLabors.Count > 0)
                {
                    await SetTaskLabors(viewModel.TaskLabors, id);
                }
                if (viewModel.TaskWorkSteps.Count > 0)
                {
                    await SetTaskWorkSteps(viewModel.TaskWorkSteps, id);
                }
                if (viewModel.TaskMaterials.Count > 0)
                {
                    await SetTaskMaterials(viewModel.TaskMaterials, id);
                }
                if (viewModel.TaskEquipments.Count > 0)
                {
                    await SetTaskEquipments(viewModel.TaskEquipments, id);
                }

            }
            return response;
        }

        private async Task SetTaskLabors(List<TaskLaborViewModel> taskLabors, long id)
        {
            try
            {
                var dbRecords = await _db.TaskLabors.Where(x => x.TaskTypeId == id).ToListAsync();
                dbRecords.ForEach(x => x.IsDeleted = true);

                var mappedTaskLabors = _mapper.Map<List<TaskLabor>>(taskLabors);
                mappedTaskLabors.ForEach(x => x.TaskTypeId = id);
                await _db.AddRangeAsync(mappedTaskLabors);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }

        private async Task SetTaskMaterials(List<TaskMaterialViewModel> taskMaterials, long id)
        {
            try
            {
                var dbRecords = await _db.TaskMaterials.Where(x => x.TaskTypeId == id).ToListAsync();
                dbRecords.ForEach(x => x.IsDeleted = true);

                var mappedTaskMaterials = _mapper.Map<List<TaskMaterial>>(taskMaterials);
                mappedTaskMaterials.ForEach(x => x.TaskTypeId = id);
                await _db.AddRangeAsync(mappedTaskMaterials);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }

        private async Task SetTaskEquipments(List<TaskEquipmentViewModel> taskEquipments, long id)
        {
            try
            {
                var dbRecords = await _db.TaskEquipments.Where(x => x.TaskTypeId == id).ToListAsync();
                dbRecords.ForEach(x => x.IsDeleted = true);

                var mappedTaskEquipments = _mapper.Map<List<TaskEquipment>>(taskEquipments);
                mappedTaskEquipments.ForEach(x => x.TaskTypeId = id);
                await _db.AddRangeAsync(mappedTaskEquipments);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }

        private void SetParentFields(TaskTypeModifyViewModel? viewModel)
        {
            viewModel.Labor = viewModel.TaskLabors.Sum(x => x.Total);
            viewModel.Material = viewModel.TaskMaterials.Sum(x => x.Cost);
            viewModel.Equipment = viewModel.TaskEquipments.Sum(x => x.Cost);
        }

        private async Task SetTaskWorkSteps(List<TaskWorkStepViewModel> taskWorkSteps, long id)
        {
            try
            {
                var dbRecords = await _db.TaskWorkSteps.Where(x => x.TaskTypeId == id).ToListAsync();
                dbRecords.ForEach(x => x.IsDeleted = true);

                var mappedTaskWorkSteps = _mapper.Map<List<TaskWorkStep>>(taskWorkSteps);
                mappedTaskWorkSteps.ForEach(x => x.TaskTypeId = id);
                await _db.AddRangeAsync(mappedTaskWorkSteps);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

        }

        private async Task<List<TaskWorkStepViewModel>> GetTaskWorkSteps(long id)
        {
            try
            {
                var taskWorkSteps = await _db.TaskWorkSteps
                                                    .Where(x => x.TaskTypeId == id)
                                                    .Select(x => new TaskWorkStepViewModel
                                                    {
                                                        Id = x.Id,
                                                        Title = x.Title,
                                                        Order = x.Order
                                                    })
                                                    .ToListAsync();
                return taskWorkSteps;
            }
            catch (Exception ex)
            {
                return new();
            }

        }

        public async Task<List<TaskMaterialViewModel>> GetTaskMaterials(long id)
        {
            try
            {
                var taskMaterials = await _db.TaskMaterials
                                            //.Include(x => x.Material)
                                            .Where(x => x.TaskTypeId == id)
                                            .Select(x => new TaskMaterialViewModel
                                            {
                                                Id = x.Id,
                                                //Material = new InventoryBriefViewModel
                                                //{
                                                //    Id = x.MaterialId,
                                                //    SystemGeneratedId = x.Material.SystemGeneratedId,
                                                //    Description = x.Material.Description
                                                //},
                                                MaterialName = x.MaterialName,
                                                //ItemNo = x.Material.ItemNo,
                                                Cost = x.Cost
                                            })
                                            .ToListAsync();
                return taskMaterials;
            }
            catch (Exception ex)
            {
                return new();
            }

        }

        public async Task<List<TaskEquipmentViewModel>> GetTaskEquipments(long id)
        {
            try
            {
                var taskEquipments = await _db.TaskEquipments
                                            //.Include(x => x.Equipment)
                                            .Where(x => x.TaskTypeId == id)
                                            .Select(x => new TaskEquipmentViewModel
                                            {
                                                Id = x.Id,
                                                //Equipment = new EquipmentBriefViewModel
                                                //{
                                                //    Id = x.EquipmentId,
                                                //    SystemGeneratedId = x.Equipment.SystemGeneratedId,
                                                //    Description = x.Equipment.Description
                                                //},
                                                EquipmentName = x.EquipmentName,
                                                Cost = x.Cost
                                            })
                                            .ToListAsync();
                return taskEquipments;
            }
            catch (Exception ex)
            {
                return new();
            }

        }

        public async Task<List<TaskLaborViewModel>> GetTaskLabors(long id)
        {
            try
            {
                var taskLabors = await _db.TaskLabors.Include(x => x.CraftSkill)
                                        .Where(x => x.TaskTypeId == id)
                                        .Select(x => new TaskLaborViewModel
                                        {
                                            Id = x.Id,
                                            CraftSkill = new CraftSkillBriefViewModel
                                            {
                                                Id = x.CraftSkillId,
                                                Name = x.CraftSkill.Name,
                                                OTRate = x.CraftSkill.OTRate,
                                                STRate = x.CraftSkill.STRate,
                                                DTRate = x.CraftSkill.DTRate,
                                            },
                                            Hours = x.Hours,
                                            Rate = x.Rate
                                        })
                                        .ToListAsync();
                return taskLabors;
            }
            catch (Exception ex)
            {
                return new();
            }

        }

        public async Task<List<TaskWorkStepViewModel>> GetTaskSteps(long id)
        {
            try
            {
                var taskSteps = await _db.TaskWorkSteps
                                        .Where(x => x.TaskTypeId == id)
                                        .Select(x => new TaskWorkStepViewModel
                                        {
                                            Id = x.Id,
                                            Title = x.Title,
                                            Order = x.Order
                                        })
                                        .ToListAsync();
                return taskSteps;
            }
            catch (Exception ex)
            {
                return new();
            }

        }

        public async override Task<IRepositoryResponse> Update(UpdateViewModel model)
        {
            var viewModel = model as TaskTypeModifyViewModel;
            SetParentFields(viewModel);
            var response = await base.Update(model);
            long id = 0;
            if (response.Status == System.Net.HttpStatusCode.OK)
            {
                var parsedResponse = response as RepositoryResponseWithModel<long>;
                id = parsedResponse?.ReturnModel ?? 0;
                if (viewModel.TaskLabors.Count > 0)
                {
                    await SetTaskLabors(viewModel.TaskLabors, id);
                }
                if (viewModel.TaskWorkSteps.Count > 0)
                {
                    await SetTaskWorkSteps(viewModel.TaskWorkSteps, id);
                }
                if (viewModel.TaskMaterials.Count > 0)
                {
                    await SetTaskMaterials(viewModel.TaskMaterials, id);
                }
                if (viewModel.TaskEquipments.Count > 0)
                {
                    await SetTaskEquipments(viewModel.TaskEquipments, id);
                }
            }
            return response;
        }

        public async override Task<IRepositoryResponse> GetById(long id)
        {
            var response = await base.GetById(id);
            var parsedResponse = response as RepositoryResponseWithModel<TaskTypeDetailViewModel>;
            parsedResponse.ReturnModel.TaskWorkSteps = await GetTaskWorkSteps(id);
            parsedResponse.ReturnModel.TaskLabors = await GetTaskLabors(id);
            parsedResponse.ReturnModel.TaskMaterials = await GetTaskMaterials(id);
            parsedResponse.ReturnModel.TaskEquipments = await GetTaskEquipments(id);
            return parsedResponse;
        }

    }
}

