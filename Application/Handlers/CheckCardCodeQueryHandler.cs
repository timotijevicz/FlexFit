using FlexFit.Application.Queries;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class CheckCardCodeQueryHandler : IRequestHandler<CheckCardCodeQuery, bool>
    {
        private readonly IUnitOfWork _uow;
        public CheckCardCodeQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(CheckCardCodeQuery request, CancellationToken cancellationToken)
        {
            var existingCard = await _uow.MembershipCards.GetByCardNumberAsync(request.Code);
            return existingCard == null;
        }
    }
}
