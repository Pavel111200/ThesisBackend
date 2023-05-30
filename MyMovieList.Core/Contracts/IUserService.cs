using MyMovieList.Core.Models;

namespace MyMovieList.Core.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<UserListViewModel>> GetUsers();

        Task<bool> Suggestion(UserSuggestionViewModel model);

        Task DeleteSuggestion(Guid suggestionId);

        Task<UserViewModel> Register(RegisterUserViewModel user);

        Task<UserViewModel> Login(LoginUserViewModel user);

        Task ChangeRole(Guid userId, string newRole);
    }
}
