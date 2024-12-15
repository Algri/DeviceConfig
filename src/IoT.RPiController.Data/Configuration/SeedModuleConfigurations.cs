using IoT.RPiController.Data.Auth;
using IoT.RPiController.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IoT.RPiController.Data.Configuration
{
    public class SeedUsers : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder) =>
            builder.HasData(
                new User
                {
                    Id = 1,
                    Login = "admin",
                    Password = AuthHelper.GeneratePasswordHash("admin"),
                    AccessTokenExpirationTime = 480,
                    RefreshTokenExpirationTime = 10080
                },
                new User
                {
                    Id = 2,
                    Login = "string",
                    Password = AuthHelper.GeneratePasswordHash("string"),
                    AccessTokenExpirationTime = 10080,
                    RefreshTokenExpirationTime = 10080
                });
    }

    public class SeedModuleConfigurations : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder) =>
            builder.HasData(
                new Module { Id = 1, Address = 32, ModuleType = "RM8" },
                new Module { Id = 2, Address = 33, ModuleType = "RM8" },
                new Module { Id = 3, Address = 34, ModuleType = "RM8" },
                new Module { Id = 4, Address = 35, ModuleType = "RM8" },
                new Module { Id = 5, Address = 36, ModuleType = "IM8" },
                new Module { Id = 6, Address = 37, ModuleType = "IM8" },
                new Module { Id = 7, Address = 38, ModuleType = "IM8" },
                new Module { Id = 8, Address = 39, ModuleType = "IM8" });
    }
}