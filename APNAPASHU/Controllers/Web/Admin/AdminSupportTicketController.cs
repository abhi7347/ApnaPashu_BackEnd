using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.SupportTicket;
using APNAPASHU.ServiceContract.Web.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace APNAPASHU.API.Controllers.Web.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminSupportTicketController : BaseController
    {
        private readonly IAdminSupportTicketService _ticketService;

        public AdminSupportTicketController(
            IAdminSupportTicketService ticketService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _ticketService = ticketService;
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(JsonModel<List<AdminSupportTicketResponseDto>>), 200)]
        public async Task<IActionResult> GetAllTickets([FromQuery] AdminSupportTicketFilterDto filter)
        {
            var result = await _ticketService.GetAllTicketsAsync(filter);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonModel<AdminSupportTicketResponseDto>), 200)]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var result = await _ticketService.GetTicketByIdAsync(id);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpPost("reply")]
        [ProducesResponseType(typeof(JsonModel<int>), 200)]
        public async Task<IActionResult> ReplyToTicket([FromForm] AdminReplyTicketDto model)
        {
            int adminUserId = GetAuthenticatedUserId();
            var result = await _ticketService.AddReplyAsync(adminUserId, model);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpPost("update-status")]
        [ProducesResponseType(typeof(JsonModel<bool>), 200)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateTicketStatusDto model)
        {
            int adminUserId = GetAuthenticatedUserId();
            var result = await _ticketService.UpdateTicketStatusAsync(adminUserId, model);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }
    }
}

