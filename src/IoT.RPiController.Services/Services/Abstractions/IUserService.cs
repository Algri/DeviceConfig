using IoT.RPiController.Data.Entities;
using IoT.RPiController.Services.Models;

namespace IoT.RPiController.Services.Services.Abstractions
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByLoginAsync(string login);
        Task<UserDto?> AddAsync(CreateOrUpdateUserDto userDto);
        Task<UserDto?> UpdateAsync(int id, CreateOrUpdateUserDto userDto);
        // TODO: switch on VOID, it never uses outcome
        Task<User?> UpdateRefreshTokenAsync(int userId, string refreshToken, int refreshTokenExpirationTime);
        Task<bool> ResetRefreshTokenAsync(int userId);
        Task DeleteByIdAsync(int id);
    }
}
