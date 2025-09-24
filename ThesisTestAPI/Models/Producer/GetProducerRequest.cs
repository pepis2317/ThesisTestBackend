using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;

namespace ThesisTestAPI.Models.Producer
{
    public class GetProducerRequest : IRequest<(ProblemDetails?, ProducerResponse?)>
    {
        public Guid ProducerId { get; set; }
    }
}
