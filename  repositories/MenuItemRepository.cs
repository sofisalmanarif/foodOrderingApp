using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace foodOrderingApp.repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly AppDbContext _context;
        // private readonly IResturantRepository _restaurantRepository;
        public MenuItemRepository(AppDbContext context)
        {
            _context = context;
            // _restaurantRepository = restaurantRepository;
        }
        public MenuItem Add(MenuItemDto newMenuItem)
        {
            if (newMenuItem == null)
            {
                throw new ArgumentNullException(nameof(newMenuItem), "Menu Item cannot be null.");
            }

            var restaurant = _context.Restaurants
                                      .FirstOrDefault(r => r.Id == newMenuItem.RestaurantId);
            if (restaurant == null)
            {
                throw new KeyNotFoundException($"Restaurant with ID {newMenuItem.RestaurantId} not found.");
            }

            bool isItemExists = _context.MenuItems
                                         .Any(item => item.Name == newMenuItem.Name &&
                                                      item.RestaurantId == restaurant.Id);

            if (isItemExists)
            {
                throw new InvalidOperationException("Item already exists in the menu.");
            }

            // Create MenuItem
            MenuItem menuItem = new MenuItem
            {
                RestaurantId = newMenuItem.RestaurantId,
                Name = newMenuItem.Name,
                Description = newMenuItem.Description,
                CategoryId = newMenuItem.Category,
                ImageUrl = newMenuItem.ImageUrl,
                IsCustomizable = newMenuItem.IsCustomizable,
                Price = !newMenuItem.IsCustomizable ? newMenuItem.Price : (newMenuItem.Variants != null && newMenuItem.Variants.Any()
        ? newMenuItem.Variants[0].Price
        : newMenuItem.Price),
                Variants = null
            };

            _context.MenuItems.Add(menuItem);
            _context.SaveChanges();

            //  if Customizable
            if (newMenuItem.IsCustomizable && newMenuItem.Variants != null)
            {
                var variants = newMenuItem.Variants.Select(v => new MenuItemVarient
                {
                    MenuItemId = menuItem.Id,
                    Size = v.Size,
                    Price = v.Price,
                    IsAvailable = v.IsAvailable,
                }).ToList();

                _context.MenuItemVarients.AddRange(variants);
                _context.SaveChanges();
            }

            return menuItem;
        }

        public string Delete(Guid id, Guid ownerId)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerId == ownerId);
            if (restaurant == null)
            {
                throw new KeyNotFoundException("Resturant  Not Found");
            }
            var item = _context.MenuItems.FirstOrDefault(i => i.Id == id && i.RestaurantId == restaurant.Id);
            if (item == null)
            {
                throw new KeyNotFoundException("item Not Found");
            }
            _context.MenuItems.Remove(item);
            _context.SaveChanges();
            return $"{item.Name} Removed From {restaurant.RestaurantName} Menu";


        }

        public IEnumerable<object> GetAll(Guid restaurantId)
        {
            var menu = _context.MenuItems
                .Where(item => item.RestaurantId == restaurantId)
                .Include(item => item.Category)
                .GroupBy(item => item.Category.Name)
                .Select(group => new
                {
                    CategoryName = group.Key,
                    Items = group.ToList()
                })
                .ToList();

            return menu;
        }

        public MenuItem? GetById(Guid id)
        {
            var menuItem = _context.MenuItems.Include(item => item.Variants).FirstOrDefault(item => item.Id == id);
            if (menuItem == null)
            {
                throw new KeyNotFoundException("Invalid item id");
            }
            return menuItem;
        }

        public string IsAvailable(Guid id)
        {
            var item = _context.MenuItems.Find(id);
            if (item == null)
            {
                throw new KeyNotFoundException("Item not Found");
            }
            // item.IsAvailable = !item.IsAvailable;
            _context.SaveChanges();
            return $"{item.Name} ";
        }

        public MenuItem Update(MenuItem menuItem)
        {
            Console.WriteLine(menuItem.Name);
            if (menuItem == null)
            {
                throw new ArgumentNullException(nameof(menuItem), "Item Can't be null");
            }
            //    Restaurant? restaurant = _restaurantRepository.GetById(menuItem.RestaurantId);
            //    res.Menu.Id
            var existingMenuItem = _context.MenuItems.Include(item => item.Variants).FirstOrDefault(item => item.Id == menuItem.Id);
            if (existingMenuItem == null)
            {
                throw new KeyNotFoundException("Item Not Found");
            }
            existingMenuItem.Name = menuItem.Name;
            existingMenuItem.Price = menuItem.Price;
            // existingMenuItem.IsAvailable = menuItem.IsAvailable;
            existingMenuItem.Description = menuItem.Description;
            existingMenuItem.RestaurantId = menuItem.RestaurantId;
            existingMenuItem.CategoryId = menuItem.CategoryId;

            _context.MenuItems.Update(existingMenuItem);
            _context.SaveChanges();
            return existingMenuItem;
        }

        public string UpdateVarient(MenuItemVarient varient)
        {
            var existingVarient = _context.MenuItemVarients.FirstOrDefault(v=>v.Id == varient.Id);

            if(existingVarient == null){
                throw new KeyNotFoundException("Varient not Found");
            }
            existingVarient.Id = varient.Id;
            existingVarient.MenuItemId = varient.MenuItemId;
            existingVarient.Size = varient.Size;
            existingVarient.Price = varient.Price;
            existingVarient.IsAvailable = varient.IsAvailable;

            _context.Update(existingVarient);
            _context.SaveChanges();
            return "Update Sucessfull";

        }
    }
}