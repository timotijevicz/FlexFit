using FlexFit.Application.DTOs;
using MediatR;

namespace FlexFit.Application.Commands
{
    public class CreateDailyCardCommand : IRequest<bool>
    {
        public CreateDailyCardDto Dto { get; }
        public CreateDailyCardCommand(CreateDailyCardDto dto) => Dto = dto;
    }
}
