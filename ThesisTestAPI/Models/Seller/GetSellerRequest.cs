using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;

namespace ThesisTestAPI.Models.Producer
{
    public class GetSellerRequest : IRequest<(ProblemDetails?, SellerResponse?)>
    {
        public Guid SellerId { get; set; }
    }
}
