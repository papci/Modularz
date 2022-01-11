using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Modularz.Data.EF;
using Modularz.Data.Repository;
using Modularz.Models;

namespace Modularz.Controllers;

public class AuthController : Controller
{
    private UserRepository _userRepository;

    public AuthController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    //
    // GET: /Account/Login
    /// <summary>
    /// Renvoie la vue d'authentification au site
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [Route("/login", Name = "Login")]
    public ActionResult Login(string returnUrl)
    {
        if (User?.Identity?.IsAuthenticated == true)
            return Redirect("/admin/posts");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
    
    // POST: /Account/Login
    /// <summary>
    /// Authentifie un utilisateur qui tente de s'authentifier de manière classique (login+mdp)
    /// </summary>
    /// <param name="model"></param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [HttpPost("/do-login", Name = "DoLogin")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userRepository.GetBySecondaryAsync(model.UserName);
            if (user != null && user.PasswordIsValid(model.Password))
            {
                await SignInAsync(user, true);
                return Redirect("/admin/posts");
            }
        }
        ModelState.AddModelError("Error", "Identifiant ou mot de passe incorrect");
        return View(model);
            
    }

    [AllowAnonymous]
    [Route("/logout", Name = "Logout")]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Redirect("/");
    }

    private async Task SignInAsync(BlogUser user, bool isPersistent)
    {
        await HttpContext.SignOutAsync();
        var claims = await BuildClaimsAsync(user);
        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsPrincipal));
    }
    
    private async Task<Claim[]> BuildClaimsAsync(BlogUser user)
    {
        return new[]
        {
            new Claim(ClaimTypes.AuthenticationMethod, "cookie"),
            new Claim(ClaimTypes.Name, user.AccountName),
            new Claim(ClaimTypes.Role, $"{(int)user.Rank}" ),
            new Claim("UserID", user.Id.ToString())
        };
    }
}