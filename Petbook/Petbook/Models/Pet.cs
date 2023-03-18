using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Petbook.Models
{
    public class Pet
    {
        [Key]
        public int PetId { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required (ErrorMessage = "The name of the pet is required")]
        [StringLength (30, ErrorMessage = "Name cannot have more than 30 characters")]
        [MinLength (1, ErrorMessage = "Name must have more than 1 character")]
        public string? PetName { get; set; }
        
        public string? PetPhoto { get; set; }

        [Required(ErrorMessage = "The category of the pet is required")]
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }

    }
}
