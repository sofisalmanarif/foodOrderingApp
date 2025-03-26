using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using foodOrderingApp.Models;
using foodOrderingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodOrderingApp.controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController:ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        [Authorize]
        [HttpPost]
        public ActionResult Create(OrderDto newOrder){

            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Invalid token or user ID not found."));
            }

            Console.WriteLine("userid {0}", userIdClaim.Value);

           if( Guid.TryParse(userIdClaim.Value,out Guid userId)){
            throw new AppException("Invalid Id Format",HttpStatusCode.BadRequest);
           }

           if(!ModelState.IsValid){
                return BadRequest(new { message = "Invalid Order data", errors = ModelState });

            }

           var order =  _orderRepository.Add(newOrder,userId);

           return Ok(new ApiResponse<Order>(true,order,"Order Placed Sucessfully"));


        }
        
    }
}