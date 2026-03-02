namespace ControleEmpresasFornecedores.Api.DTOs
{
    public class VincularEmpresasDto
    {
        public int FornecedorId { get; set; }
        public List<int> EmpresasIds { get; set; } = new();
    }
}
