using FlexFit.Application.Queries;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FlexFit.Application.Handlers
{
    public class GetAllFitnessObjectsQueryHandler : IRequestHandler<GetAllFitnessObjectsQuery, IEnumerable<FitnessObject>>
    {
        private readonly IUnitOfWork _uow;

        public GetAllFitnessObjectsQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<FitnessObject>> Handle(GetAllFitnessObjectsQuery request, CancellationToken cancellationToken)
        {
            return await _uow.FitnessObjects.GetAllAsync(request.SearchTerm, request.City);
        }
    }

    public class GetFitnessObjectByIdQueryHandler : IRequestHandler<GetFitnessObjectByIdQuery, FitnessObject>
    {
        private readonly IUnitOfWork _uow;

        public GetFitnessObjectByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FitnessObject> Handle(GetFitnessObjectByIdQuery request, CancellationToken cancellationToken)
        {
            return await _uow.FitnessObjects.GetByIdAsync(request.Id);
        }
    }
}
