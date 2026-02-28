using System.Collections.Generic;
using System;

namespace ControleEmpresasFornecedores.Api.Entities;

public class Fornecedor
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Documento { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Cep { get; set; } = string.Empty;

    public bool PessoaFisica { get; set; }

    public string? Rg { get; set; }

    public DateTime? DataNascimento { get; set; }    

    public ICollection<EmpresaFornecedor> EmpresasFornecedores { get; set; } = new List<EmpresaFornecedor>();
}