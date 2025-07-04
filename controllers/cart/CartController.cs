using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using foodOrderingApp.interfaces;
using foodOrderingApp.middlewares;
using foodOrderingApp.models.Dtos;
using foodOrderingApp.models.requists;
using foodOrderingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;



namespace foodOrderingApp.controllers.cart
{
    [ApiController]
    [Route("/api/cart")]
    public class CartController:ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            
        }
        [Authorize]
        [HttpPost]
        public ActionResult AddToCart(CartDto cart){
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Cart data", errors = ModelState });
            }
            Guid userId = HttpContext.User.GetUserIdFromClaims();
            Console.WriteLine(JsonConvert.SerializeObject(cart, Formatting.Indented));
            string msg = _cartRepository.Add(userId,cart);
                return  Ok(new ApiResponse(true ,msg));

        }

        [Authorize]
        [HttpGet]
        public ActionResult GetUserCart(){
            Guid userId = HttpContext.User.GetUserIdFromClaims();

            var cart = _cartRepository.GetUserCart(userId);
            return Ok(new ApiResponse<object>(true, cart, "Cart fetched successfully"));

        }
        [Authorize]
        [HttpDelete]
        public ActionResult DeleteCartItems([FromBody] List<Guid> cartItemIds)
        {
            
            Guid userId = HttpContext.User.GetUserIdFromClaims();
            // var itemIds = JsonSerializer.Deserialize<List<Guid>>(cartItemIds);
            // Console.WriteLine(JsonSerializer.Serialize<Guid>(cartItemIds));
            var msg = _cartRepository.RemoveItemsFromCart(userId,cartItemIds);
            return Ok(new ApiResponse(true, msg));

        }

    }
}