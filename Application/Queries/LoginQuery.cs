using FlexFit.Application.DTOs;
using MediatR;

namespace FlexFit.Application.Queries
{
    public class LoginQuery : IRequest<string>
    {
        public LoginDto LoginDto { get; set; }

        public LoginQuery(LoginDto loginDto)
        {
            LoginDto = loginDto;
        }
    }
}
