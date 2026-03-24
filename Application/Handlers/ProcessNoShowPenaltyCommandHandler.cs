using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using FlexFit.Domain.MongoModels.Models;
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
                    Console.WriteLine($"[ProcessNoShowPenalty] Reservation {request.ReservationId} not found.");
                    return false;
                }

                if (log.Status == "NoShow")
                {
                    Console.WriteLine($"[ProcessNoShowPenalty] Member {log.MemberId} was a No-Show. Issuing penalty point to MongoDB...");
                    
                    // Using the refactored IPenaltyPointRepository which is now MongoDB-backed
                    await _uow.PenaltyPoints.AddAsync(new PenaltyPoint
                    {
                        MemberId = log.MemberId,
                        Date = DateTime.UtcNow,
                        Description = $"Automatski kazneni poen zbog nedolaska na termin (Rezervacija ID: {log.Id})",
                        IsCanceled = false
                    });

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
