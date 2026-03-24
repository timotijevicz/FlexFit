
using FlexFit.Infrastructure.Repositories.Interfaces;
using FlexFit.Domain.Interfaces.Repositories;
using FlexFit.Domain.MongoModels.Repositories;


namespace FlexFit.Infrastructure.UnitOfWorkLayer
{
    public interface IUnitOfWork
    {
        IMemberRepository Members { get; }
        IEmployeeRepository Employees { get; }
        IFitnessObjectRepository FitnessObjects { get; }
        IResourceRepository Resources { get; }
        IReservationRepository Reservations { get; }
        IPenaltyCardRepository PenaltyCards { get; }
        IPenaltyPointRepository PenaltyPoints { get; }
        IMembershipCardRepository MembershipCards { get; }
        IMemberGraphRepository MemberGraph { get; }

        // MongoDB Repositories
        EntryLogRepository EntryLogs { get; }
        ReservationLogRepository ReservationLogs { get; }
        PenaltyLogRepository PenaltyLogs { get; }
        MembershipLogRepository MembershipLogs { get; }
        IncidentRepository Incidents { get; }
        LoginRepository Logins { get; }
        RateLimitViolationRepository RateLimitViolations { get; }
        
        Task SaveAsync();
    }
}