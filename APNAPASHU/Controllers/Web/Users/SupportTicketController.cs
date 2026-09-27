using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Users.SupportTicket;
using APNAPASHU.ServiceContract.Web.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace APNAPASHU.API.Controllers.Web.Users
{
    [Route("api/web/[controller]")]
    [ApiController]
    [Authorize]
    public class SupportTicketController : BaseController
    {
        private readonly ISupportTicketService _ticketService;

        public SupportTicketController(
            ISupportTicketService ticketService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _ticketService = ticketService;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(JsonModel<int>), 200)]
        public async Task<IActionResult> CreateTicket([FromForm] CreateSupportTicketDto model)
        {
            int userId = GetAuthenticatedUserId();
            if (userId <= 0)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new JsonModel<int>(0, "Unauthorized user.", (int)HttpStatusCode.Unauthorized));
            }

            var result = await _ticketService.CreateTicketAsync(userId, model);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpGet("my-tickets")]
        [ProducesResponseType(typeof(JsonModel<List<SupportTicketResponseDto>>), 200)]
        public async Task<IActionResult> GetMyTickets([FromQuery] SupportTicketFilterDto filter)
        {
            int userId = GetAuthenticatedUserId();
            if (userId <= 0)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new JsonModel<List<SupportTicketResponseDto>>(new List<SupportTicketResponseDto>(), "Unauthorized user.", (int)HttpStatusCode.Unauthorized));
            }

            var result = await _ticketService.GetUserTicketsAsync(userId, filter);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonModel<SupportTicketResponseDto>), 200)]
        public async Task<IActionResult> GetTicketById(int id)
        {
            int userId = GetAuthenticatedUserId();
            if (userId <= 0)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new JsonModel<SupportTicketResponseDto>(null, "Unauthorized user.", (int)HttpStatusCode.Unauthorized));
            }

            var result = await _ticketService.GetTicketByIdAsync(id);
            if (result.Data != null && result.Data.UserId != userId)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new JsonModel<SupportTicketResponseDto>(null, "Access denied.", (int)HttpStatusCode.Forbidden));
            }

            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpPost("add-message")]
        [ProducesResponseType(typeof(JsonModel<int>), 200)]
        public async Task<IActionResult> AddMessage([FromForm] AddTicketMessageDto model)
        {
            int userId = GetAuthenticatedUserId();
            if (userId <= 0)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new JsonModel<int>(0, "Unauthorized user.", (int)HttpStatusCode.Unauthorized));
            }

            var result = await _ticketService.AddMessageAsync(userId, "User", model);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpPost("close/{id}")]
        [ProducesResponseType(typeof(JsonModel<bool>), 200)]
        public async Task<IActionResult> CloseTicket(int id)
        {
            int userId = GetAuthenticatedUserId();
            if (userId <= 0)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new JsonModel<bool>(false, "Unauthorized user.", (int)HttpStatusCode.Unauthorized));
            }

            var ticketRes = await _ticketService.GetTicketByIdAsync(id);
            if (ticketRes.Data == null || ticketRes.Data.UserId != userId)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new JsonModel<bool>(false, "Access denied.", (int)HttpStatusCode.Forbidden));
            }

            var result = await _ticketService.UpdateTicketStatusAsync(userId, new UpdateTicketStatusDto { TicketId = id, Status = "Closed" });
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }
    }
}
