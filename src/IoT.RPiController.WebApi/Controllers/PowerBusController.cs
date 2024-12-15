using IoT.RPiController.Services.Enums;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoT.RPiController.WebApi.Controllers
{
    [ApiExplorerSettings(GroupName = "Utilities")]
    [Authorize]
    [Route("api/Utilities/PowerBus")]
    [ApiController]
    public class PowerBusController(IPowerBusService powerBusService) : ControllerBase
    {
        /// <summary>
        /// Read the state of a specified power bus.
        /// </summary>
        /// <param name="powerBus">The name of the power bus.</param>
        /// <returns>The state of the power bus.</returns>
        [HttpGet]
        public IActionResult ReadAsync(string powerBus)
        {
            if (!Enum.TryParse<PowerBusEnum>(powerBus, out var parsedPowerBus))
            {
                return BadRequest("Invalid PowerBus value");
            }

            var state = powerBusService.ReadState(parsedPowerBus);
            return Ok(state);
        }

        /// <summary>
        /// Read the states of all power buses.
        /// </summary>
        [HttpGet("all")]
        public IActionResult ReadAllAsync()
        {
            var state = powerBusService.ReadAllStates();
            return Ok(state);
        }

        /// <summary>
        /// Updates I/O state of the power bus.
        /// </summary>
        [HttpPut]
        public IActionResult PutAsync(PowerBusDto powerBusDto)
        {
            if (!Enum.TryParse<PowerBusEnum>(powerBusDto.BusName, out var parsedPowerBus))
            {
                return BadRequest("Invalid PowerBus value");
            }

            var state = powerBusService.SwitchState(parsedPowerBus, powerBusDto.State);
            return Ok(state);
        }
    }
}