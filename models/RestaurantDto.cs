using System.ComponentModel.DataAnnotations;

public class RestaurantDto
{
    // Restaurant fields
    [Required(ErrorMessage = "Restaurant name is required.")]
    [StringLength(100, ErrorMessage = "Restaurant name cannot exceed 100 characters.")]
    public string RestaurantName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Restaurant phone is required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string RestaurantPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Image  is required.")]
    public IFormFile? Photo { get; set; }
    [Required(ErrorMessage = "Image of Valid Diocument is required.")]
    public IFormFile? ValidDocument { get; set; }

    // User (Owner) fields
    [Required(ErrorMessage = "Owner name is required.")]
    [StringLength(100, ErrorMessage = "Owner name cannot exceed 100 characters.")]
    public string OwnerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Owner email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string OwnerEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Owner phone is required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string OwnerPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [DataType(DataType.Password)]
    public string OwnerPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Area { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    public string Landmark { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string ShopNumber { get; set; } = string.Empty;
}
