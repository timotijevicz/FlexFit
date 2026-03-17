using FlexFit.Application.DTOs;
using MediatR;

namespace FlexFit.Application.Commands
{
    public class CreateFitnessObjectCommand : IRequest<bool>
    {
        public CreateFitnessObjectDto Dto { get; }
        public CreateFitnessObjectCommand(CreateFitnessObjectDto dto) => Dto = dto;
    }
}
