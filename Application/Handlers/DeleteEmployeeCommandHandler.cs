using FlexFit.Application.Commands;
using FlexFit.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public DeleteEmployeeCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var emp = await _uow.Employees.GetByIdAsync(request.Id);
            if (emp == null) return false;

            await _uow.Employees.DeleteAsync(emp);
            await _uow.SaveAsync();
            return true;
        }
    }
}
