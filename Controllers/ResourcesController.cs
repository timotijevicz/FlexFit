using FlexFit.Models;
using FlexFit.UnitOfWorkLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlexFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public ResourcesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin,Employee,1,2,Redar")]
        public async Task<IActionResult> CreateResource([FromBody] CreateResourceDto dto)
        {
            var resource = new Resource
            {
                Type = dto.Type,
                Status = dto.Status,
                Floor = dto.Floor,
                IsPremium = dto.IsPremium,
                PremiumFee = dto.PremiumFee,
                FitnessObjectId = dto.FitnessObjectId
            };

            await _uow.Resources.AddAsync(resource);
            
            return Ok(new { message = "Sprava uspešno dodata." });
        }
    }

    public class CreateResourceDto
    {
        public ResourceType Type { get; set; }
        public ResourceStatus Status { get; set; }
        public int Floor { get; set; }
        public bool IsPremium { get; set; }
        public decimal PremiumFee { get; set; }
        public int FitnessObjectId { get; set; }
    }
}
