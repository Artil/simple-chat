using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ChatCore.Services
{
    public class RijndaelCrypt
    {
        public static (string Key, string IV) GenerateKey()
        {
            using (var rijAlg = Rijndael.Create())
            {
                return (Convert.ToBase64String(rijAlg.Key), Convert.ToBase64String(rijAlg.IV));
            }
        }

        public static string EncryptString(string plainText, string Key, string IV)
        {
            // Create an Rijndael object
            // with the specified key and IV.
            using (var rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Convert.FromBase64String(Key);
                rijAlg.IV = Convert.FromBase64String(IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV), CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public static string DecryptString(string cipherText, string Key, string IV)
        {
            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Convert.FromBase64String(Key);
                rijAlg.IV = Convert.FromBase64String(IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV), CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
