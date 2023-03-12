using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Petbook.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public int? PetId { get; set; }
        public virtual Pet? Pet { get; set; }

        [Required (ErrorMessage = "Photo is required")]
        public string? PostPhoto { get; set; }
        public string? Description { get; set; }
        public DateTime? PostDate {get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<PostLike>? PostLikes { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> Pets { get; set; }
    }
}
