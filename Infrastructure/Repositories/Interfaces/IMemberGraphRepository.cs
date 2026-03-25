namespace FlexFit.Infrastructure.Repositories.Interfaces
{
    public interface IMemberGraphRepository
    {
        Task RecordVisitAsync(string memberId, int fitnessObjectId, string memberName = null, string objectName = null);
        Task RecordReservationAsync(string memberId, int resourceId, string memberName = null, string resourceType = null);
        Task<IEnumerable<string>> GetRecommendedObjectsAsync(string memberId);
        
        Task AssignCardToMemberAsync(string memberId, string cardId, string cardName = null);
        Task RecordBookingAsync(string memberId, int resourceId, string bookingId = null);
        Task RecordEmployeeCheckAsync(string employeeId, string memberId, string employeeName = null);
        Task AssignPenaltyToMemberAsync(string penaltyId, string memberId, string penaltyDescription = null);
        Task LinkResourceToGymAsync(int resourceId, int gymId, string resourceName = null, string gymName = null);
        Task LinkCardToGymAsync(string cardId, int gymId);
    }
}
