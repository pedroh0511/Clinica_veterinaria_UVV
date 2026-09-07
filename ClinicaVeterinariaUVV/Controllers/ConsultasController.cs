using System.Security.Claims;
using ClinicaVeterinariaUVV.Data;
using ClinicaVeterinariaUVV.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVeterinariaUVV.Controllers;

[Authorize]
public class ConsultasController : Controller
{
    private readonly AppDbContext _context;

    public ConsultasController(AppDbContext context)
    {
        _context = context;
    }

    private int UsuarioLogadoId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var consultas = await _context.Consultas
            .Where(consulta => consulta.UsuarioId == UsuarioLogadoId)
            .OrderBy(consulta => consulta.DataHora)
            .ToListAsync();

        return View(consultas);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new ConsultaFormViewModel
        {
            Data = DateTime.Today.AddDays(1),
            Horario = "08:00"
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ConsultaFormViewModel model)
    {
        if (!TentarMontarDataHora(model, out var dataHora))
        {
            ModelState.AddModelError(
                nameof(model.Horario),
                "Selecione um horário válido.");
        }

        if (ModelState.IsValid)
        {
            ValidarConsulta(model, dataHora);
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var consulta = new Consulta
        {
            NomePet = model.NomePet,
            Especie = model.Especie,
            Raca = model.Raca,
            IdadePet = model.IdadePet,
            Especialidade = model.Especialidade,
            DataHora = dataHora,
            Descricao = model.Descricao,
            UsuarioId = UsuarioLogadoId
        };

        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Consulta veterinária agendada com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var consulta = await BuscarConsultaDoUsuario(id.Value);

        if (consulta is null)
        {
            return NotFound();
        }

        return View(ParaViewModel(consulta));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ConsultaFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        var consulta = await BuscarConsultaDoUsuario(id);

        if (consulta is null)
        {
            return NotFound();
        }

        if (!TentarMontarDataHora(model, out var dataHora))
        {
            ModelState.AddModelError(
                nameof(model.Horario),
                "Selecione um horário válido.");
        }

        if (ModelState.IsValid)
        {
            ValidarConsulta(model, dataHora);
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        consulta.NomePet = model.NomePet;
        consulta.Especie = model.Especie;
        consulta.Raca = model.Raca;
        consulta.IdadePet = model.IdadePet;
        consulta.Especialidade = model.Especialidade;
        consulta.DataHora = dataHora;
        consulta.Descricao = model.Descricao;

        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Consulta atualizada com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var consulta = await BuscarConsultaDoUsuario(id.Value);

        if (consulta is null)
        {
            return NotFound();
        }

        return View(consulta);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var consulta = await BuscarConsultaDoUsuario(id);

        if (consulta is null)
        {
            return NotFound();
        }

        _context.Consultas.Remove(consulta);
        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Consulta excluída com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    private async Task<Consulta?> BuscarConsultaDoUsuario(int id)
    {
        return await _context.Consultas
            .FirstOrDefaultAsync(consulta =>
                consulta.Id == id &&
                consulta.UsuarioId == UsuarioLogadoId);
    }

    private static ConsultaFormViewModel ParaViewModel(Consulta consulta)
    {
        return new ConsultaFormViewModel
        {
            Id = consulta.Id,
            NomePet = consulta.NomePet,
            Especie = consulta.Especie,
            Raca = consulta.Raca,
            IdadePet = consulta.IdadePet,
            Especialidade = consulta.Especialidade,
            Data = consulta.DataHora.Date,
            Horario = consulta.DataHora.ToString("HH:mm"),
            Descricao = consulta.Descricao
        };
    }

    private static bool TentarMontarDataHora(
        ConsultaFormViewModel model,
        out DateTime dataHora)
    {
        dataHora = default;

        var horariosPermitidos = new HashSet<string>
        {
            "08:00", "08:30",
            "09:00", "09:30",
            "10:00", "10:30",
            "11:00", "11:30",
            "12:00", "12:30",
            "13:00", "13:30",
            "14:00", "14:30",
            "15:00", "15:30",
            "16:00", "16:30",
            "17:00", "17:30",
            "18:00"
        };

        if (!horariosPermitidos.Contains(model.Horario))
        {
            return false;
        }

        if (!TimeSpan.TryParse(model.Horario, out var horario))
        {
            return false;
        }

        dataHora = model.Data.Date.Add(horario);

        return true;
    }

    private void ValidarConsulta(
        ConsultaFormViewModel model,
        DateTime dataHora)
    {
        if (dataHora <= DateTime.Now)
        {
            ModelState.AddModelError(
                nameof(model.Data),
                "A consulta deve ser agendada para uma data futura.");
        }

        if (model.Especialidade == EspecialidadeVeterinaria.ClinicaGeralFelina &&
            !string.Equals(
                model.Especie?.Trim(),
                "Gato",
                StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(
                nameof(model.Especialidade),
                "Clínica Geral Felina atende somente gatos.");
        }
    }
}