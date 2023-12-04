using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookShoppingCartMvcUI.ViewModel
{
    public class BookUploadModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(40)]
        public string? BookName { get; set; }

        [Required]
        [MaxLength(40)]
        public string? AuthorName { get; set; }

        [Required(ErrorMessage = "Please enter the price.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double Price { get; set; } = 0; // Default value set to 0
    

        public string? Description;
        
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        public List<OrderDetail>? OrderDetail { get; set; }
        public List<CartDetail>? CartDetail { get; set; }

        [NotMapped]
        public string GenreName { get; set; }
        [Required(ErrorMessage = "Please select an image.")]
        public IFormFile? Image { get; set; }
    }
}
