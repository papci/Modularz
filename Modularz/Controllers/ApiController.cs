using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modularz.Data.EF;
using Modularz.Data.Repository;

namespace Modularz.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private PostRepository _postRepository;
    #region API

    public ApiController(PostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    [HttpGet("/admin/get-post/{id:int}", Name = "GetPost")]
    public async Task<ActionResult<BlogPost>> GetPost(int id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        return Ok(post);
    }
    
    [HttpPost("/admin/edit-post/{id:int}", Name = "EditPost")]
    public async Task<ActionResult<BlogPost>> EditPost(int id, [FromBody] BlogPost post)
    {
        await _postRepository.UpdateAsync(post);
        return Ok(post);
    }

    [HttpPost("/admin/create-post")]
    public async Task<ActionResult<BlogPost>> AddPost([FromBody] BlogPost post)
    {
        await _postRepository.PutAsync(post);
        return Ok(post);
    }
    #endregion
}