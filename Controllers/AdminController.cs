using FlexFit.Application.Commands;
using FlexFit.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlexFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-fitness-object")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFitnessObject([FromBody] CreateFitnessObjectDto dto)
        {
            var result = await _mediator.Send(new CreateFitnessObjectCommand(dto));
            return Ok(new { message = "Fitness object created successfully." });
        }

        [HttpPut("update-fitness-object")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFitnessObject([FromBody] UpdateFitnessObjectDto dto)
        {
            var result = await _mediator.Send(new UpdateFitnessObjectCommand(dto));
            if (!result) return NotFound(new { message = "Fitness object not found." });
            return Ok(new { message = "Fitness object updated successfully." });
        }

        [HttpDelete("delete-fitness-object/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFitnessObject(int id)
        {
            var result = await _mediator.Send(new DeleteFitnessObjectCommand(id));
            if (!result) return NotFound(new { message = "Fitness object not found." });
            return Ok(new { message = "Fitness object deleted successfully." });
        }

        [HttpDelete("delete-membership-card/{cardNumber}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMembershipCard(string cardNumber)
        {
            var result = await _mediator.Send(new DeleteMembershipCardCommand(cardNumber));
            if (!result) return NotFound(new { message = "Membership card not found." });
            return Ok(new { message = "Membership card deleted successfully." });
        }
    }
}
