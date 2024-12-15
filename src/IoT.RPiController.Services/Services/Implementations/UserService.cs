using AutoMapper;
using IoT.RPiController.Data.Entities;
using IoT.RPiController.Data.Repositories.Abstractions;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.Extensions.Logging;
using IoT.RPiController.Data.Auth;
using Microsoft.EntityFrameworkCore;

namespace IoT.RPiController.Services.Services.Implementations;

public class UserService(
    IUserRepository repository,
    ILogger<UserService> logger,
    IMapper mapper) : IUserService
{
    public async Task<UserDto?> AddAsync(CreateOrUpdateUserDto userDto)
    {
        ArgumentNullException.ThrowIfNull(userDto);

        logger.LogInformation($"Adding user: {userDto.Login}");

        var userByLogin = await repository.GetByLoginAsync(userDto.Login);
        if (userByLogin is not null)
        {
            var msg = $"User with login {userDto.Login} already exists. Cannot duplicate user.";

            logger.LogWarning(msg);
            throw new ArgumentException(msg);
        }

        SetDefaultExpirationTimes(userDto);
        // Map the UserDto to the User entity
        var user = mapper.Map<User>(userDto);

        user.Password = await user.GetPasswordHash();

        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        return mapper.Map<UserDto>(user);
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await repository.GetAllAsync();
        return mapper.Map<List<UserDto>>(users);
    }

    public async Task<User?> GetByIdAsync(int id) => await repository.GetByIdAsync(id);

    public async Task DeleteByIdAsync(int id)
    {
        logger.LogInformation($"Deleting user with ID: {id}");

        try
        {
            await repository.DeleteAsync(id);
            await repository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, $"Error on deleting the user with ID: {id}.");
            throw;
        }

        logger.LogInformation($"User with ID: {id} deleted successfully.");
    }

    public async Task<UserDto?> UpdateAsync(int id, CreateOrUpdateUserDto userDto)
    {
        ArgumentNullException.ThrowIfNull(userDto);

        logger.LogInformation($"Updating user with ID: {id}");

        var existingUser = await repository.GetByIdAsync(id);
        if (existingUser is null)
        {
            var msg = $"User with ID {id} not found. Update aborted.";
            logger.LogWarning(msg);

            throw new ArgumentException(msg);
        }

        try
        {
            SetDefaultExpirationTimes(userDto);
            // Map the UserDto to the User entity
            var user = mapper.Map<User>(userDto);

            // Check if the password is explicitly set and update if necessary
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.Password = await AuthHelper.GeneratePasswordHashAsync(userDto.Password);
            }

            // Ensure the existing values that are not provided in the DTO are retained
            user.Id = id;

            // Update refresh token date
            user.RefreshTokenExpiryTime = GetRefreshTokenExpirationDateTime(user.RefreshTokenExpirationTime);

            // Detach the existing entity to avoid conflicts
            repository.Detach(existingUser);

            // Update the user entity
            repository.Update(user);
            await repository.SaveChangesAsync();

            logger.LogInformation($"User updated successfully with ID: {id}");
            return mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error updating user with ID: {id}. InnerException: {ex.InnerException?.Message}");

            throw new InvalidOperationException("An error occurred while updating the user. Please ensure the login is unique and try again.", ex);
        }
    }

    public async Task<User?> GetByLoginAsync(string login) => await repository.GetByLoginAsync(login);

    /// <summary>
    /// Updates the refresh token and its expiration time for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="refreshToken">The new refresh token.</param>
    /// <param name="refreshTokenExpirationMinutes">The expiration time of the refresh token in minutes.</param>
    /// <returns>The updated user object, or null if the user does not exist.</returns>
    public async Task<User?> UpdateRefreshTokenAsync(int userId, string refreshToken, int refreshTokenExpirationMinutes)
    {
        if (refreshTokenExpirationMinutes < 0)
        {
            const string errorMessage = "The Refresh token expiration span cannot be less then 0.";

            logger.LogError(errorMessage);
            throw new ArgumentException(errorMessage);
        }

        var existingUser = await repository.GetByIdAsync(userId);
        if (existingUser is null)
        {
            var msg = $"User with ID {userId} not found. Update aborted.";

            logger.LogWarning(msg);
            throw new ArgumentException(msg);
        }

        // Only update the refresh token if it has changed
        if (refreshToken == existingUser.RefreshToken && refreshTokenExpirationMinutes == existingUser.RefreshTokenExpirationTime)
            return existingUser;

        existingUser.RefreshToken = refreshToken;
        existingUser.RefreshTokenExpirationTime = refreshTokenExpirationMinutes;

        existingUser.RefreshTokenExpiryTime = GetRefreshTokenExpirationDateTime(refreshTokenExpirationMinutes);

        repository.Update(existingUser);
        await repository.SaveChangesAsync();

        return existingUser;
    }

    /// <summary>
    /// Resets the refresh token and its expiry time for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The flag whether the reset was successful.</returns>
    public async Task<bool> ResetRefreshTokenAsync(int userId)
    {
        var existingUser = await repository.GetByIdAsync(userId);
        if (existingUser is null)
        {
            logger.LogWarning($"User with ID {userId} not found. Update aborted.");
            return false;
        }

        existingUser.RefreshToken = string.Empty;
        existingUser.RefreshTokenExpiryTime = GetRefreshTokenExpirationDateTime(0);

        repository.Update(existingUser);
        await repository.SaveChangesAsync();

        return true;
    }

    private static void SetDefaultExpirationTimes(CreateOrUpdateUserDto userDto)
    {
        var infiniteLifeSpan = (int)(DateTime.Now.AddYears(10) - DateTime.Now).TotalMinutes;
        userDto.AccessTokenExpirationTime = userDto.AccessTokenExpirationTime == 0 ? infiniteLifeSpan : userDto.AccessTokenExpirationTime;
        userDto.RefreshTokenExpirationTime = userDto.RefreshTokenExpirationTime == 0 ? infiniteLifeSpan : userDto.RefreshTokenExpirationTime;
    }

    private static DateTime GetRefreshTokenExpirationDateTime(int refreshTokenExpirationSpan) =>
        DateTime.UtcNow.Add(TimeSpan.FromMinutes(refreshTokenExpirationSpan));
}