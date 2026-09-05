using System.Security.Claims;
using ClinicaVeterinariaUVV.Data;
using ClinicaVeterinariaUVV.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVeterinariaUVV.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Cadastro()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastro(Usuario usuario)
    {
        if (!ModelState.IsValid)
        {
            return View(usuario);
        }

        var emailJaCadastrado = await _context.Usuarios
            .AnyAsync(item => item.Email == usuario.Email);

        if (emailJaCadastrado)
        {
            ModelState.AddModelError(
                nameof(usuario.Email),
                "Este e-mail já está cadastrado.");

            return View(usuario);
        }

        usuario.DataCadastro = DateTime.Now;

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Cadastro realizado. Faça login para continuar.";

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(item =>
                item.Email == model.Email &&
                item.Senha == model.Senha);

        if (usuario is null)
        {
            ModelState.AddModelError(
                string.Empty,
                "E-mail ou senha inválidos.");

            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email)
        };

        var identidade = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identidade);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToAction("Index", "Consultas");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }
}