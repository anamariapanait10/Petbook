using System.ComponentModel.DataAnnotations;

namespace Petbook.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }
        [Required(ErrorMessage = "The tag name is required")]
        [StringLength(20, ErrorMessage = "Tag name cannot have more than 20 characters")]
        public string? TagName { get; set; }
        public virtual ICollection<BlogPostTag>? BlogPostTags { get; set; }
    }
}
