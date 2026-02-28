namespace ControleEmpresasFornecedores.Api.DTOs;

public class FornecedorCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;

    public bool PessoaFisica { get; set; }

    public string? Rg { get; set; }
    public DateTime? DataNascimento { get; set; }
}