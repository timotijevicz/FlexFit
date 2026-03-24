using MediatR;

namespace FlexFit.Application.Commands
{
    public class DeletePenaltyCommand : IRequest<bool>
    {
        public string Id { get; }
        public string Type { get; }

        public DeletePenaltyCommand(string id, string type)
        {
            Id = id;
            Type = type;
        }
    }
}
