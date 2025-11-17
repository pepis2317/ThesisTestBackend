using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class CheckReviewUser: IRequest<(ProblemDetails?, bool?)>
    {
        public Guid UserId { get; set; }
        public Guid AuthorId {  get; set; }
    }
}
