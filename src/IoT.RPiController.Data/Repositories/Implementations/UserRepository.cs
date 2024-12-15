using IoT.RPiController.Data.Entities;
using IoT.RPiController.Data.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IoT.RPiController.Data.Repositories.Implementations;

public class UserRepository(RPiContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByLoginAsync(string login) => await _dbSet.FirstOrDefaultAsync(x => x.Login == login);
}