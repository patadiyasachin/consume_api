﻿using System.Security.Cryptography;
using System.Text;

namespace loc_api_consume.Helper
{
    public static class UrlEncryptor
    {
        private static readonly string EncryptionKey = "pjsGLNYrMqU6wny4"; // Change this key

        // Method to encrypt a string
        public static string Encrypt(string text)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aesAlg.IV = new byte[16]; // Initialize the IV to 0s

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Method to decrypt an encrypted string
        public static string Decrypt(string encryptedText)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aesAlg.IV = new byte[16]; // Initialize the IV to 0s

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
