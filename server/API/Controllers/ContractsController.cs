using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController(IContractsService contractsService) : ControllerBase
{
    [HttpGet("getAll")]
    public async Task<ActionResult<List<ContractListDto>>> GetAll()
    {
        var response = await contractsService.GetAllContracts();
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok(response.Value);
    }

    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody] ContractCreateDto dto)
    {
        var response = await contractsService.CreateContract(dto);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }

    [HttpPost("update/{id:guid}/{driverId:guid}/{teamId:guid}")]
    public async Task<ActionResult> Update(Guid id, Guid driverId, Guid teamId)
    {
        var response = await contractsService.UpdateContract(id, driverId, teamId);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var response = await contractsService.DeleteContract(id);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }
}