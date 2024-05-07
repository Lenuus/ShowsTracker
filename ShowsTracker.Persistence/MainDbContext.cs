using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShowsTracker.Common.Enum;
using ShowsTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Persistence
{
    public class MainDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public MainDbContext(DbContextOptions<MainDbContext> options)
        : base(options)
        {
        }


        public DbSet<Genre> Genres { get; set; }

        public DbSet<Show> Shows { get; set; }

        public DbSet<ShowUser> ShowUsers { get; set; }

        public DbSet<ShowLink> ShowLinks { get; set; }

        public DbSet<ShowGenre> ShowGenres { get; set; }

        public DbSet<VoteSeason> VoteSeasons { get; set; }

        public DbSet<VoteShow> VoteShows { get; set; }

        public DbSet<UserVote> UserVotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region User Table
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<UserToken>().ToTable("UserToken");
            modelBuilder.Entity<RoleClaim>().ToTable("RoleClaim");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");
            modelBuilder.Entity<User>().Property(p => p.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<UserRole>().HasOne(p => p.User).WithMany(p => p.Roles).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<UserRole>().HasOne(p => p.Role).WithMany(p => p.Users).HasForeignKey(p => p.RoleId).OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Shows
            modelBuilder.Entity<Show>().ToTable("Show");
            modelBuilder.Entity<Show>().Property(p => p.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Show>().Property(p => p.Category).HasDefaultValue(Category.None);
            modelBuilder.Entity<Show>().Property(p => p.ApproveStatus).HasDefaultValue(false);
            modelBuilder.Entity<Show>().HasOne(p => p.InsertedUser).WithMany(p => p.CreatedShows).HasForeignKey(p => p.InsertedUserId).OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Show User
            modelBuilder.Entity<ShowUser>().ToTable("ShowUser");
            modelBuilder.Entity<ShowUser>().Property(p => p.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<ShowUser>().HasOne(p => p.User).WithMany(p => p.Shows).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ShowUser>().HasOne(p => p.Show).WithMany(p => p.Users).HasForeignKey(p => p.ShowId).OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Show Link
            modelBuilder.Entity<ShowLink>().ToTable("ShowLink");
            modelBuilder.Entity<ShowLink>().Property(p => p.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<ShowLink>().HasOne(p => p.Show).WithMany(p => p.Links).HasForeignKey(p => p.ShowId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ShowLink>().HasOne(p => p.User).WithMany(p => p.Links).HasForeignKey(p => p.UserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Genre
            modelBuilder.Entity<Genre>().ToTable("Genre");
            modelBuilder.Entity<Genre>().Property(p => p.IsDeleted).HasDefaultValue(false);
            #endregion

            #region Show Genre
            modelBuilder.Entity<ShowGenre>().ToTable("ShowGenre");
            modelBuilder.Entity<ShowGenre>().Property(p => p.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<ShowGenre>().HasOne(p => p.Show).WithMany(p => p.Genres).HasForeignKey(p => p.ShowId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ShowGenre>().HasOne(p => p.Genre).WithMany(p => p.Shows).HasForeignKey(p => p.GenreId).OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Voting

            modelBuilder.Entity<VoteSeason>().ToTable("VoteSeason");
            modelBuilder.Entity<VoteShow>().ToTable("VoteShow");
            modelBuilder.Entity<UserVote>().ToTable("UserVote");
            modelBuilder.Entity<VoteSeason>().Property(p => p.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<VoteShow>().Property(p => p.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<UserVote>().Property(p => p.IsDeleted).HasDefaultValue(false);

            modelBuilder.Entity<VoteSeason>().Property(p => p.IsFinished).HasDefaultValue(false);
            modelBuilder.Entity<VoteShow>().Property(p => p.IsWinner).HasDefaultValue(false);
            modelBuilder.Entity<VoteShow>().Property(p => p.DisplayOrder).HasDefaultValue(0);

            modelBuilder.Entity<VoteShow>().HasOne(p => p.Show).WithMany(p => p.Votings).HasForeignKey(p => p.ShowId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<VoteShow>().HasOne(p => p.VoteSeason).WithMany(p => p.Shows).HasForeignKey(p => p.VoteSeasonId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserVote>().HasOne(p => p.Show).WithMany(p => p.UserVotings).HasForeignKey(p => p.ShowId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<UserVote>().HasOne(p => p.VoteSeason).WithMany(p => p.UserVotes).HasForeignKey(p => p.VoteSeasonId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<UserVote>().HasOne(p => p.User).WithMany(p => p.UserVotings).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.NoAction);
            #endregion
        }
    }
}
