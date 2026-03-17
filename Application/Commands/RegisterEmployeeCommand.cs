using FlexFit.Application.DTOs;
using MediatR;

namespace FlexFit.Application.Commands
{
    public class RegisterEmployeeCommand : IRequest<bool>
    {
        public EmployeeRegistrationDto Dto { get; set; }

        public RegisterEmployeeCommand(EmployeeRegistrationDto dto)
        {
            Dto = dto;
        }
    }
}
