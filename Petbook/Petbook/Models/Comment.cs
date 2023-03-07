using System.ComponentModel.DataAnnotations;

namespace Petbook.Models
{
    public class Comment
    {
        
        [Key]
        public int CommentId { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "The message content must be specified")]
        public string? CommentContent { get; set; }
        public DateTime CommentDate { get; set; }
        public int? PostId { get; set; }
        public virtual Post? Post { get; set; }
        public virtual ICollection<CommentLike>? CommentLikes { get; set; }

    }
}
