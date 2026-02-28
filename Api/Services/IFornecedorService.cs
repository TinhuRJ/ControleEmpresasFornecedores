using ControleEmpresasFornecedores.Api.DTOs;

public interface IFornecedorService
{
    Task<List<FornecedorResponseDto>> GetAllAsync(string? nome, string? cpfCnpj);
    Task<FornecedorResponseDto?> GetByIdAsync(int id);
    Task<FornecedorResponseDto> CreateAsync(FornecedorCreateDto dto);
    Task<bool> UpdateAsync(int id, FornecedorCreateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<List<int>> GetEmpresasIdsByFornecedorAsync(int fornecedorId);
}