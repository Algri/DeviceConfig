using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoT.RPiController.WebApi.Controllers
{
    [ApiExplorerSettings(GroupName = "Output Module")]
    [Authorize]
    [Route("api/Module/output")]
    [ApiController]
    public class OutputModuleController(IRelayModuleService relayModuleService) : ControllerBase
    {
        /// <summary>
        /// Get the state information for all output modules.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var state = await relayModuleService.ReadStateAsync();
            return Ok(state);
        }

        /// <summary>
        /// Get the state information for a specific output module by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var state = await relayModuleService.ReadStateByIdAsync(id);
            return Ok(state);
        }

        /// <summary>
        /// Update output module state information.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] IEnumerable<RelayModuleDto> model)
        {
            var state = await relayModuleService.WriteStateAsync(model);
            return Ok(state);
        }
    }
}
