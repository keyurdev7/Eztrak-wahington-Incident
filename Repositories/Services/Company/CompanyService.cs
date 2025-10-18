using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Common.Interfaces;
using Pagination;
using System.Linq.Expressions;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class CompanyService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : BaseService<Company, CreateViewModel, UpdateViewModel, DetailViewModel>,
          ICompanyService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CompanyService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        public CompanyService(
            ApplicationDbContext db,
            ILogger<CompanyService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext
            )
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        public async Task<List<CompanyModifyViewModel>> GetAllCompanys()
        {
            List<CompanyModifyViewModel> companys = new();
            try
            {
                var companysList = await _db.Company.Where(p => !p.IsDeleted).ToListAsync();

                foreach (var company in companysList)
                {
                    companys.Add(new CompanyModifyViewModel()
                    {
                        Id = company.Id,
                        Name = company.Name,
                        Description = company.Description
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get All Companys.");
                return new List<CompanyModifyViewModel>()!;
            }
            return companys;
        }
        public async Task<long> SaveCompany(CompanyModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Map ViewModel → Entity
                var company = new Company
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description
                };

                // Save
                await _db.Company.AddAsync(company);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return company.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error Save Company.");
                return 0;
            }
        }
        public async Task<long> UpdateCompany(CompanyModifyViewModel viewModel)
        {
            try
            {

                var company = await _db.Company.Where(p => p.Id == viewModel.Id).FirstOrDefaultAsync();

                if (company == null)
                {
                    await SaveCompany(viewModel);
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                company.Name = viewModel.Name;
                company.Description = viewModel.Description;

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

                return company.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update Company.");
                return 0;
            }
        }
        public async Task<CompanyModifyViewModel> GetCompanyById(long Id)
        {
            var companyView = new CompanyModifyViewModel();

            try
            {
                var company = await _db.Company.FirstOrDefaultAsync(p => p.Id == Id);

                if (company == null)
                {
                    return new CompanyModifyViewModel();
                }

                companyView.Name = company.Name;
                companyView.Description = company.Description;
                companyView.Id = company.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get Company ById.");
                return new CompanyModifyViewModel();
            }

            return companyView;
        }
        public async Task<long> DeleteCompany(long Id)
        {
            try
            {

                var company = await _db.Company.Where(p => p.Id == Id).FirstOrDefaultAsync();

                if (company == null)
                {
                    return 0;
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                company.IsDeleted = true;

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

                return company.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Delete Company.");
                return 0;
            }
        }
    }
}