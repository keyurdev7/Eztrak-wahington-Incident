using Models.Common.Interfaces;
using ViewModels.Shared;
using Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Pagination;
using System.Linq.Expressions;
using ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using DocumentFormat.OpenXml.Office2010.Excel;
using Repositories.Services.AttachmentService;
using Repositories.Services.AttachmentService.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Centangle.Common.ResponseHelpers;
using Enums;
using DocumentFormat.OpenXml.Spreadsheet;
using Helpers.Extensions;
using ViewModels.Shared.Notes;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.ExtendedProperties;
using ViewModels.Shared.Notification;
using Repositories.Shared.NotificationServices.Interface;
using Microsoft.AspNetCore.Hosting;
using ViewModels.Ticket;

namespace Repositories.Common
{
    public class TicketService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<Ticket, CreateViewModel, UpdateViewModel, DetailViewModel>, ITicketService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly IRepositoryResponse _response;
        private readonly IAttachment _attachmentService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TicketService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;

        public TicketService
            (
                ApplicationDbContext db,
                ILogger<TicketService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
                IMapper mapper,
                IRepositoryResponse response,
                IActionContextAccessor actionContext,
                IAttachment attachmentService,
                IHostingEnvironment hostingEnvironment,
                INotificationService notificationService
            ) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _attachmentService = attachmentService;
            _hostingEnvironment = hostingEnvironment;
            _notificationService = notificationService;
            _logger = logger;
            _mapper = mapper;
            _response = response;
        }

        public async override Task<IRepositoryResponse> Create(CreateViewModel model)
        {

            var viewModel = model as TicketModifyViewModel;
            int totalTickets = await _db.Set<Ticket>().IgnoreQueryFilters().CountAsync();
            viewModel.TicketNo = (totalTickets + 1).ToString("D4");
            var response = await base.Create(model);
            long id = 0;
            if (response.Status == System.Net.HttpStatusCode.OK)
            {
                var parsedResponse = response as RepositoryResponseWithModel<long>;
                id = parsedResponse?.ReturnModel ?? 0;
                if (viewModel.Images.Count > 0)
                {
                    var attachments = new List<AttachmentVM>();
                    foreach (var image in viewModel.Images)
                    {
                        attachments.Add(new AttachmentVM() { File = image });
                    }
                    attachments.ForEach(x => x.EntityId = id);
                    attachments.ForEach(x => x.EntityType = Enums.AttachmentEntityType.Tickets);
                    await _attachmentService.CreateMultiple(attachments);
                }
            }
            return response;

        }
        public async Task<bool> UpdateTicket(TicketStatusUpdateViewModel model)
        {
            try
            {
                var dbTicket = await _db.Tickets.FindAsync(model.Id);
                dbTicket.Status = model.Status;
                dbTicket.ITResponse = model.ITResponse;
                var ticketHistory = _mapper.Map<TicketHistory>(dbTicket);
                //ticketHistory.ShopperId = dbTicket.CreatedBy;
                //ticketHistory.TicketId = dbTicket.Id;
                ticketHistory.SubmittedOn = DateTime.Now;
                await _db.AddAsync(ticketHistory);
                await _db.SaveChangesAsync();
                var shopper = await _db.Users.Where(x => x.Id == dbTicket.CreatedBy).FirstOrDefaultAsync();
                if (model.ITResponse != null)
                {
                    await SendNotificationToShopper(shopper.Email, shopper.FirstName + " " + shopper.LastName, dbTicket.TicketNo, model.ITResponse, dbTicket.Id);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> SendNotificationToShopper(string shopperEmail, string shopperName, string ticketNo, string iTResponse, long ticketId)
        {
            try
            {
                if (shopperEmail != null)
                {
                    string subject = "Ticket Response Submitted";
                    string bodyForMail = GetBodyForMail(shopperName, ticketNo, iTResponse);

                    var mailRequest = new MailRequestViewModel(shopperEmail, subject, bodyForMail, ticketId, NotificationEntityType.TicketResponseSubmitted, NotificationType.Email);

                    await _notificationService.MappNotification(mailRequest);
                    _logger.LogInformation("Notification saved Successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
            return true;
        }

        private string GetBodyForMail(string shopperName, string ticketNo, string iTResponse)
        {
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Templates", "AdminReplySupportTicket.html");
            if (File.Exists(filePath))
            {
                var str = File.ReadAllText(filePath);
                return str.Replace("<%NAME%>", shopperName)
                    .Replace("<%TicketNumber%>", ticketNo)
                     .Replace("<%Response%>", iTResponse);
            }
            else
            {
                return string.Empty;
            }
        }


        public async override Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            try
            {

                var searchFilters = search as TicketSearchViewModel;
                var ticketsQueryable = (from t in _db.Tickets
                                        join cu in _db.Users on t.CreatedBy equals cu.Id
                                        join uu in _db.Users on t.UpdatedBy equals uu.Id
                                        where
                                        (
                                            (
                                                string.IsNullOrEmpty(searchFilters.Search.value)
                                                ||
                                                t.Description.ToLower().Contains(searchFilters.Search.value.ToLower())
                                                ||
                                                t.TicketNo.ToLower().Contains(searchFilters.Search.value.ToLower())
                                            )
                                            &&
                                            (searchFilters.Status == null || t.Status == searchFilters.Status)
                                            &&
                                            (string.IsNullOrEmpty(searchFilters.TicketNo) || t.TicketNo == searchFilters.TicketNo)
                                        )
                                        select new TicketDetailViewModel
                                        {
                                            Id = t.Id,
                                            Description = t.Description,
                                            ITResponse = t.ITResponse,
                                            Category = t.Category,
                                            CreatedOn = t.CreatedOn,
                                            CreatedBy = cu.FirstName + " " + cu.LastName,
                                            UpdatedBy = uu.FirstName + " " + uu.LastName,
                                            Status = t.Status,
                                            TicketNo = t.TicketNo
                                        }).AsQueryable();

                var result = await ticketsQueryable.Paginate(search);
                if (result != null)
                {
                    var paginatedResult = new PaginatedResultModel<M>();
                    paginatedResult.Items = _mapper.Map<List<M>>(result.Items);
                    paginatedResult._meta = result._meta;
                    paginatedResult._links = result._links;
                    var response = new RepositoryResponseWithModel<PaginatedResultModel<M>> { ReturnModel = paginatedResult };
                    return response;
                }
                _logger.LogWarning($"No record found for {typeof(WorkOrder).FullName} in GetAll()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAll() method for {typeof(WorkOrder).FullName} threw an exception.");
                return Response.BadRequestResponse(_response);
            }
        }

        public async override Task<IRepositoryResponse> GetById(long id)
        {
            try
            {
                var dbModel = await _db.Tickets
                                       .Where(x => x.Id == id)
                                       .Select(x => new TicketDetailViewModel
                                       {
                                           Id = x.Id,
                                           TicketNo = x.TicketNo,
                                           Description = x.Description,
                                           ITResponse = x.ITResponse,
                                           Category = x.Category, // Assuming this is an enum property directly on the ticket
                                           Status = x.Status,
                                       })
                                       .FirstOrDefaultAsync();


                if (dbModel != null)
                {
                    dbModel.ImagesList = await GetTicketAttachments(dbModel.Id);
                    var response = new RepositoryResponseWithModel<TicketDetailViewModel> { ReturnModel = dbModel };
                    return response;
                }
                _logger.LogWarning($"No record found for id:{id} for {typeof(Ticket).FullName} in GetById()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(Ticket).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }


        public override async Task<Expression<Func<Ticket, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as TicketSearchViewModel;

            return x =>
                        (
                            (
                                string.IsNullOrEmpty(searchFilters.Search.value)
                                ||
                                x.Description.ToLower().Contains(searchFilters.Search.value.ToLower())
                            )
                        )
                        &&
                        (string.IsNullOrEmpty(searchFilters.Description) || x.Description.ToLower().Contains(searchFilters.Description.ToLower()))
                         &&
                        (searchFilters.Status != null || x.Status == searchFilters.Status)
                        ;
        }

        public async Task<List<AttachmentVM>> GetTicketAttachments(long id)
        {
            var attachments = await _db.Attachments.Where(x => x.EntityId == id && x.EntityType == Enums.AttachmentEntityType.Tickets).Select(x => new AttachmentVM
            {
                Id = x.Id,
                EntityId = (long)x.EntityId,
                EntityType = (Enums.AttachmentEntityType)x.EntityType,
                Url = x.Url,
                Name = x.Name,
                Type = x.Type,
                CreatedOn = x.CreatedOn
            }).ToListAsync();

            return attachments;
        }
        public async Task<string> GetAttachmentUrl(long id)
        {
            try
            {
                var attachmentUrl = await _db.Attachments.Where(x => x.EntityId == id).Select(x => x.Url).FirstOrDefaultAsync();
                return attachmentUrl;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<TicketHistoryDetailViewModel>> GetTicketHistory(int id)
        {
            try
            {
                var ticketHistories = await (from t in _db.TicketHistories.Include(x => x.Ticket)
                                             join cu in _db.Users on t.CreatedBy equals cu.Id
                                             join uu in _db.Users on t.UpdatedBy equals uu.Id
                                             where (t.TicketId == id)
                                             select new TicketHistoryDetailViewModel
                                             {
                                                 TicketId = t.TicketId,
                                                 Id = t.Id,
                                                 TicketNo = t.TicketNo,
                                                 ITResponse = t.ITResponse,
                                                 Category = t.Category,
                                                 CreatedOn = t.CreatedOn,
                                                 CreatedBy = cu.FirstName + " " + cu.LastName,
                                                 SubmittedOn = (DateTime)t.SubmittedOn,
                                                 Description = t.Description,
                                                 Status = t.Status,
                                                 UpdatedBy = uu.FirstName + " " + uu.LastName,
                                               }).ToListAsync();
                return ticketHistories;
            }
            catch (Exception ex)
            {
                return new List<TicketHistoryDetailViewModel>();
            }
        }
    }
}

