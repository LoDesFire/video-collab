using Microsoft.AspNetCore.Mvc;
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
    
    
}