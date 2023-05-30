using Microsoft.EntityFrameworkCore;
using MyMovieList.Core.Contracts;
using MyMovieList.Core.Models;
using MyMovieList.Infrastructure.Data;
using MyMovieList.Infrastructure.Data.Repositories;

namespace MyMovieList.Core.Services
{
    public class MovieService : IMovieService
    {
        private readonly IApplicationDbRepository repo;

        public MovieService(IApplicationDbRepository repo)
        {
            this.repo = repo;
        }

        public async Task<bool> AddMovie(AddMovieViewModel model)
        {
            bool isSaved = false;
            List<Genre> genres = new List<Genre>();
            List<Writer> writers = new List<Writer>();
            List<Director> directors = new List<Director>();

            if (repo.All<Movie>().Any(m => model.Title == m.Title))
            {
                return true;
            }

            foreach (var genre in model.Genre.Split(", "))
            {
                var genreToAdd = await repo.All<Genre>().FirstOrDefaultAsync(g => genre == g.Name);
                if (genreToAdd == null)
                {                  
                    genreToAdd = new Genre { Name = genre };
                }
                genres.Add(genreToAdd);
            }
            foreach (var director in model.Director.Split(", "))
            {
                string firstName = director.Split(" ")[0];
                string lastName = director.Split(" ")[1];

                var directorToAdd = await repo.All<Director>()
                    .FirstOrDefaultAsync(d => d.Firstname == firstName && d.Lastname == lastName);

                if (directorToAdd != null)
                {
                    directors.Add(directorToAdd);
                }
                else
                {
                    directors.Add(new Director { Firstname = firstName, Lastname = lastName });
                }
            }
            foreach (var writer in model.Writer.Split(", "))
            {
                string firstName = writer.Split(" ")[0];
                string lastName = writer.Split(" ")[1];

                var writerToAdd = await repo.All<Writer>()
                    .FirstOrDefaultAsync(w => w.Firstname == firstName && w.Lastname == lastName);

                if (writerToAdd != null)
                {
                    writers.Add(writerToAdd);
                }
                else
                {
                    writers.Add(new Writer { Firstname = firstName, Lastname = lastName });
                }
            }
            var movie = new Movie
            {
                CreatedOn = model.CreatedOn,
                Title = model.Title,
                Description = model.Description,
                Genre = genres,
                Director = directors,
                Writer = writers,
                Runtime = TimeSpan.Parse(model.Runtime),
                Image = model.Image
            };
            try
            {
                await repo.AddAsync<Movie>(movie);
                await repo.SaveChangesAsync();
                isSaved = true;
            }
            catch (Exception)
            {
            }
            return isSaved;
        }

        public async Task<bool> AddMovieReview(Guid movieId, AddMovieReviewViewModel model)
        {
            MovieRating movieRating = new MovieRating
            {
                UserId = model.UserId,
                Title = model.Title,
                Rating = model.Rating,
                Review = model.Review,
                MovieId = movieId
            };

            try
            {
                await repo.AddAsync<MovieRating>(movieRating);
                await repo.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteMovie(Guid movieId)
        {
            await repo.DeleteAsync<Movie>(movieId);
            await repo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<AllMoviesViewModel>> GetAllMovies()
        {
            var allMovies = await repo.All<Movie>()
                .Select(movie => new AllMoviesViewModel()
                {
                    Id = movie.Id,
                    Image = movie.Image,
                    Title = movie.Title,
                })
                .ToListAsync();

            foreach (var movie in allMovies)
            {
                movie.Rating = await GetRating(movie.Id);
                movie.Rating = Math.Round(movie.Rating, 1);
            }

            return allMovies;
        }

        public async Task<IEnumerable<LikedMoviesViewModel>> GetLikedMovies(Guid userId)
        {
            var likedMovies = await repo.All<MovieRating>()
                .Where(mr => mr.UserId == userId)
                .Select(mr => new LikedMoviesViewModel
                {
                    Id = mr.Movie.Id,
                    Image = mr.Movie.Image,
                    Title = mr.Movie.Title,
                    YourRating = mr.Rating,
                })
                .ToListAsync();

            if (!likedMovies.Any())
            {
                throw new ArgumentException("No liked movies found");
            }

            foreach (var movie in likedMovies)
            {
                movie.OverallRating = await GetRating(movie.Id);
                movie.OverallRating = Math.Round(movie.OverallRating, 1);
            }

            return likedMovies;
        }

        public async Task<MovieDetailsViewModel> GetMovieDetails(Guid id)
        {
            var movie = await repo.All<Movie>()
                .Include(m => m.Genre)
                .Include(m => m.Writer)
                .Include(m => m.Director)
                .FirstOrDefaultAsync(m => m.Id == id);

            IEnumerable<ReviewViewModel> reviews = await repo.All<MovieRating>()
                .Where(m => m.MovieId == id)
                .Select(m => new ReviewViewModel { Rating = m.Rating, Review = m.Review, Title = m.Title })
                .ToListAsync();

            if (movie == null)
            {
                throw new ArgumentException("The given id doesn't exist.");
            }

            double rating = await GetRating(id);
            rating = Math.Round(rating, 1);

            return new MovieDetailsViewModel
            {
                Id = movie.Id,
                CreatedOn = movie.CreatedOn,
                Description = movie.Description,
                Director = string.Join(", ", movie.Director.Select(d => $"{d.Firstname} {d.Lastname}")),
                Genre = string.Join(", ", movie.Genre.Select(g => g.Name)),
                Image = movie.Image,
                Runtime = movie.Runtime,
                Title = movie.Title,
                Writer = string.Join(", ", movie.Writer.Select(w => $"{w.Firstname} {w.Lastname}")),
                Rating = rating,
                Reviews = reviews
            };
        }

        public async Task<IEnumerable<SuggestedMovies>> GetUserSuggestedMovies()
        {
            var movies = await repo.All<UserSuggestion>().Where(s => s.Type == "Movie")
                .Select(s => new SuggestedMovies() { Title = s.Title, Id=s.Id })
                .ToListAsync();

            if(movies.Count == 0)
            {
                throw new ArgumentNullException("There are no movie suggestions");
            }

            return movies;
        }

        public async Task<bool> UpdateMovie(Guid movieId, EditMovieViewModel model)
        {
            var movieToUpdate = await repo.All<Movie>()
                .Include(m => m.Director)
                .Include(m => m.Genre)
                .Include(m => m.Writer)
                .FirstOrDefaultAsync(m => m.Id == movieId);

            if(movieToUpdate == null)
            {
                return false;
            }

            bool isSaved = false;
            List<Genre> genres = new List<Genre>();
            List<Writer> writers = new List<Writer>();
            List<Director> directors = new List<Director>();

            foreach (var genre in model.Genre.Split(", "))
            {
                var genreToAdd = await repo.All<Genre>().FirstOrDefaultAsync(g => genre == g.Name);
                if (genreToAdd != null)
                {
                    genres.Add(genreToAdd);
                }
                else
                {
                    var genreToCreate = new Genre{ Name = genre };
                    await repo.AddAsync<Genre>(genreToCreate);
                    genres.Add(genreToCreate);
                }
            }
            foreach (var director in model.Director.Split(", "))
            {
                string firstName = director.Split(" ")[0];
                string lastName = director.Split(" ")[1];

                var directorToAdd = await repo.All<Director>()
                    .FirstOrDefaultAsync(d => d.Firstname == firstName && d.Lastname == lastName);

                if (directorToAdd != null)
                {
                    directors.Add(directorToAdd);
                }
                else
                {
                    var directorToCreate = new Director { Firstname = firstName, Lastname = lastName };
                    await repo.AddAsync<Director>(directorToCreate);
                    directors.Add(directorToCreate);
                }
            }
            foreach (var writer in model.Writer.Split(", "))
            {
                string firstName = writer.Split(" ")[0];
                string lastName = writer.Split(" ")[1];

                var writerToAdd = await repo.All<Writer>()
                    .FirstOrDefaultAsync(w => w.Firstname == firstName && w.Lastname == lastName);

                if (writerToAdd != null)
                {
                    writers.Add(writerToAdd);
                }
                else
                {
                    var writterToCreate = new Writer { Firstname = firstName, Lastname = lastName };
                    await repo.AddAsync<Writer>(writterToCreate);
                    writers.Add(writterToCreate);
                }
            }
            await repo.SaveChangesAsync();

            if (movieToUpdate != null)
            {
                movieToUpdate.Id = movieId;
                movieToUpdate.Director = directors;
                movieToUpdate.Writer = writers;
                movieToUpdate.Genre = genres;
                movieToUpdate.Title = model.Title;
                movieToUpdate.Description = model.Description;
                movieToUpdate.CreatedOn = model.CreatedOn;
                movieToUpdate.Image = model.Image;
                movieToUpdate.Runtime = TimeSpan.Parse(model.Runtime);
            }
            try
            {
                repo.Update<Movie>(movieToUpdate);
                await repo.SaveChangesAsync();
                isSaved = true;
            }
            catch (Exception)
            {
            }
            return isSaved;
        }

        private async Task<double> GetRating(Guid movieId)
        {
            List<double> allRatings = await repo.All<MovieRating>()
                 .Where(m => movieId == m.MovieId)
                 .Select(x => x.Rating)
                 .ToListAsync();

            if (allRatings.Count == 0)
            {
                return 1.00;
            }

            double sumOfRatings = 0.00;
            foreach (var rating in allRatings)
            {
                sumOfRatings += rating;
            }
            
            return sumOfRatings / allRatings.Count;
        }
    }
}
