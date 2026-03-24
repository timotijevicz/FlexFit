using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using FlexFit.Domain.MongoModels.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlexFit.Application.Handlers
{
    public class CreatePenaltyPointCommandHandler : IRequestHandler<CreatePenaltyPointCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public CreatePenaltyPointCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<bool> Handle(CreatePenaltyPointCommand request, CancellationToken cancellationToken)
        {
            try 
            {
                // Direct MongoDB creation
                await _uow.PenaltyLogs.AddAsync(new PenaltyLog
                {
                    MemberId = request.MemberId,
                    Timestamp = DateTime.UtcNow,
                    Type = "Point",
                    Reason = request.Description
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreatePenaltyPointHandler] Error: {ex.Message}");
                return false;
            }
        }
    }
}
