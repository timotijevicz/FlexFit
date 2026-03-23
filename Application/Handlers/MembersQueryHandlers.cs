using FlexFit.Application.Queries;
using FlexFit.Models;
using FlexFit.UnitOfWorkLayer;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlexFit.Application.Handlers
{
    public class GetAllMembersQueryHandler : IRequestHandler<GetAllMembersQuery, IEnumerable<Member>>
    {
        private readonly IUnitOfWork _uow;

        public GetAllMembersQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<Member>> Handle(GetAllMembersQuery request, CancellationToken cancellationToken)
        {
            var all = await _uow.Members.GetAllAsync();
            return all.Where(m => m.Role == FlexFit.Models.Role.Member);
        }
    }
}
