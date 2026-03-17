using FlexFit.Application.Queries;
using FlexFit.Data;
using FlexFit.MongoModels.Models;
using FlexFit.MongoModels.Repositories;
using FlexFit.Token;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace FlexFit.Application.Handlers
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, string>
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly EntryLogRepository _entryLogRepository;

        public LoginQueryHandler(AppDbContext context, ITokenService tokenService, EntryLogRepository entryLogRepository)
        {
            _context = context;
            _tokenService = tokenService;
            _entryLogRepository = entryLogRepository;
        }

        public async Task<string> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.LoginDto.Email, cancellationToken);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.LoginDto.Password, user.Password))
            {
                return null; // Invalid credentials
            }

            // Create Token
            var token = _tokenService.CreateToken(user);

            // Log login to MongoDB
            var log = new EntryLog
            {
                Time = DateTime.UtcNow,
                CardStatus = "N/A", // Default for login event
                Incident = false
            };

            if (user.Role == FlexFit.Models.Role.Member)
            {
                log.MemberId = user.Id;
            }
            else if (user.Role == FlexFit.Models.Role.Employee)
            {
                log.EmployeeId = user.Id;
            }

            await _entryLogRepository.AddAsync(log);

            return token;
        }
    }
}
