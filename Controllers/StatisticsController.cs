using FlexFit.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlexFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatisticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _mediator.Send(new GetStatisticsQuery());
            return Ok(result);
        }
    }
}
