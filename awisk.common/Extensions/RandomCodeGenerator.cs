using System.Security.Cryptography;
using System.Text;

namespace awisk.common.Extensions
{
    public static partial class RandomCodeGenerator
    {
        private const string Alphanumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const string Symbols = ",.-_|^=+*()!@$";
        private const string Digits = "0123456789";
        private const string HexChars = "0123456789ABCDEF";

        // ───────────────────────────────
        // ✅ Generate Secure Password
        // ───────────────────────────────
        public static string GeneratePassword(int length = 16, bool includeSymbols = true)
        {
            string valid = includeSymbols ? Alphanumeric + Symbols : Alphanumeric;
            return GenerateFromCharset(valid, length);
        }

        // ───────────────────────────────
        // ✅ Generate Secure OTP (Digits Only)
        // ───────────────────────────────
        public static string GenerateOTP(int length = 6)
        {
            return GenerateFromCharset(Digits, length);
        }

        // ───────────────────────────────
        // ✅ Generate Secure API Token
        // ───────────────────────────────
        public static string GenerateApiToken(int byteLength = 32)
        {
            byte[] bytes = new byte[byteLength];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToHexString(bytes); // uppercase hex string
        }

        // ───────────────────────────────
        // ✅ Generate Secure Short ID (for URLs / filenames)
        // ───────────────────────────────
        public static string GenerateShortId(int length = 10)
        {
            return GenerateFromCharset(Alphanumeric, length);
        }

        // ───────────────────────────────
        // ✅ Generate Secure Hex String
        // ───────────────────────────────
        public static string GenerateHex(int length = 16)
        {
            return GenerateFromCharset(HexChars, length);
        }

        // ───────────────────────────────
        // ✅ Generate Base64 Token (URL Safe)
        // ───────────────────────────────
        public static string GenerateBase64UrlSafeToken(int byteLength = 24)
        {
            byte[] buffer = new byte[byteLength];
            RandomNumberGenerator.Fill(buffer);
            return Convert.ToBase64String(buffer)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        // ───────────────────────────────
        // ✅ Helper Core Generator
        // ───────────────────────────────
        private static string GenerateFromCharset(string charset, int length)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

            if (string.IsNullOrEmpty(charset)) throw new ArgumentNullException(nameof(charset));

            StringBuilder result = new(length);
            for (int i = 0; i < length; i++)
                result.Append(charset[RandomNumberGenerator.GetInt32(charset.Length)]);
            return result.ToString();
        }

        // ───────────────────────────────
        // ✅ Generate Human Readable Code (e.g., Invite Code)
        // ───────────────────────────────
        public static string GenerateReadableCode(int segments = 3, int segmentLength = 4)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < segments; i++)
            {
                if (i > 0) sb.Append('-');
                sb.Append(GenerateFromCharset(Alphanumeric.ToUpper(), segmentLength));
            }
            return sb.ToString();
        }

        // ───────────────────────────────
        // ✅ Generate Cryptographic Salt
        // ───────────────────────────────
        public static string GenerateSalt(int byteLength = 16)
        {
            byte[] salt = new byte[byteLength];
            RandomNumberGenerator.Fill(salt);
            return Convert.ToBase64String(salt);
        }

        // ───────────────────────────────
        // ✅ Generate Secure Hash (SHA-256)
        // ───────────────────────────────
        public static string GenerateHash(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            Span<byte> hash = stackalloc byte[32];
            SHA256.HashData(bytes, hash);
            return Convert.ToHexString(hash);
        }
    }
}
