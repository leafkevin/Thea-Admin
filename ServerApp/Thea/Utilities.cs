using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Thea;

public class Utilities
{
    private static int _IterCount = 10000;
    public static string DESEncrypt(string data, string key)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(data);
        var desProvider = TripleDES.Create();
        var byteKey = Encoding.UTF8.GetBytes(key);
        byte[] allKey = new byte[24];
        Buffer.BlockCopy(byteKey, 0, allKey, 0, 16);
        Buffer.BlockCopy(byteKey, 0, allKey, 16, 8);
        desProvider.Key = allKey;
        desProvider.Mode = CipherMode.ECB;
        desProvider.Padding = PaddingMode.PKCS7;
        var transform = desProvider.CreateEncryptor();
        bytes = transform.TransformFinalBlock(bytes, 0, bytes.Length);
        return Convert.ToBase64String(bytes);
    }
    public static string DESDecrypt(string data, string key)
    {
        byte[] bytes = Convert.FromBase64String(data);
        var desProvider = TripleDES.Create();
        var byteKey = Encoding.UTF8.GetBytes(key);
        byte[] allKey = new byte[24];
        Buffer.BlockCopy(byteKey, 0, allKey, 0, 16);
        Buffer.BlockCopy(byteKey, 0, allKey, 16, 8);
        desProvider.Key = allKey;
        desProvider.Mode = CipherMode.ECB;
        desProvider.Padding = PaddingMode.PKCS7;
        var transform = desProvider.CreateDecryptor();
        bytes = transform.TransformFinalBlock(bytes, 0, bytes.Length);
        return Encoding.UTF8.GetString(bytes);
    }
    public static string HashPassword(string password, out string salt)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        salt = GenerateSalt(out var saltBytes);
        var bytes = HashPassword(password, _IterCount, saltBytes, 32);
        return Convert.ToBase64String(bytes);
    }
    public static bool VerifyPassword(string password, string salt, string hashedPassword)
    {
        byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);
        if (decodedHashedPassword.Length == 0)
            return false;
        return VerifyHashedPassword(decodedHashedPassword, password, salt, out _);
    }
    private static bool VerifyHashedPassword(byte[] hashedPassword, string password, string salt, out int iterCount)
    {
        iterCount = default(int);
        try
        {
            // Read header information
            KeyDerivationPrf prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 0);
            iterCount = (int)ReadNetworkByteOrder(hashedPassword, 4);
            int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 8);

            // Read the salt: must be >= 128 bits
            if (saltLength < 16)
                return false;

            byte[] saltBytes = Convert.FromBase64String(salt);
            if (saltBytes.Length != saltLength)
                return false;

            Buffer.BlockCopy(hashedPassword, 12, saltBytes, 0, saltBytes.Length);

            // Read the subkey (the rest of the payload): must be >= 128 bits
            int subkeyLength = hashedPassword.Length - 12 - saltBytes.Length;
            if (subkeyLength < 128 / 8)
            {
                return false;
            }
            byte[] expectedSubkey = new byte[subkeyLength];
            Buffer.BlockCopy(hashedPassword, 12 + saltBytes.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the incoming password and verify it
            byte[] actualSubkey = KeyDerivation.Pbkdf2(password, saltBytes, prf, iterCount, subkeyLength);

            return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
        }
        catch
        {
            return false;
        }
    }
    private static byte[] HashPassword(string password, int iterCount, byte[] saltBytes, int numBytesRequested)
    {
        // Produce a version 3 (see comment above) text hash.
        var saltSize = saltBytes.Length;
        var prf = KeyDerivationPrf.HMACSHA256;
        byte[] subkey = KeyDerivation.Pbkdf2(password, saltBytes, prf, iterCount, numBytesRequested);
        var outputBytes = new byte[12 + saltSize + subkey.Length];
        WriteNetworkByteOrder(outputBytes, 0, (uint)prf);
        WriteNetworkByteOrder(outputBytes, 4, (uint)iterCount);
        WriteNetworkByteOrder(outputBytes, 8, (uint)saltSize);
        Buffer.BlockCopy(saltBytes, 0, outputBytes, 12, saltSize);
        Buffer.BlockCopy(subkey, 0, outputBytes, 12 + saltSize, subkey.Length);
        return outputBytes;
    }
    private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
    {
        buffer[offset + 0] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)(value >> 0);
    }
    private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
    {
        return ((uint)(buffer[offset + 0]) << 24)
            | ((uint)(buffer[offset + 1]) << 16)
            | ((uint)(buffer[offset + 2]) << 8)
            | ((uint)(buffer[offset + 3]));
    }
    private static string GenerateSalt(out byte[] saltBytes)
    {
        saltBytes = new byte[16];
        var generator = RandomNumberGenerator.Create();
        generator.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }
}
