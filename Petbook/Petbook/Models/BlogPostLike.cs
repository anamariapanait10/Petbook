    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Petbook.Models
{
    public class BlogPostLike
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? BlogPostId { get; set; }
        public virtual BlogPost? BlogPost { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; } 
    }
}
