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
public class MovieController: ControllerBase
{
    private readonly IMovieRepository _repository;
    
    public MovieController(IMovieRepository repository)
    {
        _repository = repository;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddFilm([FromBody] CreateMovieDto createMovieDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorsList());
        var createdMovieDto =  await _repository.CreateMovieAsync(createMovieDto);
        
        if (createdMovieDto == null)
            return BadRequest(); 
        
        return Ok(createdMovieDto);
    }
    
    [HttpPost("upload")]
    [RequestSizeLimit(1024 * 1024 * 1024)]
    public async Task<IActionResult> UploadFilm([FromQuery][Required] int movieId, [Required] IFormFile file)
    {
        var extension = file.FileName.Split(".").Last();

        var supportedVideoExtensions = new List<string>
        {
            "mp4", "mov"
        };
        
        if (!supportedVideoExtensions.Contains(extension)) 
            return BadRequest(new List<string>
            {
                "Unsupported video file extension"
            });

        if (! await _repository.ContainsMovieAsync(movieId))
            return BadRequest(new List<string>
            {
                "Movie doesn't exist"
            });
        
        var fileName = $"mov_{movieId}.{extension}";
        
        await using (var fs = System.IO.File.Create($"uploads/{fileName}"))
        {
            await file.CopyToAsync(fs);   
        }
        
        return Ok();
    }
    
    [HttpGet("watch/{movieId}")]
    public IActionResult Watch([FromRoute]int movieId)
    {
        var path = $"uploads/mov_{movieId}/playlist.m3u8";
        var fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            return BadRequest();

        var stream = fileInfo.OpenRead();

        return new FileStreamResult(stream, new MediaTypeHeaderValue(new StringSegment("application/x-mpegURL")));
    }
    
    [HttpGet("watch/{movieId}/{resolution}/{file}")]
    public IActionResult WatchFile([FromRoute] int movieId, string resolution, string file)
    {
        var path = $"uploads/mov_{movieId}/{resolution}/{file}";
        var fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            return BadRequest();

        var stream = fileInfo.OpenRead();

        var contentType = fileInfo.Extension switch
        {
            ".ts" => "video/MP2T",
            ".m3u8" => "application/x-mpegURL",
            _ => "error"
        };

        if (contentType == "error")
            return BadRequest();

        return new FileStreamResult(stream, new MediaTypeHeaderValue(new StringSegment(contentType)));
    }
}