public class VincularFornecedoresDto
{
    public int EmpresaId { get; set; }
    public List<int> FornecedoresIds { get; set; } = new();
}