using System;
using System.Security.Cryptography;

namespace MyApp.Infrastructure
{

    internal sealed class PasswordHasher
    {
        private const int saltSize = 16; // 128 bits
        private const int hashSize = 32; // 256 bits
        private const int iterations = 100000;


        private static readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA256;

        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                hashAlgorithm,
                hashSize);

            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        public bool Verify(string password, string storedHash)
        {
            string[] parts = storedHash.Split('-');
            if (parts.Length != 2)
            {
                throw new FormatException("Invalid stored hash format.");
            }
            byte[] hash = Convert.FromHexString(parts[0]);
            byte[] salt = Convert.FromHexString(parts[1]);
            byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                hashAlgorithm,
                hashSize);
            return CryptographicOperations.FixedTimeEquals(hash, computedHash);
        }
    }
}