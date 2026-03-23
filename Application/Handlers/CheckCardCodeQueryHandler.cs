using FlexFit.Application.Queries;
using FlexFit.UnitOfWorkLayer;
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
            // Returns true if unique (i.e. no card with this code exists)
            return existingCard == null;
        }
    }
}
