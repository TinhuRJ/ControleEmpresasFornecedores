using ControleEmpresasFornecedores.Api.Data;
using ControleEmpresasFornecedores.Api.DTOs;
using ControleEmpresasFornecedores.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControleEmpresasFornecedores.Api.Services;

public class EmpresaService : IEmpresaService
{
    private readonly AppDbContext _context;

    public EmpresaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmpresaResponseDto>> GetAllAsync()
    {
        return await _context.Empresas
            .Select(e => new EmpresaResponseDto
            {
                Id = e.Id,
                Cnpj = e.Cnpj,
                NomeFantasia = e.NomeFantasia,
                Cep = e.Cep,
                Estado = e.Estado
            })
            .ToListAsync();
    }

    public async Task<EmpresaResponseDto?> GetByIdAsync(int id)
    {
        var empresa = await _context.Empresas.FindAsync(id);

        if(empresa == null) return null;

        return new EmpresaResponseDto
        {
            Id = empresa.Id,
            Cnpj = empresa.Cnpj,
            NomeFantasia = empresa.NomeFantasia,
            Cep = empresa.Cep,
            Estado = empresa.Estado
        };
    }

    public async Task<EmpresaResponseDto> CreateAsync(EmpresaCreateDto dto)
    {
        var empresa = new Empresa
        {
            Cnpj = dto.Cnpj,
            NomeFantasia = dto.NomeFantasia,
            Cep = dto.Cep,
            Estado = string.Empty // vamos preencher depois via CEP
        };

        _context.Empresas.Add(empresa);
        await _context.SaveChangesAsync();

        return new EmpresaResponseDto
        {
            Id = empresa.Id,
            Cnpj = empresa.Cnpj,
            NomeFantasia = empresa.NomeFantasia,
            Cep = empresa.Cep,
            Estado = empresa.Estado
        };
    }

    public async Task<bool> UpdateAsync(int id, EmpresaCreateDto dto)
    {
        var empresa = await _context.Empresas.FindAsync(id);

        if(empresa == null) return false;

        empresa.Cnpj = dto.Cnpj;
        empresa.NomeFantasia = dto.NomeFantasia;
        empresa.Cep = dto.Cep;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var empresa = await _context.Empresas.FindAsync(id);

        if(empresa == null) return false;

        _context.Empresas.Remove(empresa);
        await _context.SaveChangesAsync();

        return true;
    }
}