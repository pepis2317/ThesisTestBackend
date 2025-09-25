using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Producer
{
    public class SellerQuery : IRequest<(ProblemDetails?, PaginatedSellersResponse?)>
    {
        public string? searchTerm {  get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
