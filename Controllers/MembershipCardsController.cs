using FlexFit.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlexFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipCardsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MembershipCardsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? type = null)
        {
            var result = await _mediator.Send(new GetAllMembershipCardsQuery { Type = type });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetMembershipCardByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("check-code/{code}")]
        public async Task<IActionResult> CheckCode(string code)
        {
            var isUnique = await _mediator.Send(new CheckCardCodeQuery(code));
            return Ok(new { isUnique = isUnique });
        }
    }
}
