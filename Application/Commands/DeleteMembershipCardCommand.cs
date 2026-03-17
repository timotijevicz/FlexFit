using MediatR;

namespace FlexFit.Application.Commands
{
    public class DeleteMembershipCardCommand : IRequest<bool>
    {
        public string CardNumber { get; }
        public DeleteMembershipCardCommand(string cardNumber) => CardNumber = cardNumber;
    }
}
