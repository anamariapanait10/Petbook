namespace Petbook.Models
{
    public class CommentLike
    {
        public int? CommentId { get; set; }
        public Comment? Comment { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
