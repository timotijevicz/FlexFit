using FlexFit.Application.Commands;
using FlexFit.Application.DTOs;
using FlexFit.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlexFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _mediator.Send(new LoginQuery(loginDto));

            if (token == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(token);
        }

        [HttpPost("register-member")]
        public async Task<IActionResult> RegisterMember([FromBody] MemberRegistrationDto dto)
        {
            var result = await _mediator.Send(new RegisterMemberCommand(dto));

            if (!result)
            {
                return BadRequest(new { message = "Registration failed. Please check your card number." });
            }

            return Ok(new { message = "Member registered successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-employee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] EmployeeRegistrationDto dto)
        {
            var result = await _mediator.Send(new RegisterEmployeeCommand(dto));

            if (!result)
            {
                return BadRequest(new { message = "Failed to register employee." });
            }

            return Ok(new { message = "Employee registered successfully." });
        }
    }
}
