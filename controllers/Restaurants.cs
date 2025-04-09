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

            if (restaurantDto.Photo == null)
            {
                throw new AppException("Please Select photo", HttpStatusCode.BadRequest);
            }
            string ImageUrl = UploadFiles.Photo(restaurantDto.Photo);

            if(string.IsNullOrWhiteSpace(ImageUrl)){
                throw new AppException("failed to upload file", HttpStatusCode.InternalServerError);

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

         

            if (restaurantDto.Photo == null)
            {
                throw new AppException("Please Select photo", HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = new Restaurant()
            {
                RestaurantName = restaurantDto.RestaurantName,
                RestaurantPhone = restaurantDto.RestaurantPhone,
                Description = restaurantDto.Description,
                ImageUrl = ImageUrl,
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
                RefId = createdRestaurant.Id,
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
                throw new AppException("No Restautant id passed in params", HttpStatusCode.BadGateway);
            }
            if (!Guid.TryParse(id, out Guid guidId))
            {
                throw new AppException("Invalid Restaurnat id", HttpStatusCode.BadRequest);
            }
            var restaurant = _restaurantRepository.GetById(guidId);
            if (restaurant == null)
            {
                throw new AppException("Invalid Restaurnat id", HttpStatusCode.BadRequest);
            }
            return Ok(new ApiResponse<Restaurant>(true, restaurant));
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
                throw new AppException("No Restautant id passed in params", HttpStatusCode.BadGateway);
            }
            if (!Guid.TryParse(id, out Guid guidId))
            {
                throw new AppException("Invalid Restaurnat id", HttpStatusCode.BadRequest);
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
                throw new AppException("No Restautant id passed in params", HttpStatusCode.BadGateway);
            }
            if (!Guid.TryParse(id, out Guid restaurantId))
            {
                throw new AppException("Invalid  id  Format", HttpStatusCode.BadRequest);
            }

            string message = _restaurantRepository.Delete(restaurantId);

            return Ok(new ApiResponse(true, message));

        }

        [HttpGet("category/{id}")]
        public ActionResult GetRestautrantsByCategory(string id){
                if(string.IsNullOrWhiteSpace(id)){
                    throw new AppException("please provide id in params",HttpStatusCode.BadRequest);
                }

                if(!Guid.TryParse(id,out Guid categoryId)){
                throw new AppException("please provide id in valid Format", HttpStatusCode.BadRequest);

            }
            var restaurnats = _restaurantRepository.GetAllRestaurantsByCategory(categoryId);
            return Ok(new ApiResponse<IEnumerable<Restaurant>>(true ,restaurnats));


        }

    }
}