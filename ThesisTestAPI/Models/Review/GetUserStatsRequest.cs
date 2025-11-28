using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class GetUserStatsRequest: IRequest<(ProblemDetails?, UserStatsResponse)>
    {
        public Guid UserId {  get; set; }
    }
}
