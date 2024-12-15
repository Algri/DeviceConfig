using IoT.RPiController.Data.Entities;

namespace IoT.RPiController.Data.Repositories.Abstractions
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByLoginAsync(string login);
    }
}
