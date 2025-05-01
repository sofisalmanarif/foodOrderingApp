using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cashfree_pg.Client;
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
        public ActionResult AddToCart(CartRequist cartRequist){
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Cart data", errors = ModelState });
            }
            Console.WriteLine(JsonConvert.SerializeObject(cartRequist, Formatting.Indented));

            Guid userId = HttpContext.User.GetUserIdFromClaims();
            CartDto cartDto = new CartDto(){
                CartId=cartRequist.CartId,
                UserId=userId,
                ItemId = cartRequist.ItemId,
                VariantId = cartRequist.VariantId,
                Quantity = cartRequist.Quantity,
            };

        string msg = _cartRepository.Add(cartDto);



            return  Ok(new ApiResponse(true ,msg));



        }

    }
}