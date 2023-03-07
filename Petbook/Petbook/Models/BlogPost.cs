using System.ComponentModel.DataAnnotations;

namespace Petbook.Models
{
    public class BlogPost
    {
        [Key]
        public int BlogPostId { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        [Required(ErrorMessage = "Continutul postarii este obligatoriu")]
        public string? BlogPostContent { get; set; }
        public virtual ICollection<BlogPostLike>? BlogPostLikes { get; set; }

        public virtual ICollection<BlogPostTag>? BlogPostTags { get; set; }
    }
}
