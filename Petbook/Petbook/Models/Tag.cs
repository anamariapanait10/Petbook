using System.ComponentModel.DataAnnotations;

namespace Petbook.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }
        [Required(ErrorMessage = "The tag name is required")]
        public string? TagName { get; set; }
        public virtual ICollection<BlogPostTag>? BlogPostTags { get; set; }
    }
}
