namespace MyMovieList.Core.Models
{
    public class UserListViewModel
    {
        public Guid Id { get; set; }
        public string Role { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
