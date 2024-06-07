using System.ComponentModel.DataAnnotations;

public class CreateUserModel
{
    [Required]
    [Display(Name = "Nome de Usuário")]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password { get; set; }
}
