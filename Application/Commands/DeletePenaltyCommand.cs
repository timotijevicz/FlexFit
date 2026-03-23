using MediatR;

namespace FlexFit.Application.Commands
{
    public class DeletePenaltyCommand : IRequest<bool>
    {
        public int Id { get; }
        public string Type { get; }

        public DeletePenaltyCommand(int id, string type)
        {
            Id = id;
            Type = type;
        }
    }
}
