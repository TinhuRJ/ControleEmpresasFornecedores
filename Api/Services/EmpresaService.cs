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
                Estado = e.Estado,
                FornecedoresCount = e.EmpresasFornecedores.Count()
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
            Estado = empresa.Estado,
            FornecedoresCount = empresa.EmpresasFornecedores.Count()
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

        var cnpjExiste = await _context.Empresas.AnyAsync(e => e.Cnpj == dto.Cnpj);

        if(cnpjExiste)
            throw new Exception("Já existe uma empresa com este CNPJ.");

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

        if(empresa.Cnpj != dto.Cnpj)
        {
            var cnpjExiste = await _context.Empresas
                .AnyAsync(e => e.Cnpj == dto.Cnpj);

            if(cnpjExiste)
                throw new Exception("Já existe uma empresa com este CNPJ.");
        }

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

    public async Task<int?> VincularFornecedoresAsync(int empresaId, List<int> fornecedoresIds)
    {
        var empresa = await _context.Empresas
            .Include(e => e.EmpresasFornecedores)
            .FirstOrDefaultAsync(e => e.Id == empresaId);

        if(empresa == null)
            return null;

        fornecedoresIds ??= new List<int>();

        // mantém só IDs válidos (existentes)
        var fornecedoresValidos = await _context.Fornecedores
            .Where(f => fornecedoresIds.Contains(f.Id))
            .Select(f => f.Id)
            .ToListAsync();

        var atuais = empresa.EmpresasFornecedores.Select(x => x.FornecedorId).ToHashSet();

        var paraAdicionar = fornecedoresValidos.Where(id => !atuais.Contains(id)).ToList();
        var paraRemover = atuais.Where(id => !fornecedoresValidos.Contains(id)).ToList();

        if(paraRemover.Count > 0)
        {
            var remove = empresa.EmpresasFornecedores.Where(x => paraRemover.Contains(x.FornecedorId)).ToList();

            foreach(var item in remove)
            {
                empresa.EmpresasFornecedores.Remove(item);
            }
        }

        foreach(var fornecedorId in paraAdicionar)
        {
            empresa.EmpresasFornecedores.Add(new EmpresaFornecedor
            {
                EmpresaId = empresaId,
                FornecedorId = fornecedorId
            });
        }

        await _context.SaveChangesAsync();

        return empresa.EmpresasFornecedores.Count;
    }
    public async Task<List<int>> GetFornecedoresIdsByEmpresaAsync(int empresaId)
    {
        return await _context.EmpresasFornecedores
            .AsNoTracking()
            .Where(x => x.EmpresaId == empresaId)
            .Select(x => x.FornecedorId)
            .ToListAsync();
    }
}