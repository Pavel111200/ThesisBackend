using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyMovieList.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomUser>().HasIndex(u=>u.Email).IsUnique();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Writer> Writers { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<TVShow> TVShows { get; set; }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<UserSuggestion> UserSuggestions { get; set; }
        public DbSet<MovieRating> MovieRatings { get; set; }
        public DbSet<TVShowRating> TVShowRatings { get; set; }
        public DbSet<CustomUser> CustomUsers { get; set; }
    }
}