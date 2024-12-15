using System.Security.Claims;
using AutoMapper;
using IoT.RPiController.Services.Services.Abstractions;
using IoT.RPiController.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoT.RPiController.WebApi.Controllers
{
    [ApiExplorerSettings(GroupName = "Users")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService, IMapper mapper) : ControllerBase
    {
        /// <summary>
        /// Get all users.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get a user by ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await userService.GetByIdAsync(id);
            return user is null ? NoContent() : Ok(mapper.Map<UserDto>(user));
        }

        /// <summary>
        /// Add a new user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateOrUpdateUserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var addedUser = await userService.AddAsync(userDto);
            return Ok(addedUser);
        }

        /// <summary>
        /// Update an existing user by ID.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] CreateOrUpdateUserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedUser = await userService.UpdateAsync(id, userDto);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Delete a user by ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tokenUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id == Convert.ToInt32(tokenUserId))
            {
                return BadRequest("You cannot delete your own account. At least for now.");
            }

            await userService.DeleteByIdAsync(id);
            return Ok("User deleted");
        }
    }
}