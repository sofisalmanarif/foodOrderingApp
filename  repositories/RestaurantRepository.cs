using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace foodOrderingApp.reprositries
{
    public class RestaurantRepository : IResturantRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        public RestaurantRepository(AppDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository =userRepository;
        }
        public Restaurant Add(Restaurant restaurant)
        {   
            if(restaurant==null){
                throw new ArgumentNullException(nameof(restaurant), "Restaurant cannot be null.");
            }
            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();
            return restaurant;
        }

        public string Delete(Guid restaurantId)
        {
           var restaurant =  _context.Restaurants.Find(restaurantId);

           if (restaurant==null){
                throw new KeyNotFoundException("No Resturant with this id Found");
            }
            _context.Restaurants.Remove(restaurant);
            _userRepository.Delete(restaurant.OwnerId);
            _context.SaveChanges();
            return $"{restaurant.RestaurantName} Deleted Sucessfully";
            
        }

        public IEnumerable<Restaurant> GelAllNotVerified()
        {
            return _context.Restaurants.Where(r => r.IsVerified == false);
        }

        public IEnumerable<Restaurant> GetAll()
        {
            return _context.Restaurants.Where(r => r.IsVerified == true);
        }

        public object? GetById(Guid id)
        {
            var restaurant = _context.Restaurants
                .Include(r => r.Owner)
                    .ThenInclude(o => o.Address)
                .FirstOrDefault(r => r.Id == id);

            if (restaurant == null) return null;

            return new
            {
                restaurant.Id,
                restaurant.RestaurantName,
                restaurant.Description,
                restaurant.ValidDocument,
                restaurant.Owner.Phone,
                Owner = new
                {
                    restaurant.Owner.Id,
                    restaurant.Owner.Name,
                    restaurant.Owner.Email,
                    restaurant.Owner.Phone,

                    Address = restaurant.Owner.Address == null ? null : new
                    {
                        restaurant.Owner.Address.AddressType,
                        restaurant.Owner.Address.City,
                        restaurant.Owner.Address.Landmark,
                        restaurant.Owner.Address.ShopNumber,
                        restaurant.Owner.Address.Floor,


                    }
                }
            };
        }



        public string ToggleOrders(Guid ownerId)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r=>r.OwnerId ==ownerId);
            if (restaurant == null)
            {
                throw new KeyNotFoundException("No Resturant with this id Found");
            }
            
            restaurant.IsActive = !restaurant.IsActive;
            _context.SaveChanges();
            return restaurant.IsActive?" Restaurant is Accepting  Orders": " Restaurant is Not Accepting  Orders";
        }

        public string Update(Restaurant restaurant)
        {
            throw new NotImplementedException();
        }

        public string Verify(Guid id)
        {
            var restaurant = _context.Restaurants.Find(id);
            if(restaurant==null){
                throw new KeyNotFoundException("No Resturant with this id Found");
            }
            if(restaurant.IsVerified){
                return "Restaurnat is Already Verified";
            }
            restaurant.IsVerified = true;
            _context.SaveChanges();
            return $"{restaurant.RestaurantName} Restaurant Verified Sucessfully";
        }

        public IEnumerable<Restaurant> GetAllRestaurantsByCategory(Guid categoryId)
        {
            return _context.Restaurants
        .Include(r => r.MenuItems)
        .Where(r => r.MenuItems != null && r.MenuItems.Any(m => m.CategoryId == categoryId))
        .ToList();
        }

    }
}