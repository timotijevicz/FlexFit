//using FlexFit.Models;
//using FlexFit.MongoModels.Repositories;
//using FlexFit.UnitOfWorkLayer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//[ApiController]
//[Route("api/member")]
//[Authorize(Roles = "Member")]
//public class MemberController : ControllerBase
//{
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly EntryLogRepository _entryLogRepo;

//    public MemberController(IUnitOfWork unitOfWork, EntryLogRepository entryLogRepo)
//    {
//        _unitOfWork = unitOfWork;
//        _entryLogRepo = entryLogRepo;
//    }

//    [HttpGet("check-availability")]
//    public async Task<IActionResult> CheckAvailability(int fitnessId)
//    {
//        var fitness = await _unitOfWork.FitnessObjects.GetByIdAsync(fitnessId);
//        if (fitness == null)
//            return NotFound("Fitness object not found");

//        int freeResources = fitness.Resources.Count(r => r.Status == ResourceStatus.Slobodan);
//        return Ok(new { fitness.Id, fitness.Name, FreeResources = freeResources });
//    }

//    [HttpPost("activate-card")]
//    public async Task<IActionResult> ActivateCard([FromBody] string cardCode)
//    {
//        var subscription = (await _unitOfWork.MembershipCards.GetAllAsync())
//                                .OfType<SubscriptionCard>()
//                                .FirstOrDefault(c => c.Code == cardCode);

//        if (subscription == null)
//            return BadRequest("Invalid card code");

//        subscription.ValidFrom = DateTime.UtcNow;
//        subscription.ValidTo = DateTime.UtcNow.AddDays(30);

//        await _unitOfWork.MembershipCards.UpdateAsync(subscription);
//        _unitOfWork.SaveAsync();
//        return Ok(new { subscription.MemberId, subscription.ValidFrom, subscription.ValidTo });
//    }

//    [HttpGet("status/{memberId}")]
//    public async Task<IActionResult> GetStatus(int memberId)
//    {
//        var member = await _unitOfWork.Members.GetByIdAsync(memberId);
//        if (member == null)
//            return NotFound();

//        var penalties = await _unitOfWork.PenaltyPoints.GetAllAsync();
//        var memberPenalties = penalties.Where(p => p.MemberId == memberId);

//        return Ok(new
//        {
//            member.Id,
//            PenaltyPoints = member.PenaltyPoints,
//            PenaltyHistory = memberPenalties
//        });
//    }
//}