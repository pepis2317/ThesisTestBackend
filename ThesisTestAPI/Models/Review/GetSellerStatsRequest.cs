using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class GetSellerStatsRequest:IRequest<(ProblemDetails?, SellerStatsResponse?)>
    {
        public Guid SellerId {  get; set; }
    }
}
