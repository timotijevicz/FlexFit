using MediatR;

namespace FlexFit.Application.Commands
{
    public class ExtendMembershipCommand : IRequest<bool>
    {
        public string CardNumber { get; }

        public ExtendMembershipCommand(string cardNumber)
        {
            CardNumber = cardNumber;
        }
    }
}
