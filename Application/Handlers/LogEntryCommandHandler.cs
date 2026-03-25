using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Domain.MongoModels.Models;
using FlexFit.Domain.MongoModels.Repositories;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using MediatR;
using System.Linq;
using FlexFit.Infrastructure.Repositories.Interfaces;

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
                    }
                }

                try 
                {
                    if (dto.MemberId > 0)
                    {
                        Console.WriteLine($"[LogEntryHandler] Updating Neo4j for visit...");
                        await _graphRepo.RecordVisitAsync(dto.MemberId.ToString(), dto.FitnessObjectId);
                        
                        if (dto.EmployeeId > 0)
                        {
                            await _graphRepo.RecordEmployeeCheckAsync(dto.EmployeeId.ToString(), dto.MemberId.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LogEntryHandler] WARNING: Neo4j update failed: {ex.Message}");
                }

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
                            var penalty = new PenaltyLog
                            {
                                MemberId = dto.MemberId,
                                FitnessObjectId = dto.FitnessObjectId,
                                Date = DateTime.UtcNow,
                                Price = 1000,
                                Type = "DailyTicket",
                                Reason = $"Automatski izdata kazna zbog nevažeće kartice ({dto.CardNumber}). Tip: {dto.CardType}, Status: {dto.CardStatus}"
                            };
                            await _uow.PenaltyLogs.AddAsync(penalty);
                            
                            try {
                                await _graphRepo.AssignPenaltyToMemberAsync(penalty.Id, dto.MemberId.ToString(), penalty.Reason);
                            } catch (Exception ex) {
                                Console.WriteLine($"[LogEntryHandler] Neo4j Penalty Sync Error: {ex.Message}");
                            }

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
