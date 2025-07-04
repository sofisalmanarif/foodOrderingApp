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

        public IEnumerable<DiscountCoupons> AllCoupons(int pageNumber,int pageSize)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var coupons = _context.DiscountCoupons
                .Where(dc => dc.ValidTill >= today).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                
            return coupons;
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

        public string Delete(Guid id)
        {
            var exisingCoupon = _context.DiscountCoupons.Find(id);
            if (exisingCoupon == null)
            {
                throw new KeyNotFoundException("Coupon not found");
            }
            _context.DiscountCoupons.Remove(exisingCoupon);
            _context.SaveChanges();
            return $"Coupon [{exisingCoupon.Code}]- Deleted sucessfully";
        }

        public float Verify(VerifyCouponDto coupon)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
           var existingCoupon =  _context.DiscountCoupons.FirstOrDefault(cp=>cp.Code ==coupon.Code && cp.ValidTill>=today);

           if(existingCoupon==null){
                throw new InvalidOperationException("The coupon is either invalid or has expired.");

            }
            if(!(existingCoupon.MinOrderValue <= coupon.CartAmount)){
                throw new InvalidOperationException($"This coupon requires a minimum cart value of {existingCoupon.MinOrderValue} ");
            }

            if(existingCoupon.Type == DiscountType.Percentage){
                return   (existingCoupon.DiscountValue / 100) * coupon.CartAmount;
            }
            else{
                return existingCoupon.DiscountValue;
            }
        }
    }
}