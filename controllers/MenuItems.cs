
using System.Net;
using System.Security.Claims;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using foodOrderingApp.Models;
using foodOrderingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public ActionResult Create([FromBody] MenuItemDto newMenuItem)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Menu item data", errors = ModelState });
            }
            MenuItem createdMenuItem = _menuItemRepository.Add(newMenuItem);

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
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Invalid token or user ID not found."));
            }

            Console.WriteLine("userid {0}", userIdClaim.Value);

            Guid ownerId = Guid.Parse(userIdClaim.Value);
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
        [HttpPut]
        public ActionResult Update(MenuItem menuItem){
            Console.WriteLine("menuitem {0}",menuItem.Name);
            if(!ModelState.IsValid){
                return BadRequest(new { message = "Invalid Item data", errors = ModelState });
            }

            var res = _menuItemRepository.Update(menuItem);

            return Ok(new ApiResponse<MenuItem>(true,res, $"{res.Name} Updated Sucessfully"));

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