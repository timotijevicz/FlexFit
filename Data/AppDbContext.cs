using Microsoft.EntityFrameworkCore;
using FlexFit.Models;

namespace FlexFit.Data
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
        public DbSet<PenaltyCard> PenaltyCards { get; set; }
        public DbSet<PenaltyPoint> PenaltyPoints { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

      
            protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER NASLEĐIVANJE
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Member>("Member")
                .HasValue<Employee>("Employee");

            // KARTICE NASLEĐIVANJE (Ovo je ključno za tebe)
            modelBuilder.Entity<MembershipCard>()
                .HasDiscriminator<CardType>("CardType")
                .HasValue<DailyCard>(CardType.Daily)
                .HasValue<SubscriptionCard>(CardType.Subscription);

            // Veza 1:1 (Jedan član - jedna aktivna kartica)
            modelBuilder.Entity<Member>()
                .HasOne(m => m.ActiveCard)
                .WithOne(c => c.Member)
                .HasForeignKey<MembershipCard>(c => c.MemberId);

            // Indeks za skeniranje
            modelBuilder.Entity<MembershipCard>()
                .HasIndex(c => c.CardNumber)
                .IsUnique();
        }
    }
    
}