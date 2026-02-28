using AutoMapper;
using ControleEmpresasFornecedores.Api.Data;
using ControleEmpresasFornecedores.Api.DTOs;
using ControleEmpresasFornecedores.Api.Entities;
using Microsoft.EntityFrameworkCore;

public class FornecedorService : IFornecedorService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public FornecedorService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<FornecedorResponseDto>> GetAllAsync(string? nome, string? cpfCnpj)
    {
        var query = _context.Fornecedores
            .Include(f => f.EmpresasFornecedores)
            .ThenInclude(ef => ef.Empresa)
            .AsQueryable();

        if(!string.IsNullOrWhiteSpace(nome))
            query = query.Where(f => f.Nome.Contains(nome));

        if(!string.IsNullOrWhiteSpace(cpfCnpj))
            query = query.Where(f => f.Documento.Contains(cpfCnpj));

        var fornecedores = await query.ToListAsync();

        return _mapper.Map<List<FornecedorResponseDto>>(fornecedores);
    }

    public async Task<FornecedorResponseDto?> GetByIdAsync(int id)
    {
        var fornecedor = await _context.Fornecedores
            .Include(f => f.EmpresasFornecedores)
            .ThenInclude(ef => ef.Empresa)
            .FirstOrDefaultAsync(f => f.Id == id);

        if(fornecedor == null) return null;

        return _mapper.Map<FornecedorResponseDto>(fornecedor);
    }

    public async Task<FornecedorResponseDto> CreateAsync(FornecedorCreateDto dto)
    {
        if(dto.PessoaFisica)
        {
            if(!dto.DataNascimento.HasValue)
                throw new Exception("Data de nascimento é obrigatória para pessoa física.");

            var idade = DateTime.Today.Year - dto.DataNascimento.Value.Year;

            if(dto.DataNascimento.Value.Date > DateTime.Today.AddYears(-idade))
                idade--;

            if(idade < 18)
            {
                var empresaPR = await _context.Empresas
                    .AnyAsync(e => e.Cep.StartsWith("8"));

                if(empresaPR)
                    throw new Exception("Fornecedor menor de idade não pode ser vinculado a empresa do Paraná.");
            }
        }

        var fornecedor = _mapper.Map<Fornecedor>(dto);

        var documentoExiste = await _context.Fornecedores.AnyAsync(f => f.Documento == dto.Documento);

        if(documentoExiste)
            throw new Exception("Já existe um fornecedor com este CPF/CNPJ.");

        _context.Fornecedores.Add(fornecedor);
        await _context.SaveChangesAsync();

        return _mapper.Map<FornecedorResponseDto>(fornecedor);
    }

    public async Task<bool> UpdateAsync(int id, FornecedorCreateDto dto)
    {
        var fornecedor = await _context.Fornecedores.FindAsync(id);

        if(fornecedor == null) return false;

        if(fornecedor.Documento != dto.Documento)
        {
            var documentoExiste = await _context.Fornecedores
                .AnyAsync(f => f.Documento == dto.Documento);

            if(documentoExiste)
                throw new Exception("Já existe um fornecedor com este CPF/CNPJ.");
        }

        _mapper.Map(dto, fornecedor);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var fornecedor = await _context.Fornecedores.FindAsync(id);

        if(fornecedor == null) return false;

        _context.Fornecedores.Remove(fornecedor);
        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<List<int>> GetEmpresasIdsByFornecedorAsync(int fornecedorId)
    {
        var fornecedor = await _context.Fornecedores
            .Include(f => f.EmpresasFornecedores)
            .FirstOrDefaultAsync(f => f.Id == fornecedorId);

        if(fornecedor == null)
            return new List<int>();

        return fornecedor.EmpresasFornecedores
            .Select(ef => ef.EmpresaId)
            .ToList();
    }
}