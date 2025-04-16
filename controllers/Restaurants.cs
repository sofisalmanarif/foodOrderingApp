using System.Net;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using foodOrderingApp.Models;
using foodOrderingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using foodOrderingApp.middlewares;
using foodOrderingApp.services;

namespace foodOrderingApp.controllers
{
    [ApiController]
    [Route("api/restaurants")]
    public class Restaurants : ControllerBase
    {
        private readonly IResturantRepository _restaurantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;


        public Restaurants(IResturantRepository resturantRepository, IUserRepository userRepository, IAddressRepository addressRepository)
        {
            _restaurantRepository = resturantRepository;
            _userRepository = userRepository;
            _addressRepository  =addressRepository;


        }
        [Consumes("multipart/form-data")]
        [HttpPost]
        public ActionResult Create([FromForm] RestaurantDto restaurantDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid restaurant data", errors = ModelState });
            }

            if (restaurantDto.Photo == null   )
            {
                return BadRequest("Please Select photos");
            }
            if (restaurantDto.ValidDocument == null)
            {
                return BadRequest("Please Upload Image Of Valid restaurant Document");
            }


            User owner = new User()
            {
                Name = restaurantDto.OwnerName,
                Email = restaurantDto.OwnerEmail,
                Phone = restaurantDto.OwnerPhone,
                Password = restaurantDto.OwnerPassword,
                Role = Role.Owner,
                CreatedAt = DateTime.UtcNow
            };

            Guid userId = _userRepository.Add(owner);



            string ImageUrl = UploadFiles.Photo(restaurantDto.Photo);
            string validDocUrl = UploadFiles.Photo(restaurantDto.ValidDocument);

            if (string.IsNullOrWhiteSpace(ImageUrl) || string.IsNullOrWhiteSpace(validDocUrl))
            {
                throw new AppException("failed to upload files", HttpStatusCode.InternalServerError);

            }
            Restaurant restaurant = new Restaurant()
            {
                RestaurantName = restaurantDto.RestaurantName,
                RestaurantPhone = restaurantDto.RestaurantPhone,
                Description = restaurantDto.Description,
                ImageUrl = ImageUrl,
                ValidDocument = validDocUrl,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow,
                IsVerified = false,
                IsActive = true
            };

            Restaurant createdRestaurant = _restaurantRepository.Add(restaurant); // 🔹 Save restaurant
            // Guid menuId = _MenuRepository.Add(createdRestaurant);
            Address restaurnatAddress = new Address(){
                AddressType = AddressType.Restaurant,
                Area = restaurantDto.Area,
                City=restaurantDto.City,
                Floor =restaurantDto.Floor,
                Landmark =restaurantDto.Landmark,
                RefId = owner.Id,
            };
            _addressRepository.Add(restaurnatAddress);


            return Ok(new ApiResponse<object>(true, new { restaurant_id = restaurant.Id,admin_id =userId }, "Restaurant created successfully!"));
        }

        [HttpGet("{id}")]
        public ActionResult GetById(string id)
        {
            Console.Write(id);
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("No Restautant id passed in params");
            }
            if (!Guid.TryParse(id, out Guid guidId))
            {
                return BadRequest("Invalid Restaurnat id");
            }
            var restaurant = _restaurantRepository.GetById(guidId);
            if (restaurant == null)
            {
                return BadRequest("Invalid Restaurnat id");
            }
            return Ok(new ApiResponse<object>(true, restaurant));
        }
       
        [HttpGet]
        public ActionResult AllVerifiedRestaurants()
        {
            var allVerifiedRestaurants = _restaurantRepository.GetAll();
            return Ok(new ApiResponse<IEnumerable<Restaurant>>(true, allVerifiedRestaurants));
        }


        [Authorize]
        [Authorize(Roles = "Admin")]
        [HttpPatch("varify/{id}")]
        public ActionResult Verify(string id)
        {
            Console.Write(id);
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("No Restautant id passed in params");
            }
            if (!Guid.TryParse(id, out Guid guidId))
            {
                return BadRequest("Invalid Restaurnat id");
            }
            string res = _restaurantRepository.Verify(guidId);
            return Ok(new ApiResponse(true, res));
        }


        
        [Authorize]
        [Authorize(Roles ="Owner")]
        [HttpPatch("toggle-orders")]
        public ActionResult ToggleOrders()
        {
            Guid ownerId = HttpContext.User.GetUserIdFromClaims();  //it will get user id
            string res = _restaurantRepository.ToggleOrders(ownerId);
            return Ok(new ApiResponse(true, res));
        }

        [Authorize]
        [Authorize(Roles ="Admin")]
        [HttpGet("requests")]
        public ActionResult NotVerifiedRestaurants()
        {
            var notVerifiedRestaurants = _restaurantRepository.GelAllNotVerified();
            return Ok(new ApiResponse<IEnumerable<Restaurant>>(true, notVerifiedRestaurants));
        }
        [Authorize]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("No Restautant id passed in params");
            }
            if (!Guid.TryParse(id, out Guid restaurantId))
            {
                return BadRequest("Invalid  id  Format");
            }

            string message = _restaurantRepository.Delete(restaurantId);

            return Ok(new ApiResponse(true, message));

        }

        [HttpGet("category/{id}")]
        public ActionResult GetRestautrantsByCategory(string id){
                if(string.IsNullOrWhiteSpace(id)){
                return BadRequest("please provide id in params");
                }

                if(!Guid.TryParse(id,out Guid categoryId)){
                return BadRequest("please provide id in valid Format");

            }
            var restaurnats = _restaurantRepository.GetAllRestaurantsByCategory(categoryId);
            return Ok(new ApiResponse<IEnumerable<Restaurant>>(true ,restaurnats));


        }

    }
}