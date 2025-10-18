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
    public class SeverityLevelService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : BaseService<SeverityLevel, CreateViewModel, UpdateViewModel, DetailViewModel>,
          ISeverityLevelService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<SeverityLevelService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;

        public SeverityLevelService(
            ApplicationDbContext db,
            ILogger<SeverityLevelService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext
        ) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        //public override async Task<Expression<Func<SeverityLevel, bool>>> SetQueryFilter(IBaseSearchModel filters)
        //{
        //    var searchFilters = filters as SeverityLevelSearchViewModel;

        //    return x =>
        //                (
        //                    string.IsNullOrEmpty(searchFilters.Search.value) ||
        //                    x.Name.ToLower().Contains(searchFilters.Search.value.ToLower()) ||
        //                    x.Description.ToLower().Contains(searchFilters.Search.value.ToLower()) ||
        //                    x.Color.ToLower().Contains(searchFilters.Search.value.ToLower())
        //                )
        //                &&
        //                (string.IsNullOrEmpty(searchFilters.Name) || x.Name.ToLower().Contains(searchFilters.Name.ToLower()))
        //                &&
        //                (string.IsNullOrEmpty(searchFilters.Description) || x.Description.ToLower().Contains(searchFilters.Description.ToLower()))
        //                &&
        //                (string.IsNullOrEmpty(searchFilters.Color) || x.Color.ToLower().Contains(searchFilters.Color.ToLower()));
        //}


        public async Task<List<SeverityLevelModifyViewModel>> GetAllSeverityLevels()
        {
            List<SeverityLevelModifyViewModel> SeverityLevel = new();
            try
            {
                var SeverityLevelList = await _db.SeverityLevels.Where(p => !p.IsDeleted).ToListAsync();

                foreach (var severityLevel in SeverityLevelList)
                {
                    SeverityLevel.Add(new SeverityLevelModifyViewModel()
                    {
                        Id = severityLevel.Id,
                        Name = severityLevel.Name,
                        Color = severityLevel.Color,
                        Description = severityLevel.Description
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllSeverityLevel.");
                return new List<SeverityLevelModifyViewModel>()!;
            }
            return SeverityLevel;
        }
        public async Task<long> SaveSeverityLevel(SeverityLevelModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Map ViewModel → Entity
                var SeverityLevel = new SeverityLevel
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description ?? string.Empty,
                    Color = viewModel.Color ?? string.Empty
                };

                // Save
                await _db.SeverityLevels.AddAsync(SeverityLevel);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return SeverityLevel.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveSeverityLevel.");
                return 0;
            }
        }
        public async Task<long> UpdateSeverityLevel(SeverityLevelModifyViewModel viewModel)
        {
            try
            {

                var SeverityLevel = await _db.SeverityLevels.Where(p => p.Id == viewModel.Id).FirstOrDefaultAsync();

                if (SeverityLevel == null)
                {
                    await SaveSeverityLevel(viewModel);
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                SeverityLevel.Name = viewModel.Name;
                SeverityLevel.Description = viewModel.Description ?? string.Empty;
                SeverityLevel.Color = viewModel.Color ?? string.Empty;
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

                return SeverityLevel.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateSeverityLevel.");
                return 0;
            }
        }
        public async Task<SeverityLevelModifyViewModel> GetSeverityLevelById(long Id)
        {
            var SeverityLevelView = new SeverityLevelModifyViewModel();

            try
            {
                var SeverityLevel = await _db.SeverityLevels.FirstOrDefaultAsync(p => p.Id == Id);

                if (SeverityLevel == null)
                {
                    return new SeverityLevelModifyViewModel();
                }

                SeverityLevelView.Name = SeverityLevel.Name;
                SeverityLevelView.Description = SeverityLevel.Description;
                SeverityLevelView.Color = SeverityLevel.Color;
                SeverityLevelView.Id = SeverityLevel.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetSeverityLevelById.");
                return new SeverityLevelModifyViewModel();
            }

            return SeverityLevelView;
        }
        public async Task<long> DeleteSeverityLevel(long Id)
        {
            try
            {

                var SeverityLevel = await _db.SeverityLevels.Where(p => p.Id == Id).FirstOrDefaultAsync();

                if (SeverityLevel == null)
                {
                    return 0;
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                SeverityLevel.IsDeleted = true;

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

                return SeverityLevel.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteSeverityLevel.");
                return 0;
            }
        }
    }
}
