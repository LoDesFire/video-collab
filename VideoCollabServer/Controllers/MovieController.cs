using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using VideoCollabServer.Dtos.Movie;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Models;
using VideoCollabServer.Utils;

namespace VideoCollabServer.Controllers;

[Route("api/movie")]
[ApiController]
public class MovieController(
    IMovieRepository repository,
    IHlsService hlsService,
    ITranscodingMovieRepository transcodingMovieRepository)
    : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetAllMovies()
    {
        var moviesResult = await repository.ReadyToViewMoviesAsync();
        return !moviesResult.Succeeded ? StatusCode(500, "Internal Server Error") : Ok(moviesResult.Value);
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMovie([FromQuery] int movieId)
    {
        var moviesResult = await repository.MovieById(movieId);
        return !moviesResult.Succeeded ? BadRequest(moviesResult.Errors) : Ok(moviesResult.Value);
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteMovie([FromQuery] [Required] int movieId)
    {
        await repository.DeleteMovieAsync(movieId);
        hlsService.DeleteMovie(movieId);

        return Ok();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddMovie([FromBody] CreateMovieDto createMovieDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorsList());
        var createdMovie = await repository.CreateMovieAsync(createMovieDto);

        if (!createdMovie.Succeeded)
            return BadRequest();

        return Ok(createdMovie.Value);
    }

    [Authorize]
    [HttpPost("upload")]
    [RequestSizeLimit(1024 * 1024 * 1024)]
    public async Task<IActionResult> UploadMovie([FromQuery] [Required] int movieId, [Required] IFormFile file)
    {
        var upload = await hlsService.UploadMovieAsync(movieId, file, repository);

        if (!upload.Succeeded)
            return BadRequest(upload.Errors);

        await transcodingMovieRepository.ChangeMovieStatusAsync(movieId, Movie.Statuses.InQueue);
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

        return new FileStreamResult(hlsFile.Value!.Stream,
            new MediaTypeHeaderValue(new StringSegment(hlsFile.Value!.ContentType)));
    }
}