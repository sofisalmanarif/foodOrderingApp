using foodOrderingApp.interfaces;
using Microsoft.AspNetCore.Mvc;
using foodOrderingApp.models;
using foodOrderingApp.Models;
using foodOrderingApp.Services;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace foodOrderingApp.controllers
{
    [ApiController]
    [Route("api/address")]
    public class AddressController:ControllerBase

    {
        private readonly IAddressRepository _addressRepository;

        public AddressController(IAddressRepository addressRepository)
        {
            _addressRepository  = addressRepository;
        }

        [HttpPost]
        public ActionResult Create([FromBody] Address address){
            if(!ModelState.IsValid){
                return BadRequest(new { message = "Invalid address data", errors = ModelState });
            }
            string msg = _addressRepository.Add(address);

            return Ok(new ApiResponse(true,msg));
        }

        [HttpGet("{id}")]
        public ActionResult GetAddressById(string id){
            if(string.IsNullOrWhiteSpace(id)){
                throw new AppException("Please provide UserId in params",HttpStatusCode.BadRequest);
            }

            if(!Guid.TryParse(id,out Guid userId)){
                throw new AppException("Invalid id Format", HttpStatusCode.BadRequest);

            }
            Address address = _addressRepository.Get(userId);
            return Ok(new ApiResponse<Address>(true,address));
        }

        [Authorize]
        [HttpDelete]
        public ActionResult DeleteAddress(){
             var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Invalid token or user ID not found."));
            }
            if(!Guid.TryParse(userIdClaim.Value,out Guid userId)){
                throw new AppException("Invalid token",HttpStatusCode.BadRequest);
            }
            string msg = _addressRepository.Delete(userId);

            return Ok(new ApiResponse(true,msg));
        }
        }
        
    }
