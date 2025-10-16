﻿using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RTools_NTS.Util;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Biteship;
using ThesisTestAPI.Models.OrderRequests;

namespace ThesisTestAPI.Handlers.Biteship
{
    public class CreateShipmentByPostalCodeHandler : IRequestHandler<CreateShipmentByPostalCodeRequest, (ProblemDetails?, OrderCreatedResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BiteshipOptions _opt;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        public CreateShipmentByPostalCodeHandler(HttpClient httpClient,ThesisDbContext db, IOptions<BiteshipOptions> opt, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _opt = opt.Value;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }
        private ProblemDetails ProblemDetailTemplate(string detail)
        {
            return new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Biteship error",
                Detail = detail,
                Instance = _httpContextAccessor.HttpContext?.Request.Path
            };
        }
        public async Task<(ProblemDetails?, OrderCreatedResponse?)> Handle(CreateShipmentByPostalCodeRequest request, CancellationToken cancellationToken)
        {
            var sender = await _db.Users.Where(q => q.UserId == request.OriginUserId).FirstOrDefaultAsync();
            if (sender == null)
            {
                return (ProblemDetailTemplate("Invalid sender"), null);
            }
            var receiver = await _db.Users.Where(q => q.UserId == request.DestinationUserId).FirstOrDefaultAsync();
            if (receiver == null)
            {
                return (ProblemDetailTemplate("Invalid receiver"), null);
            }
            if (string.IsNullOrEmpty(sender.Address) || sender.PostalCode == null)
            {
                return (ProblemDetailTemplate("Sender lacks address or postal code"), null);
            }
            if(string.IsNullOrEmpty(receiver.Address) || receiver.PostalCode == null)
            {
                return (ProblemDetailTemplate("Receiver lacks address or postal code"), null);
            }
            var orderRequest = new BiteshipOrderBody
            {
                origin_contact_name = sender.UserName,
                origin_contact_phone = sender.Phone,
                origin_contact_email = sender.Email,
                origin_address = sender.Address,
                origin_note = request.OriginNote,
                origin_postal_code = (int)sender.PostalCode,
                destination_contact_name = receiver.UserName,
                destination_contact_phone = receiver.Phone,
                destination_contact_email = receiver.Email,
                destination_address = receiver.Address,
                destination_note = request.DestinationNote,
                destination_postal_code= (int)receiver.PostalCode,
                delivery_type = request.DeliveryType,
                order_note = request.OrderNote,
            };
            foreach(var item in request.Items)
            {
                var orderItem = new BiteshipItem
                {
                    name = item.Name,
                    description = item.Description,
                    value = item.Value,
                    quantity = item.Quantity,
                    height = item.Height,
                    length = item.Length,
                    weight = item.Weight,
                    width = item.Width
                };
                orderRequest.items.Add(orderItem);
            }
            var json = System.Text.Json.JsonSerializer.Serialize(orderRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue(_opt.ApiKey);
            var response = await _httpClient.PostAsync("https://api.biteship.com/v1/orders", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return (null, new OrderCreatedResponse
                {
                    Result = "Failed",
                    Response = System.Text.Json.JsonSerializer.Serialize(body)
                });
            }
            return (null, new OrderCreatedResponse
            {
                Result = "Success",
                Response = System.Text.Json.JsonSerializer.Serialize(body)
            });
        }
    }
}
