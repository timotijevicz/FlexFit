using FlexFit.Application.DTOs;
using MediatR;

namespace FlexFit.Application.Commands
{
    public class UpdateEmployeeCommand : IRequest<bool>
    {
        public UpdateEmployeeDto Dto { get; }
        public UpdateEmployeeCommand(UpdateEmployeeDto dto) => Dto = dto;
    }
}
