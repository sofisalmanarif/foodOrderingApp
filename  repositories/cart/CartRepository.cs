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
            var isVAlidItemId =_context.MenuItems.FirstOrDefault(i=>i.Id==cartDto.ItemId);
            if(isVAlidItemId==null){
                return "Invlid item data";
            }

            if(cartDto.VariantId !=null){
                var isDateValid = _context.MenuItemVarients.Where(mi => mi.MenuItemId == cartDto.ItemId && mi.Id == cartDto.VariantId).Any();
                if (!isDateValid)
                {
                    return "Invlid varient id";
                }
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

        public object GetUserCart(Guid userId)
        {
            return _context.CartItems
                .Where(ci => ci.Cart.UserId == userId)
                .Select(ci => new
                {   Id = ci.Id,
                    ItemId = ci.ItemId,
                    ItemName = ci.Item.Name,
                    ImageUrl = ci.Item.ImageUrl,
                    Quantity = ci.Quantity,
                    VariantId = ci.VariantId ?? null,
                    IsAvailable = ci.Item.IsAvailable,
                    Price = ci.VariantId != null ? (ci.Variant != null ? ci.Variant.Price : ci.Item.Price) : ci.Item.Price,

                    Variant = ci.VariantId != null && ci.Variant != null ? new
                    {
                        VariantId = ci.Variant.Id,
                        VariantName = ci.Variant.Size,
                        VariantPrice = ci.Variant.Price,
                        IsAvailable = ci.Variant.IsAvailable,
                    } : null
                })
                .ToList();

        }

        public string RemoveItemsFromCart(Guid userId, List<Guid> itemIds)
        {
            // First, find the cart ID for this user
            var cart = _context.Carts
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                return "No cart found for this user.";
            }

            var itemsToRemove = _context.CartItems
                .Where(ci => ci.CartId == cart.Id && itemIds.Contains(ci.Id))
                .ToList();

            if (!itemsToRemove.Any())
            {
                return "No matching items found in the cart.";
            }

            _context.CartItems.RemoveRange(itemsToRemove);
            _context.SaveChanges();
            return "Items removed from the cart successfully.";
        }
    }
}