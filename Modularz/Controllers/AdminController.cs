using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modularz.Data.EF;
using Modularz.Data.Repository;
using UnitSense.Repositories.Abstractions;

namespace Modularz.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;

    private PostRepository _postRepository;

    public AdminController(ILogger<AdminController> logger, PostRepository postRepository)
    {
        _logger = logger;
        _postRepository = postRepository;
    }


    [Route("/admin/posts", Name = "ListPost")]
    public async Task<IActionResult> ListPost()
    {
        PostQueryFilter queryFilter = new PostQueryFilter()
        {
            Nb = 10,
            Page = 1,
        };

        FilteredDataSetResult<BlogPost> data = await _postRepository.GetListAsync(queryFilter);
        return View(data);
    }

    [Route("/admin/add-post", Name = "AddPost")]
    public async Task<ActionResult> AddPost()
    {
        return View(null);
    }

    [Route("/admin/edit-post/{id:int}", Name = "EditPost")]
    public async Task<ActionResult> EditPost(int id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        return View("AddPost", id);
    }
}