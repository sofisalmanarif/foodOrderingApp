using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using foodOrderingApp.interfaces;
using foodOrderingApp.middlewares;
using foodOrderingApp.models;
using foodOrderingApp.Models;
using foodOrderingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodOrderingApp.controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(OrderDto newOrder)
        {

            Guid userId = HttpContext.User.GetUserIdFromClaims();


            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Order data", errors = ModelState });

            }

            var order = await _orderRepository.Add(newOrder, userId);

            return Ok(new ApiResponse<Order>(true, order, "Order Placed Sucessfully"));


        }

        [Authorize]
        [Authorize(Roles = "Owner")]
        [HttpGet]

        public ActionResult GetOrders()
        {
            Guid ownerId = HttpContext.User.GetUserIdFromClaims();
            var orders = _orderRepository.GetOrders(ownerId);
            return Ok(new ApiResponse<IEnumerable<Order>>(true, orders));
        }


        [Authorize]
        [Authorize(Roles = "Owner")]
        [HttpGet("accepted-orders")]

        public ActionResult GetAcceptedOrders()
        {
           Guid ownerId = HttpContext.User.GetUserIdFromClaims();

            // if (!Guid.TryParse(userId, out Guid ownerId))
            // {
            //     throw new AppException("Invalid Id Format", HttpStatusCode.BadRequest);
            // }
            var orders = _orderRepository.GetAcceptedOrders(ownerId);
            return Ok(new ApiResponse<IEnumerable<Order>>(true, orders));
        }

        [Authorize(Roles ="Owner")]
        [HttpPatch("{id}")]
        public async Task<ActionResult> ProcessOrder(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new AppException("No OrderId passed in params", HttpStatusCode.BadRequest);
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                throw new AppException("Please provide id in Correct Format", HttpStatusCode.BadRequest);

            }

            string msg = await _orderRepository.ProcessOrder(orderId);

            return Ok(new ApiResponse(true, msg));
        }


        [Authorize(Roles = "Owner")]
        [HttpGet("{id}")]
        public ActionResult OrderDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new AppException("No OrderId passed in params", HttpStatusCode.BadRequest);
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                throw new AppException("Please provide id in Correct Format", HttpStatusCode.BadRequest);

            }

            var orderDetails = _orderRepository.OrderDetails(orderId);

            return Ok(new ApiResponse<Object>(true, orderDetails));
        }

        [Authorize]
        [HttpGet("my-orders")]
        public ActionResult GetMyOrders()
        {
            Guid userId = HttpContext.User.GetUserIdFromClaims();
            var orders = _orderRepository.MyOrders(userId);

            return Ok(new ApiResponse<object>(true, orders));
        }

        [Authorize]
        [HttpGet("user-order-history")]
        public ActionResult GetUsersOrderHistory(){
            Guid userId = HttpContext.User.GetUserIdFromClaims();

            object orders = _orderRepository.UserOrderHistory(userId);

            return Ok(new ApiResponse<object>(true,orders));

        }

        [Authorize(Roles = "Owner")]
        [HttpGet("restaurant-order-history")]
        public ActionResult GetRestaurantsOrderHistory()
        {
            Guid ownerId = HttpContext.User.GetUserIdFromClaims();

            object orders = _orderRepository.RestaurnatOrderHistory(ownerId);

            return Ok(new ApiResponse<object>(true, orders));

        }

    }
}