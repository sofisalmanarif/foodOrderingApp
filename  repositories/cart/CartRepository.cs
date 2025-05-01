using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using foodOrderingApp.models.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace foodOrderingApp.repositories.cart
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        public CartRepository(AppDbContext context)
        {
            _context = context;


        }
        public string Add(CartDto cartDto)
        {
            if (cartDto.UserId == null)
            {
                return "User ID is required";
            }

            var existingUserCart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == cartDto.UserId);

            if (existingUserCart != null)
            {
                // Add new cart item to existing cart
                var newCartItem = new CartItem
                {
                    ItemId = cartDto.ItemId,
                    VariantId = cartDto.VariantId,
                    Quantity = cartDto.Quantity,
                    CartId = existingUserCart.Id
                };

                // Add the item directly to the context instead of through the collection
                _context.CartItems.Add(newCartItem);
            }
            else
            {
                // Create a new cart with the item
                var newCart = new Cart
                {
                    UserId = cartDto.UserId.Value,
                    // No need to initialize CartItems here since we're adding separately
                };

                _context.Carts.Add(newCart);
                _context.SaveChanges(); // Save to generate the Cart ID

                // Now add the cart item with the proper cart ID
                var newCartItem = new CartItem
                {
                    ItemId = cartDto.ItemId,
                    VariantId = cartDto.VariantId,
                    Quantity = cartDto.Quantity,
                    CartId = newCart.Id
                };

                _context.CartItems.Add(newCartItem);
            }

            _context.SaveChanges();
            return "Item Added Successfully";
        }

    }
}