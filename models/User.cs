
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace foodOrderingApp.models
{
    public enum Role
    {
        Customer,
        Owner, //resturant owner
        Admin // app admin
    }
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name must be between 3 and 50 characters.", MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.Customer;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public Restaurant? Restaurant { get; set; }
        
        [JsonIgnore]
        public Address? Address {get;set;}

        public static string Roles(int r)
        {
            switch (r)
            {
                case 1:
                    return "Admin";
                case 2:
                    return "Customer";
                case 3:
                    return "Owner";
                default:
                    return "Unknown";
            }
        }

    }
}