using FlexFit.Application.DTOs;
using MediatR;

namespace FlexFit.Application.Commands
{
    public class CreateSubscriptionCardCommand : IRequest<bool>
    {
        public CreateSubscriptionCardDto Dto { get; }
        public CreateSubscriptionCardCommand(CreateSubscriptionCardDto dto) => Dto = dto;
    }
}
