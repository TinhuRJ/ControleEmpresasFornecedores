namespace ControleEmpresasFornecedores.Api.DTOs;

public class EmpresaCreateDto
{
    public string Cnpj { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
}