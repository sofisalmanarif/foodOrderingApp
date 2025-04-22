
using System.Net;
using System.Security.Claims;
using foodOrderingApp.interfaces;
using foodOrderingApp.middlewares;
using foodOrderingApp.models;
using foodOrderingApp.Models;
using foodOrderingApp.services;
using foodOrderingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace foodOrderingApp.controllers
{
    [ApiController]
    [Route("api/menu")]
    public class MenuItems : ControllerBase
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuItems(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        [Consumes("multipart/form-data")]
        [HttpPost]
        public ActionResult Create([FromForm] MenuItemRequest newMenuItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Menu item data", errors = ModelState });
            }

            if (newMenuItem.Photo == null)
            {
                return BadRequest("Please select a photo");
            }

            List<FoodItemVariantRequest> variants =null;
            if(newMenuItem.IsCustomizable){

                try
                {
                    variants = JsonConvert.DeserializeObject<List<FoodItemVariantRequest>>(newMenuItem.VariantsJson);
                    if (variants == null || variants.Count == 0)
                    {
                        return BadRequest("Variants list cannot be empty.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        message = "Invalid JSON in VariantsJson",
                        raw = newMenuItem.VariantsJson,
                        error = ex.Message
                    });
                }

            }
            // Simulate image upload
            string imageUrl = UploadFiles.Photo(newMenuItem.Photo);

            MenuItemDto menuItem = new MenuItemDto
            {
                Name = newMenuItem.Name,
                Category = newMenuItem.Category,
                ImageUrl = imageUrl,
                IsCustomizable = newMenuItem.IsCustomizable,
                RestaurantId = newMenuItem.RestaurantId,
                Description = newMenuItem.Description,
                Price = newMenuItem.Price,
                Variants = variants
            };

            MenuItem createdMenuItem = _menuItemRepository.Add(menuItem);

            return StatusCode(201, new ApiResponse(true, $"{createdMenuItem.Name} added successfully"));
        }



        [HttpGet("{id}")]
        public ActionResult GetMenu(string id)
        {
            Console.Write(id);
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new AppException("No Restaurant Id passed in params", HttpStatusCode.BadGateway);
            }
            if (!Guid.TryParse(id, out Guid restaurantId))
            {
                throw new AppException("Invalid Restaurant id", HttpStatusCode.BadRequest);
            }
            var menu = _menuItemRepository.GetAll(restaurantId);
            return Ok(new ApiResponse<Object>(true, menu));
        }

        
        [Authorize]
        [Authorize(Roles = "Owner")]
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var ownerId = HttpContext.User.GetUserIdFromClaims();
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new AppException("No item Id passed in params", HttpStatusCode.BadGateway);
            }
            if (!Guid.TryParse(id, out Guid itemid))
            {
                throw new AppException("Invalid Item id", HttpStatusCode.BadRequest);
            }
            string msg = _menuItemRepository.Delete(itemid, ownerId);

            return Ok(new ApiResponse(true, msg));


        }

        [Authorize]
        [Authorize(Roles = "Owner")]
        [HttpPatch("toggle/{id}")]
        public ActionResult ToggleAvalibilityOfItem(string id)
        {
            Console.Write(id);
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new AppException("No Restaurant Id passed in params", HttpStatusCode.BadGateway);
            }
            if (!Guid.TryParse(id, out Guid itemId))
            {
                throw new AppException("Invalid Restaurant id", HttpStatusCode.BadRequest);
            }
            string msg = _menuItemRepository.IsAvailable(itemId);

            return Ok(new ApiResponse(true, msg));
        }

        [Authorize]
        [Authorize(Roles ="Owner")]
        [HttpPatch]
        public ActionResult Update(MenuItem menuItem){
            Console.WriteLine("menuitem {0}",menuItem.Name);
            if(!ModelState.IsValid){
                return BadRequest(new { message = "Invalid Item data", errors = ModelState });
            }

            var res = _menuItemRepository.Update(menuItem);

            return Ok(new ApiResponse<MenuItem>(true,res, $"{res.Name} Updated Sucessfully"));

        }

        
        [HttpPatch("update-varient")]
        [Authorize(Roles ="Admin")]
        public ActionResult UpdateVarient(MenuItemVarient varient){
            if(!ModelState.IsValid){
                return BadRequest(new{message = "Invalid Data",errors = ModelState});
            }
           string msg =  _menuItemRepository.UpdateVarient(varient);

           return Ok(new ApiResponse(true,msg));
        }



        [HttpGet("item/{id}")]
        public ActionResult GetMenuItemById(string id){
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new AppException("No Item Id passed in params", HttpStatusCode.BadGateway);
            }
            if (!Guid.TryParse(id, out Guid itemId))
            {
                throw new AppException("Invalid Restaurant id", HttpStatusCode.BadRequest);
            }

            var menuItem = _menuItemRepository.GetById(itemId);

            return Ok(new ApiResponse<MenuItem>(true,menuItem));

        }





    }
}