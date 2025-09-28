using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Producer;

namespace ThesisTestAPI.Models.Seller
{
    public class GetSellerFromOwnerIdRequest : IRequest<(ProblemDetails?, SellerResponse?)>
    {
        public Guid OwnerId {  get; set; }
    }
}
