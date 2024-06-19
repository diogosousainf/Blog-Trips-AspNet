using System.ComponentModel.DataAnnotations;

public class EditUserModel
{
    public string Id { get; set; }

    [Required]
    [Display(Name = "Nome de User")]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
}
