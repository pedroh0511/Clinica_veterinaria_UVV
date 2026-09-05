using System.ComponentModel.DataAnnotations;

namespace ClinicaVeterinariaUVV.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do tutor.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    [Display(Name = "Nome do tutor")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe uma senha.")]
    [StringLength(100, MinimumLength = 6,
        ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;

    [Display(Name = "Data de cadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
}