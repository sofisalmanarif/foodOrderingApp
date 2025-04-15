namespace foodOrderingApp.Models
{
    public class DiscountCoupons
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        public string Code { get; set; } =string.Empty;

        public DateOnly ValidTill { get; set; }

        public DiscountType Type { get; set; }

       
        public float DiscountValue { get; set; }

        public float? MinOrderValue { get; set; } = 160;

      
    }

    // Enum to represent type of discount
    public enum DiscountType
    {
        Percentage,
        Flat
    }
}
