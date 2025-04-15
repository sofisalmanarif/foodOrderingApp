using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using foodOrderingApp.Models;

namespace foodOrderingApp. repositories
{
    public class DiscountCouponRespository : IDiscountCouponRepository
    {
        private readonly AppDbContext _context;
        public DiscountCouponRespository(AppDbContext context)
        {
            _context =context;
            
        }
        public string Create(DiscountCouponDto coupon)
        {
           var existingCoupon =  _context.DiscountCoupons.FirstOrDefault(d=>d.Code ==coupon.Code);
           if(existingCoupon !=null){
                throw new InvalidOperationException("Coupon code already exists.");
            }
            DiscountCoupons newCoupon = new DiscountCoupons(){
                Code =coupon.Code,
                DiscountValue =coupon.DiscountValue,
                MinOrderValue =coupon.MinOrderValue,
                Type =coupon.Type,
                ValidTill = coupon.ValidTill
            };

            _context.DiscountCoupons.Add(newCoupon );
            _context.SaveChanges();
            return $"Coupon created successfully - [{newCoupon.Code}]";
        }
    }
}