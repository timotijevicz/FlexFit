using FlexFit.Infrastructure.Data;
using FlexFit.Infrastructure.Repositories;
using FlexFit.Infrastructure.Repositories.Interfaces;
using FlexFit.Domain.Interfaces.Repositories;
using FlexFit.Domain.MongoModels.Repositories;

namespace FlexFit.Infrastructure.UnitOfWorkLayer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IMemberGraphRepository _graphRepo;
        private readonly MongoDbContext _mongoContext;

        public UnitOfWork(
            AppDbContext context, 
            IMemberGraphRepository graphRepo, 
            MongoDbContext mongoContext,
            IReservationRepository reservationRepo,
            IResourceRepository resourceRepo,
            ITimeSlotRepository timeSlotRepo,
            IPenaltyCardRepository penaltyCardRepo,
            IPenaltyPointRepository penaltyPointRepo)
        {
            _context = context;
            _graphRepo = graphRepo;
            _mongoContext = mongoContext;
            
            // MongoDB repositories (Direct assignment)
            EntryLogs = new EntryLogRepository(_mongoContext);
            PenaltyLogs = new PenaltyLogRepository(_mongoContext);
            ReservationLogs = new ReservationLogRepository(_mongoContext);
            MembershipLogs = new MembershipLogRepository(_mongoContext);
            Incidents = new IncidentRepository(_mongoContext);
            Logins = new LoginRepository(_mongoContext);
            RateLimitViolations = new RateLimitViolationRepository(_mongoContext);

            // Interface-based repositories (DI Injected)
            Resources = resourceRepo;
            Reservations = reservationRepo;
            PenaltyCards = penaltyCardRepo;
            PenaltyPoints = penaltyPointRepo;

            // Rest (Manual for now, consistency with original)
            Members = new MemberRepository(_context);
            Employees = new EmployeeRepository(_context);
            FitnessObjects = new FitnessObjectRepository(_context);
            MembershipCards = new MembershipCardRepository(_context);
        }

        public IMemberRepository Members { get; private set; }
        public IEmployeeRepository Employees { get; private set; }
        public IFitnessObjectRepository FitnessObjects { get; private set; }
        public IResourceRepository Resources { get; private set; }
        public IReservationRepository Reservations { get; private set; }
        public IPenaltyCardRepository PenaltyCards { get; private set; }
        public IPenaltyPointRepository PenaltyPoints { get; private set; }
        public IMembershipCardRepository MembershipCards { get; private set; }
        public IMemberGraphRepository MemberGraph => _graphRepo;

        // MongoDB Repositories
        public EntryLogRepository EntryLogs { get; private set; }
        public ReservationLogRepository ReservationLogs { get; private set; }
        public PenaltyLogRepository PenaltyLogs { get; private set; }
        public MembershipLogRepository MembershipLogs { get; private set; }
        public IncidentRepository Incidents { get; private set; }
        public LoginRepository Logins { get; private set; }
        public RateLimitViolationRepository RateLimitViolations { get; private set; }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}