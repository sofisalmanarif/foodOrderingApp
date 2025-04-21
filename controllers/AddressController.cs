using foodOrderingApp.interfaces;
using Microsoft.AspNetCore.Mvc;
using foodOrderingApp.models;
using foodOrderingApp.Models;
using foodOrderingApp.Services;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using foodOrderingApp.middlewares;

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

        [Authorize]
        [HttpPost]
        public ActionResult Create([FromBody] UserAddressDto address){
            if(!ModelState.IsValid){
                return BadRequest(new { message = "Invalid address data", errors = ModelState });
            }
            // if (!Enum.TryParse<AddressType>(address.AddressType, true, out AddressType addressType))
            // {
            //     throw new AppException("Invalid Address Type",HttpStatusCode.BadRequest);
            // }
            if (!Enum.TryParse<AddressType>(address.AddressType.ToString(), true, out var addressType))
            {
                return BadRequest("Invalid Address Type");
            }
            var userId = HttpContext.User.GetUserIdFromClaims();
            Address userAddress = new Address(){
                AddressType  = addressType,
                Area =address.Area,
                City =address.City,
                Floor = address.Floor,
                Landmark = address.Landmark,
                RefId = userId,

            };
            string msg = _addressRepository.Add(userAddress);

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
             var userId = HttpContext.User.GetUserIdFromClaims();
            string msg = _addressRepository.Delete(userId);

            return Ok(new ApiResponse(true,msg));
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetUserAddresses()
        {
            var userId = HttpContext.User.GetUserIdFromClaims();
            var addresses = _addressRepository.GetUserAddresses(userId);

            return Ok(new ApiResponse<IEnumerable<Address>>(true, addresses));
        }
    }
        
    }
