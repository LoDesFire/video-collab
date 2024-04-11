using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using VideoCollabServer.Dtos.Movie;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Utils;

namespace VideoCollabServer.Controllers;

[Route("api/movie")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly IMovieRepository _repository;
    private readonly IHlsService _hlsService;

    public MovieController(IMovieRepository repository, IHlsService hlsService)
    {
        _repository = repository;
        _hlsService = hlsService;
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
        var createdMovieDto = await _repository.CreateMovieAsync(createMovieDto);

        if (createdMovieDto == null)
            return BadRequest();

        return Ok(createdMovieDto);
    }

    [HttpPost("upload")]
    [RequestSizeLimit(1024 * 1024 * 1024)]
    public async Task<IActionResult> UploadMovie([FromQuery] [Required] int movieId, [Required] IFormFile file)
    {
        var uploadMovieDto = await _hlsService.UploadMovieAsync(movieId, file, _repository);

        if (!uploadMovieDto.Succeeded)
            return BadRequest(new List<string> { uploadMovieDto.Error! });

        return Ok();
    }

    [HttpGet("watch/{movieId}/.m3u8")]
    public IActionResult Watch([FromRoute] int movieId)
    {
        var playlistDto = _hlsService.GetPlaylistByMovieId(movieId);

        if (!playlistDto.Exists)
            return BadRequest(new List<string?> { playlistDto.Error });

        return new FileStreamResult(playlistDto.Stream!,
            new MediaTypeHeaderValue(new StringSegment("application/x-mpegURL")));
    }

    [HttpGet("watch/{movieId}/{quality}/{file}")]
    public IActionResult WatchFile([FromRoute] int movieId, string quality, string file)
    {
        var hlsFileDto = _hlsService.GetHlsFile(movieId, quality, file);

        if (!hlsFileDto.Succeeded)
            return BadRequest();

        return new FileStreamResult(hlsFileDto.Stream!,
            new MediaTypeHeaderValue(new StringSegment(hlsFileDto.ContentType)));
    }
}