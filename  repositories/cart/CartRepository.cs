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
        public string Add(Guid userId, CartDto cartDto)
        {
            // Validate the ItemId
            var isValidItemId = _context.MenuItems.FirstOrDefault(i => i.Id == cartDto.ItemId);
            if (isValidItemId == null)
            {
                return "Invalid item data";
            }

            // Validate the VariantId if provided
            if (cartDto.VariantId != null)
            {
                var isVariantValid = _context.MenuItemVarients
                    .Where(mi => mi.MenuItemId == cartDto.ItemId && mi.Id == cartDto.VariantId)
                    .Any();

                if (!isVariantValid)
                {
                    return "Invalid variant id";
                }
            }

            // Check if the user already has a cart
            var existingUserCart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            if (existingUserCart != null)
            {
                // Check if the item with the variant already exists in the cart
                var existingItem = existingUserCart.CartItems
                    .FirstOrDefault(ci => ci.ItemId == cartDto.ItemId && ci.VariantId == cartDto.VariantId);

                if (existingItem != null)
                {
                    // If the item exists, update the quantity (if quantity is positive, update it, else decrement)
                    if (cartDto.Quantity > 0)
                    {
                        existingItem.Quantity += cartDto.Quantity;
                    }
                    else if (cartDto.Quantity == -1 && existingItem.Quantity > 1)
                    {
                        existingItem.Quantity -= 1;
                    }
                    else if (cartDto.Quantity == -1 && existingItem.Quantity == 1)
                    {
                        _context.CartItems.Remove(existingItem);
                    }
                    // else
                    // {
                    //     _context.CartItems.Remove(existingItem); //remove item from cart i
                    // }

                    _context.CartItems.Update(existingItem);
                }
                else
                {
                    // If the item does not exist, create a new cart item
                    var newCartItem = new CartItem
                    {
                        ItemId = cartDto.ItemId,
                        VariantId = cartDto.VariantId,
                        Quantity = cartDto.Quantity > 0 ? cartDto.Quantity : 1, // Default to 1 if quantity is not provided
                        CartId = existingUserCart.Id
                    };

                    _context.CartItems.Add(newCartItem);
                }
            }
            else
            {
                // If the user does not have a cart, create a new cart
                var newCart = new Cart
                {
                    UserId = userId,
                };

                _context.Carts.Add(newCart);
                _context.SaveChanges(); // Save to generate the Cart ID

                // Now add the cart item with the proper cart ID
                var newCartItem = new CartItem
                {
                    ItemId = cartDto.ItemId,
                    VariantId = cartDto.VariantId,
                    Quantity = cartDto.Quantity > 0 ? cartDto.Quantity : 1, // Default to 1 if quantity is not provided
                    CartId = newCart.Id
                };

                _context.CartItems.Add(newCartItem);
            }

            // Save changes to the database
            _context.SaveChanges();
            return "Item Added/Updated Successfully";
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