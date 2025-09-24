using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Producer;

namespace ThesisTestAPI.Models.Post
{
    public class GetProducerFromOwnerIdRequest : IRequest<(ProblemDetails?, ProducerResponse?)>
    {
        public Guid OwnerId {  get; set; }
    }
}
