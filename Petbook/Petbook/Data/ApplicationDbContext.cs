using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Petbook.Models;

namespace Petbook.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogPostLike> BlogPostLikes { get; set; }
        public DbSet<BlogPostTag> BlogPostTags{ get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // compound primary key
            modelBuilder.Entity<BlogPostTag>()
                .HasKey(bpt => new { bpt.BlogPostTagId, bpt.BlogPostId, bpt.TagId });

            modelBuilder.Entity<BlogPostLike>()
               .HasKey(bpt => new { bpt.BlogPostId, bpt.UserId });

            modelBuilder.Entity<CommentLike>()
               .HasKey(bpt => new { bpt.CommentId, bpt.UserId });

            modelBuilder.Entity<PostLike>()
               .HasKey(bpt => new { bpt.PostId, bpt.UserId });

            // define reltionship with BlogPost and Tag (FK)
            modelBuilder.Entity<BlogPostTag>()
                .HasOne(bpt => bpt.BlogPost)
                .WithMany(bpt => bpt.BlogPostTags)
                .HasForeignKey(bpt => bpt.BlogPostId);

            modelBuilder.Entity<BlogPostTag>()
                .HasOne(bpt => bpt.Tag)
                .WithMany(bpt => bpt.BlogPostTags)
                .HasForeignKey(bpt => bpt.TagId);
        }
    }
}