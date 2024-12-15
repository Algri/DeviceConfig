using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;
using IoT.RPiController.Services.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoT.RPiController.WebApi.Controllers
{
    [ApiExplorerSettings(GroupName = "Output Module")]
    [Authorize]
    [Route("api/Module/output/relay")]
    [ApiController]
    public class OutputModuleRelayController(
        IRelayService relayService,
        ITimerTokenService timerTokenService,
        ILogger<OutputModuleRelayController> logger,
        ConcurrentQueueService queueService)
        : ControllerBase
    {
        /// <summary>
        /// Get the relay state information by its address and number.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] RelayBaseDto model)
        {
            var state = await relayService.ReadRelayAsync(model.Address, model.RelayNumber);
            return Ok(state);
        }

        /// <summary>
        /// Update a relay state for a specific output module.
        /// </summary>
        [HttpPost]
        public IActionResult Post([FromBody] RelayStateDto relayStateDto)
        {
            // Enqueue the request to the queue instead of handling it directly
            queueService.EnqueueRequest(
                async () => await relayService.WriteRelayAsync(
                    relayStateDto.Address,
                    relayStateDto.RelayNumber,
                    relayStateDto.State
                )
            );

            return Accepted();
        }

        /// <summary>
        /// Switch a relay state for a specific output module.
        /// </summary>
        [HttpPost("toggle")]
        public IActionResult Toggle([FromBody] RelayBaseDto relayBaseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Enqueue the request to the queue instead of handling it directly
            queueService.EnqueueRequest(async () => { await relayService.ToggleRelayAsync(relayBaseDto); });

            return Ok();
        }

        /// <summary>
        /// Stop the timer.
        /// Urgently cancel the the working relay switch timer.  
        /// </summary>
        /// <param name="relayState">The relay state information including address, relay number, and desired state.</param>
        [HttpDelete("cancel-timer")]
        public IActionResult CancelTimer([FromBody] RelayStateDto relayState)
        {
            timerTokenService.CancelAndRemoveTimer(relayState.Address, relayState.RelayNumber);
            return Ok("Timer canceled successfully.");
        }
    }
}