using System.ComponentModel.DataAnnotations;
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
public class MovieController : ControllerBase
{
    private readonly IMovieRepository _repository;
    private readonly IHlsService _hlsService;
    private readonly ITranscodingMovieRepository _transcodingMovieRepository;

    public MovieController(IMovieRepository repository, IHlsService hlsService, ITranscodingMovieRepository transcodingMovieRepository)
    {
        _repository = repository;
        _hlsService = hlsService;
        _transcodingMovieRepository = transcodingMovieRepository;
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteMovie([FromQuery][Required] int movieId)
    {
        await _repository.DeleteMovieAsync(movieId);
        _hlsService.DeleteMovie(movieId);

        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> AddMovie([FromBody] CreateMovieDto createMovieDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorsList());
        var createdMovie = await _repository.CreateMovieAsync(createMovieDto);

        if (!createdMovie.Succeeded)
            return BadRequest();

        return Ok(createdMovie.Value);
    }

    [HttpPost("upload")]
    [RequestSizeLimit(1024 * 1024 * 1024)]
    public async Task<IActionResult> UploadMovie([FromQuery] [Required] int movieId, [Required] IFormFile file)
    {
        var upload = await _hlsService.UploadMovieAsync(movieId, file, _repository);

        if (!upload.Succeeded)
            return BadRequest(upload.Errors);

        await _transcodingMovieRepository.ChangeMovieStatusAsync(movieId, Movie.Statuses.InQueue);
        return Ok();
    }

    [HttpGet("watch/{movieId:int}/.m3u8")]
    public IActionResult Watch([FromRoute] int movieId)
    {
        var playlist = _hlsService.GetPlaylistByMovieId(movieId);

        if (!playlist.Succeeded)
            return BadRequest(playlist.Errors);

        return new FileStreamResult(playlist.Value!,
            new MediaTypeHeaderValue(new StringSegment("application/x-mpegURL")));
    }

    [HttpGet("watch/{movieId:int}/{quality}/{file}")]
    public IActionResult WatchFile([FromRoute] int movieId, string quality, string file)
    {
        var hlsFile = _hlsService.GetHlsFile(movieId, quality, file);

        if (!hlsFile.Succeeded)
            return BadRequest();

        return new FileStreamResult(hlsFile.Value!.Stream,
            new MediaTypeHeaderValue(new StringSegment(hlsFile.Value!.ContentType)));
    }
}