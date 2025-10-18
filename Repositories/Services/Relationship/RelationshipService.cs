using AutoMapper;

using Centangle.Common.ResponseHelpers.Models;

using DataLibrary;

using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Vml.Office;

using Enums;

using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models;
using Models.Common.Interfaces;

using Pagination;

using System.Linq.Expressions;

using ViewModels;
using ViewModels.Incident;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class RelationshipService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : BaseService<Relationship, CreateViewModel, UpdateViewModel, DetailViewModel>,
          IRelationshipService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RelationshipService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        public RelationshipService(
            ApplicationDbContext db,
            ILogger<RelationshipService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext)
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
        }

        //public override async Task<Expression<Func<Relationship, bool>>> SetQueryFilter(IBaseSearchModel filters)
        //{
        //    var searchFilters = filters as RelationshipSearchViewModel;

        //    return x =>
        //                (
        //                    (
        //                        string.IsNullOrEmpty(searchFilters.Search.value)
        //                        ||
        //                        x.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
        //                    )
        //                )
        //                &&
        //                (string.IsNullOrEmpty(searchFilters.Name) || x.Name.ToLower().Contains(searchFilters.Name.ToLower()))
        //                ;
        //}

        public async Task<List<RelationshipModifyViewModel>> GetAllRelationships()
        {
            List<RelationshipModifyViewModel> relationships = new();
            try
            {

                var reslations = await _db.Relationships.Where(p => !p.IsDeleted).ToListAsync();

                foreach (var relation in reslations)
                {
                    relationships.Add(new RelationshipModifyViewModel()
                    {
                        Id = relation.Id,
                        Name = relation.Name
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllRelationships.");
                return new List<RelationshipModifyViewModel>()!;
            }
            return relationships;
        }
        public async Task<long> SaveRelation(RelationshipModifyViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Map ViewModel → Entity
                var relationship = new Relationship
                {
                    Name = viewModel.Name
                };

                // Save
                await _db.Relationships.AddAsync(relationship);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return relationship.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveRelation.");
                return 0;
            }
        }
        public async Task<long> UpdateRelation(RelationshipModifyViewModel viewModel)
        {
            try
            {

                var relation = await _db.Relationships.Where(p => p.Id == viewModel.Id).FirstOrDefaultAsync();

                if (relation == null)
                {
                    await SaveRelation(viewModel);
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                relation.Name = viewModel.Name;

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

                return relation.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error SaveRelation.");
                return 0;
            }
        }
        public async Task<RelationshipModifyViewModel> GetRelationById(long Id)
        {
            var relationshipView = new RelationshipModifyViewModel();

            try
            {
                var relationship = await _db.Relationships.FirstOrDefaultAsync(p => p.Id == Id);

                if (relationship == null)
                {
                    return new RelationshipModifyViewModel();
                }

                relationshipView.Name = relationship.Name;
                relationshipView.Id = relationship.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetById.");
                return new RelationshipModifyViewModel();
            }

            return relationshipView;
        }
        public async Task<long> DeleteRelation(long Id)
        {
            try
            {

                var relation = await _db.Relationships.Where(p => p.Id == Id).FirstOrDefaultAsync();

                if (relation == null)
                {
                    return 0;
                }

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();

                relation.IsDeleted = true;

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

                return relation.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteRelation.");
                return 0;
            }
        }
    }
}
