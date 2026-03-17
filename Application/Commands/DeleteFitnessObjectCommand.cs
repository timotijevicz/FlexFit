using MediatR;

namespace FlexFit.Application.Commands
{
    public class DeleteFitnessObjectCommand : IRequest<bool>
    {
        public int Id { get; }
        public DeleteFitnessObjectCommand(int id) => Id = id;
    }
}
