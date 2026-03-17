using FlexFit.Application.Commands;
using FlexFit.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlexFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-daily-card")]
        public async Task<IActionResult> CreateDailyCard([FromBody] CreateDailyCardDto dto)
        {
            var result = await _mediator.Send(new CreateDailyCardCommand(dto));
            if (!result) return BadRequest(new { message = "Card number already exists or invalid data." });
            return Ok(new { message = "Daily card created successfully." });
        }

        [HttpPost("create-subscription-card")]
        public async Task<IActionResult> CreateSubscriptionCard([FromBody] CreateSubscriptionCardDto dto)
        {
            var result = await _mediator.Send(new CreateSubscriptionCardCommand(dto));
            if (!result) return BadRequest(new { message = "Card number already exists or invalid data." });
            return Ok(new { message = "Subscription card created successfully." });
        }

        [HttpPost("create-fitness-object")]
        public async Task<IActionResult> CreateFitnessObject([FromBody] CreateFitnessObjectDto dto)
        {
            var result = await _mediator.Send(new CreateFitnessObjectCommand(dto));
            return Ok(new { message = "Fitness object created successfully." });
        }

        [HttpPut("update-fitness-object")]
        public async Task<IActionResult> UpdateFitnessObject([FromBody] UpdateFitnessObjectDto dto)
        {
            var result = await _mediator.Send(new UpdateFitnessObjectCommand(dto));
            if (!result) return NotFound(new { message = "Fitness object not found." });
            return Ok(new { message = "Fitness object updated successfully." });
        }

        [HttpDelete("delete-fitness-object/{id}")]
        public async Task<IActionResult> DeleteFitnessObject(int id)
        {
            var result = await _mediator.Send(new DeleteFitnessObjectCommand(id));
            if (!result) return NotFound(new { message = "Fitness object not found." });
            return Ok(new { message = "Fitness object deleted successfully." });
        }

        [HttpDelete("delete-membership-card/{cardNumber}")]
        public async Task<IActionResult> DeleteMembershipCard(string cardNumber)
        {
            var result = await _mediator.Send(new DeleteMembershipCardCommand(cardNumber));
            if (!result) return NotFound(new { message = "Membership card not found." });
            return Ok(new { message = "Membership card deleted successfully." });
        }
    }
}
