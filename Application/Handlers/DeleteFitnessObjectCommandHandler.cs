using FlexFit.Application.Commands;
using FlexFit.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class DeleteFitnessObjectCommandHandler : IRequestHandler<DeleteFitnessObjectCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public DeleteFitnessObjectCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(DeleteFitnessObjectCommand request, CancellationToken cancellationToken)
        {
            var fitnessObject = await _uow.FitnessObjects.GetByIdAsync(request.Id);
            if (fitnessObject == null) return false;

            await _uow.FitnessObjects.DeleteAsync(fitnessObject);
            await _uow.SaveAsync();
            return true;
        }
    }
}
