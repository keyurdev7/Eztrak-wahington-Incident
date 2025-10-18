using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Models.Common.Interfaces;
using Models;
using Pagination;
using System.Linq.Expressions;
using ViewModels.Shared;
using ViewModels;
using Microsoft.EntityFrameworkCore;
using Centangle.Common.ResponseHelpers;
using Repositories.Services.AttachmentService.Interface;
using Helpers.Extensions;
using ViewModels.Asset;
using Helpers.File;
using static ViewModels.AssetDetailViewModel;
using Enums;
using Repositories.Services.ExcelHelper.Interface;
using DocumentFormat.OpenXml.Office2010.Excel;



namespace Repositories.Common
{
    public class AssetService<CreateViewModel, UpdateViewModel, DetailViewModel> :
        BaseService<Asset, CreateViewModel, UpdateViewModel, DetailViewModel>,
        IAssetService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, IDynamicColumns, IAssetProperties, new()
        where CreateViewModel : class, IBaseCrudViewModel, IDynamicColumns, IAssetProperties, IIdentitifier, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IDynamicColumns, IIdentitifier, IAssetProperties, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AssetService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        private readonly IFileHelper _fileHelper;
        private readonly IAttachment _attachmentService;
        private readonly IExcelHelper _reader;
        private readonly IDynamicColumnService<DynamicColumnModifyViewModel, DynamicColumnModifyViewModel, DynamicColumnModifyViewModel> _dynamicColumnService;

        public AssetService(
            ApplicationDbContext db
            , ILogger<AssetService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger
            , IMapper mapper
            , IRepositoryResponse response
            , IFileHelper fileHelper
            , IActionContextAccessor actionContext
            , IAttachment attachmentService
            , IExcelHelper reader
            , IDynamicColumnService<DynamicColumnModifyViewModel, DynamicColumnModifyViewModel, DynamicColumnModifyViewModel> dynamicColumnService
            ) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
            _fileHelper = fileHelper;
            _attachmentService = attachmentService;
            _reader = reader;
            this._dynamicColumnService = dynamicColumnService;
        }

        public override async Task<Expression<Func<Asset, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as AssetSearchViewModel;

            return x =>
                        (
                                           (
                                               string.IsNullOrEmpty(searchFilters.Search.value)
                                               ||
                                               x.Manufacturer.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                                               ||
                                               x.Condition.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                                               ||
                                               x.AssetType.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                                                ||
                                               x.MUTCD.Code.ToLower().Contains(searchFilters.Search.value.ToLower())
                                           )
                                           &&
                                           (searchFilters.AssetType.Id == null || x.AssetType.Id == searchFilters.AssetType.Id)
                                           &&
                                           (searchFilters.Condition.Id == null || x.Condition.Id == searchFilters.Condition.Id)
                                           &&
                                           (searchFilters.Manufacturer.Id == null || x.Manufacturer.Id == searchFilters.Manufacturer.Id)
                                           &&
                                           (searchFilters.MUTCD.Id == null || x.MUTCD.Id == searchFilters.MUTCD.Id)
                                           &&
                                           (searchFilters.Id == null || x.Id == searchFilters.Id)

                                       );
        }

        public async Task<Expression<Func<AssetViewModel, bool>>> SetCustomQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as AssetSearchViewModel;
            var ignoreAssetLevel2Filters = searchFilters.AssetTypeLevel2 == null || searchFilters.AssetTypeLevel2.Count == 0;
            var showOutOfServiceAssets = searchFilters.ShowOutOfServiceAssets;
            return x =>
                        (
                                                              (
                                                                  string.IsNullOrEmpty(searchFilters.Search.value)
                                                                  ||
                                                                  x.ManufacturerName.ToLower().Contains(searchFilters.Search.value.ToLower())
                                                                  ||
                                                                  x.ConditionName.ToLower().Contains(searchFilters.Search.value.ToLower())
                                                                  ||
                                                                  x.AssetTypeName.ToLower().Contains(searchFilters.Search.value.ToLower())
                                                                   ||
                                                                  x.MUTCDCode.ToLower().Contains(searchFilters.Search.value.ToLower())
                                                                   ||
                                                                  x.SystemGeneratedId.ToLower().Contains(searchFilters.Search.value.ToLower())
                                                              )
                                                              &&
                                                              (searchFilters.AssetType.Id == null || x.AssetTypeId == searchFilters.AssetType.Id)
                                                              &&
                                                              (searchFilters.Condition.Id == null || x.ConditionId == searchFilters.Condition.Id)
                                                               &&
                                                              (searchFilters.ShowOutOfServiceAssets || x.ConditionName != "Out Of Service")
                                                              &&
                                                              (searchFilters.Manufacturer.Id == null || x.ManufacturerId == searchFilters.Manufacturer.Id)
                                                              &&
                                                              (searchFilters.MUTCD.Id == null || x.MUTCDId == searchFilters.MUTCD.Id)
                                                               &&
                                           (
                                                ignoreAssetLevel2Filters ||
                                                searchFilters.AssetTypeLevel2.Contains(x.AssetTypeLevel2Id)
                                           )
                                       );
        }

        public async override Task<IRepositoryResponse> Create(CreateViewModel model)
        {

            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (await CanCreate() == false)
                {
                    return UnAuthorizedResponse();
                }
                var mappedModel = _mapper.Map<Asset>(model);
                var totalAssetCount = await _db.Assets.IgnoreQueryFilters().CountAsync();
                mappedModel.SystemGeneratedId = "AST-" + (totalAssetCount + 1).ToString("D4");
                mappedModel.ReplacementDate = mappedModel.InstalledDate.AddYears((int)mappedModel.ReplacementYear);
                mappedModel.NextMaintenanceDate = mappedModel.InstalledDate.AddYears((int)mappedModel.NextMaintenanceYear);
                await _db.AddAsync(mappedModel);
                await _db.SaveChangesAsync();
                model.Id = mappedModel.Id;
                await CreateAssetAttachments(model, mappedModel.Id);
                await UpdateAssetAssociation(model.AssetAssociations, mappedModel.Id);
                await _dynamicColumnService.UpdateValues(model);
                await transaction.CommitAsync();
                var response = new RepositoryResponseWithModel<long> { ReturnModel = mappedModel.Id };
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Exception thrown in Create method of Asset");
                return Response.BadRequestResponse(_response);
            }
        }

        public async override Task<IRepositoryResponse> Update(UpdateViewModel model)
        {
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (await CanUpdate(model.Id) == false)
                {
                    return UnAuthorizedResponse();
                }
                var updateModel = model as BaseUpdateVM;
                if (updateModel != null)
                {
                    var record = await _db.Assets.FindAsync(updateModel?.Id);
                    if (record != null)
                    {
                        var dbModel = _mapper.Map(model, record);
                        await _db.SaveChangesAsync();
                        await CreateAssetAttachments(model, dbModel.Id);
                        await UpdateAssetAssociation(model.AssetAssociations, dbModel.Id);
                        await _dynamicColumnService.UpdateValues(model);
                        await transaction.CommitAsync();
                        var response = new RepositoryResponseWithModel<long> { ReturnModel = record.Id };
                        return response;
                    }
                    _logger.LogWarning($"Record for id: {updateModel?.Id} not found in Asset in Update()");
                }
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Update() for Asset threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async override Task<IRepositoryResponse> GetById(long id)
        {
            try
            {
                var dbModel = await _db.Assets
                                        .Include(x => x.AssetType)
                                        .Include(x => x.Condition)
                                        .Include(x => x.Manufacturer)
                                        .Include(x => x.MUTCD).IgnoreQueryFilters()
                                        .Include(x => x.MountType)
                                        .Where(x => x.Id == id)
                                        .FirstOrDefaultAsync();

                if (dbModel != null)
                {
                    var result = _mapper.Map<DetailViewModel>(dbModel);
                    result.ImagesList = await GetAssetAttachments(dbModel.Id);
                    result.AssetAssociations = await GetAssetsSubLevels(new AssetSearchViewModel { Id = id });
                    result.DynamicColumns = await _dynamicColumnService.GetDynamicColumns(DynamicColumnEntityType.Asset, dbModel.Id);
                    var response = new RepositoryResponseWithModel<DetailViewModel> { ReturnModel = result };
                    return response;
                }
                _logger.LogWarning($"No record found for id:{id} for {typeof(Asset).FullName} in GetById()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(Asset).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<List<AttachmentVM>> GetAssetAttachments(long id)
        {
            var attachments = await _db.Attachments.Where(x => x.EntityId == id && x.EntityType == Enums.AttachmentEntityType.Assets).Select(x => new AttachmentVM
            {
                Id = x.Id,
                EntityId = (long)x.EntityId,
                EntityType = (Enums.AttachmentEntityType)x.EntityType,
                Url = x.Url,
                Name = x.Name,
                Type = x.Type,
                CreatedOn = x.CreatedOn
            }).ToListAsync();

            return attachments;
        }

        public async override Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            try
            {
                var searchFilters = search as AssetSearchViewModel;
                searchFilters.AssetTypeLevel2 = searchFilters.AssetTypeLevel2.Where(x => x > -1).ToList();
                var assetsQueryable = await GetAssetsQueryable(search);
                //var query = assetsQueryable.ToQueryString();
                //var tes = await assetsQueryable.ToListAsync();
                var assetsGroupedQueryable = assetsQueryable.GroupBy(x => x.AssetPkId).Select(a => new AssetDetailViewModel
                {
                    Id = a.Key,
                    SystemGeneratedId = a.Max(x => x.SystemGeneratedId),
                    AssetClass = a.Max(x => x.AssetClass),
                    PoleId = a.Max(x => x.PoleId),
                    AssetType = new AssetTypeBriefViewModel
                    {
                        Id = a.Max(x => x.AssetTypeId),
                        Name = a.Max(x => x.AssetTypeName)
                    },
                    Condition = new ConditionBriefViewModel
                    {
                        Id = a.Max(x => x.ConditionId),
                        Name = a.Max(x => x.ConditionName)
                    },
                    MaintenanceCost = a.Max(x => x.MaintenanceCost),
                    NextMaintenanceYear = a.Max(x => x.NextMaintenanceYear),
                    Description = a.Max(x => x.Description),
                    InstalledDate = a.Max(x => x.InstalledDate),
                    Intersection = a.Max(x => x.Intersection),
                    Latitude = a.Max(x => x.Latitude),
                    Longitude = a.Max(x => x.Longitude),
                    Manufacturer = new ManufacturerBriefViewModel
                    {
                        Id = a.Max(x => x.ManufacturerId),
                        Name = a.Max(x => x.ManufacturerName)
                    },
                    MUTCD = new MUTCDBriefViewModel
                    {
                        Id = a.Max(x => x.MUTCDId),
                        ImageUrl = a.Max(x => x.MUTCDImageUrl),
                        Code = a.Max(x => x.MUTCDCode)
                    },

                    ReplacementYear = a.Max(x => x.ReplacementYear),
                    Value = a.Max(x => x.Value),
                    ReplacementDate = a.Max(x => x.ReplacementDate),
                    NextMaintenanceDate = a.Max(x => x.NextMaintenanceDate)
                }).AsQueryable();

                var result = await assetsGroupedQueryable.Paginate(search);
                var associations = await GetAssetsSubLevels(searchFilters);
                var attachments = await GetAssetsAttachments(search);
                var notes = await _db.AssetNotes.GroupBy(x => x.AssetId).Select(x => new { Id = x.Key, Count = x.Count() }).Where(x => x.Count > 0).ToListAsync();
                if (result != null)
                {
                    var paginatedResult = new PaginatedResultModel<AssetDetailViewModel>();
                    paginatedResult.Items = result.Items.ToList();
                    foreach (var item in paginatedResult.Items)
                    {
                        var assetAssociations = associations.Where(x => x.Asset.Id == item.Id).ToList();
                        foreach (var association in assetAssociations)
                        {
                            //var key = searchFilters.ForMapData ? association.AssetTypeLevel1.Name.ToString() : association.AssetTypeLevel1.Id.ToString();
                            item.AssetAssociations.Add(association);
                        }
                        item.HasNotes = notes.Any(x => x.Id == item.Id);
                        item.ImagesList = attachments.Where(x => x.EntityId == item.Id).ToList();

                    }
                    paginatedResult._meta = result._meta;
                    paginatedResult._links = result._links;
                    var response = new RepositoryResponseWithModel<PaginatedResultModel<AssetDetailViewModel>> { ReturnModel = paginatedResult };
                    return response;
                }
                _logger.LogWarning($"No record found for {typeof(Asset).FullName} in GetAll()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAll() method for {typeof(Asset).FullName} threw an exception.");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<bool> IsAssetIdUnique(long id, string assetId)
        {
            return (await _db.Assets.Where(x => x.SystemGeneratedId == assetId && x.Id != id && x.IsDeleted == false).CountAsync()) < 1;
        }

        public async Task<IRepositoryResponse> GetNotesByAssetId(long id)
        {
            try
            {
                var notes = await (from n in _db.AssetNotes.Include(x => x.Asset)
                                   join u in _db.Users on n.CreatedBy equals u.Id
                                   where (n.AssetId == id)
                                   select new AssetNotesViewModel
                                   {
                                       Id = n.Id,
                                       AssetId = n.AssetId,
                                       Description = n.Description,
                                       FileUrl = n.FileUrl,
                                       CreatedOn = n.CreatedOn,
                                       CreatedBy = u.FirstName + " " + u.LastName,
                                   }).ToListAsync();
                var response = new RepositoryResponseWithModel<List<AssetNotesViewModel>> { ReturnModel = notes };
                return response;
            }
            catch (Exception ex)
            {
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> SaveNotes(AssetNotesViewModel model)
        {
            try
            {
                model.FileUrl = _fileHelper.Save(model);
                var mappedNotes = _mapper.Map<AssetNotes>(model);
                await _db.AddAsync(mappedNotes);
                await _db.SaveChangesAsync();
                return _response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(WorkOrder).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> RePin(AssetRePinViewModel model)
        {
            try
            {
                var asset = await _db.Assets.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (asset != null)
                {
                    asset.Latitude = model.Latitude;
                    asset.Longitude = model.Longitude;
                    await _db.SaveChangesAsync();
                    return _response;
                }
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RePin() for {typeof(WorkOrder).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<string> GetAttachmentUrl(long id)
        {
            try
            {
                var attachmentUrl = await _db.Attachments.Where(x => x.EntityId == id).Select(x => x.Url).FirstOrDefaultAsync();
                return attachmentUrl;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IRepositoryResponse> GetAssetsForMap()
        {
            try
            {
                var assets = await _db.Assets.Where(x => x.IsDeleted == false).ToListAsync();
                var model = new AssetsMapViewModel();
                if (assets != null && assets.Count > 0)
                {
                    model.Longitude = assets.Select(x => x.Longitude).FirstOrDefault();
                    model.Latitude = assets.Select(x => x.Latitude).FirstOrDefault();
                    model.Assets = _mapper.Map<List<AssetDetailViewModel>>(assets);

                    var response = new RepositoryResponseWithModel<AssetsMapViewModel> { ReturnModel = model };
                    return response;
                }

                _logger.LogWarning($"No record found for {typeof(Asset).FullName} in GetAssetsForMap()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAssetsForMap() for {typeof(Asset).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }


        }

        public async Task<IRepositoryResponse> GetAssetTypeLevels(long assetTypeId, long? assetId, bool showAll = false)
        {
            try
            {
                var result = await _db.AssetTypeLevels1.Include(x => x.AssetTypeLevel2)
                    .Where(x =>
                        (showAll || x.AssetTypeId == assetTypeId)
                        &&
                        x.AssetTypeLevel2.Count() > 0
                    )
                    .AsNoTracking()
                    .ToListAsync();
                if (result != null)
                {
                    var mappedResult = _mapper.Map<List<AssetTypeLevel1DetailViewModel>>(result);
                    if (assetId != null && assetId > 0)
                    {
                        var assetAssociations = await _db.AssetAssociations.Where(x => x.AssetId == assetId).ToListAsync();
                        foreach (var item in assetAssociations)
                        {
                            var assetLevel1 = mappedResult.Where(x => x.Id == item.AssetTypeLevel1Id).FirstOrDefault();
                            var assetLevel2 = assetLevel1?.AssetTypeLevel2.Where(x => x.Id == item.AssetTypeLevel2Id).FirstOrDefault();
                            if (assetLevel2 != null)
                            {
                                assetLevel2.IsChecked = true;
                            }
                        }
                    }
                    var response = new RepositoryResponseWithModel<List<AssetTypeLevel1DetailViewModel>> { ReturnModel = mappedResult };
                    return response;
                }
                _logger.LogWarning($"No record found for id:{assetTypeId} for {typeof(Asset).FullName} in GetById()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(Asset).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> GetAssetTypeLevelsForAPI(long assetTypeId, long assetId)
        {
            try
            {
                var result = await _db.AssetTypeLevels1.Include(x => x.AssetTypeLevel2)
                    .Where(x =>
                        (x.AssetTypeId == assetTypeId)
                        &&
                        x.AssetTypeLevel2.Count() > 0
                    )
                    .AsNoTracking()
                    .ToListAsync();
                if (result != null)
                {
                    var resultList = new List<AssetAssociationDetailAPIViewModel>();
                    if (assetId != null && assetId > 0)
                    {
                        var assetAssociations = await _db.AssetAssociations.Where(x => x.AssetId == assetId).ToListAsync();
                        foreach (var item in result)
                        {
                            var association = assetAssociations.Where(x => x.AssetTypeLevel1Id == item.Id).FirstOrDefault();
                            var assetAssociation = new AssetAssociationDetailAPIViewModel
                            {
                                Id = association?.Id ?? 0,
                                AssetId = assetId,
                                AssetTypeId = assetTypeId,
                                AssetTypeLevel1 = new AssetTypeLevel1DetailAPIViewModel
                                {
                                    Id = item.Id,
                                    Name = item.Name,
                                    SelectedAssetTypeLevel2Id = association?.AssetTypeLevel2Id ?? 0,
                                    AssetTypeLevel2 = item.AssetTypeLevel2.Select(x => new AssetTypeLevel2DetailAPIViewModel
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                        CreatedOn = x.CreatedOn
                                    }).ToList()
                                }
                            };
                            resultList.Add(assetAssociation);
                        }
                    }
                    var response = new RepositoryResponseWithModel<List<AssetAssociationDetailAPIViewModel>> { ReturnModel = resultList };
                    return response;
                }
                _logger.LogWarning($"No record found for id:{assetTypeId} for {typeof(Asset).FullName} in GetById()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(Asset).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> UploadImages(AssetAddImageViewModel model)
        {
            try
            {
                await CreateAssetAttachments(model, model.Id);
                return _response;
            }
            catch (Exception ex)
            {
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> UpdateAssociations(AssetModifyViewModel model)
        {
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var assetId = model.AssetAssociations.Select(x => x.Asset.Id).FirstOrDefault();
                await UpdateAssetAssociation(model.AssetAssociations.Where(x => x.AssetTypeLevel2.Id > 0).ToList(), assetId ?? 0);
                if (model.Condition.Id > 0)
                {
                    var asset = await _db.Assets.Where(x => x.Id == assetId).FirstOrDefaultAsync();
                    if (asset != null)
                    {
                        asset.ConditionId = model.Condition.Id;
                        await _db.SaveChangesAsync();
                    }
                }
                await transaction.CommitAsync();
                return _response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response.BadRequestResponse(_response);
            }
        }

        private async Task UpdateAssetAssociation(List<AssetAssociationDetailViewModel> associations, long id)
        {
            //remove existing associations
            var exisitngRecords = await _db.AssetAssociations.Where(x => x.AssetId == id).IgnoreAutoIncludes().ToListAsync();
            if (exisitngRecords?.Count > 0)
                _db.RemoveRange(exisitngRecords);

            //create new
            var mappedModels = _mapper.Map<List<AssetAssociation>>(associations);
            if (mappedModels.Any(x => x.AssetId < 1))
                mappedModels.ForEach(x => x.AssetId = id);

            await _db.AddRangeAsync(mappedModels);
            await _db.SaveChangesAsync();
        }

        private async Task CreateAssetAttachments(IAttachmentList viewModel, long id)
        {
            if (viewModel.Images.Count > 0)
            {
                var attachments = new List<AttachmentVM>();
                foreach (var image in viewModel.Images)
                {
                    attachments.Add(new AttachmentVM() { File = image });
                }
                attachments.ForEach(x => x.EntityId = id);
                attachments.ForEach(x => x.EntityType = Enums.AttachmentEntityType.Assets);
                await _attachmentService.CreateMultiple(attachments);
            }
        }

        public async Task<List<AssetAssociationDetailViewModel>> GetAssetsSubLevels(AssetSearchViewModel search)
        {
            var searchFilters = search as AssetSearchViewModel;
            var filters = await SetQueryFilter(searchFilters);
            var assetAssociationQueryable = (from a in _db.Assets.Where(filters)
                                             join at in _db.AssetTypes on a.AssetTypeId equals at.Id
                                             join aa in _db.AssetAssociations on a.Id equals aa.AssetId
                                             join al1 in _db.AssetTypeLevels1 on aa.AssetTypeLevel1Id equals al1.Id
                                             join al2 in _db.AssetTypeLevels2 on aa.AssetTypeLevel2Id equals al2.Id
                                             //where
                                             //(
                                             //   searchFilters == null ||
                                             //   searchFilters.AssetTypeLevel2 == null ||
                                             //   searchFilters.AssetTypeLevel2.Count == 0 ||
                                             //   searchFilters.AssetTypeLevel2.Contains(al2.Id)
                                             //)
                                             select new AssetAssociationDetailViewModel()
                                             {
                                                 Id = aa.Id,
                                                 Asset = new AssetBriefViewModel()
                                                 {
                                                     Id = a.Id,
                                                     Name = a.SystemGeneratedId,
                                                 },
                                                 AssetType = new AssetTypeBriefViewModel()
                                                 {
                                                     Id = at.Id,
                                                     Name = at.Name,
                                                 },
                                                 AssetTypeLevel1 = new AssetTypeLevel1BriefViewModel()
                                                 {
                                                     Id = al1.Id,
                                                     Name = al1.Name
                                                 },
                                                 AssetTypeLevel2 = new AssetTypeLevel2BriefViewModel()
                                                 {
                                                     Id = al2.Id,
                                                     Name = al2.Name
                                                 }
                                             }).AsQueryable();
            return await assetAssociationQueryable.ToListAsync();
        }

        private async Task<List<AttachmentVM>> GetAssetsAttachments(IBaseSearchModel search)
        {
            var searchFilters = search as AssetSearchViewModel;
            var filters = await SetQueryFilter(searchFilters);
            return await (from att in _db.Attachments
                          join a in _db.Assets.Where(filters) on new { AssetId = att.EntityId ?? 0, EntityType = (att.EntityType ?? AttachmentEntityType.Assets) } equals new { AssetId = a.Id, EntityType = AttachmentEntityType.Assets }
                          select new AttachmentVM
                          {
                              Id = att.Id,
                              Name = att.Name,
                              Url = att.Url,
                              EntityId = (long)att.EntityId
                          }).ToListAsync();
        }

        private async Task<IQueryable<AssetViewModel>> GetAssetsQueryable(IBaseSearchModel search)
        {
            var defaultDate = DateTime.MinValue;
            var searchFilters = search as AssetSearchViewModel;
            var filters = await SetCustomQueryFilter(searchFilters);

            return (from a in _db.Assets
                    join at in _db.AssetTypes on a.AssetTypeId equals at.Id
                    join c in _db.Conditions on a.ConditionId equals c.Id into cl
                    from c in cl.DefaultIfEmpty()
                    join m in _db.Manufacturers on a.ManufacturerId equals m.Id into ml
                    from m in ml.DefaultIfEmpty()
                    join mutc in _db.MUTCDs on a.MUTCDId equals mutc.Id into mutcl
                    from mutc in mutcl.DefaultIfEmpty()
                    join aa in _db.AssetAssociations on a.Id equals aa.AssetId into aal
                    from aa in aal.DefaultIfEmpty()
                    join atl1 in _db.AssetTypeLevels1 on aa.AssetTypeLevel1Id equals atl1.Id into atl1l
                    from atl1 in atl1l.DefaultIfEmpty()
                    join atl2 in _db.AssetTypeLevels2 on aa.AssetTypeLevel2Id equals atl2.Id into atl2l
                    from atl2 in atl2l.DefaultIfEmpty()
                    select new AssetViewModel
                    {
                        AssetPkId = a.Id,
                        SystemGeneratedId = a.SystemGeneratedId,
                        AssetTypeId = at.Id,
                        AssetTypeName = at.Name,
                        AssetClass = a.AssetClass,
                        PoleId = a.PoleId,
                        ConditionId = c == null ? 0 : c.Id,
                        ConditionName = c == null ? "" : c.Name,
                        MaintenanceCost = a.MaintenanceCost ?? 0,
                        NextMaintenanceYear = a.NextMaintenanceYear,
                        Description = a.Description,
                        InstalledDate = a.InstalledDate,
                        Intersection = a.Intersection,
                        Latitude = a.Latitude,
                        Longitude = a.Longitude,
                        ManufacturerId = m == null ? 0 : m.Id,
                        ManufacturerName = m == null ? "" : m.Name,
                        MUTCDId = mutc == null ? 0 : mutc.Id,
                        MUTCDCode = mutc == null ? "" : mutc.Code,
                        MUTCDImageUrl = mutc == null ? "" : mutc.ImageUrl,
                        ReplacementYear = a.ReplacementYear,
                        Value = a.Value ?? 0,
                        ReplacementDate = a.ReplacementDate ?? defaultDate,
                        NextMaintenanceDate = a.NextMaintenanceDate ?? defaultDate,
                        AssetTypeLevel2Id = atl2.Id,
                    }).Where(filters).AsQueryable();

        }

        public async Task<bool> InitializeExcelData(ExcelFileVM model)
        {
            var data = _reader.GetData(model.File.OpenReadStream());
            var excelDataList = _reader.AddModel<AssetExcelSheetViewModel>(data, 0, 0);
            var dynamicColumnNames = _reader.GetColumnNamesFromExcel(model.File.OpenReadStream(), 11, 15);

            var response = await ProcessExcelData(excelDataList, dynamicColumnNames);
            return response;
        }

        private async Task<bool> ProcessExcelData(List<AssetExcelSheetViewModel> excelDataList, List<string> dynamicColumnNames)
        {
            try
            {
                bool insertStopSignsLevel = false;
                bool insertSchoolSignsLevel = false;
                var mountTypes = await _db.MountTypes.ToListAsync();
                var mutcds = await _db.MUTCDs.ToListAsync();
                var assets = await _db.Assets.ToListAsync();
                var assetTypes = await _db.AssetTypes.ToListAsync();

                var assetTypeId = await GetAssetTypeId(assetTypes, excelDataList.First().AssetType);

                var assetTypeLevel1s = await _db.AssetTypeLevels1.Include(x => x.AssetTypeLevel2).ToListAsync();


                //var signPosts = excelDataList.Select(x => x.SignPost).Where(x => x != null && x?.Trim() != "").Distinct().ToList();
                //var signMounts = excelDataList.Select(x => x.SignMount).Where(x => x != null && x?.Trim() != "").Distinct().ToList();
                //var signHardwares = excelDataList.Select(x => x.SignHardware).Where(x => x != null && x?.Trim() != "").Distinct().ToList();
                //var signConditions = excelDataList.Select(x => x.SignCondition).Where(x => x != null && x?.Trim() != "").Distinct().ToList();
                //var signDimensions = excelDataList.Select(x => x.SignDimension).Where(x => x != null && x?.Trim() != "").Distinct().ToList();
                //var signMountTypes = excelDataList.Select(x => x.SignMountType).Where(x => x != null && x?.Trim() != "").Distinct().ToList();


                //var signPostId = await SetLevel1(assetTypeLevel1s, signPosts, "Sign Post", assetTypeId);
                //var signMountId = await SetLevel1(assetTypeLevel1s, signMounts, "Sign Mount", assetTypeId);
                //var signHardwareId = await SetLevel1(assetTypeLevel1s, signHardwares, "Sign Hardware", assetTypeId);
                //var signConditionId = await SetLevel1(assetTypeLevel1s, signConditions, "Sign Condition", assetTypeId);
                //var signDimensionId = await SetLevel1(assetTypeLevel1s, signDimensions, "Sign Dimension", assetTypeId);
                //var signMountTypeId = await SetLevel1(assetTypeLevel1s, signMountTypes, "Mount Type", assetTypeId);


                var distinctValues = new Dictionary<string, List<string>>();

                foreach (var columnName in dynamicColumnNames)
                {
                    var property = typeof(AssetExcelSheetViewModel).GetProperty(columnName);
                    if (property != null)
                    {
                        var values = excelDataList
                            .Select(x => property.GetValue(x)?.ToString())
                            .Where(x => x != null && x?.Trim() != "")
                            .Distinct()
                            .ToList();

                        distinctValues[columnName] = values;
                    }
                }

                var levelIds = new Dictionary<string, string>();

                foreach (var columnName in distinctValues.Keys)
                {
                    var values = distinctValues[columnName];
                    var levelId = await SetLevel1(assetTypeLevel1s, values, columnName.AddSpaceBeforeCapital(), assetTypeId);
                    levelIds[columnName] = levelId.ToString();
                }


                var assetTypeLevel2s = await _db.AssetTypeLevels2.ToListAsync();



                var count = 1;
                foreach (var item in excelDataList)
                {

                    item.MUTCDId = await GetMUTCDId(mutcds, item);
                    //item.MountTypeId = await GetMountTypeId(mountTypes, item);
                    var dbAsset = assets.Where(x => x.Latitude == item.Latitude && x.Longitude == item.Longitude).FirstOrDefault();
                    if (dbAsset == null)
                    {
                        var mappedData = _mapper.Map<Asset>(item);
                        if (string.IsNullOrEmpty(mappedData.SystemGeneratedId))
                        {
                            mappedData.SystemGeneratedId = "AST-" + (count + 1).ToString("D4");
                        }
                        mappedData.AssetTypeId = assetTypeId;
                        mappedData.MountTypeId = null;
                        mappedData.Intersection = item.StreetName;
                        mappedData.InstalledDate = DateTime.UtcNow;
                        mappedData.ReplacementDate = DateTime.UtcNow.AddYears(1);
                        mappedData.NextMaintenanceDate = DateTime.UtcNow.AddYears(1);
                        mappedData.NextMaintenanceYear = Enums.MaintenanceCycleCatalog.OneYear;
                        mappedData.ReplacementYear = Enums.ReplacementCycleCatalog.OneYear;
                        await _db.AddAsync(mappedData);
                        await _db.SaveChangesAsync();
                        assets.Add(mappedData);

                        await SetAssetAssociations(assetTypeId, levelIds, assetTypeLevel2s, item, mappedData);

                        //  await SetAssetAssociations(assetTypeId, signPostId, signMountId, signHardwareId, signConditionId, signDimensionId, signMountTypeId, assetTypeLevel2s, item, mappedData);
                    }
                    count++;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task SetAssetAssociations(long assetTypeId, Dictionary<string, string> levelIds, List<AssetTypeLevel2> assetTypeLevel2s, AssetExcelSheetViewModel item, Asset mappedData)
        {
            var assetAssociationList = new List<AssetAssociation>();
            Random rand = new Random();

            var properties = typeof(AssetExcelSheetViewModel).GetProperties()
                .Where(prop => levelIds.ContainsKey(prop.Name));

            foreach (var property in properties)
            {
                var columnName = property.Name;
                var level1Id = Convert.ToInt64(levelIds[columnName]);
                var propertyValue = property.GetValue(item)?.ToString();

                if (propertyValue == " ")
                {
                    var trimmed = propertyValue.Trim();
                    var removed = propertyValue.RemoveWhitespace();
                }

                var level2Id =
                !string.IsNullOrEmpty(propertyValue.RemoveWhitespace())
                ?
                    assetTypeLevel2s
                        .Where(x => x.AssetTypeLevel1Id == level1Id && x.Name.ToLower().Trim() == propertyValue.ToLower().Trim())
                        .Select(x => x.Id)
                        .FirstOrDefault()
                :
                    assetTypeLevel2s
                        .Where(x => x.AssetTypeLevel1Id == level1Id)
                        .OrderBy(x => rand.Next())
                        .Select(x => x.Id)
                        .FirstOrDefault();

                var assetAssociation = new AssetAssociation
                {
                    AssetId = mappedData.Id,
                    AssetTypeId = assetTypeId,
                    AssetTypeLevel1Id = level1Id,
                    AssetTypeLevel2Id = level2Id
                };
                assetAssociationList.Add(assetAssociation);
            }

            // Add all associations to the database
            await _db.AddRangeAsync(assetAssociationList);
            await _db.SaveChangesAsync();
        }



        private async Task SetAssetAssociations(long assetTypeId, long signPostId, long signMountId, long signHardwareId, long signConditionId, long signDimensionId, long signMountTypeId, List<AssetTypeLevel2> assetTypeLevel2s, AssetExcelSheetViewModel item, Asset mappedData)
        {
            var assetAssociationList = new List<AssetAssociation>();
            Random rand = new Random();
            var assetAssociationForSignPost = new AssetAssociation
            {
                AssetId = mappedData.Id,
                AssetTypeId = assetTypeId,
                AssetTypeLevel1Id = signPostId,
                AssetTypeLevel2Id =
                    string.IsNullOrEmpty(item.SignPost)
                    ?
                        assetTypeLevel2s
                            .Where(x => x.AssetTypeLevel1Id == signPostId && x.Name.ToLower().Trim() == item.SignPost.ToLower().Trim())
                            .Select(x => x.Id)
                            .FirstOrDefault()
                    :
                        assetTypeLevel2s
                            .Where(x => x.AssetTypeLevel1Id == signPostId)
                            .OrderBy(x => rand.Next())
                            .Select(x => x.Id)
                            .FirstOrDefault()
            };
            assetAssociationList.Add(assetAssociationForSignPost);

            var assetAssociationForSignMount = new AssetAssociation
            {
                AssetId = mappedData.Id,
                AssetTypeId = assetTypeId,
                AssetTypeLevel1Id = signMountId,
                AssetTypeLevel2Id =
                   string.IsNullOrEmpty(item.SignMount)
                   ?
                       assetTypeLevel2s
                           .Where(x => x.AssetTypeLevel1Id == signMountId && x.Name.ToLower().Trim() == item.SignMount.ToLower().Trim())
                           .Select(x => x.Id)
                           .FirstOrDefault()
                   :
                       assetTypeLevel2s
                           .Where(x => x.AssetTypeLevel1Id == signMountId)
                           .OrderBy(x => rand.Next())
                           .Select(x => x.Id)
                           .FirstOrDefault()
            };
            assetAssociationList.Add(assetAssociationForSignMount);

            var assetAssociationForSignHardware = new AssetAssociation
            {
                AssetId = mappedData.Id,
                AssetTypeId = assetTypeId,
                AssetTypeLevel1Id = signHardwareId,
                AssetTypeLevel2Id =
                   string.IsNullOrEmpty(item.SignHardware)
                   ?
                       assetTypeLevel2s
                           .Where(x => x.AssetTypeLevel1Id == signHardwareId && x.Name.ToLower().Trim() == item.SignHardware.ToLower().Trim())
                           .Select(x => x.Id)
                           .FirstOrDefault()
                   :
                       assetTypeLevel2s
                           .Where(x => x.AssetTypeLevel1Id == signHardwareId)
                           .OrderBy(x => rand.Next())
                           .Select(x => x.Id)
                           .FirstOrDefault()
            };
            assetAssociationList.Add(assetAssociationForSignHardware);


            var assetAssociationForSignCondition = new AssetAssociation
            {
                AssetId = mappedData.Id,
                AssetTypeId = assetTypeId,
                AssetTypeLevel1Id = signConditionId,
                AssetTypeLevel2Id =
                   string.IsNullOrEmpty(item.SignCondition)
                   ?
                       assetTypeLevel2s
                           .Where(x => x.AssetTypeLevel1Id == signConditionId && x.Name.ToLower().Trim() == item.SignCondition.ToLower().Trim())
                           .Select(x => x.Id)
                           .FirstOrDefault()
                   :
                       assetTypeLevel2s
                           .Where(x => x.AssetTypeLevel1Id == signConditionId)
                           .OrderBy(x => rand.Next())
                           .Select(x => x.Id)
                           .FirstOrDefault()
            };
            assetAssociationList.Add(assetAssociationForSignCondition);


            var assetAssociationForSignDimension = new AssetAssociation
            {
                AssetId = mappedData.Id,
                AssetTypeId = assetTypeId,
                AssetTypeLevel1Id = signDimensionId,
                AssetTypeLevel2Id =
                   string.IsNullOrEmpty(item.SignDimension)
                   ?
                       assetTypeLevel2s
                           .Where(x => x.AssetTypeLevel1Id == signDimensionId && x.Name.ToLower().Trim() == item.SignDimension.ToLower().Trim())
                           .Select(x => x.Id)
                           .FirstOrDefault()
                   :
                       assetTypeLevel2s
                           .Where(x => x.AssetTypeLevel1Id == signDimensionId)
                           .OrderBy(x => rand.Next())
                           .Select(x => x.Id)
                           .FirstOrDefault()
            };
            assetAssociationList.Add(assetAssociationForSignDimension);


            var assetAssociationForSignMountType = new AssetAssociation
            {
                AssetId = mappedData.Id,
                AssetTypeId = assetTypeId,
                AssetTypeLevel1Id = signMountTypeId,
                AssetTypeLevel2Id =
                  string.IsNullOrEmpty(item.SignMountType)
                  ?
                      assetTypeLevel2s
                          .Where(x => x.AssetTypeLevel1Id == signMountTypeId && x.Name.ToLower().Trim() == item.SignMountType.ToLower().Trim())
                          .Select(x => x.Id)
                          .FirstOrDefault()
                  :
                      assetTypeLevel2s
                          .Where(x => x.AssetTypeLevel1Id == signMountTypeId)
                          .OrderBy(x => rand.Next())
                          .Select(x => x.Id)
                          .FirstOrDefault()
            };
            assetAssociationList.Add(assetAssociationForSignMountType);
            await _db.AddRangeAsync(assetAssociationList);
            await _db.SaveChangesAsync();
        }




        private async Task<long> GetMUTCDId(List<MUTCD> dbMutcds, AssetExcelSheetViewModel item)
        {
            var dbMutcd = dbMutcds.Where(x => x.Code.ToLower().Trim() == item.MUTCD.ToLower().Trim()).FirstOrDefault();
            var mutcdId = 0L;
            if (dbMutcd != null)
            {
                mutcdId = dbMutcd.Id;
            }
            else
            {
                var mutcdModel = new MUTCD
                {
                    Code = item.MUTCD,
                    Description = item.MUTCD
                };
                await _db.AddAsync(mutcdModel);
                await _db.SaveChangesAsync();
                dbMutcds.Add(mutcdModel);
                mutcdId = mutcdModel.Id;
            }
            return mutcdId;
        }

        private async Task<long> GetMountTypeId(List<MountType> dbMountTypes, AssetExcelSheetViewModel item)
        {
            var dbMountType = dbMountTypes.Where(x => x.Name.ToLower().Trim() == item.MountType.ToLower().Trim()).FirstOrDefault();
            var MountTypeId = 0L;
            if (dbMountType != null)
            {
                MountTypeId = dbMountType.Id;
            }
            else
            {
                var MountTypeModel = new MountType
                {
                    Name = item.MountType,
                };
                await _db.AddAsync(MountTypeModel);
                await _db.SaveChangesAsync();
                dbMountTypes.Add(MountTypeModel);
                MountTypeId = MountTypeModel.Id;
            }
            return MountTypeId;
        }

        private async Task<long> GetAssetTypeId(List<AssetType> dbAssetTypes, string assetName)
        {
            var dbAssetType = dbAssetTypes.Where(x => x.Name.ToLower().Trim() == assetName.ToLower().Trim()).FirstOrDefault();
            var AssetTypeId = 0L;
            if (dbAssetType != null)
            {
                AssetTypeId = dbAssetType.Id;
            }
            else
            {
                var AssetTypeModel = new AssetType
                {
                    Name = assetName,
                    Color = ""
                };
                await _db.AddAsync(AssetTypeModel);
                await _db.SaveChangesAsync();
                dbAssetTypes.Add(AssetTypeModel);
                AssetTypeId = AssetTypeModel.Id;
            }
            return AssetTypeId;
        }

        private async Task<long> SetLevel1(List<AssetTypeLevel1> dbLevel1s, List<string> excelSheetLevel2s, string name, long assetTypeId)
        {
            var dbLevel1 = dbLevel1s.Where(x => x.Name.ToLower().Trim() == name.ToLower().Trim()).FirstOrDefault();
            var level1Id = 0L;
            if (dbLevel1 != null)
            {
                level1Id = dbLevel1.Id;
                foreach (var item in excelSheetLevel2s)
                {
                    var dbLevel2 = dbLevel1.AssetTypeLevel2.Where(x => x.Name.ToLower().Trim() == item.ToLower().Trim()).FirstOrDefault();
                    if (dbLevel2 == null)
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            var level2 = new AssetTypeLevel2
                            {
                                AssetTypeId = assetTypeId,
                                Name = item,
                            };
                            dbLevel1.AssetTypeLevel2.Add(level2);
                        }
                    }
                }
                await _db.SaveChangesAsync();
            }
            else
            {
                var assetTypeLevel1 = new AssetTypeLevel1
                {
                    Name = name,
                    AssetTypeId = assetTypeId
                };
                foreach (var item in excelSheetLevel2s)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        var level2 = new AssetTypeLevel2
                        {
                            AssetTypeId = assetTypeId,
                            Name = item,
                        };
                        assetTypeLevel1.AssetTypeLevel2.Add(level2);
                    }

                }
                await _db.AddAsync(assetTypeLevel1);
                await _db.SaveChangesAsync();
                dbLevel1s.Add(assetTypeLevel1);
                level1Id = assetTypeLevel1.Id;
            }
            return level1Id;
        }



    }
}
