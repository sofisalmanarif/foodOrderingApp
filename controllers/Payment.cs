using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.interfaces;
using foodOrderingApp.middlewares;
using foodOrderingApp.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodOrderingApp.controllers
{
    [ApiController]
    [Route("/api/payment")]
    public class Payment:ControllerBase
    {
        private readonly IPayment _payment;
        public Payment(IPayment payment)
        {

            _payment = payment;
            
        }
        
        [Authorize]
        [HttpPost]
        public ActionResult Pay([FromBody] PaymentDto payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid payment data", errors = ModelState });
            }

            Guid userId = HttpContext.User.GetUserIdFromClaims();
            double  amount = (double)payment.Amount;
            var result = _payment.CreateOrder(amount, userId);

            return Ok(new 
            {
                success = true,
                paymentSessionId = result,
                 amount,
                currency = "INR",
                timestamp = DateTime.UtcNow
            });
        }

    }
}