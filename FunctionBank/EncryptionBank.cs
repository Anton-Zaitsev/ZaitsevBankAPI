using System.Security.Cryptography;
using System.Text;

namespace ZaitsevBankAPI.FunctionBank
{
    public static class EncryptionBank
    {
        
        public static string sha256(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public static string EncryptCard(string clearText,string userID)
        {
            string key256 = userID.Replace("-", "").Substring(0, 32); // 32 bytes for AES256
            string IV = "ZaitsevBank_API_"; // 16 bytes for IV

            byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                encryptor.Key = Encoding.UTF8.GetBytes(key256);
                encryptor.IV = Encoding.UTF8.GetBytes(IV);

                using (MemoryStream ms = new ())
                {
                    using (CryptoStream cs = new (ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }


        public static string DecryptCard(string cipherText, string userID)
        {
            string key256 = userID.Replace("-","").Substring(0,32); // 32 bytes for AES256
            string IV = "ZaitsevBank_API_"; // 16 bytes for IV
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                encryptor.Key = Encoding.UTF8.GetBytes(key256);
                encryptor.IV = Encoding.UTF8.GetBytes(IV);
                using (MemoryStream ms = new ())
                {
                    using (CryptoStream cs = new (ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}
