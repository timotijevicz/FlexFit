using FlexFit.Application.Commands;
using FlexFit.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class UpdateFitnessObjectCommandHandler : IRequestHandler<UpdateFitnessObjectCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public UpdateFitnessObjectCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(UpdateFitnessObjectCommand request, CancellationToken cancellationToken)
        {
            var fitnessObject = await _uow.FitnessObjects.GetByIdAsync(request.Dto.Id);
            if (fitnessObject == null) return false;

            fitnessObject.Name = request.Dto.Name;
            fitnessObject.Address = request.Dto.Address;
            fitnessObject.City = request.Dto.City;
            fitnessObject.Capacity = request.Dto.Capacity;
            fitnessObject.WorkingHours = request.Dto.WorkingHours;

            await _uow.FitnessObjects.UpdateAsync(fitnessObject);
            await _uow.SaveAsync();
            return true;
        }
    }
}
