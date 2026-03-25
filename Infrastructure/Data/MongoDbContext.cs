using MongoDB.Driver;
using FlexFit.Domain.MongoModels.Models;

namespace FlexFit.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly IConfiguration _configuration;  

        public MongoDbContext(IConfiguration configuration) 
        {
            _configuration = configuration;
            var connectionString = _configuration["MongoSettings:ConnectionString"] ?? "mongodb://localhost:27017";
            var databaseName = _configuration["MongoSettings:Database"] ?? "flexfit_logs";

            
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ConnectTimeout = TimeSpan.FromSeconds(5);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            
            var client = new MongoClient(settings); 
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<EntryLog> EntryLogs =>
            _database.GetCollection<EntryLog>("entry_logs");

        public IMongoCollection<Incident> Incidents =>
            _database.GetCollection<Incident>("incidents");

        public IMongoCollection<RateLimitViolation> RateLimitViolations =>
            _database.GetCollection<RateLimitViolation>("rate_limit_violations");

        public IMongoCollection<Login> Login =>
            _database.GetCollection<Login>("logins");

        public IMongoCollection<ReservationLog> ReservationLogs => 
            _database.GetCollection<ReservationLog>("reservation_logs");

        public IMongoCollection<PenaltyLog> PenaltyLogs => 
            _database.GetCollection<PenaltyLog>("penalty_logs");

        public IMongoCollection<MembershipLog> MembershipLogs => 
            _database.GetCollection<MembershipLog>("membership_logs");
    }
}