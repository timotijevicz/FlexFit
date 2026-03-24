using FlexFit.Application.Commands;
using FlexFit.Application.DTOs;
using FlexFit.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlexFit.Token;
using Microsoft.EntityFrameworkCore;
using FlexFit.Data;

namespace FlexFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;

        public AuthController(IMediator mediator, ITokenService tokenService, AppDbContext context)
        {
            _mediator = mediator;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var tokenResponse = await _mediator.Send(new LoginQuery(loginDto));

            if (tokenResponse == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(tokenResponse);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenResponseDto tokenDto)
        {
            if (tokenDto == null) return BadRequest("Invalid request");

            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var userId = int.Parse(principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return BadRequest("Invalid client request");

            var newTokens = _tokenService.CreateToken(user);

            user.RefreshToken = newTokens.RefreshToken;
            await _context.SaveChangesAsync();

            return Ok(newTokens);
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