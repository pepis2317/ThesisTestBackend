using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Biteship;
using ThesisTestAPI.Models.Shipment;

namespace ThesisTestAPI.Services
{
    public class BiteshipService
    {
        private readonly ThesisDbContext _db;
        private readonly HttpClient _httpClient;
        private readonly BiteshipOptions _opt;
        public BiteshipService(ThesisDbContext db, IOptions<BiteshipOptions> opt, HttpClient httpClient)
        {
            _db = db;
            _opt = opt.Value;
            _httpClient = httpClient;
        }

        public async Task<BiteshipOrderResponse?> CreateOrder(Guid shipmentId, string originNote, string destinationNote, string deliveryType, string courierCompany, string courierType, string orderNote)
        {
            var shipment = await _db.Shipments.Include(q=>q.Process).ThenInclude(q=>q.Request).Where(q => q.ShipmentId == shipmentId).FirstOrDefaultAsync();
            if(shipment == null)
            {
                return null;
            }
            var senderId = await _db.Sellers.Where(q => q.SellerId == shipment.Process.Request.SellerId).Select(q => q.OwnerId).FirstOrDefaultAsync();
            var sender = await _db.Users.Where(q => q.UserId == senderId).FirstOrDefaultAsync();
            if (sender == null || sender.PostalCode == null || sender.Address == null)
            {
                return null;
            }
            var receiverId = await _db.Contents.Where(q => q.ContentId == shipment.Process.Request.RequestId).Select(q => q.AuthorId).FirstOrDefaultAsync();
            var receiver = await _db.Users.Where(q => q.UserId == receiverId).FirstOrDefaultAsync();
            if (receiver == null || receiver.PostalCode == null || receiver.Address == null)
            {
                return null;
            }
            var orderRequest = new BiteshipOrderBody
            {
                origin_contact_name = sender.UserName,
                origin_contact_phone = sender.Phone,
                origin_contact_email = sender.Email,
                origin_address = sender.Address,
                origin_note = originNote,
                origin_postal_code = (int)sender.PostalCode,
                destination_contact_name = receiver.UserName,
                destination_contact_phone = receiver.Phone,
                destination_contact_email = receiver.Email,
                destination_address = receiver.Address,
                destination_note = destinationNote,
                destination_postal_code = (int)receiver.PostalCode,
                delivery_type = deliveryType,
                courier_company = courierCompany,
                courier_type = courierType,
                order_note = orderNote,
            };
            orderRequest.items.Add(new BiteshipItem
            {
                name = shipment.Name,
                description = string.IsNullOrEmpty(shipment.Description) ? "" : shipment.Description,
                value = shipment.Value / 100,
                quantity = shipment.Quantity,
                height = shipment.Height,
                length = shipment.Length,
                weight = shipment.Weight,
                width = shipment.Width,
            });
            var json = System.Text.Json.JsonSerializer.Serialize(orderRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_opt.ApiKey);
            var response = await _httpClient.PostAsync("https://api.biteship.com/v1/orders", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var orderResponse = System.Text.Json.JsonSerializer.Deserialize<BiteshipOrderResponse>(body,new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return orderResponse;
        }
        public async Task<BiteshipRatesResponse?> GetRates(Guid shipmentId, string couriers)
        {
            var shipment = await _db.Shipments.Include(q => q.Process).ThenInclude(q => q.Request).Where(q => q.ShipmentId == shipmentId).FirstOrDefaultAsync();
            if (shipment == null)
            {
                return null;
            }
            var senderId = await _db.Sellers.Where(q => q.SellerId == shipment.Process.Request.SellerId).Select(q => q.OwnerId).FirstOrDefaultAsync();
            var sender = await _db.Users.Where(q => q.UserId == senderId).FirstOrDefaultAsync();
            if (sender == null || sender.PostalCode == null || sender.Address == null)
            {
                return null;
            }
            var receiverId = await _db.Contents.Where(q => q.ContentId == shipment.Process.Request.RequestId).Select(q => q.AuthorId).FirstOrDefaultAsync();
            var receiver = await _db.Users.Where(q => q.UserId == receiverId).FirstOrDefaultAsync();
            if (receiver == null || receiver.PostalCode == null || receiver.Address == null)
            {
                return null;
            }
            var ratesBody = new BiteshipRatesBody
            {
                origin_postal_code = (int)sender.PostalCode,
                destination_postal_code = (int)receiver.PostalCode,
                couriers = couriers,
            };
            ratesBody.items.Add(new Models.Biteship.BiteshipItem
            {
                name = shipment.Name,
                description = string.IsNullOrEmpty(shipment.Description) ? "" : shipment.Description,
                value = shipment.Value,
                quantity = shipment.Quantity,
                height = shipment.Height,
                length = shipment.Length,
                weight = shipment.Weight,
                width = shipment.Width,
            });
            var json = System.Text.Json.JsonSerializer.Serialize(ratesBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_opt.ApiKey);
            var response = await _httpClient.PostAsync("https://api.biteship.com/v1/rates/couriers", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var ratesResponse = System.Text.Json.JsonSerializer.Deserialize<BiteshipRatesResponse>(body,new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return ratesResponse;
        }
        public async Task<BiteshipTrackResponse?> Track(string orderId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_opt.ApiKey);
            var response = await _httpClient.GetAsync($"https://api.biteship.com/v1/orders/{orderId}");
            var body = await response.Content.ReadAsStringAsync();
            var trackResponse = System.Text.Json.JsonSerializer.Deserialize<BiteshipTrackResponse>(body,new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return trackResponse;
        }
    }
}
