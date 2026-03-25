using FlexFit.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlexFit.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FitnessObjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FitnessObjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string searchTerm = null, [FromQuery] string city = null)
        {
            var result = await _mediator.Send(new GetAllFitnessObjectsQuery(searchTerm, city));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetFitnessObjectByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
