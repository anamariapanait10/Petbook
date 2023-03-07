namespace Petbook.Models
{
    public class PostLike
    {
        public int? PostId { get; set; }
        public Post? Post { get; set; }
        public String? UserId { get; set; }
        public ApplicationUser? User { get; set; }

    }
}
