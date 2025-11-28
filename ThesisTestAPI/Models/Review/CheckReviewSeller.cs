using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class CheckReviewSeller:IRequest<(ProblemDetails?, bool?)>
    {
        public Guid SellerId {  get; set; }
        public Guid AuthorId { get; set; }
    }
}
