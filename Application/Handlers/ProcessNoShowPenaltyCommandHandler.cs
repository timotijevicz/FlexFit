using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using FlexFit.Domain.MongoModels.Models;
using FlexFit.Infrastructure.Repositories.Interfaces;
using FlexFit.Domain.MongoModels.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlexFit.Application.Handlers
{
    public class ProcessNoShowPenaltyCommandHandler : IRequestHandler<ProcessNoShowPenaltyCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ProcessNoShowPenaltyCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ProcessNoShowPenaltyCommand request, CancellationToken cancellationToken)
        {
            try 
            {
                Console.WriteLine($"[ProcessNoShowPenalty] Handling No-Show for Reservation: {request.ReservationId}");
                
                var log = await _uow.Reservations.GetByIdAsync(request.ReservationId);
                if (log == null) 
                {
                    Console.WriteLine($"[ProcessNoShowPenalty] ERROR: Reservation {request.ReservationId} not found in MongoDB.");
                    return false;
                }

                Console.WriteLine($"[ProcessNoShowPenalty] Reservation found. ID: {log.Id}, Current Status: {log.Status}, MemberId: {log.MemberId}");

                if (log.Status == "NoShow")
                {
                    Console.WriteLine($"[ProcessNoShowPenalty] Member {log.MemberId} was a No-Show. Issuing penalty point to MongoDB...");
                    
                    var penalty = new PenaltyPoint
                    {
                        MemberId = log.MemberId,
                        Date = DateTime.UtcNow,
                        Description = $"Automatski kazneni poen zbog nedolaska na termin (Rezervacija ID: {log.Id})",
                        IsCanceled = false
                    };
                    await _uow.PenaltyPoints.AddAsync(penalty);

                    try {
                        // Record that the booking actually happened (as a no-show)
                        await _uow.MemberGraph.RecordBookingAsync(log.MemberId.ToString(), log.ResourceId, log.Id);
                        // Record the penalty
                        await _uow.MemberGraph.AssignPenaltyToMemberAsync(penalty.Id, log.MemberId.ToString(), penalty.Description);
                    } catch (Exception ex) {
                        Console.WriteLine($"[ProcessNoShowPenalty] Neo4j Sync Error: {ex.Message}");
                    }

                    Console.WriteLine($"[ProcessNoShowPenalty] Penalty point recorded successfully in MongoDB.");
                    return true;
                }
                else 
                {
                    Console.WriteLine($"[ProcessNoShowPenalty] Reservation {request.ReservationId} status is {log.Status}, not NoShow. Skipping penalty.");
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProcessNoShowPenaltyHandler] FATAL ERROR: {ex.Message}");
                return false;
            }
        }
    }
}
