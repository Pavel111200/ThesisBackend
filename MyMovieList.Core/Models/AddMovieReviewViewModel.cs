namespace MyMovieList.Core.Models
{
    public class AddMovieReviewViewModel
    {
        public Guid UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public double Rating { get; set; }

        public string Review { get; set; } = string.Empty;
    }
}
