using FlexFit.Application.Commands;
using FlexFit.Models;
using FlexFit.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class CreatePenaltyPointCommandHandler : IRequestHandler<CreatePenaltyPointCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public CreatePenaltyPointCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(CreatePenaltyPointCommand request, CancellationToken cancellationToken)
        {
            var point = new PenaltyPoint
            {
                MemberId = request.MemberId,
                Date = DateTime.UtcNow,
                Description = request.Description,
                IsCanceled = false
            };

            await _uow.PenaltyPoints.AddAsync(point);
            await _uow.SaveAsync();

            return true;
        }
    }
}
