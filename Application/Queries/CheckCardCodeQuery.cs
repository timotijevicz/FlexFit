using MediatR;

namespace FlexFit.Application.Queries
{
    public class CheckCardCodeQuery : IRequest<bool>
    {
        public string Code { get; set; }
        public CheckCardCodeQuery(string code) => Code = code;
    }
}
