using Models.Common.Interfaces;
using ViewModels.Shared;
using Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Centangle.Common.ResponseHelpers.Models;
using Repositories.Common.Example.Interface;
using DataLibrary;
using Pagination;
using System.Linq.Expressions;
using ViewModels.AssignedPermissions;
using Microsoft.EntityFrameworkCore;
using ViewModels.Permission;
using Microsoft.AspNetCore.Mvc;
using Centangle.Common.ResponseHelpers;
using ViewModels.Users;
using Enums;
using ViewModels.Role;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Client;
using System.Reflection;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Repositories.Shared.UserInfoServices.Interface;
using DocumentFormat.OpenXml.InkML;
using Helpers.Authorization;
using Helpers.Extensions;

namespace Repositories.Common.Example
{
    public class AssignedPermissionService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<AssignedPermission, CreateViewModel, UpdateViewModel, DetailViewModel>, IAssignedPermissionService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        private readonly IMemoryCache _cache;
        private readonly IUserInfoService _userInfo;

        public AssignedPermissionService(
            ApplicationDbContext db,
            ILogger<AssignedPermissionService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IMemoryCache cache,
            IUserInfoService userInfo) : base(db, logger, mapper, response)
        {
            _db = db;
            _mapper = mapper;
            _response = response;
            _cache = cache;
            _userInfo = userInfo;
        }

        public async Task<IRepositoryResponse> GetAssignedPermissions(IBaseSearchModel search)
        {
            try
            {
                var filter = search as AssignedPermissionSearchViewModel;
                var permissions = _db.Permissions
                    .Select(x => new AssignedPermissionUpdateViewModel
                    {
                        Permission = new PermissionBriefViewModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Screen = x.Screen
                        }
                    })
                    .AsQueryable();

                var assignedPermissions = _db.AssignedPermissions.Include(x => x.Permission).Where(x =>
                        filter.EntityType == x.EntityType
                        &&
                        filter.EntityId == x.EntityId
                    ).AsNoTracking().IgnoreAutoIncludes().AsQueryable();

                var joinedData = await (from p in permissions
                                        join ap in assignedPermissions on p.Permission.Id equals ap.PermissionId into apl
                                        from ap in apl.DefaultIfEmpty()
                                        select new AssignedPermissionUpdateViewModel
                                        {
                                            Id = ap != null ? ap.Id : 0,
                                            ActiveStatus = ap != null ? ap.ActiveStatus : 0,
                                            EntityId = ap != null ? ap.EntityId : 0,
                                            EntityType = ap != null ? ap.EntityType : 0,
                                            Priority = ap != null ? ap.Priority : 0,
                                            Status = ap != null ? ap.Status : 0,
                                            Permission = new PermissionBriefViewModel
                                            {
                                                Id = p.Permission.Id,
                                                Name = p.Permission.Name,
                                                Screen = p.Permission.Screen
                                            }
                                        }).ToListAsync();

                var responseModel = new AssignPermissionDataViewModel
                {
                    Permissions = joinedData,
                    EntityId = filter.EntityId ?? 0,
                    EntityType = filter.EntityType ?? 0,
                };
                var response = new RepositoryResponseWithModel<AssignPermissionDataViewModel> { ReturnModel = responseModel };
                return response;
            }
            catch (Exception ex)
            {
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> AssignPermissions(AssignPermissionDataViewModel model)
        {
            try
            {
                var previousPermissions = await _db.AssignedPermissions.Where(x => x.EntityId == model.EntityId && x.EntityType == model.EntityType).ToListAsync();
                _db.RemoveRange(previousPermissions);
                await RemovePermissions(model.EntityId, model.EntityType);
                foreach (var i in model.Permissions)
                {
                    _db.AssignedPermissions.Add(new AssignedPermission
                    {
                        EntityId = model.EntityId,
                        EntityType = model.EntityType,
                        Status = i.Status,
                        PermissionId = i.Permission.Id ?? 0,
                    });
                }
                await _db.SaveChangesAsync();
                return _response;
            }
            catch (Exception ex)
            {
                _response.Message = $"Permissions not Saved. Error Message: {ex.Message}";
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<List<string>> SetUserPermissions(string cacheKey, string userIdentifierKey, long userId, List<long> roleIdList)
        {
            try
            {
                var cachedPermissions = _cache.Get<Dictionary<string, string>>(cacheKey) ?? new Dictionary<string, string>();
                var permissions = await _db.AssignedPermissions
                .Include(x => x.Permission)
                .Where(x =>
                    (
                        (userId > 0 && x.EntityType == PermissionEntityType.User && x.EntityId == userId)
                        ||
                        (roleIdList.Count > 0 && x.EntityType == PermissionEntityType.Role && roleIdList.Contains(x.EntityId))
                    )
                )
                .ToListAsync();

                var allowedPermissions = new List<string>();
                foreach (var p in permissions.GroupBy(x => x.PermissionId))
                {
                    var userAllowed = p.Any(x => x.EntityType == PermissionEntityType.User && x.Status == PermissionStatus.Allowed);
                    var userNotAllowed = p.Any(x => x.EntityType == PermissionEntityType.User && x.Status == PermissionStatus.NotAllowed);
                    var roleAllowed = p.Any(x => x.EntityType == PermissionEntityType.Role && x.Status == PermissionStatus.Allowed);
                    var roleNotAllowed = p.Any(x => x.EntityType == PermissionEntityType.Role && x.Status == PermissionStatus.NotAllowed);

                    if (userAllowed && !userNotAllowed)
                    {
                        allowedPermissions.Add(p.Max(x => x.Permission?.Name.RemoveWhitespace()));
                    }
                    else if (roleAllowed && (!roleNotAllowed && !userNotAllowed))
                    {
                        allowedPermissions.Add(p.Max(x => x.Permission?.Name.RemoveWhitespace()));
                    }
                }

                if (allowedPermissions.Count > 0)
                {
                    cachedPermissions.Add(userIdentifierKey, string.Join(",", allowedPermissions));
                }
                _cache.Set(cacheKey, cachedPermissions, TimeSpan.FromMinutes(10));
                return allowedPermissions;
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task RemovePermissions(long entityId, PermissionEntityType entityType)
        {
            var cacheKey = AuthorizationContants.CacheKey; ;
            var cachedPermissions = _cache.Get<Dictionary<string, string>>(cacheKey) ?? new();
            string key = "";
            if (entityType == PermissionEntityType.User)
            {
                key = AuthorizationHelper.GetUserIdentifierKey(entityId.ToString());
            }
            else if (entityType == PermissionEntityType.Role)
            {
                key = AuthorizationHelper.GetRoleIdentifierKey(entityId.ToString());
            }

            var permissionsToRemove = cachedPermissions?.Where(x => x.Key.Contains(key)).ToList() ?? new();
            foreach (var permissionToRemove in permissionsToRemove)
            {
                cachedPermissions?.Remove(permissionToRemove.Key);
            }
            _cache.Set(cacheKey, cachedPermissions, TimeSpan.FromMinutes(20));
        }

        public async Task<UserBriefViewModel> GetUser(long entityId)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Id == entityId).FirstOrDefaultAsync();
                return new UserBriefViewModel { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Username = user.UserName };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<RoleBriefViewModel> GetRole(long entityId)
        {
            try
            {
                var role = await _db.Roles.Where(x => x.Id == entityId).FirstOrDefaultAsync();
                return new RoleBriefViewModel { Id = role.Id, Name = role.Name };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

