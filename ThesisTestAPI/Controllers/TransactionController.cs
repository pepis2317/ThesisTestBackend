﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ThesisTestAPI.Models.Comment;
using ThesisTestAPI.Models.Midtrans;
using ThesisTestAPI.Models.Transaction;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }
        private ProblemDetails Invalid(string details)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Invalid Request Data",
                Detail = details,
                Instance = HttpContext.Request.Path
            };
            return problemDetails;
        }
        [HttpPost("transaction-test")]
        public async Task<IActionResult> TestTransaction(TestTransactionRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result.Item2);
        }
        [HttpPost("deposit-funds")]
        public async Task<IActionResult> DepositFunds([FromBody] long amount)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(Invalid("User id not found in JWT"));
            }
            var result = await _mediator.Send(new DepositRequest
            {
                UserId = Guid.Parse(userId),
                Amount = amount
            });
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPost("withdraw-funds")]
        public async Task<IActionResult> WithdrawFunds([FromBody] WithdrawRequest request)
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;

            //if (string.IsNullOrEmpty(userId))
            //{
            //    return BadRequest(Invalid("User id not found in JWT"));
            //}
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPost("/api/midtrans/notify")]
        public async Task<IActionResult> Notify([FromBody] MidtransNotification request)
        {
            var result = await _mediator.Send(request);
            return Ok(result.Item2);
        }
        [HttpPost("/api/midtrans/iris/notify")]
        public async Task<IActionResult> IrisNotify([FromBody] IrisPayoutNotification request)
        {
            var result = await _mediator.Send(request);
            return Ok(result.Item2);
        }
        [HttpPost("/api/approve-and-pay-step")]
        public async Task<IActionResult> ApproveAndPayForStep([FromBody] ApproveAndPayStepRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
    }
}
