using FlexFit.Application.Commands;
using FlexFit.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlexFit.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Employee,Admin")]
    public class GuardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GuardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("sell-daily-ticket")]
        public async Task<IActionResult> SellDailyTicket([FromBody] SellDailyTicketDto dto)
        {
            var result = await _mediator.Send(new SellDailyTicketCommand(dto));

            if (!result)
            {
                return BadRequest(new { message = "Invalid or already active daily ticket." });
            }

            return Ok(new { message = "Daily ticket sold successfully." });
        }

        [HttpPost("create-daily-card")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> CreateDailyCard([FromBody] CreateDailyCardDto dto)
        {
            var result = await _mediator.Send(new CreateDailyCardCommand(dto));
            if (!result) return BadRequest(new { message = "Card number already exists or invalid data." });
            return Ok(new { message = "Daily card created successfully." });
        }

        [HttpPost("create-subscription-card")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> CreateSubscriptionCard([FromBody] CreateSubscriptionCardDto dto)
        {
            var result = await _mediator.Send(new CreateSubscriptionCardCommand(dto));
            if (!result) return BadRequest(new { message = "Card number already exists or invalid data." });
            return Ok(new { message = "Subscription card created successfully." });
        }
        [HttpPost("extend-membership")]
        public async Task<IActionResult> ExtendMembership([FromBody] string cardNumber)
        {
            var result = await _mediator.Send(new ExtendMembershipCommand(cardNumber));

            if (!result)
            {
                return BadRequest(new { message = "Invalid or non-subscription membership card." });
            }

            return Ok(new { message = "Membership extended successfully." });
        }

        [HttpPost("log-entry")]
        public async Task<IActionResult> LogEntry([FromBody] LogEntryDto dto)
        {
            var result = await _mediator.Send(new LogEntryCommand(dto));
            if (!result) return BadRequest(new { message = "GreÅ¡ka pri logovanju ulaza." });
            return Ok(new { message = "Ulaz uspeÅ¡no zabeleÅ¾en." });
        }
    }
}
