using BCrypt.Net;
using FlexFit.Application.Queries;
using FlexFit.Data;
using FlexFit.Models;
using FlexFit.MongoModels.Models;
using FlexFit.MongoModels.Repositories;
using FlexFit.Token;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Application.Handlers
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, string>
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly LoginRepository _loginRepository;

        public LoginQueryHandler(AppDbContext context, ITokenService tokenService, LoginRepository loginRepository)
        {
            _context = context;
            _tokenService = tokenService;
            _loginRepository = loginRepository;
        }
        public async Task<string> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.LoginDto.Email, cancellationToken);

            if (user == null)
            {
               
                if (request.LoginDto.IsGoogle)
                {
                    user = new Member
                    {
                        Email = request.LoginDto.Email,
                        Password = null,
                        Role = Role.Member
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return null; 
                }
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

            
            var token = _tokenService.CreateToken(user);

            var log = new Login
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                Role = user.Role.ToString(),
                Time = DateTime.UtcNow
            };

            await _loginRepository.AddAsync(log);

            return token;
        }

    }
}






//public async Task<string> Handle(LoginQuery request, CancellationToken cancellationToken)
//{
//    // 1?? Provera korisnika u SQL bazi
//    var user = await _context.Users
//        .FirstOrDefaultAsync(u => u.Email == request.LoginDto.Email, cancellationToken);

//    if (user == null || !BCrypt.Net.BCrypt.Verify(request.LoginDto.Password, user.Password))
//    {
//        return null; // Nevalidni kredencijali
//    }

//    // 2?? Kreiranje tokena
//    var token = _tokenService.CreateToken(user);

//    // 3?? Logovanje u MongoDB kolekciju Login
//    var log = new Login
//    {
//        UserId = user.Id.ToString(),     // ili samo user.Id ako je string
//        Email = user.Email,
//        Role = user.Role.ToString(),     // "Member" ili "Employee"
//        Time = DateTime.UtcNow
//    };

//    await _loginRepository.AddAsync(log);

//    return token;
//}