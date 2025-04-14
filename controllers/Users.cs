using System;
using System.Net;
using System.Security.Claims;
using foodOrderingApp.interfaces;
using foodOrderingApp.middlewares;
using foodOrderingApp.models;
using foodOrderingApp.Models;
using foodOrderingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;




namespace foodOrderingApp.controllers
{   
    [ApiController]
    [Route("api/users")]
    public class Users:ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public Users(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            
        }
        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="user">User details for registration.</param>
        /// <returns>Returns success message if the user is registered.</returns>
        /// <response code="200">User registered successfully.</response>
        /// <response code="400">Invalid user data.</response>
        [HttpPost] 
        public ActionResult Create([FromBody] User user){

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid User data", errors = ModelState });
            }
            _userRepository.Add(user);

            return Ok(new ApiResponse(true,"User Registerd Successfully"));
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDto user){
            Console.WriteLine(user.Email);
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid User data", errors = ModelState });
            }
            var data = _userRepository.Login(user);
            return Ok(new ApiResponse<object>(true,data));
        }
        
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public ActionResult All(){
            var users =  _userRepository.GetAll();
            return Ok(new ApiResponse<IEnumerable<User>>(true ,users));
        }

        [Authorize]
        [HttpGet("profile")]
        public ActionResult Profile()
        {
            Guid userId = HttpContext.User.GetUserIdFromClaims();  //it will get user id
            var user = _userRepository.Profile(userId);
            return Ok(new ApiResponse<object>(true, new{user,restaurant_id = user?.Restaurant?.Id.ToString() ?? ""}));
        }

        [Authorize]
        [HttpPatch]
        public ActionResult Update([FromBody] UpdateUserDto user)
        {   
            if(!ModelState.IsValid){
                return BadRequest(new { message = "Invalid User data", errors = ModelState });

            }
            Guid userId = HttpContext.User.GetUserIdFromClaims();  //it will get user id
            string  msg = _userRepository.Update(user,userId);
            return Ok(new ApiResponse(true, msg));
        }
        [Authorize]
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public ActionResult Delete(string id){
            if(string.IsNullOrWhiteSpace(id)){
               return BadRequest("No user Id passed in params");
            }

            if(!Guid.TryParse(id,out Guid userId)){
                throw new AppException("Invalid Id Format",HttpStatusCode.BadRequest);
            }

            string msg = _userRepository.Delete(userId);
            return Ok(new ApiResponse(true,msg));
        }

    }
}