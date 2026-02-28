using Microsoft.AspNetCore.Mvc;
using ControleEmpresasFornecedores.Api.DTOs;

namespace ControleEmpresasFornecedores.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FornecedorController : ControllerBase
{
    private readonly IFornecedorService _service;

    public FornecedorController(IFornecedorService service)
    {
        _service = service;
    }

    [HttpGet("GetFornecedores")]
    public async Task<ActionResult<List<FornecedorResponseDto>>> GetAll(
        [FromQuery] string? nome,
        [FromQuery] string? documento)
    {
        var fornecedores = await _service.GetAllAsync(nome, documento);
        return Ok(fornecedores);
    }

    [HttpGet("GetFornecedorById/{id}")]
    public async Task<ActionResult<FornecedorResponseDto>> GetById(int id)
    {
        var fornecedor = await _service.GetByIdAsync(id);

        if(fornecedor == null)
            return NotFound();

        return Ok(fornecedor);
    }

    [HttpPost("CreateFornecedor")]
    public async Task<ActionResult<FornecedorResponseDto>> Create(FornecedorCreateDto dto)
    {
        var fornecedor = await _service.CreateAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id = fornecedor.Id }, fornecedor);
    }

    [HttpPut("UpdateFornecedor/{id}")]
    public async Task<IActionResult> Update(int id, FornecedorCreateDto dto)
    {
        var atualizado = await _service.UpdateAsync(id, dto);

        if(!atualizado)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("DeleteFornecedor/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _service.DeleteAsync(id);

        if(!removido)
            return NotFound();

        return NoContent();
    }
}