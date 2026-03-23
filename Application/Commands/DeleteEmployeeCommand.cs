using MediatR;

namespace FlexFit.Application.Commands
{
    public class DeleteEmployeeCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public DeleteEmployeeCommand(int id) => Id = id;
    }
}
