using System.Security.Cryptography;
using System.Text;

namespace IoT.RPiController.Data.Auth
{
    public static class AuthHelper
    {
        public static string GeneratePasswordHash(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(passwordBytes);
            string passwordHash = Convert.ToBase64String(hash);

            return passwordHash;
        }

        #region wrappers

        /// <summary>
        /// Asynchronous approach to free the computational powers.
        /// </summary>
        public static Task<string> GeneratePasswordHashAsync(string password) => Task.Run(() => GeneratePasswordHash(password));

        /// <summary>
        /// Compare a plain password to the hashed value.
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <param name="hashedPassword">Hashed value of comparable password</param>
        /// <returns>true - if same, false otherwise</returns>
        public static bool ComparePasswordsHash(string password, string hashedPassword)
        {
            using var sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(passwordBytes);
            string passwordHash = Convert.ToBase64String(hash);

            return passwordHash == hashedPassword;
        }

        #endregion
    }
}
