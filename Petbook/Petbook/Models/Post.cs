using System.ComponentModel.DataAnnotations;

namespace Petbook.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public int? PetId { get; set; }
        public virtual Pet? Pet { get; set; }

        [Required (ErrorMessage = "Photo is required")]
        public string? Photo { get; set; }
        public string? Description { get; set; }
        public DateTime? PostDate {get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<PostLike>? PostLikes { get; set; }
    }
}
