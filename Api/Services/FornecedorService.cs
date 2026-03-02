using AutoMapper;
using ControleEmpresasFornecedores.Api.Data;
using ControleEmpresasFornecedores.Api.DTOs;
using ControleEmpresasFornecedores.Api.Entities;
using ControleEmpresasFornecedores.Api.Services;
using Microsoft.EntityFrameworkCore;

public class FornecedorService : IFornecedorService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICepService _cepService;

    public FornecedorService(AppDbContext context, IMapper mapper, ICepService cepService)
    {
        _context = context;
        _mapper = mapper;
        _cepService = cepService;
    }

    public async Task<List<FornecedorResponseDto>> GetAllAsync(string? nome, string? cpfCnpj)
    {
        var query = _context.Fornecedores
            .AsNoTracking()
            .AsQueryable();

        if(!string.IsNullOrWhiteSpace(nome))
            query = query.Where(f => f.Nome.Contains(nome));

        if(!string.IsNullOrWhiteSpace(cpfCnpj))
            query = query.Where(f => f.Documento.Contains(cpfCnpj));

        return await query
            .Select(f => new FornecedorResponseDto
            {
                Id = f.Id,
                Nome = f.Nome,
                Documento = f.Documento,
                Email = f.Email,
                Cep = f.Cep,
                PessoaFisica = f.PessoaFisica,                
                EmpresasCount = f.EmpresasFornecedores.Count(),                
                Rg = f.Rg,
                DataNascimento = f.DataNascimento
            })
            .ToListAsync();
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

        var cepValido = await _cepService.CepValidoAsync(dto.Cep);

        if(!cepValido)
            throw new Exception("CEP inválido.");

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

        if(fornecedor.Cep != dto.Cep) 
        {
            var cepValido = await _cepService.CepValidoAsync(dto.Cep);

            if(!cepValido)
                throw new Exception("CEP inválido.");
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
        return await _context.EmpresasFornecedores
            .AsNoTracking()
            .Where(x => x.FornecedorId == fornecedorId)
            .Select(x => x.EmpresaId)
            .ToListAsync();
    }

    public async Task<int?> VincularEmpresasAoFornecedorAsync(int fornecedorId, List<int> empresasIds)
    {        
        var fornecedorExiste = await _context.Fornecedores.AnyAsync(f => f.Id == fornecedorId);
        if(!fornecedorExiste) return null;

        empresasIds ??= new List<int>();
                
        var empresasValidas = await _context.Empresas
            .Where(e => empresasIds.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync();

        var atuais = await _context.EmpresasFornecedores
            .Where(x => x.FornecedorId == fornecedorId)
            .Select(x => x.EmpresaId)
            .ToListAsync();

        var atuaisSet = atuais.ToHashSet();

        var paraAdicionar = empresasValidas.Where(id => !atuaisSet.Contains(id)).ToList();
        var paraRemover = atuais.Where(id => !empresasValidas.Contains(id)).ToList();

        if(paraRemover.Count > 0)
        {
            var linksRemover = await _context.EmpresasFornecedores
                .Where(x => x.FornecedorId == fornecedorId && paraRemover.Contains(x.EmpresaId))
                .ToListAsync();

            _context.EmpresasFornecedores.RemoveRange(linksRemover);
        }

        foreach(var empresaId in paraAdicionar)
        {
            _context.EmpresasFornecedores.Add(new EmpresaFornecedor
            {
                EmpresaId = empresaId,
                FornecedorId = fornecedorId
            });
        }

        await _context.SaveChangesAsync();
                
        var count = await _context.EmpresasFornecedores
            .CountAsync(x => x.FornecedorId == fornecedorId);

        return count;
    }
}