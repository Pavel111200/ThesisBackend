using MyMovieList.Core.Models;

namespace MyMovieList.Core.Contracts
{
    public interface ITVShowService
    {
        Task<bool> AddShow(AddTVShowViewModel model);

        Task<IEnumerable<AllTVShowsViewModel>> GetAllTVShows();

        Task<TVShowDetailsViewModel> GetTVShowDetails(Guid id);

        Task<bool> UpdateTVShow(Guid showId, EditTVShowViewModel model);

        Task<IEnumerable<LikedShowsViewModel>> GetLikedShows(Guid userId);

        Task<bool> DeleteShow(Guid id);

        Task<IEnumerable<SuggestedShows>> GetUserSuggestedShows();

        Task<bool> AddShowReview(Guid showId, AddMovieReviewViewModel model);
    }
}
