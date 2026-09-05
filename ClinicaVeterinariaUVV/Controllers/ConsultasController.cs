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
        return View(new Consulta
        {
            DataHora = DateTime.Now.AddDays(1)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Consulta consulta)
    {
        ValidarConsulta(consulta);

        if (!ModelState.IsValid)
        {
            return View(consulta);
        }

        consulta.UsuarioId = UsuarioLogadoId;

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

        return View(consulta);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Consulta consulta)
    {
        if (id != consulta.Id)
        {
            return NotFound();
        }

        var consultaExistente = await BuscarConsultaDoUsuario(id);

        if (consultaExistente is null)
        {
            return NotFound();
        }

        ValidarConsulta(consulta);

        if (!ModelState.IsValid)
        {
            return View(consulta);
        }

        consultaExistente.NomePet = consulta.NomePet;
        consultaExistente.Especie = consulta.Especie;
        consultaExistente.Raca = consulta.Raca;
        consultaExistente.IdadePet = consulta.IdadePet;
        consultaExistente.Especialidade = consulta.Especialidade;
        consultaExistente.DataHora = consulta.DataHora;
        consultaExistente.Descricao = consulta.Descricao;

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

    private void ValidarConsulta(Consulta consulta)
    {
        if (consulta.DataHora <= DateTime.Now)
        {
            ModelState.AddModelError(
                nameof(consulta.DataHora),
                "A consulta deve ser agendada para uma data futura.");
        }

        if (consulta.Especialidade == EspecialidadeVeterinaria.ClinicaGeralFelina &&
            !string.Equals(
                consulta.Especie?.Trim(),
                "Gato",
                StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(
                nameof(consulta.Especialidade),
                "Clínica Geral Felina atende somente gatos.");
        }
    }
}