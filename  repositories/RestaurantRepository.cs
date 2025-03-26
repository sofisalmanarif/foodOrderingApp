using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using Microsoft.EntityFrameworkCore;

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

        public Restaurant? GetById(Guid id)
        {
            return _context.Restaurants.FirstOrDefault(r=>r.Id==id);

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