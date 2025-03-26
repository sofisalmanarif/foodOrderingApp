
namespace foodOrderingApp.models
{
    public class Menu
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RestaurantId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Restaurant? Restaurant { get; set; } 
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();
    }
}