using FlexFit.Infrastructure.Repositories.Interfaces;
using FlexFit.Infrastructure.Data;
using Neo4j.Driver;

namespace FlexFit.Infrastructure.Repositories
{
    public class MemberGraphRepository : IMemberGraphRepository
    {
        private readonly Neo4jContext _context;

        public MemberGraphRepository(Neo4jContext context)
        {
            _context = context;
        }

        public async Task RecordVisitAsync(string memberId, int fitnessObjectId, string memberName = null, string objectName = null)
        {
            using var session = _context.GetSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(@"
                    MERGE (m:Member {id: $mId})
                    ON CREATE SET m.name = $mName
                    MERGE (o:FitnessObject {id: $oId})
                    ON CREATE SET o.name = $oName
                    MERGE (m)-[v:VISITED]->(o)
                    ON CREATE SET v.count = 1, v.lastVisit = datetime()
                    ON MATCH SET v.count = v.count + 1, v.lastVisit = datetime()", 
                    new { mId = memberId, mName = memberName, oId = fitnessObjectId.ToString(), oName = objectName });
            });
        }

        public async Task RecordReservationAsync(string memberId, int resourceId, string memberName = null, string resourceType = null)
        {
            using var session = _context.GetSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(@"
                    MERGE (m:Member {id: $mId})
                    ON CREATE SET m.name = $mName
                    MERGE (r:Resource {id: $rId})
                    ON CREATE SET r.type = $rType
                    MERGE (m)-[:RESERVED]->(r)", 
                    new { mId = memberId, mName = memberName, rId = resourceId.ToString(), rType = resourceType });
            });
        }

        public async Task<IEnumerable<string>> GetRecommendedObjectsAsync(string memberId)
        {
            using var session = _context.GetSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(@"
                    MATCH (me:Member {id: $id})-[:RESERVED]->(r:Resource)
                    WITH me, r.type AS favType, count(r) AS usageCount
                    ORDER BY usageCount DESC LIMIT 1
                    MATCH (otherO:FitnessObject)<-[:BELONGS_TO]-(otherR:Resource {type: favType})
                    WHERE NOT (me)-[:VISITED]->(otherO)
                    RETURN DISTINCT otherO.name AS name", 
                    new { id = memberId });

                var list = new List<string>();
                while (await result.FetchAsync())
                {
                    list.Add(result.Current["name"].As<string>());
                }
                return list;
            });
        }

        public async Task AssignCardToMemberAsync(string memberId, string cardId, string cardName = null)
        {
            using var session = _context.GetSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(@"
                    MERGE (m:Member {id: $mId})
                    MERGE (c:Card {id: $cId})
                    ON CREATE SET c.name = $cName
                    MERGE (m)-[:HAS_CARD]->(c)", 
                    new { mId = memberId, cId = cardId, cName = cardName });
            });
        }

        public async Task RecordBookingAsync(string memberId, int resourceId, string bookingId = null)
        {
            using var session = _context.GetSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(@"
                    MERGE (m:Member {id: $mId})
                    MERGE (r:Resource {id: $rId})
                    MERGE (m)-[:BOOKED]->(r)
                    WITH m, r
                    WHERE $bId IS NOT NULL
                    MERGE (b:Booking {id: $bId})
                    MERGE (b)-[:FOR]->(r)", 
                    new { mId = memberId, rId = resourceId.ToString(), bId = bookingId });
            });
        }

        public async Task RecordEmployeeCheckAsync(string employeeId, string memberId, string employeeName = null)
        {
            using var session = _context.GetSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(@"
                    MERGE (e:Employee {id: $eId})
                    ON CREATE SET e.name = $eName
                    MERGE (m:Member {id: $mId})
                    MERGE (e)-[:CHECKED]->(m)", 
                    new { eId = employeeId, eName = employeeName, mId = memberId });
            });
        }

        public async Task AssignPenaltyToMemberAsync(string penaltyId, string memberId, string penaltyDescription = null)
        {
            using var session = _context.GetSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(@"
                    MERGE (p:Penalty {id: $pId})
                    ON CREATE SET p.description = $pDesc
                    MERGE (m:Member {id: $mId})
                    MERGE (p)-[:ASSIGNED_TO]->(m)", 
                    new { pId = penaltyId, pDesc = penaltyDescription, mId = memberId });
            });
        }

        public async Task LinkResourceToGymAsync(int resourceId, int gymId, string resourceName = null, string gymName = null)
        {
            using var session = _context.GetSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(@"
                    MERGE (r:Resource {id: $rId})
                    ON CREATE SET r.name = $rName
                    MERGE (g:Gym {id: $gId})
                    ON CREATE SET g.name = $gName
                    MERGE (r)-[:BELONGS_TO]->(g)", 
                    new { rId = resourceId.ToString(), rName = resourceName, gId = gymId.ToString(), gName = gymName });
            });
        }

        public async Task LinkCardToGymAsync(string cardId, int gymId)
        {
            using var session = _context.GetSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(@"
                    MERGE (c:Card {id: $cId})
                    MERGE (g:Gym {id: $gId})
                    MERGE (c)-[:VALID_IN]->(g)", 
                    new { cId = cardId, gId = gymId.ToString() });
            });
        }
    }
}
