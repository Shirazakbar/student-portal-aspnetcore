using Microsoft.EntityFrameworkCore;

namespace StudentPortalApi.Models
{
    public class StudentPortalContext : DbContext
    {
        public StudentPortalContext(DbContextOptions<StudentPortalContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FeedPost> FeedPosts { get; set; }
        public DbSet<PostReaction> PostReactions { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<VideoItem> VideoItems { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<PostReaction>()
                .HasOne(pr => pr.User)
                .WithMany()
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PostReaction>()
                .HasOne(pr => pr.FeedPost)
                .WithMany()
                .HasForeignKey(pr => pr.FeedPostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notice>()
                .HasOne(n => n.CreatedBy)
                .WithMany()
                .HasForeignKey(n => n.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VideoItem>()
                .HasOne(v => v.CreatedBy)
                .WithMany()
                .HasForeignKey(v => v.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.CreatedBy)
                .WithMany()
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
