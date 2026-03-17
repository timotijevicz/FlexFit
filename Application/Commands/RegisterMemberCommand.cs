using FlexFit.Application.DTOs;
using MediatR;

namespace FlexFit.Application.Commands
{
    public class RegisterMemberCommand : IRequest<bool>
    {
        public MemberRegistrationDto Dto { get; set; }

        public RegisterMemberCommand(MemberRegistrationDto dto)
        {
            Dto = dto;
        }
    }
}
