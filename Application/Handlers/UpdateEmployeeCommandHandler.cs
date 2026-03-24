using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using MediatR;
using BCrypt.Net;

namespace FlexFit.Application.Handlers
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public UpdateEmployeeCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _uow.Employees.GetByIdAsync(request.Dto.Id);
            if (employee == null) return false;

            employee.FirstName = request.Dto.FirstName;
            employee.LastName = request.Dto.LastName;
            employee.Address = request.Dto.Address;
            employee.Email = request.Dto.Email;
            employee.License = request.Dto.License;
            employee.EmployeeType = (EmployeeType)request.Dto.EmployeeType;

            if (!string.IsNullOrEmpty(request.Dto.Password))
            {
                employee.Password = BCrypt.Net.BCrypt.HashPassword(request.Dto.Password);
            }

            await _uow.Employees.UpdateAsync(employee);
            await _uow.SaveAsync();

            return true;
        }
    }
}
