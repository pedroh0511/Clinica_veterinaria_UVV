using System.ComponentModel.DataAnnotations;

namespace ClinicaVeterinariaUVV.Models;

public class Consulta
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do animal.")]
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

    [Required(ErrorMessage = "Informe a data e hora da consulta.")]
    [Display(Name = "Data e hora")]
    [DataType(DataType.DateTime)]
    public DateTime DataHora { get; set; }

    [Required(ErrorMessage = "Descreva os sintomas do animal.")]
    [StringLength(1000, MinimumLength = 10,
        ErrorMessage = "A descrição deve ter entre 10 e 1000 caracteres.")]
    [Display(Name = "Sintomas / descrição")]
    [DataType(DataType.MultilineText)]
    public string Descricao { get; set; } = string.Empty;

    public int UsuarioId { get; set; }

    public Usuario? Usuario { get; set; }
}