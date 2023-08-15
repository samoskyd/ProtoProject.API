using Microsoft.EntityFrameworkCore;
using ProtoProject.API.Models;

namespace ProtoProject.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<CardGroup> CardGroups { get; set; }
        public virtual DbSet<DocFolder> DocFolders { get; set; }
        public virtual DbSet<ImageFolder> ImageFolders { get; set; }
        public virtual DbSet<LinkFolder> LinkFolders { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<Friendship> Friendships { get; set; }
        public virtual DbSet<ShareHash> ShareHashes { get; set; }
        public virtual DbSet<CardSequence> CardSequences { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Cards)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Friendship>()
                .HasOne(friendship => friendship.FUser)
                .WithMany(user => user.Friendships)
                .HasForeignKey(friendship => friendship.FUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(friendship => friendship.SUser)
                .WithMany()
                .HasForeignKey(friendship => friendship.SUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Card>()
                .HasOne(card => card.PreviousCard)
                .WithOne()
                .HasForeignKey<Card>(card => card.PreviousCardId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Card>()
                .HasOne(card => card.NextCard)
                .WithOne()
                .HasForeignKey<Card>(card => card.NextCardId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CardSequence>()
                .HasOne(cs => cs.User)
                .WithMany(u => u.CardSequences)
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CardSequences)
                .WithOne(cs => cs.User)
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserGroups)
                .WithMany(ug => ug.Users)
                .UsingEntity(j => j.ToTable("UserUserGroup"));

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.Admin)
                .WithMany()
                .HasForeignKey(ug => ug.AdminId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
