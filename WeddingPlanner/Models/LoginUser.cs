#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models;

[NotMapped]
public class LoginUser {


    [Required(ErrorMessage ="is required")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string LoginEmail { get; set; }

    [Required(ErrorMessage ="is required.")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage ="must be at least 8 charcters.")]
    [Display(Name ="Password")]
    public string LoginPassword { get; set; }
}