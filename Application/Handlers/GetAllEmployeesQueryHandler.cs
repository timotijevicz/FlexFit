using FlexFit.Application.Queries;
using FlexFit.Models;
using FlexFit.UnitOfWorkLayer;
using MediatR;

namespace FlexFit.Application.Handlers
{
    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, List<Employee>>
    {
        private readonly IUnitOfWork _uow;
        public GetAllEmployeesQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<List<Employee>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _uow.Employees.GetAllAsync();
            return employees.ToList();
        }
    }
}
