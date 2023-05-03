using System.Security.Cryptography;
using System.Text;

namespace Networking
{
    public static class XAPIGenerator 
    {
        public static string GenerateXapiKey(string lastLogin,string token,string key)
        {
            string privateKey = "FSC0C5MLV9YUONLF5M3MGGAG4SOWFTLM";
            string xapiRaw = $"{lastLogin}:{token}:{privateKey}:{key}";
            string md5 = GetMD5(xapiRaw);
            return md5;
        }

        public static string GetMD5(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(Encoding.ASCII.GetBytes(input));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
    }
}