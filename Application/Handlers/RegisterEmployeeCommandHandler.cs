using FlexFit.Application.Commands;
using FlexFit.Models;
using FlexFit.UnitOfWorkLayer;
using MediatR;
using BCrypt.Net;

namespace FlexFit.Application.Handlers
{
    public class RegisterEmployeeCommandHandler : IRequestHandler<RegisterEmployeeCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public RegisterEmployeeCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(RegisterEmployeeCommand request, CancellationToken cancellationToken)
        {
             // 1. Hash Password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Dto.Password);

            // 2. Create Employee
            var newEmployee = new Employee
            {
                FirstName = request.Dto.FirstName,
                LastName = request.Dto.LastName,
                Address = request.Dto.Address,
                Email = request.Dto.Email,
                Password = hashedPassword,
                License = request.Dto.License,
                EmployeeType = request.Dto.EmployeeType,
                Role = Role.Employee
            };

            await _uow.Employees.AddAsync(newEmployee);
            await _uow.SaveAsync(); 

            return true;
        }
    }
}
