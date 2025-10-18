using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
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
    public class ProgressService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : BaseService<Progress, CreateViewModel, UpdateViewModel, DetailViewModel>,
          IProgressService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ProgressService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        public ProgressService(
            ApplicationDbContext db,
            ILogger<ProgressService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext)
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        public async Task<List<ProgressModifyViewModel>> GetAllProgress()
        {
            List<ProgressModifyViewModel> progressre = new();
            try
            {

                var progress = await _db.Progress.Where(p => !p.IsDeleted).ToListAsync();

                foreach (var Progress in progress)
                {
                    progressre.Add(new ProgressModifyViewModel()
                    {
                        Id = Progress.Id,
                        Name = Progress.Name
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get All Status Master.");
                return new List<ProgressModifyViewModel>()!;
            }
            return progressre;
        }
        public async Task<long> SaveProgress(ProgressModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Map ViewModel → Entity
                var progress = new Progress
                {
                    Name = viewModel.Name
                };

                // Save
                await _db.Progress.AddAsync(progress);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return progress.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error Save Status Master.");
                return 0;
            }
        }
        public async Task<long> UpdateProgress(ProgressModifyViewModel viewModel)
        {
            try
            {

                var progress = await _db.Progress.Where(p => p.Id == viewModel.Id).FirstOrDefaultAsync();

                if (progress == null)
                {
                    await SaveProgress(viewModel);
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                progress.Name = viewModel.Name;

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

                return progress.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Save Status Master.");
                return 0;
            }
        }
        public async Task<ProgressModifyViewModel> GetProgressById(long Id)
        {
            var progressView = new ProgressModifyViewModel();

            try
            {
                var progress = await _db.Progress.FirstOrDefaultAsync(p => p.Id == Id);

                if (progress == null)
                {
                    return new ProgressModifyViewModel();
                }

                progressView.Name = progress.Name;
                progressView.Id = progress.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetById.");
                return new ProgressModifyViewModel();
            }

            return progressView;
        }
        public async Task<long> DeleteProgress(long Id)
        {
            try
            {

                var progress = await _db.Progress.Where(p => p.Id == Id).FirstOrDefaultAsync();

                if (progress == null)
                {
                    return 0;
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                progress.IsDeleted = true;

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

                return progress.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Delete Status Master.");
                return 0;
            }
        }
    }
}
