
namespace foodOrderingApp.models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }= string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}