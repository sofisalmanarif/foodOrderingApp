using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.models;

namespace foodOrderingApp.interfaces
{
    public interface IDiscountCouponRepository
    {
        string Create(DiscountCouponDto coupon);
    }
}