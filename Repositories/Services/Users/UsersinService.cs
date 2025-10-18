using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using DocumentFormat.OpenXml.Office2010.Excel;
using Enums;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Common.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class UsersinService<CreateViewModel, UpdateViewModel, DetailViewModel>
     : BaseService<IncidentUser, CreateViewModel, UpdateViewModel, DetailViewModel>,
       IUsersinService<CreateViewModel, UpdateViewModel, DetailViewModel>
     where DetailViewModel : class, IBaseCrudViewModel, new()
     where CreateViewModel : class, IBaseCrudViewModel, new()
     where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UsersinService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;

        public UsersinService(
            ApplicationDbContext db,
            ILogger<UsersinService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext)
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }
        public async Task<List<UserModifyViewModel.TeamViewModel>> GetAllTeams()
        {
            try
            {
                var teams = await _db.IncidentTeams
                    .Where(t => !t.IsDeleted)
                    .Select(t => new UserModifyViewModel.TeamViewModel
                    {
                        TeamId = t.Id,
                        TeamName = t.Name
                    })
                    .ToListAsync();

                return teams;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllTeams.");
                return new List<UserModifyViewModel.TeamViewModel>();
            }
        }
        public async Task<List<UserModifyViewModel.CompanyViewModel>> GetAllCompanies()
        {
            try
            {
                var companies = await _db.Company
                    .Where(c => !c.IsDeleted)
                    .Select(c => new UserModifyViewModel.CompanyViewModel
                    {
                        CompanyId = c.Id,
                        CompanyName = c.Name
                    })
                    .ToListAsync();
                return companies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllCompanies.");
                return new List<UserModifyViewModel.CompanyViewModel>();
            }
        }
        public async Task<List<UserModifyViewModel.IncidentRoleViewModel>> GetAllIncidentRoles()
        {
            try
            {
                var roles = await _db.IncidentRoles
                    .Where(r => !r.IsDeleted)
                    .Select(r => new UserModifyViewModel.IncidentRoleViewModel
                    {
                        IncidentRoleId = r.Id,
                        IncidentRoleName = r.Name
                    })
                    .ToListAsync();
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllIncidentRoles.");
                return new List<UserModifyViewModel.IncidentRoleViewModel>();
            }
        }
        public async Task<List<UserModifyViewModel>> GetAllUsers()
        {
            var list = new List<UserModifyViewModel>();
            try
            {
                var users = await (
                          from u in _db.IncidentUsers
                              .AsNoTracking()
                          where !u.IsDeleted
                          join c in _db.Company on u.CompanyId equals c.Id into companyGroup
                          from c in companyGroup.DefaultIfEmpty() // <-- LEFT JOIN
                          join r in _db.IncidentRoles on u.IncidentRoleId equals r.Id into roleGroup
                          from r in roleGroup.DefaultIfEmpty() // <-- LEFT JOIN
                          select new
                          {
                              u.Id,
                              TeamId = (long?)u.TeamId ?? 0,
                              CompanyId = (long?)u.CompanyId ?? 0,
                              CompanyName = c != null ? c.Name : "",
                              IncidentRoleId = (long?)u.IncidentRoleId ?? 0,
                              IncidentRoleName = r != null ? r.Name : "",
                              FirstName = u.FirstName ?? "",
                              LastName = u.LastName ?? "",
                              u.Telephone,
                              u.Email,
                              u.PinHash,
                              u.EmployeeType
                          }
                      ).ToListAsync();

                foreach (var t in users)
                {
                    // fetch team name using TeamId
                    var teamName = await _db.IncidentTeams
                        .Where(x => x.Id == t.TeamId)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync();

                    list.Add(new UserModifyViewModel
                    {
                        Id = t.Id,
                        TeamId = t.TeamId,
                        TeamName = teamName ?? string.Empty,
                        CompanyId = t.CompanyId,
                        CompanyName = t.CompanyName,
                        IncidentRoleId = t.IncidentRoleId,
                        IncidentRoleName = t.IncidentRoleName,
                        FirstName = t.FirstName,
                        LastName = t.LastName,
                        Telephone = t.Telephone,
                        Email = t.Email,
                        PinHash = t.PinHash,
                        EmployeeType = t.EmployeeType
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllUsers.");
                return new List<UserModifyViewModel>();
            }

            return list;
        }

        public async Task<long> SaveUser(UserModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var userId = 0L; // replace with actual user id from auth context if available

                var user = new IncidentUser
                {
                    TeamId = viewModel.TeamId,
                    CompanyId = viewModel.CompanyId,
                    IncidentRoleId = viewModel.IncidentRoleId,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Telephone = viewModel.Telephone,
                    Email = viewModel.Email,
                    PinHash = viewModel.PinHash,
                    IsDeleted = false,
                    ActiveStatus = ActiveStatus.Active,
                    CreatedOn = now,
                    CreatedBy = userId,
                    UpdatedOn = now,
                    UpdatedBy = userId,
                    EmployeeType = viewModel.EmployeeType
                };

                await _db.IncidentUsers.AddAsync(user);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return user.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveUser.");
                return 0;
            }
        }

        public async Task<long> UpdateUser(UserModifyViewModel viewModel)
        {
            try
            {
                // Fetch as no tracking (or using projection if you need)
                var user = await (
                            from u in _db.IncidentUsers
                            where !u.IsDeleted && u.Id == viewModel.Id
                            join c in _db.Company on u.CompanyId equals c.Id into companyGroup
                            from c in companyGroup.DefaultIfEmpty()
                            join r in _db.IncidentRoles on u.IncidentRoleId equals r.Id into roleGroup
                            from r in roleGroup.DefaultIfEmpty()
                            select new IncidentUser
                            {
                                Id = u.Id,
                                TeamId = (long?)u.TeamId ?? 0,
                                CompanyId = (long?)u.CompanyId ?? 0,
                                IncidentRoleId = (long?)u.IncidentRoleId ?? 0,
                                FirstName = u.FirstName ?? "",
                                LastName = u.LastName ?? "",
                                Telephone = u.Telephone,
                                Email = u.Email,
                                PinHash = u.PinHash,
                                EmployeeType = u.EmployeeType
                            }
                        ).FirstOrDefaultAsync();

                if (user == null)
                {
                    // if not found, create new
                    return await SaveUser(viewModel);
                }

                // Reattach the entity
                _db.IncidentUsers.Attach(user);

                // Update properties
                user.TeamId = viewModel.TeamId;
                user.CompanyId = viewModel.CompanyId;
                user.IncidentRoleId = viewModel.IncidentRoleId;
                user.FirstName = viewModel.FirstName;
                user.LastName = viewModel.LastName;
                user.Telephone = viewModel.Telephone;
                user.Email = viewModel.Email;
                user.PinHash = viewModel.PinHash;
                user.UpdatedOn = DateTime.UtcNow;
                user.EmployeeType = viewModel.EmployeeType;

                // Mark updated properties explicitly
                _db.Entry(user).Property(u => u.TeamId).IsModified = true;
                _db.Entry(user).Property(u => u.CompanyId).IsModified = true;
                _db.Entry(user).Property(u => u.IncidentRoleId).IsModified = true;
                _db.Entry(user).Property(u => u.FirstName).IsModified = true;
                _db.Entry(user).Property(u => u.LastName).IsModified = true;
                _db.Entry(user).Property(u => u.Telephone).IsModified = true;
                _db.Entry(user).Property(u => u.Email).IsModified = true;
                _db.Entry(user).Property(u => u.PinHash).IsModified = true;
                _db.Entry(user).Property(u => u.UpdatedOn).IsModified = true;
                _db.Entry(user).Property(u => u.EmployeeType).IsModified = true;

                await _db.SaveChangesAsync();
                return user.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateIncidentTeam.");
                return 0;
            }
        }

        public async Task<UserModifyViewModel> GetUserById(long id)
        {
            try
            {
                var user = await (
                            from u in _db.IncidentUsers.AsNoTracking()
                            where !u.IsDeleted && u.Id == id
                            join c in _db.Company on u.CompanyId equals c.Id into companyGroup
                            from c in companyGroup.DefaultIfEmpty()
                            join r in _db.IncidentRoles on u.IncidentRoleId equals r.Id into roleGroup
                            from r in roleGroup.DefaultIfEmpty()
                            select new
                            {
                                u.Id,
                                TeamId = (long?)u.TeamId ?? 0,
                                CompanyId = (long?)u.CompanyId ?? 0,
                                CompanyName = c != null ? c.Name : "",
                                IncidentRoleId = (long?)u.IncidentRoleId ?? 0,
                                IncidentRoleName = r != null ? r.Name : "",
                                FirstName = u.FirstName ?? "",
                                LastName = u.LastName ?? "",
                                u.Telephone,
                                u.Email,
                                u.PinHash,
                                u.EmployeeType
                            }
                        ).FirstOrDefaultAsync();

                if (user == null)
                    return new UserModifyViewModel();

                return new UserModifyViewModel
                {
                    Id = user.Id,
                    TeamId = user.TeamId,
                    CompanyId = user.CompanyId,
                    IncidentRoleId = user.IncidentRoleId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Telephone = user.Telephone,
                    Email = user.Email,
                    PinHash = user.PinHash,
                    VerifyPIN = user.PinHash,
                    EmployeeType = user.EmployeeType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetUserById.");
                return new UserModifyViewModel();
            }
        }

        public async Task<long> DeleteUser(long id)
        {
            try
            {
                var user = await (
                            from u in _db.IncidentUsers
                            where !u.IsDeleted && u.Id == id
                            join c in _db.Company on u.CompanyId equals c.Id into companyGroup
                            from c in companyGroup.DefaultIfEmpty()
                            join r in _db.IncidentRoles on u.IncidentRoleId equals r.Id into roleGroup
                            from r in roleGroup.DefaultIfEmpty()
                            select new IncidentUser
                            {
                                Id = u.Id,
                                TeamId = (long?)u.TeamId ?? 0,
                                CompanyId = (long?)u.CompanyId ?? 0,
                                IncidentRoleId = (long?)u.IncidentRoleId ?? 0,
                                FirstName = u.FirstName ?? "",
                                LastName = u.LastName ?? "",
                                Telephone = u.Telephone,
                                Email = u.Email,
                                PinHash = u.PinHash,
                                EmployeeType = u.EmployeeType
                            }
                        ).FirstOrDefaultAsync();

                if (user == null)
                    return 0;

                await using var transaction = await _db.Database.BeginTransactionAsync();


                _db.IncidentUsers.Attach(user);
                user.IsDeleted = true;
                user.UpdatedOn = DateTime.UtcNow;

                _db.Entry(user).Property(x => x.IsDeleted).IsModified = true;
                _db.Entry(user).Property(x => x.UpdatedOn).IsModified = true;


                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                return user.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteIncidentTeam.");
                return 0;
            }
        }
    }
}
