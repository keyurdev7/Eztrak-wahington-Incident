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
    }
}
