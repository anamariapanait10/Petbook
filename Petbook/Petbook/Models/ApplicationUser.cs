﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Petbook.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? JoinDate { get; set; }

        public string? ProfilePhoto { get; set; }

        public virtual ICollection<Pet>? Pets { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<ApplicationUser>? Followers { get; set; }

        public virtual ICollection<BlogPost>? BlogPosts { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
    }
}