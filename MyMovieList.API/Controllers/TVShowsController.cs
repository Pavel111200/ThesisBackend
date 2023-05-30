using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMovieList.Core.Contracts;
using MyMovieList.Core.Models;
using MyMovieList.Core.Services;
using System.Data;

namespace MyMovieList.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TVShowsController : ControllerBase
    {
        private ITVShowService _tvShowService;

        public TVShowsController(ITVShowService tVShowService)
        {
            _tvShowService = tVShowService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(AllTVShowsViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTVShows()
        {
            var result = await _tvShowService.GetAllTVShows();

            return Ok(result);
        }

        [HttpGet("{showId}")]
        [ProducesResponseType(typeof(TVShowDetailsViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShowById(Guid showId)
        {
            TVShowDetailsViewModel result = new TVShowDetailsViewModel();
            try
            {
                result = await _tvShowService.GetTVShowDetails(showId);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return Ok(result);
        }

        [HttpGet("LikedShows/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(LikedShowsViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLikedTVShows(Guid userId)
        {
            IEnumerable<LikedShowsViewModel> result = new List<LikedShowsViewModel>();
            try
            {
                result = await _tvShowService.GetLikedShows(userId);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return Ok(result);
        }

        [HttpPost("{showId}")]
        [Authorize]
        public async Task<IActionResult> CreateReview(Guid showId, AddMovieReviewViewModel model)
        {
            bool isSaved = false;
            try
            {
                isSaved = await _tvShowService.AddShowReview(showId, model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (isSaved)
            {
                return Ok();
            }
            return BadRequest("Error occured while creating the show");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddTVShowViewModel model)
        {
            bool isSaved = false;
            try
            {
                isSaved = await _tvShowService.AddShow(model);
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

        [HttpPut("{showId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid showId, EditTVShowViewModel model)
        {
            bool isSaved = false;
            try
            {
                isSaved = await _tvShowService.UpdateTVShow(showId, model);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            if (!isSaved)
            {
                return BadRequest("An error occured when tring to update the TV show");
            }
            return Ok();
        }

        [HttpDelete("{showId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid showId)
        {
            bool isSaved = false;
            try
            {
                isSaved = await _tvShowService.DeleteShow(showId);
            }
            catch (ArgumentNullException)
            {
                return NotFound("Show not found");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }

        [HttpGet("SuggestedShows")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(SuggestedShows), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserSuggestedShow()
        {
            IEnumerable<SuggestedShows> result = new List<SuggestedShows>();
            try
            {
                result = await _tvShowService.GetUserSuggestedShows();
            }
            catch (ArgumentNullException e)
            {
                return NotFound(e.Message);
            }

            return Ok(result);
        }
    }
}
