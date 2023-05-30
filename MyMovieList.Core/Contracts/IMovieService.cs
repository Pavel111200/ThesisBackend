using MyMovieList.Core.Models;

namespace MyMovieList.Core.Contracts
{
    public interface IMovieService
    {
        Task<bool> AddMovie(AddMovieViewModel model);

        Task<IEnumerable<AllMoviesViewModel>> GetAllMovies();

        Task<MovieDetailsViewModel> GetMovieDetails(Guid id);

        Task<bool> UpdateMovie(Guid movieId, EditMovieViewModel model);

        Task<IEnumerable<LikedMoviesViewModel>> GetLikedMovies(Guid userId);

        Task<bool> DeleteMovie(Guid movieId);

        Task<IEnumerable<SuggestedMovies>> GetUserSuggestedMovies();

        Task<bool> AddMovieReview(Guid movieId, AddMovieReviewViewModel model);
    }
}
