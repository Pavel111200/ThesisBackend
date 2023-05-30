using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMovieList.Core.Contracts;
using MyMovieList.Core.Models;

namespace MyMovieList.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(AllMoviesViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMovies()
        {
            var result = await _movieService.GetAllMovies();

            return Ok(result);
        }

        [HttpGet("{movieId}")]
        [ProducesResponseType(typeof(MovieDetailsViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShowById(Guid movieId)
        {
            MovieDetailsViewModel result = new MovieDetailsViewModel();
            try
            {
                result = await _movieService.GetMovieDetails(movieId);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return Ok(result);
        }

        [HttpGet("LikedMovies/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(LikedMoviesViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLikedMovies(Guid userId)
        {
            IEnumerable<LikedMoviesViewModel> result = new List<LikedMoviesViewModel>();

            try
            {
                result = await _movieService.GetLikedMovies(userId);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddMovieViewModel model)
        {
            bool isSaved = false;
            try
            {
                isSaved = await _movieService.AddMovie(model);
            }
            catch (IndexOutOfRangeException)
            {
                return BadRequest("Invalid format");
            }

            if (isSaved)
            {
                return Ok();
            }
            return BadRequest("Error occured while creating the movie");
        }

        [HttpPost("{movieId}")]
        [Authorize]
        public async Task<IActionResult> CreateReview(Guid movieId, AddMovieReviewViewModel model)
        {
            bool isSaved = false;
            try
            {
                isSaved = await _movieService.AddMovieReview(movieId, model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (isSaved)
            {
                return Ok();
            }
            return BadRequest("Error occured while creating the movie");
        }

        [HttpPut("{movieId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid movieId, EditMovieViewModel model)
        {
            bool isSaved = false;
            isSaved = await _movieService.UpdateMovie(movieId, model);

            if (!isSaved)
            {
                return BadRequest("An error occured when tring to update the movie");
            }
            return Ok();
        }

        [HttpDelete("{movieId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid movieId)
        {
            bool isSaved = false;
            try
            {
                isSaved = await _movieService.DeleteMovie(movieId);
            }
            catch (ArgumentNullException)
            {
                return NotFound("Movie not found");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }

        [HttpGet("SuggestedMovies")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(SuggestedMovies), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserSuggestedMovies()
        {
            IEnumerable<SuggestedMovies> result = new List<SuggestedMovies>();
            try
            {
                result = await _movieService.GetUserSuggestedMovies();
            }
            catch (ArgumentNullException e)
            {
                return NotFound(e.Message);
            }

            return Ok(result);
        }
    }
}
