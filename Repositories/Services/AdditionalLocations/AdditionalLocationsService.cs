using AutoMapper;

using Azure;

using Centangle.Common.ResponseHelpers;
using Centangle.Common.ResponseHelpers.Models;

using DataLibrary;

using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;

using Enums;

using Helpers.Extensions;
using Helpers.File;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models;
using Models.Common.Interfaces;

using Pagination;

using Repositories.Shared.UserInfoServices.Interface;

using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;

using ViewModels;
using ViewModels.Dashboard;
using ViewModels.Incident;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class AdditionalLocationsService : IAdditionalLocationsService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AdditionalLocationsService> _logger;

        public AdditionalLocationsService(ApplicationDbContext db, ILogger<AdditionalLocationsService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<string> SaveadditionalLocations(List<AdditionalLocationViewModel> additionalLocations)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var locationEntities = additionalLocations.Select(l => new AdditionalLocations
                {
                    LocationAddress = l.LocationAddress,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude,
                    IncidentID = l.IncidentId,
                    NearestIntersection = l.NearestIntersection,
                    ServiceAccount = l.ServiceAccount,
                    PerimeterType = l.PerimeterType,
                    PerimeterTypeDigit = l.PerimeterTypeDigit,
                    AssetIds = l.AssetIDs,
                    ActiveStatus = Enums.ActiveStatus.Active,
                    IsPrimaryLocation = l.IsPrimaryLocation,

                }).ToList();

                await _db.AdditionalLocations.AddRangeAsync(locationEntities);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return "";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveIncident.");
                return string.Empty;
            }
        }

        public async Task<List<AdditionalLocationViewModel>> GetVerificationLocationsByIncidentId(long incidentId)
        {
            try
            {
                var rows = await _db.AdditionalLocations
                    .Where(x => !x.IsDeleted
                                && x.IncidentID.HasValue
                                && x.IncidentID.Value == incidentId
                                && x.IsVerificationPoint)
                    .OrderByDescending(x => x.UpdatedOn)
                    .ThenByDescending(x => x.Id)
                    .ToListAsync();

                return rows.Select(x => new AdditionalLocationViewModel
                {
                    Id = x.Id,
                    IncidentId = x.IncidentID,
                    LocationAddress = x.LocationAddress ?? string.Empty,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    NearestIntersection = x.NearestIntersection,
                    ServiceAccount = x.ServiceAccount,
                    PerimeterType = x.PerimeterType,
                    PerimeterTypeDigit = x.PerimeterTypeDigit,
                    AssetIDs = x.AssetIds ?? string.Empty,
                    IsPrimaryLocation = x.IsPrimaryLocation,
                    IsVerificationPoint = x.IsVerificationPoint,
                    VerificationStatus = x.VerificationStatus,
                    VerificationNotes = x.VerificationNotes,
                    VerificationPhotoUrl = x.VerificationPhotoUrl,
                    VerifiedOn = x.VerifiedOn,
                    VerifiedByUserId = x.VerifiedByUserId,
                    VerifiedByUserName = x.VerifiedByUserName,
                    ImportBatchId = x.ImportBatchId
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetVerificationLocationsByIncidentId.");
                return new List<AdditionalLocationViewModel>();
            }
        }

        public async Task<AdditionalLocationViewModel?> GetVerificationLocationById(long id)
        {
            try
            {
                var x = await _db.AdditionalLocations
                    .Where(r => !r.IsDeleted && r.Id == id && r.IsVerificationPoint)
                    .FirstOrDefaultAsync();

                if (x == null) return null;

                return new AdditionalLocationViewModel
                {
                    Id = x.Id,
                    IncidentId = x.IncidentID,
                    LocationAddress = x.LocationAddress ?? string.Empty,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    NearestIntersection = x.NearestIntersection,
                    ServiceAccount = x.ServiceAccount,
                    PerimeterType = x.PerimeterType,
                    PerimeterTypeDigit = x.PerimeterTypeDigit,
                    AssetIDs = x.AssetIds ?? string.Empty,
                    IsPrimaryLocation = x.IsPrimaryLocation,
                    IsVerificationPoint = x.IsVerificationPoint,
                    VerificationStatus = x.VerificationStatus,
                    VerificationNotes = x.VerificationNotes,
                    VerificationPhotoUrl = x.VerificationPhotoUrl,
                    VerifiedOn = x.VerifiedOn,
                    VerifiedByUserId = x.VerifiedByUserId,
                    VerifiedByUserName = x.VerifiedByUserName,
                    ImportBatchId = x.ImportBatchId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetVerificationLocationById.");
                return null;
            }
        }

        public async Task<int> AddVerificationLocations(long incidentId, IEnumerable<AdditionalLocationViewModel> locations, Guid importBatchId)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;

                var entities = locations
                    .Where(l => !string.IsNullOrWhiteSpace(l.LocationAddress))
                    .Select(l => new AdditionalLocations
                    {
                        IncidentID = incidentId,
                        LocationAddress = l.LocationAddress,
                        Latitude = l.Latitude,
                        Longitude = l.Longitude,
                        NearestIntersection = l.NearestIntersection,
                        ServiceAccount = l.ServiceAccount,
                        AssetIds = l.AssetIDs,
                        PerimeterType = l.PerimeterType,
                        PerimeterTypeDigit = l.PerimeterTypeDigit,
                        IsPrimaryLocation = false,

                        IsVerificationPoint = true,
                        VerificationStatus = string.IsNullOrWhiteSpace(l.VerificationStatus) ? "Pending" : l.VerificationStatus,
                        ImportBatchId = importBatchId,

                        ActiveStatus = Enums.ActiveStatus.Active,
                        IsDeleted = false,
                        CreatedOn = now,
                        UpdatedOn = now
                    })
                    .ToList();

                if (entities.Count == 0) return 0;

                await _db.AdditionalLocations.AddRangeAsync(entities);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return entities.Count;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error AddVerificationLocations.");
                return 0;
            }
        }

        public async Task<bool> UpdateVerificationLocation(long id, string status, string? notes, string? serviceAccount, string? assetIds, string? photoUrl, long? userId, string? userName)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var row = await _db.AdditionalLocations
                    .Where(r => !r.IsDeleted && r.Id == id && r.IsVerificationPoint)
                    .FirstOrDefaultAsync();

                if (row == null) return false;

                row.VerificationStatus = status;
                row.VerificationNotes = notes;
                row.ServiceAccount = serviceAccount;
                row.AssetIds = assetIds;

                if (!string.IsNullOrWhiteSpace(photoUrl))
                {
                    if (string.IsNullOrWhiteSpace(row.VerificationPhotoUrl))
                        row.VerificationPhotoUrl = photoUrl;
                    else
                        row.VerificationPhotoUrl = row.VerificationPhotoUrl + "|" + photoUrl;
                }

                if (!string.IsNullOrWhiteSpace(status) && status.Trim().Equals("Verified", StringComparison.OrdinalIgnoreCase))
                {
                    row.VerifiedOn = DateTime.UtcNow;
                    row.VerifiedByUserId = userId;
                    row.VerifiedByUserName = userName;
                }

                row.UpdatedOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error UpdateVerificationLocation.");
                return false;
            }
        }
    }
}
