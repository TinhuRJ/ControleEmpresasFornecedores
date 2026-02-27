namespace ControleEmpresasFornecedores.Api.Entities;

public class EmpresaFornecedor
{
    public int EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;

    public int FornecedorId { get; set; }
    public Fornecedor Fornecedor { get; set; } = null!;
}