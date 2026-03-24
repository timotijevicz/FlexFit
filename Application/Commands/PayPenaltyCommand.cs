using MediatR;

namespace FlexFit.Application.Commands
{
    public record PayPenaltyCommand(string PenaltyId) : IRequest<bool>;
}
