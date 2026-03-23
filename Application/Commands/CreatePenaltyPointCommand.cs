using MediatR;

namespace FlexFit.Application.Commands
{
    public class CreatePenaltyPointCommand : IRequest<bool>
    {
        public int MemberId { get; set; }
        public string Description { get; set; }
    }
}
