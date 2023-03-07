using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Petbook.Models
{
    public class BlogPostTag
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogPostTagId { get; set; }
        public int? TagId { get; set; }
        public virtual Tag? Tag { get; set; }
        public int? BlogPostId { get; set; }
        public virtual BlogPost? BlogPost { get; set; }
    }
}
