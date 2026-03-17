using FlexFit.Application.DTOs;
using MediatR;

namespace FlexFit.Application.Commands
{
    public class UpdateFitnessObjectCommand : IRequest<bool>
    {
        public UpdateFitnessObjectDto Dto { get; }
        public UpdateFitnessObjectCommand(UpdateFitnessObjectDto dto) => Dto = dto;
    }
}
