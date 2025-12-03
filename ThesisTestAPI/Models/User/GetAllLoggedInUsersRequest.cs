using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.User
{
    public class GetAllLoggedInUsersRequest:IRequest<(ProblemDetails?, PaginatedUserResponse?)>
    {
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
