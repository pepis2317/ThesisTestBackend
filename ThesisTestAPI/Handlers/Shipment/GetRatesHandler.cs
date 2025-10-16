using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Shipment;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Shipment
{
    public class GetRatesHandler : IRequestHandler<GetRatesRequest, (ProblemDetails?, BiteshipRatesResponse?)>
    {
        private readonly BiteshipService _biteshipService;
        public GetRatesHandler(BiteshipService biteshipService)
        {
            _biteshipService = biteshipService;
        }
        public async Task<(ProblemDetails?, BiteshipRatesResponse?)> Handle(GetRatesRequest request, CancellationToken cancellationToken)
        {
            var ratesResponse = await _biteshipService.GetRates(request.ShipmentId, request.Couriers);
            return (null, ratesResponse);
        }
    }
}
