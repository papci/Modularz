using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Modularz.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Veuillez saisir votre nom d'utilisateur")]
    [Display(Name = "Nom d’utilisateur")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Veuillez saisir votre mot de passe")]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; }
    



}