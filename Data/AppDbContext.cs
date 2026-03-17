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

            // USER TPH
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("Admin")
                .HasValue<Member>("Member")
                .HasValue<Employee>("Employee");

           
           

            modelBuilder.Entity<Member>()
        .HasMany(m => m.SubscriptionCards)
        .WithOne(c => c.Member)
        .HasForeignKey(c => c.MemberId)
        .OnDelete(DeleteBehavior.Cascade); // jer subscription ide uz registraciju

            modelBuilder.Entity<Member>()
                .HasMany(m => m.DailyCards)
                .WithOne(c => c.Member)
                .HasForeignKey(c => c.MemberId)
                .OnDelete(DeleteBehavior.SetNull); // dnevne karte opciono
            // UNIQUE
            modelBuilder.Entity<MembershipCard>()
                .HasIndex(c => c.CardNumber)
                .IsUnique();

            // MANY-TO-MANY
            modelBuilder.Entity<DailyCard>()
                .HasMany(d => d.FitnessObjects)
                .WithMany(f => f.DailyCards);
        }
    }
}