namespace ControleEmpresasFornecedores.Api.DTOs;

public class EmpresaResponseDto
{
    public int Id { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}