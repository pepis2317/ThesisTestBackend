using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Image
{
    public class GetImagesRequest : IRequest<(ProblemDetails?, List<string>?)>
    {
        public Guid ContentId { get; set; }
    }
}
