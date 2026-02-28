using ControleEmpresasFornecedores.Api.DTOs;
using ControleEmpresasFornecedores.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ControleEmpresasFornecedores.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresasController : ControllerBase
{
    private readonly IEmpresaService _service;

    public EmpresasController(IEmpresaService service)
    {
        _service = service;
    }

    [HttpGet("GetEmpresas")]
    public async Task<IActionResult> Get()
    {
        var empresas = await _service.GetAllAsync();
        return Ok(empresas);
    }

    [HttpGet("GetEmpresa/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var empresa = await _service.GetByIdAsync(id);

        if(empresa == null) return NotFound();

        return Ok(empresa);
    }

    [HttpPost("CreateEmpresa")]
    public async Task<IActionResult> Post([FromBody] EmpresaCreateDto dto)
    {
        var empresa = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = empresa.Id }, empresa);
    }

    [HttpPut("UpdateEmpresa/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] EmpresaCreateDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);

        if(!updated) return NotFound();

        return NoContent();
    }

    [HttpDelete("DeleteEmpresa/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if(!deleted) return NotFound();

        return NoContent();
    }
}