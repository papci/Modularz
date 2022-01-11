using Microsoft.AspNetCore.Mvc;
using Modularz.Models;
using System.Diagnostics;
using Modularz.Data.EF;
using Modularz.Data.Repository;
using UnitSense.Repositories.Abstractions;

namespace Modularz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private PostRepository _postRepository;
        public HomeController(ILogger<HomeController> logger, PostRepository postRepository)
        {
            _logger = logger;
            _postRepository = postRepository;
        }

        [Route("", Name ="Index")]
        public async Task<IActionResult> Index()
        {
            PostQueryFilter queryFilter = new PostQueryFilter()
            {
                Nb = 10,
                Page = 1,
                State = BlogPost.BlogState.Published
            };

            FilteredDataSetResult<BlogPost> data = await _postRepository.GetListAsync(queryFilter);
            return View(data);
        }

        [Route("/article/{seoUrl}", Name = "ViewPost")]
        public async Task<IActionResult> ViewPost(string seoUrl)
        {
            var article = await _postRepository.GetBySecondaryAsync(seoUrl);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}