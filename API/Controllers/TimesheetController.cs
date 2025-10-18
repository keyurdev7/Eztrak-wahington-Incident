using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repositories;
using System.Security.Claims;
using ViewModels.Timesheet;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimesheetController : ApiBaseController
    {
        private readonly ITimesheetService _timesheetService;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        private readonly ITimesheetLimit _timesheetLimit;
        private readonly ILogger<TimesheetController> _logger;


        public TimesheetController(
            ILogger<TimesheetController> logger,
            ITimesheetService timesheetService,
            IMapper mapper,
            IRepositoryResponse response,
            ITimesheetLimit timesheetLimit)
        {
            _logger = logger;
            _timesheetService = timesheetService;
            _mapper = mapper;
            _response = response;
            _timesheetLimit = timesheetLimit;
        }


        [HttpPut]
        public async Task<IActionResult> Put([FromBody] TimesheetBreakdownUpdateViewModel model)
        {
            IRepositoryResponse result;
            if (ModelState.IsValid)
            {
                _logger.LogInformation("TimesheetAPI Update modelstate is valid");
                var timeSheetId = await _timesheetService.ModifyApiTimeSheetBreakdowns(model);
                if (timeSheetId > 0)
                {
                    var responseModel = new RepositoryResponseWithModel<long>();
                    responseModel.ReturnModel = timeSheetId;
                    return ReturnProcessedResponse<Timesheet>(responseModel);
                }
            }
            _logger.LogError("TimesheetAPI Update modelstate is not valid");
            result = Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response);
            return ReturnProcessedResponse(result);
        }

        [HttpGet]
        [Route("/api/Timesheet/GetWorkOrders")]
        public async Task<IActionResult> GetWorkOrders()
        {
            long technicianId = long.Parse(User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(x => x.Value).FirstOrDefault() ?? "0");
            var result = await _timesheetService.GetWorkOrdersByTechnicianId(technicianId);
            if (result != null)
            {
                var responseModel = new RepositoryResponseWithModel<TimesheetProjectsViewModel>();
                responseModel.ReturnModel = result;
                return ReturnProcessedResponse<TimesheetProjectsViewModel>(responseModel);
            }
            _logger.LogError("Timesheet create modelstate is not valid");
            return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
        }

        [HttpGet]
        [Route("/api/Timesheet/GetTimesheet")]
        public async Task<IActionResult> GetTimesheet([FromQuery] TimesheetEmployeeSearchViewModel model)
        {
            var result = await _timesheetService.GetTimesheet(model);
            if (result != null)
            {
                var responseModel = new RepositoryResponseWithModel<TimesheetBriefViewModel>();
                responseModel.ReturnModel = result;
                return ReturnProcessedResponse<TimesheetBriefViewModel>(responseModel);

            }
            _logger.LogError("Timesheet create modelstate is not valid");
            return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
        }

        [HttpGet]
        [Route("/api/Timesheet/GetHoursBreakdown")]
        public async Task<IActionResult> GetHoursBreakdown(DayOfWeek day, float hours, float stRate, float otRate, float dtRate)
        {
            var breakdown = _timesheetLimit.GetHoursBreakdown(day, hours);
            if (breakdown != null)
            {
                var responseModel = new RepositoryResponseWithModel<TimesheetHoursBreakdownViewModel>();
                breakdown.STRate = stRate;
                breakdown.OTRate = otRate;
                breakdown.DTRate = dtRate;
                responseModel.ReturnModel = breakdown;
                return ReturnProcessedResponse<TimesheetHoursBreakdownViewModel>(responseModel);

            }
            _logger.LogError("GetHoursBreakdown modelstate is not valid");
            return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
        }

    }
}
