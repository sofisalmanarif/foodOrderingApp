
using cashfree_pg.Client;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using foodOrderingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace foodOrderingApp.controllers
{
    [ApiController]
    [Route("/api/coupons")]
    public class DiscountCouponsController : ControllerBase
    {
        private readonly IDiscountCouponRepository _discountCouponRepository;
        public DiscountCouponsController(IDiscountCouponRepository discountCouponRepository)
        {
            _discountCouponRepository =discountCouponRepository;
        }

        [HttpPost]
        public ActionResult Create([FromBody] DiscountCouponDto coupon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Coupon data", errors = ModelState });
            }

            string msg = _discountCouponRepository.Create(coupon);

            return Ok(new ApiResponse(true,msg));



        }

        [HttpGet]
        public ActionResult AllCoupons([FromQuery] int pageNumber=1,int pageSize =5)
        {
           var coupons =  _discountCouponRepository.AllCoupons(pageNumber,pageSize);
            return Ok(new foodOrderingApp.Models.ApiResponse<IEnumerable<DiscountCoupons>>(true, coupons));

        }

        [HttpPost]
        [Route("verify")]
        public ActionResult VerifyCoupon([FromBody] VerifyCouponDto coupon){
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Coupon data", errors = ModelState });
            }

           float discount =  _discountCouponRepository.Verify(coupon);
           return Ok(new {message =$"You Saved {discount}rs ",discount});
        }
    }
}