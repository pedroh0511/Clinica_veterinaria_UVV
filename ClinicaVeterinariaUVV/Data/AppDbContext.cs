using ClinicaVeterinariaUVV.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVeterinariaUVV.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Consulta> Consultas => Set<Consulta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(usuario => usuario.Email)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasMany(usuario => usuario.Consultas)
            .WithOne(consulta => consulta.Usuario)
            .HasForeignKey(consulta => consulta.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}