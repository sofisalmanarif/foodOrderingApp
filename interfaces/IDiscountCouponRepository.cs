using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.models;
using foodOrderingApp.Models;

namespace foodOrderingApp.interfaces
{
    public interface IDiscountCouponRepository
    {
        string Create(DiscountCouponDto coupon);
        IEnumerable<DiscountCoupons> AllCoupons(int pageNumber, int pageSize);
        float Verify(VerifyCouponDto coupon);
        string Delete(Guid id);
    }
}