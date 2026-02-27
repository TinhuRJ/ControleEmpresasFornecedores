using System.Collections.Generic;

namespace ControleEmpresasFornecedores.Api.Entities;

public class Empresa
{
    public int Id { get; set; }

    public string Cnpj { get; set; } = string.Empty;

    public string NomeFantasia { get; set; } = string.Empty;

    public string Cep { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public ICollection<EmpresaFornecedor> EmpresasFornecedores { get; set; } = new List<EmpresaFornecedor>();
}