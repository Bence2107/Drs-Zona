using DTOs.Standings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController(IContractsService contractsService) : ControllerBase 
{
    [Authorize(Policy = "EditorOrAdmin")]
    [HttpGet("getAll")]
    public async Task<ActionResult<List<ContractListDto>>> GetAll()
    {
        var response = await contractsService.GetAllContracts();
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok(response.Value);
    }

    [Authorize(Policy = "EditorOrAdmin")]
    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody] ContractCreateDto dto)
    {
        var response = await contractsService.Create(dto);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }
    
    [Authorize(Policy = "EditorOrAdmin")]
    [HttpPost("update/{id:guid}/{driverId:guid}/{teamId:guid}")]
    public async Task<ActionResult> Update(Guid id, Guid driverId, Guid teamId)
    {
        var response = await contractsService.Update(id, driverId, teamId);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }
    
    [Authorize(Policy = "EditorOrAdmin")]
    [HttpDelete("delete/{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var response = await contractsService.Delete(id);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }
}