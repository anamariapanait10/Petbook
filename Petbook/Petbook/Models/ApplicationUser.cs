using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Petbook.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Pet>? Pets { get; set; }
        
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<ApplicationUser>? Followers { get; set; }

        public virtual ICollection<BlogPost>? BlogPosts { get; set; }
        [Required(ErrorMessage = "The username must be specified")]
        public string Username { get; set; }
        [Required(ErrorMessage = "The email must be specified")]
        public string Email { get; set; }   

        public  DateTime? JoinDate { get; set; }

        public string? ProfilePhoto { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
    }
}
