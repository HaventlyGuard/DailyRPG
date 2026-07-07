using System.Security.Cryptography;

namespace identity_service.Core.Security;

public class PasswordHasher
{
    private const int SaltSize = 16; // 128 бит
    private const int HashSize = 32; // 256 бит
    private const int Iterations = 100000; // Количество итераций PBKDF2

    /// <summary>
    /// Хэширует пароль с солью
    /// </summary>
    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize
        );

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Проверяет пароль на соответствие хэшу
    /// </summary>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.');
        if (parts.Length != 3)
            return false;

        int iterations = int.Parse(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] storedHash = Convert.FromBase64String(parts[2]);

        byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            storedHash.Length
        );

        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }
}