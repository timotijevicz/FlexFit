using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using FlexFit.Domain.MongoModels.Models;
using FlexFit.Infrastructure.Repositories.Interfaces;
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
                var penalty = new PenaltyLog
                {
                    MemberId = request.MemberId,
                    Date = DateTime.UtcNow,
                    Type = "Point",
                    Reason = request.Description
                };
                
                await _uow.PenaltyLogs.AddAsync(penalty);

                try {
                    await _uow.MemberGraph.AssignPenaltyToMemberAsync(penalty.Id, request.MemberId.ToString(), request.Description);
                } catch (Exception ex) {
                    Console.WriteLine($"[CreatePenaltyPointHandler] Neo4j Sync Error: {ex.Message}");
                }

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
