using Microsoft.AspNetCore.Mvc;
using Modularz.Data.EF;
using Modularz.Data.Repository;
using Modularz.Models;

namespace Modularz.Controllers;

public class InitController : Controller
{
    private readonly ILogger<InitController> _logger;
    private BlogDbContext _dbContext;
    private UserRepository _userRepository;
    public InitController(ILogger<InitController> logger, BlogDbContext dbContext, UserRepository userRepository)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userRepository = userRepository;
    }

    [Route("/startup", Name = "Startup")]
    public async Task<ActionResult> Startup()
    {
        return View();
    }
    
    [HttpPost("/start-init", Name = "StartInit")]
    public async Task<ActionResult> StartInit(InitModel model)
    {
        if (!_dbContext.IsInitialized())
        {
            var createdUser = await _userRepository.CreateAsync(model.UserName, model.Email, model.Password);
            await _userRepository.SetAsAdmin(createdUser.User.Id);
            AppState.IsInit = true;
        }
        return Redirect("/");
    }
}