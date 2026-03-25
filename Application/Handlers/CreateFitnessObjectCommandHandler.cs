using FlexFit.Application.Commands;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using MediatR;

using FlexFit.Infrastructure.Repositories.Interfaces;

namespace FlexFit.Application.Handlers
{
    public class CreateFitnessObjectCommandHandler : IRequestHandler<CreateFitnessObjectCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public CreateFitnessObjectCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(CreateFitnessObjectCommand request, CancellationToken cancellationToken)
        {
            var fitnessObject = new FitnessObject
            {
                Name = request.Dto.Name,
                Address = request.Dto.Address,
                City = request.Dto.City,
                Capacity = request.Dto.Capacity,
                WorkingHours = request.Dto.WorkingHours
            };

            await _uow.FitnessObjects.AddAsync(fitnessObject);
            await _uow.SaveAsync();

            return true;
        }
    }
}
