using Models.Common.Interfaces;
using ViewModels.Shared;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Centangle.Common.ResponseHelpers.Models;
using Repositories.Common.Permission.Interface;
using DataLibrary;
using Pagination;
using System.Linq.Expressions;
using ViewModels.Permission;
using Microsoft.EntityFrameworkCore;
using Models;
using Centangle.Common.ResponseHelpers;
using Repositories.Services.ExcelHelper.Interface;

namespace Repositories.Common.Permission
{
    public class PermissionService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<Models.Permission, CreateViewModel, UpdateViewModel, DetailViewModel>, IPermissionService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ApplicationDbContext _db;
        private readonly IRepositoryResponse _response;
        private readonly IExcelHelper _excelReader;

        public PermissionService(ApplicationDbContext db, ILogger<PermissionService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response,
            IExcelHelper excelReader
            ) : base(db, logger, mapper, response)
        {
            _db = db;
            _response = response;
           _excelReader = excelReader;
        }

        public override async Task<Expression<Func<Models.Permission, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as PermissionSearchViewModel;

            return x =>
                            (string.IsNullOrEmpty(searchFilters.Search.value) || x.Name.ToLower().Contains(searchFilters.Search.value.ToLower()))
                            &&
                            (string.IsNullOrEmpty(searchFilters.Name) || x.Name.ToLower().Contains(searchFilters.Name.ToLower()))
                        ;
        }

        public async Task<IRepositoryResponse> ProcessExcelPermissionsData(ExcelFileVM model)
        {
            var data = _excelReader.GetData(model.File.OpenReadStream());
            var mappedPermissions = _excelReader.AddModel<PermissionExcelViewModel>(data, 0, 0);
            var response = await SavePermissions(mappedPermissions);
            if (response)
            {
                return _response;
            }
            return Response.BadRequestResponse(_response);
        }

        private async Task<bool> SavePermissions(List<PermissionExcelViewModel> mappedPermissions)
        {
            try
            {
                var dbPermissions = await _db.Permissions.AsNoTracking().ToListAsync();
                var dbRoles = await _db.Roles.AsNoTracking().ToListAsync();
                var dbAssignedPermissions = await _db.AssignedPermissions.AsNoTracking().ToListAsync();

                foreach (var item in mappedPermissions)
                {
                    long permissionId = await SetPermission(dbPermissions, item);
                    var roleNames = item.Roles.Split(',');

                    foreach (var roleName in roleNames)
                    {
                        long roleId = await SetRole(dbRoles, roleName);
                        await SetAssignPermissions(item, permissionId, roleId, dbAssignedPermissions);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task SetAssignPermissions(PermissionExcelViewModel item, long permissionId, long roleId, List<AssignedPermission> dbAssignedPermissions)
        {
            var isAssignedPermissionsExists = dbAssignedPermissions.Any(x => x.EntityId == roleId && x.EntityType == Enums.PermissionEntityType.Role && x.PermissionId == permissionId);
            if (!isAssignedPermissionsExists)
            {
                var assignedPermission = new AssignedPermission
                {
                    PermissionId = permissionId,
                    EntityType = Enums.PermissionEntityType.Role,
                    EntityId = roleId,
                    Priority = item.Priority,
                    Status = Enums.PermissionStatus.Allowed,
                };
                await _db.AssignedPermissions.AddAsync(assignedPermission);
                await _db.SaveChangesAsync();
            }
        }

        private async Task<long> SetRole(List<ApplicationRole> dbRoles, string roleName)
        {
            var roleId = dbRoles.Where(x => x.Name.ToLower().Trim() == roleName.ToLower().Trim()).Select(x => x.Id).FirstOrDefault();
            if (roleId < 1)
            {
                var roleModel = new ApplicationRole
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                await _db.Roles.AddAsync(roleModel);
                await _db.SaveChangesAsync();
                dbRoles.Add(roleModel);
                roleId = roleModel.Id;
            }

            return roleId;
        }

        private async Task<long> SetPermission(List<Models.Permission> dbPermissions, PermissionExcelViewModel item)
        {
            var permissionId = dbPermissions.Where(x => x.Name.ToLower().Trim() == item.PermissionName.ToLower().Trim()).Select(x => x.Id).FirstOrDefault();
            if (permissionId < 1)
            {
                var permissionModel = new Models.Permission
                {
                    Name = item.PermissionName,
                    Screen = item.Screen
                };
                await _db.Permissions.AddAsync(permissionModel);
                await _db.SaveChangesAsync();
                dbPermissions.Add(permissionModel);
                permissionId = permissionModel.Id;
            }

            return permissionId;
        }
    }
}

