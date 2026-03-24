using BCrypt.Net;
using FlexFit.Application.Queries;
using FlexFit.Infrastructure.Data;
using FlexFit.Domain.Models;
using FlexFit.Domain.MongoModels.Models;
using FlexFit.Domain.MongoModels.Repositories;
using FlexFit.Infrastructure.Token;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FlexFit.Application.DTOs;
using FlexFit.Infrastructure.UnitOfWorkLayer;

namespace FlexFit.Application.Handlers
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, TokenResponseDto?>
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _uow;

        public LoginQueryHandler(AppDbContext context, ITokenService tokenService, IUnitOfWork uow)
        {
            _context = context;
            _tokenService = tokenService;
            _uow = uow;
        }
        public async Task<TokenResponseDto?> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.LoginDto.Email, cancellationToken);

            if (user == null)
            {
                return null;
            }


            if (!request.LoginDto.IsGoogle)
            {
                if (user.Password == null || !BCrypt.Net.BCrypt.Verify(request.LoginDto.Password, user.Password))
                    return null;
            }

            if (user is Member member)
            {
                var unactivatedCard = await _context.SubscriptionCards
                    .FirstOrDefaultAsync(c => c.MemberId == member.Id && c.ValidFrom == null, cancellationToken);

                if (unactivatedCard != null)
                {
                    unactivatedCard.ValidFrom = DateTime.UtcNow;
                    unactivatedCard.ValidTo = DateTime.UtcNow.AddDays(30);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }


            var tokenResponse = _tokenService.CreateToken(user);
            user.RefreshToken = tokenResponse.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync(cancellationToken);

            var log = new Login
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                Role = user.Role.ToString(),
                Time = DateTime.UtcNow
            };

            await _uow.Logins.AddAsync(log);

            return tokenResponse;
        }

    }
}



