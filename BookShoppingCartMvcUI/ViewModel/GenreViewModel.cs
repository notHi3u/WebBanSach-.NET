using System.ComponentModel.DataAnnotations;

namespace BookShoppingCartMvcUI.ViewModel
{
    public class GenreViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string GenreName { get; set; }
    }
}
