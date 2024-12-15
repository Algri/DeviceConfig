using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoT.RPiController.WebApi.Controllers
{
    [ApiExplorerSettings(GroupName = "Input Module")]
    [Authorize]
    [Route("api/Module/input")]
    [ApiController]
    public class InputModuleController(IInputModuleService inputModuleService) : ControllerBase
    {
        /// <summary>
        /// Get state information for a specific input module by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var state = await inputModuleService.ReadStateByIdAsync(id);
            return Ok(state);
        }

        /// <summary>
        /// Get state information for all input modules.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var state = await inputModuleService.ReadStateAsync();
            return Ok(state);
        }
    }
}
