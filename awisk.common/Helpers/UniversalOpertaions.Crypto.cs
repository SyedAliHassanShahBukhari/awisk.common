using System;
using System.Buffers;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static string Base64Encode(this string? s)
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(s ?? string.Empty));

        public static string Base64Decode(this string? s)
            => string.IsNullOrEmpty(s) ? string.Empty : Encoding.UTF8.GetString(Convert.FromBase64String(s));

        public static string Sha256Hex(this string? s)
        {
            var data = Encoding.UTF8.GetBytes(s ?? string.Empty);
            Span<byte> hash = stackalloc byte[32];
            SHA256.HashData(data, hash);
            return Convert.ToHexStringLower(hash);
        }

        public static string SecureToken(int byteLength = 32)
        {
            byteLength = Math.Max(1, byteLength);
            byte[] rented = ArrayPool<byte>.Shared.Rent(byteLength);
            try
            {
                RandomNumberGenerator.Fill(rented.AsSpan(0, byteLength));
                return Convert.ToHexStringLower(rented, 0, byteLength);
            }
            finally { ArrayPool<byte>.Shared.Return(rented); }
        }

        public static string RandomNumericCode(int digits = 6)
        {
            digits = Math.Clamp(digits, 1, 32);
            var sb = new StringBuilder(digits);
            Span<byte> buf = stackalloc byte[4];
            for (int i = 0; i < digits; i++)
            {
                RandomNumberGenerator.Fill(buf);
                sb.Append(BitConverter.ToUInt32(buf) % 10);
            }
            return sb.ToString();
        }

        // Optional light AES for config secrets (IV is prefixed to ciphertext).
        public static string EncryptAES(this string plainText, string key)
        {
            using var aes = Aes.Create();
            aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            aes.GenerateIV();
            using var enc = aes.CreateEncryptor(aes.Key, aes.IV);
            var bytes = enc.TransformFinalBlock(Encoding.UTF8.GetBytes(plainText), 0, plainText.Length);
            return Convert.ToBase64String(aes.IV.Concat(bytes).ToArray());
        }

        public static string DecryptAES(this string cipherText, string key)
        {
            var all = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            int ivLen = aes.BlockSize / 8;
            aes.IV = all[..ivLen];
            using var dec = aes.CreateDecryptor(aes.Key, aes.IV);
            var data = dec.TransformFinalBlock(all, ivLen, all.Length - ivLen);
            return Encoding.UTF8.GetString(data);
        }
    }
}
