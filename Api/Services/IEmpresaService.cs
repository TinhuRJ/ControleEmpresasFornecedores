using ControleEmpresasFornecedores.Api.DTOs;

namespace ControleEmpresasFornecedores.Api.Services;

public interface IEmpresaService
{
    Task<IEnumerable<EmpresaResponseDto>> GetAllAsync();
    Task<EmpresaResponseDto?> GetByIdAsync(int id);
    Task<EmpresaResponseDto> CreateAsync(EmpresaCreateDto dto);
    Task<bool> UpdateAsync(int id, EmpresaCreateDto dto);
    Task<bool> DeleteAsync(int id);
}