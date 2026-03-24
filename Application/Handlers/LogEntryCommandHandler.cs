using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Domain.MongoModels.Models;
using FlexFit.Domain.MongoModels.Repositories;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using MediatR;
using System.Linq;
using FlexFit.Domain.Interfaces.Repositories;

namespace FlexFit.Application.Handlers
{
    public class LogEntryCommandHandler : IRequestHandler<LogEntryCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMemberGraphRepository _graphRepo;

        public LogEntryCommandHandler(
            IUnitOfWork uow, 
            IMemberGraphRepository graphRepo)
        {
            _uow = uow;
            _graphRepo = graphRepo;
        }

        public async Task<bool> Handle(LogEntryCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            try 
            {
                Console.WriteLine($"[LogEntryHandler] START - MemberId: {dto.MemberId}, Card: {dto.CardNumber}");

                Console.WriteLine($"[LogEntryHandler] Preparing EntryLog model...");
                bool isInvalid = dto.CardStatus == "Invalid" || dto.CardStatus == "Nevažeća";
                bool isNotActive = dto.CardStatus != "Active" && dto.CardStatus != "Aktivna";

                var log = new EntryLog
                {
                    MemberId = dto.MemberId,
                    EmployeeId = dto.EmployeeId,
                    Time = DateTime.UtcNow,
                    CardStatus = dto.CardStatus,
                    CardType = dto.CardType,
                    Incident = isNotActive 
                        ? (isInvalid ? "Invalid card attempt" : "Unauthorized entry attempt")
                        : null
                };

                Console.WriteLine($"[LogEntryHandler] Calling EntryLogs.AddAsync...");
                await _uow.EntryLogs.AddAsync(log);
                Console.WriteLine($"[LogEntryHandler] EntryLogs.AddAsync finished.");

                // 2. Log Incident to MongoDB if applicable
                // For "Invalid" cards, we only record in EntryLog, not in Incidents (as per requirement "it is not saved in the mongo database")
                if (log.Incident != null && !isInvalid)
                {
                    try 
                    {
                        Console.WriteLine($"[LogEntryHandler] Logging incident to MongoDB (Member: {dto.MemberId}, Reason: {log.Incident})...");
                        await _uow.Incidents.AddAsync(new Incident
                        {
                            MemberId = dto.MemberId,
                            FitnessObjectId = dto.FitnessObjectId,
                            Reason = log.Incident,
                            Time = DateTime.UtcNow
                        });
                        Console.WriteLine($"[LogEntryHandler] Incident logged successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LogEntryHandler] WARNING: Failed to log incident to MongoDB: {ex.Message}");
                        // We continue so the penalty can still be issued.
                    }
                }

                // 3. Update Neo4j
                try 
                {
                    if (dto.MemberId > 0)
                    {
                        Console.WriteLine($"[LogEntryHandler] Updating Neo4j for visit...");
                        await _graphRepo.RecordVisitAsync(dto.MemberId.ToString(), dto.FitnessObjectId);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LogEntryHandler] WARNING: Neo4j update failed: {ex.Message}");
                }

                // 4. Automated Penalty Check (MongoDB Only)
                // Do NOT issue penalty for "Invalid" cards
                bool isDailyOrSub = dto.CardType == "Daily" || dto.CardType == "Dnevna" || 
                                    dto.CardType == "Subscription" || dto.CardType == "Pretplatna" ||
                                    dto.CardType == "DailyTicket" || dto.CardType == "DnevnaKarta";

                Console.WriteLine($"[LogEntryHandler] Penalty Check: isNotActive={isNotActive}, isInvalid={isInvalid}, isDailyOrSub={isDailyOrSub}, MemberId={dto.MemberId}");

                if (isNotActive && !isInvalid && isDailyOrSub && dto.MemberId > 0)
                {
                    try 
                    {
                        Console.WriteLine($"[LogEntryHandler] Checking for recent penalties...");
                        bool hasRecentPenalty = await _uow.PenaltyCards.HasRecentPenaltyAsync(dto.MemberId, 12);
                        
                        if (!hasRecentPenalty)
                        {
                            Console.WriteLine($"[LogEntryHandler] Issuing penalty for member {dto.MemberId}");
                            // Bypassing SQL model entirely, using MongoDB Repository directly via UoW
                            await _uow.PenaltyLogs.AddAsync(new PenaltyLog
                            {
                                MemberId = dto.MemberId,
                                FitnessObjectId = dto.FitnessObjectId,
                                Timestamp = DateTime.UtcNow,
                                Price = 1000,
                                Type = "DailyTicket",
                                Reason = $"Automatski izdata kazna zbog nevažeće kartice ({dto.CardNumber}). Tip: {dto.CardType}, Status: {dto.CardStatus}"
                            });
                            Console.WriteLine($"[LogEntryHandler] Penalty recorded in MongoDB.");
                        }
                        else 
                        {
                            Console.WriteLine($"[LogEntryHandler] Member {dto.MemberId} already has a recent penalty. Skipping.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LogEntryHandler] ERROR: Penalty issuance failed: {ex.Message}");
                    }
                }

                // 5. Mark Reservations as Used
                if (dto.MemberId > 0)
                {
                    var now = DateTime.UtcNow;
                    var allMemRes = await _uow.Reservations.GetByMemberIdAsync(dto.MemberId);
                    var reservations = allMemRes.Where(r => 
                        r.Status == "Reserved" &&
                        r.StartTime <= now.AddMinutes(30) && 
                        r.EndTime >= now.AddMinutes(-30)).ToList();

                    foreach (var res in reservations)
                    {
                        res.Status = "Used";
                        await _uow.Reservations.UpdateAsync(res);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LogEntryHandler] FATAL ERROR: {ex.Message}");
                return false;
            }
        }
    }
}
