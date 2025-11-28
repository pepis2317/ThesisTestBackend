using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class GetUserReviewsRequest:IRequest<(ProblemDetails?, PaginatedReviewsResponse?)>
    {
        public Guid UserId { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
