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
    public class StatusLegendService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : BaseService<StatusLegend, CreateViewModel, UpdateViewModel, DetailViewModel>,
          IStatusLegendService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<StatusLegendService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;

        public StatusLegendService(
            ApplicationDbContext db,
            ILogger<StatusLegendService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext)
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        public async Task<List<StatusLegendModifyViewModel>> GetAllStatusLegends()
        {
            var statusLegend = new List<StatusLegendModifyViewModel>();
            try
            {
                var statusLegendList = await _db.StatusLegends.Where(p => !p.IsDeleted).ToListAsync();

                foreach (var statu in statusLegendList)
                {
                    statusLegend.Add(new StatusLegendModifyViewModel()
                    {
                        Id = statu.Id,
                        Name = statu.Name,
                        Color = statu.Color,
                        Description = statu.Description    // <- added
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllStatusLegend.");
                return new List<StatusLegendModifyViewModel>();
            }
            return statusLegend;
        }

        public async Task<long> SaveStatusLegend(StatusLegendModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Map ViewModel → Entity
                var statusLegend = new StatusLegend
                {
                    Name = viewModel.Name,
                    Color = viewModel.Color,
                    Description = viewModel.Description ?? string.Empty    // <- added
                };

                // Save
                await _db.StatusLegends.AddAsync(statusLegend);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return statusLegend.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveStatusLegend.");
                return 0;
            }
        }

        public async Task<long> UpdateStatusLegend(StatusLegendModifyViewModel viewModel)
        {
            try
            {
                var statusLegend = await _db.StatusLegends.Where(p => p.Id == viewModel.Id).FirstOrDefaultAsync();

                if (statusLegend == null)
                {
                    return await SaveStatusLegend(viewModel);
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                statusLegend.Name = viewModel.Name;
                statusLegend.Color = viewModel.Color;
                statusLegend.Description = viewModel.Description ?? string.Empty; // <- added

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

                return statusLegend.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateStatusLegend.");
                return 0;
            }
        }

        public async Task<StatusLegendModifyViewModel> GetStatusLegendById(long Id)
        {
            var statusLegendView = new StatusLegendModifyViewModel();

            try
            {
                var statusLegend = await _db.StatusLegends.FirstOrDefaultAsync(p => p.Id == Id);

                if (statusLegend == null)
                {
                    return new StatusLegendModifyViewModel();
                }

                statusLegendView.Name = statusLegend.Name;
                statusLegendView.Color = statusLegend.Color;
                statusLegendView.Description = statusLegend.Description; // <- added
                statusLegendView.Id = statusLegend.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetStatusLegendById.");
                return new StatusLegendModifyViewModel();
            }

            return statusLegendView;
        }

        public async Task<long> DeleteStatusLegend(long Id)
        {
            try
            {
                var statusLegend = await _db.StatusLegends.Where(p => p.Id == Id).FirstOrDefaultAsync();

                if (statusLegend == null)
                {
                    return 0;
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                statusLegend.IsDeleted = true;

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

                return statusLegend.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteStatusLegend.");
                return 0;
            }
        }
    }
}
