using FlexFit.Models;
using MediatR;

namespace FlexFit.Application.Commands
{
    public class CreatePenaltyCardCommand : IRequest<bool>
    {
        public int MemberId { get; set; }
        public int FitnessObjectId { get; set; }
        public decimal Price { get; set; }
        public string Reason { get; set; }
    }
}
