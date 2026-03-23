using MediatR;

namespace FlexFit.Application.Commands
{
    public class CancelPenaltyCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Type { get; set; } // "Card" or "Point"
        public string Reason { get; set; }

        public CancelPenaltyCommand(int id, string type, string reason)
        {
            Id = id;
            Type = type;
            Reason = reason;
        }
    }
}
