using ControleEmpresasFornecedores.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControleEmpresasFornecedores.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<EmpresaFornecedor> EmpresasFornecedores => Set<EmpresaFornecedor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmpresaFornecedor>()
            .HasKey(ef => new { ef.EmpresaId, ef.FornecedorId });

        modelBuilder.Entity<EmpresaFornecedor>()
            .HasOne(ef => ef.Empresa)
            .WithMany(e => e.EmpresasFornecedores)
            .HasForeignKey(ef => ef.EmpresaId);

        modelBuilder.Entity<EmpresaFornecedor>()
            .HasOne(ef => ef.Fornecedor)
            .WithMany(f => f.EmpresasFornecedores)
            .HasForeignKey(ef => ef.FornecedorId);

        modelBuilder.Entity<Empresa>()
            .HasIndex(e => e.Cnpj)
            .IsUnique();

        modelBuilder.Entity<Fornecedor>()
            .HasIndex(f => f.Documento)
            .IsUnique();
    }
}