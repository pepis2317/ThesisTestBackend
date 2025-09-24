using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Comment
{
    public class GetCommentsRequest: IRequest<(ProblemDetails?, PaginatedCommentsResponse?)>
    {
        public Guid ContentId { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
