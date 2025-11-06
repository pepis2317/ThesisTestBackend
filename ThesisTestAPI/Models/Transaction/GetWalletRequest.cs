using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Transaction
{
    public class GetWalletRequest : IRequest<(ProblemDetails?, long?)>
    {
        public Guid UserId { get; set; }
    }
}
