using MediatR;

namespace FlexFit.Application.Commands
{
    public class ProcessNoShowPenaltyCommand : IRequest<bool>
    {
        public string ReservationId { get; }

        public ProcessNoShowPenaltyCommand(string reservationId)
        {
            ReservationId = reservationId;
        }
    }
}
