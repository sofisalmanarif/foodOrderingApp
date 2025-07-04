using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using foodOrderingApp.services.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace foodOrderingApp.reprositries
{
    public class RestaurantRepository : IResturantRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _casheService;
        public RestaurantRepository(AppDbContext context, IUserRepository userRepository, ICacheService casheService)
        {
            _context = context;
            _userRepository = userRepository;
            _casheService = casheService;
        }
        public Restaurant Add(Restaurant restaurant)
        {
            if (restaurant == null)
            {
                throw new ArgumentNullException(nameof(restaurant), "Restaurant cannot be null.");
            }
            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();
            return restaurant;
        }

        public string Delete(Guid restaurantId)
        {
            var restaurant = _context.Restaurants.Find(restaurantId);

            if (restaurant == null)
            {
                throw new KeyNotFoundException("No Resturant with this id Found");
            }
            _context.Restaurants.Remove(restaurant);
            _userRepository.Delete(restaurant.OwnerId);
            _context.SaveChanges();
            return $"{restaurant.RestaurantName} Deleted Sucessfully";

        }

        public IEnumerable<Restaurant> GelAllNotVerified(int pageSize, int pageNumber)
        {
            var restaurants = _casheService.Get<IEnumerable<Restaurant>>("unverified-restaurants");
            if (restaurants != null && restaurants.Any())
            {
                return restaurants;
            }

            restaurants = _context.Restaurants
                .Where(r => !r.IsVerified).Skip(pageSize * (pageNumber - 1)).Take(pageSize);

            TimeSpan ttl = TimeSpan.FromMinutes(30);
            _casheService.Set("unverified-restaurants", restaurants, ttl);
            return restaurants;
        }

        public IEnumerable<Restaurant> GetAll(int pageSize, int pageNumber)
        {

            var restaurants = _casheService.Get<IEnumerable<Restaurant>>("verified-restaurants");
            if (restaurants != null && restaurants.Any())
            {
                Console.WriteLine("Got from Cahce");
                return restaurants;
            }
            restaurants =  _context.Restaurants.Where(r => r.IsVerified == true ).Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            TimeSpan ttl = TimeSpan.FromMinutes(30);
            _casheService.Set<IEnumerable<Restaurant>>("verified-restaurants", restaurants, ttl);
            return restaurants;


        }

        public object? GetById(Guid id)
        {
            var restaurant = _context.Restaurants
                .Include(r => r.Owner)  // Include the Owner
                .ThenInclude(owner => owner.Addresses)  // Include the Addresses related to the Owner
                .FirstOrDefault(r => r.Id == id);

            if (restaurant == null) return null;

            // Filter the addresses to get the one with AddressType = "Restaurant"
            var restaurantAddress = restaurant.Owner.Addresses?
                .FirstOrDefault(address => address.AddressType == AddressType.Restaurant);

            return new
            {
                restaurant.Id,
                restaurant.RestaurantName,
                restaurant.Description,
                restaurant.ValidDocument,
                restaurant.Owner.Phone,
                restaurant.ImageUrl,
                restaurant.IsActive,
                Owner = new
                {
                    restaurant.Owner.Name,
                    restaurant.Owner.Email,
                    restaurant.Owner.Phone,

                    // Include the address with AddressType "Restaurant" (if it exists)
                    Address = restaurantAddress == null ? null : new
                    {
                        restaurantAddress.AddressType,
                        restaurantAddress.City,
                        restaurantAddress.Landmark,
                        restaurantAddress.ShopNumber,
                        restaurantAddress.Floor
                    }
                }
            };
        }





        public string ToggleOrders(Guid ownerId)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerId == ownerId);
            if (restaurant == null)
            {
                throw new KeyNotFoundException("No Resturant with this id Found");
            }

            restaurant.IsActive = !restaurant.IsActive;
            _context.SaveChanges();
            return restaurant.IsActive ? " Restaurant is Accepting  Orders" : " Restaurant is Not Accepting  Orders";
        }

        public string Update(Restaurant restaurant)
        {
            throw new NotImplementedException();
        }

        public string Verify(Guid id)
        {
            var restaurant = _context.Restaurants.Find(id);
            if (restaurant == null)
            {
                throw new KeyNotFoundException("No Resturant with this id Found");
            }
            if (restaurant.IsVerified)
            {
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