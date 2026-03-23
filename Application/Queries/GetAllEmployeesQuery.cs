using FlexFit.Models;
using MediatR;

namespace FlexFit.Application.Queries
{
    public class GetAllEmployeesQuery : IRequest<List<Employee>> { }
}
