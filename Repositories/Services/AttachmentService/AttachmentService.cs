using AutoMapper;
using Centangle.Common.ResponseHelpers;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Enums;
using Helpers.Extensions;
using Helpers.File;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Models.Shared;
using Pagination;
using Repositories.Services.AttachmentService.Interface;
using Repositories.Services.AuthenticationService;
using ViewModels.Shared;

namespace Repositories.Services.AttachmentService
{
    public class AttachmentService : IAttachment
    {
        private readonly IMapper _mapper;
        private readonly IFileHelper _fileHelper;
        public readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepositoryResponse _response;
        private readonly ILogger<IdentityService> _logger;

        public AttachmentService(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            IRepositoryResponse response, ILogger<IdentityService> logger, IMapper mapper, IFileHelper fileHelper)
        {
            _db = db;
            _userManager = userManager;
            _response = response;
            _logger = logger;
            _mapper = mapper;
            _fileHelper = fileHelper;
        }

        public async Task<IRepositoryResponse> CreateMultiple(List<AttachmentVM> attachments)
        {
            try
            {
                await CreateAttachments(attachments);
                return new RepositoryResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create Attachment threw the following exception");
            }

            return Response.BadRequestResponse(_response);
        }

        public async Task<IRepositoryResponse> CreateSingle(AttachmentVM attachment)
        {
            try
            {
                List<string> urls = new List<string>();
                _logger.LogDebug("Create Attachment method, number of Attachments: ");

                var imgName = DateTime.Now.Ticks;
                attachment.Url = _fileHelper.Save(attachment);
                if (!string.IsNullOrEmpty(attachment.Url))
                {
                    var dbAttachments = _mapper.Map<Attachment>(attachment);
                    var contentLengthInBytes = attachment.File.Length;
                    var contentLengthInKB = Math.Round((contentLengthInBytes / 1024.0), 2);
                    dbAttachments.Size = contentLengthInKB + " Kb";
                    _logger.LogDebug("Create Attachmentt method, attachments mapped");

                    await _db.AddAsync(dbAttachments);
                    await _db.SaveChangesAsync();
                    _logger.LogDebug("Create Attachmentt method, attachments saved");

                    return _response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create Attachment threw the following exception");
            }

            return Response.BadRequestResponse(_response);
        }

        public async Task<IRepositoryResponse> Update(List<AttachmentVM> attachments, long entityId, AttachmentEntityType entityType)
        {
            try
            {
                if (entityId > 0)
                {
                    var attachmentOld = await _db.Attachments.Where(x => x.EntityId == entityId && x.EntityType == entityType).ToListAsync();

                    var attachmentsNew = _mapper.Map<List<Attachment>>(attachments.Where(x => x.Id != null && x.Id > 0));

                    DeleteAttachment(attachmentsNew, attachmentOld);

                    await CreateAttachments(attachments.Where(x => (x.Id == null || x.Id == 0) && x.File != null).ToList());

                    var responseModel = new RepositoryResponseWithModel<List<AttachmentVM>>();
                    responseModel.ReturnModel = attachments;
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update Attachment threw the following exception");
            }
            return Response.BadRequestResponse(_response);
        }

        private async Task CreateAttachments(List<AttachmentVM> attachments)
        {
            if (attachments.Count() > 0)
            {
                _logger.LogDebug("Create Attachment method, number of Attachments: ", attachments.Count());

                foreach (var attachment in attachments)
                {
                    attachment.Url = _fileHelper.Save(attachment);
                    attachment.Name = attachment.File.FileName;
                }

                var dbAttachments = _mapper.Map<List<Attachment>>(attachments);
                _logger.LogDebug("Create Attachmentt method, attachments mapped");

                await _db.AddRangeAsync(dbAttachments);
                await _db.SaveChangesAsync();
                _logger.LogDebug("Create Attachmentt method, attachments saved");
            }
        }

        private void DeleteAttachment(List<Attachment> attachmentNew, List<Attachment> attachmentOld)
        {
            var removedAttachments = attachmentOld.Except(attachmentNew, new IdComparer<Attachment>());

            foreach (var attachment in removedAttachments)
            {
                attachment.IsDeleted = true;
            }
        }

        public async Task<IRepositoryResponse> GetAll<T>(IBaseSearchModel search)
        {
            var mappedResponseModel = new RepositoryResponseWithModel<PaginatedResultModel<T>>();
            try
            {
                var data = search as AttachmentSearchVM;
                IQueryable<Attachment> attachmentQueryable = (from i in _db.Attachments
                                                              where
                                                              (string.IsNullOrEmpty(data.Name) || i.Name.ToLower().Trim().Equals(data.Name.Trim().ToLower()))
                                                              &&
                                                              (data.EntityType == null || i.EntityType == data.EntityType)
                                                              &&
                                                              (data.EntityId == null || i.EntityId == data.EntityId)

                                                              select i).AsQueryable();

                var responseModel = new RepositoryResponseWithModel<PaginatedResultModel<Attachment>>();
                responseModel.ReturnModel = await attachmentQueryable.Paginate(search);

                if (responseModel?.ReturnModel?.Items?.Count > 0)
                {
                    var mappedList = _mapper.Map<List<T>>(responseModel?.ReturnModel?.Items);
                    mappedResponseModel.ReturnModel.Items = mappedList;
                    mappedResponseModel.ReturnModel._meta = responseModel?.ReturnModel?._meta ?? new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception thrown in GetAll of Attachment");
            }
            return mappedResponseModel;
        }

        public async Task<IRepositoryResponse> DeleteByEntity(long entityId, AttachmentEntityType entityType)
        {
            try
            {
                var attachmentQuery = _db.Attachments.Where(x => x.EntityId == entityId && x.EntityType == entityType).Select(x => x.Id);
                var attachmentIds = await attachmentQuery.ToListAsync();
                if (await DeleteAttachment(attachmentIds))
                    return _response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteByParent threw the above exception");
            }
            return Response.BadRequestResponse(_response);
        }

        public async Task<IRepositoryResponse> Delete(List<long> attachmentIds)
        {
            try
            {
                if (await DeleteAttachment(attachmentIds))
                    return _response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete Attachment threw the following exception");
            }

            return Response.BadRequestResponse(_response);
        }

        private async Task<bool> DeleteAttachment(List<long> attachmentIds)
        {
            try
            {
                var attachments = await _db.Attachments.Where(x => attachmentIds.Contains(x.Id)).ToListAsync();
                attachments.ForEach(x => x.IsDeleted = true);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteAttachment is AttachmentService threw the above exception");
                return false;
            }
        }

        public async Task<List<string>> GetUrls(AttachmentEntityType type, long entityId)
        {
            try
            {
                return (await _db.Attachments.Where(x => x.EntityType == type && x.EntityId == entityId).Select(x => x.Url ?? "").ToListAsync()) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetUrls in AttachmentService threw the following exception");
                return new List<string>();
            }
        }
    }

    public class IdComparer<T> : IEqualityComparer<T> where T : BaseDBModel
    {
        public bool Equals(T x, T y)
        {
            return x.Id == y.Id;
        }
        public int GetHashCode(T obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
