using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using VideoCollab.Core.Domain.Abstractions;
using VideoCollab.Core.Domain.Models;
using VideoCollabServer.Dtos.Movie;
using VideoCollabServer.Mappers;
using VideoCollabServer.Utils;

namespace VideoCollabServer.Controllers;

[Route("api/movie")]
[ApiController]
public class MovieController(
    IMovieService movieService,
    IHlsService hlsService)
    : ControllerBase
{
    [HttpGet("all")]
    [Authorize]
    public async Task<IActionResult> GetAllMovies()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var id = identity!.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value;
        var moviesResult = await movieService.ReadyToViewMoviesAsync();
        var movies = moviesResult.Value!
            .Select(m => m.ToItemDto(id))
            .OrderByDescending(m => m.Pinned)
            .ToList();
        return !moviesResult.Succeeded ? StatusCode(500, "Internal Server Error") : Ok(movies);
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMovie([FromQuery] int movieId)
    {
        var moviesResult = await movieService.MovieById(movieId);
        return !moviesResult.Succeeded ? BadRequest(moviesResult.Errors) : Ok(moviesResult.Value!.ToMoviePageDto());
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteMovie([FromQuery] [Required] int movieId)
    {
        await movieService.DeleteMovieAsync(movieId);
        hlsService.DeleteMovie(movieId);

        return Ok();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddMovie([FromBody] CreateMovieDto createMovieDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorsList());
        var createdMovie = await movieService.CreateMovieAsync(
            createMovieDto.TrailerLink,
            createMovieDto.PosterLink,
            createMovieDto.Name,
            createMovieDto.Description
        );

        if (!createdMovie.Succeeded)
            return BadRequest();

        return Ok(
            new CreatedMovieDto
            {
                Id = createdMovie.Value!.Id
            }
        );
    }

    [Authorize]
    [HttpPost("upload")]
    [RequestSizeLimit(1024 * 1024 * 1024)]
    public async Task<IActionResult> UploadMovie([FromQuery] [Required] int movieId, [Required] IFormFile file)
    {
        var upload = await hlsService.UploadMovieAsync(movieId, file.OpenReadStream(), file.FileName, movieService);

        if (!upload.Succeeded)
            return BadRequest(upload.Errors);

        await movieService.ChangeMovieStatusAsync(movieId, Movie.Statuses.InQueue);
        return Ok();
    }

    [HttpGet("watch/{movieId:int}/.m3u8")]
    public IActionResult Watch([FromRoute] int movieId)
    {
        var playlist = hlsService.GetPlaylistByMovieId(movieId);

        if (!playlist.Succeeded)
            return BadRequest(playlist.Errors);

        return new FileStreamResult(playlist.Value!,
            new MediaTypeHeaderValue(new StringSegment("application/x-mpegURL")));
    }

    [HttpGet("watch/{movieId:int}/{quality}/{file}")]
    public IActionResult WatchFile([FromRoute] int movieId, string quality, string file)
    {
        var hlsFile = hlsService.GetHlsFile(movieId, quality, file);

        if (!hlsFile.Succeeded)
            return BadRequest();

        var (contentType, stream) = hlsFile.Value;

        return new FileStreamResult(stream,
            new MediaTypeHeaderValue(new StringSegment(contentType)));
    }
}