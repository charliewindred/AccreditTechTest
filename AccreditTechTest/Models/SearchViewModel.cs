using System.ComponentModel.DataAnnotations;

namespace AccreditTechTest.Models
{
    public class SearchViewModel
    {
        [Required]
        [StringLength(39)]
        [RegularExpression(@"^[a-zA-Z0-9-]+$", ErrorMessage = "Only letters, numbers, and hyphens are allowed.")]
        public string Username { get; set; }
    }
}