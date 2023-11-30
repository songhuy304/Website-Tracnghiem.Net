using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Security.Cryptography;
using System.Text;
namespace DoAnCs.Controllers.Hard
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(salt);
            }

            // Hash the password with the generated salt
            var hashBytes = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = hashBytes.GetBytes(20); // 20 bytes hash

            // Combine the salt and password hash
            byte[] hashWithSalt = new byte[36];
            Array.Copy(salt, 0, hashWithSalt, 0, 16);
            Array.Copy(hash, 0, hashWithSalt, 16, 20);

            // Convert the combined bytes to a string
            string savedPasswordHash = Convert.ToBase64String(hashWithSalt);
            return savedPasswordHash;
        }

        public static bool VerifyPassword(string savedPasswordHash, string password)
        {
            // Extract the bytes from the saved password hash
            byte[] hashWithSalt = Convert.FromBase64String(savedPasswordHash);

            // Get the salt from the saved hash
            byte[] salt = new byte[16];
            Array.Copy(hashWithSalt, 0, salt, 0, 16);

            // Compute the hash with the given password and salt
            var hashBytes = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = hashBytes.GetBytes(20);

            // Compare the computed hash with the saved hash
            for (int i = 0; i < 20; i++)
            {
                if (hashWithSalt[i + 16] != hash[i])
                {
                    return false; // Passwords don't match
                }
            }
            return true; // Passwords match
        }
    }
}