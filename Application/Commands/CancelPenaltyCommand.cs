using MediatR;

namespace FlexFit.Application.Commands
{
    public class CancelPenaltyCommand : IRequest<bool>
    {
        public string Id { get; set; }
        public string Type { get; set; } 
        public string Reason { get; set; }

        public CancelPenaltyCommand(string id, string type, string reason)
        {
            Id = id;
            Type = type;
            Reason = reason;
        }
    }
}
