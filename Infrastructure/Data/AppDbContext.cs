using Microsoft.EntityFrameworkCore;
using FlexFit.Domain.Models;

namespace FlexFit.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<FitnessObject> FitnessObjects { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<MembershipCard> MembershipCards { get; set; }
        public DbSet<DailyCard> DailyCards { get; set; }
        public DbSet<SubscriptionCard> SubscriptionCards { get; set; }
        // public DbSet<PenaltyCard> PenaltyCards { get; set; }
        // public DbSet<PenaltyPoint> PenaltyPoints { get; set; }
        // public DbSet<Reservation> Reservations { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("Admin")
                .HasValue<Member>("Member")
                .HasValue<Employee>("Employee");

            modelBuilder.Entity<Member>()
                .HasMany(m => m.SubscriptionCards)
                .WithOne(c => c.Member)
                .HasForeignKey(c => c.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MembershipCard>()
                .HasIndex(c => c.CardNumber)
                .IsUnique();

            modelBuilder.Entity<DailyCard>()
                .HasMany(d => d.FitnessObjects)
                .WithMany(f => f.DailyCards);

          
        }
    }
}