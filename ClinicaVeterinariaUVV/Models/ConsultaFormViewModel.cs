using System.ComponentModel.DataAnnotations;

namespace ClinicaVeterinariaUVV.Models;

public class ConsultaFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do pet.")]
    [StringLength(80)]
    [Display(Name = "Nome do pet")]
    public string NomePet { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a espécie do animal.")]
    [StringLength(50)]
    [Display(Name = "Espécie")]
    public string Especie { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Raça")]
    public string? Raca { get; set; }

    [Range(0, 40, ErrorMessage = "Informe uma idade válida.")]
    [Display(Name = "Idade do pet")]
    public int? IdadePet { get; set; }

    [Required(ErrorMessage = "Selecione a especialidade.")]
    [Display(Name = "Especialidade")]
    public EspecialidadeVeterinaria Especialidade { get; set; }

    [Required(ErrorMessage = "Informe a data.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data")]
    public DateTime Data { get; set; } = DateTime.Today.AddDays(1);

    [Required(ErrorMessage = "Informe o horário.")]
    [Display(Name = "Horário")]
    public string Horario { get; set; } = "08:00";

    [Required(ErrorMessage = "Descreva os sintomas.")]
    [StringLength(
        1000,
        MinimumLength = 10,
        ErrorMessage = "A descrição deve ter entre 10 e 1000 caracteres.")]
    [Display(Name = "Sintomas / descrição")]
    [DataType(DataType.MultilineText)]
    public string Descricao { get; set; } = string.Empty;
}