using System.Security.Cryptography;
using System.Text;

namespace awisk.common.Extensions
{
    public static partial class RandomCodeGenerator
    {
        public static string GeneratePassword(int length = 32)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890,.-_|^=+*()!@$";
            StringBuilder res = new();
            while (0 < length--)
            {
                res.Append(valid[RandomNumberGenerator.GetInt32(valid.Length)]);
            }
            return res.ToString();
        }
        public static string GenerateOTP(int length = 6)
        {
            // OTPs are usually numeric, so we use digits only
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new();

            while (0 < length--)
            {
                res.Append(valid[RandomNumberGenerator.GetInt32(valid.Length)]);
            }

            return res.ToString();
        }
    }
}
