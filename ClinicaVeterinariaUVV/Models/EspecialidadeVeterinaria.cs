using System.ComponentModel.DataAnnotations;

namespace ClinicaVeterinariaUVV.Models;

public enum EspecialidadeVeterinaria
{
    [Display(Name = "Gastroenterologia")]
    Gastroenterologia,

    [Display(Name = "Clínica Geral Felina")]
    ClinicaGeralFelina
}