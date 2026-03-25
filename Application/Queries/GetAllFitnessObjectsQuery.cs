using FlexFit.Domain.Models;
using MediatR;
using System.Collections.Generic;

namespace FlexFit.Application.Queries
{
    public class GetAllFitnessObjectsQuery : IRequest<IEnumerable<FitnessObject>>
    {
        public string SearchTerm { get; set; }
        public string City { get; set; }

        public GetAllFitnessObjectsQuery(string searchTerm = null, string city = null)
        {
            SearchTerm = searchTerm;
            City = city;
        }
    }
}
