using IoT.RPiController.Data.Entities;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoT.RPiController.WebApi.Controllers;

[ApiExplorerSettings(GroupName = "Relay Information")]
[Authorize]
[Route("api/relay-info")]
[ApiController]
public class RelayInfoController(IRelayInfoService relayInfoService) : ControllerBase
{
    /// <summary>
    /// Get All relays information.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var timers = await relayInfoService.GetAllAsync();
        return timers.Any() ? Ok(timers) : NoContent();
    }

    /// <summary>
    /// Get relay information by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var timers = await relayInfoService.GetByIdAsync(id);
        return timers is null ? NoContent() : Ok(timers);
    }

    /// <summary>
    /// Add a batch information details for relays.
    /// </summary>
    [HttpPost("batch")]
    public async Task<IActionResult> PostBatch([FromBody] List<RelayInfoDto> relayInfos)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await relayInfoService.AddBatchAsync(relayInfos);
        return result is not null ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Update a batch information details for relays.
    /// </summary>
    [HttpPut("batch")]
    public async Task<IActionResult> Put([FromBody] List<RelayInfoDto> relayInfos)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await relayInfoService.UpdateBatchAsync(relayInfos);
        return result is not null ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Delete a batch information details for relays.
    /// </summary>
    [HttpDelete("batch")]
    public async Task<IActionResult> Delete(int[] ids)
    {
        var result = await relayInfoService.DeleteBatchAsync(ids);
        return result ? Ok($"Module records deleted successfully. Ids: {string.Join(", ", ids)}") : NotFound();
    }
}