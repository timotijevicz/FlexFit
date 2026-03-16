using FlexFit.Data;
using FlexFit.Repositories;
using FlexFit.Repositories.Interfaces;
using FlexFit.Repositoires;
namespace FlexFit.UnitOfWorkLayer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Members = new MemberRepository(_context);
            Employees = new EmployeeRepository(_context);
            FitnessObjects = new FitnessObjectRepository(_context);
            Resources = new ResourceRepository(_context);
            Reservations = new ReservationRepository(_context);
            PenaltyCards = new PenaltyCardRepository(_context);
            PenaltyPoints = new PenaltyPointRepository(_context);
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

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}