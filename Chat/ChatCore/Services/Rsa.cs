using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ChatCore.Services
{
    public static class Rsa
    {

        public static string PublicKey;
        public static string PrivateKey;

        public static void GenerateKey()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                PublicKey = GetKeyString(rsa.ExportParameters(false));
                PrivateKey = GetKeyString(rsa.ExportParameters(true));
            }
        }

        private static string GetKeyString(RSAParameters publicKey)
        {
            var stringWriter = new System.IO.StringWriter();
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xmlSerializer.Serialize(stringWriter, publicKey);
            return stringWriter.ToString();
        }


        public static string Encrypt(string textToEncrypt, string publicKeyString)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    rsa.FromXmlString(publicKeyString);
                    return Convert.ToBase64String(rsa.Encrypt(Convert.FromBase64String(textToEncrypt), true));
                }
                catch(Exception ex) { return null; }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static string Decrypt(string textToDecrypt, string privateKeyString)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    // server decrypting data with private key                    
                    rsa.FromXmlString(privateKeyString);
                    return Convert.ToBase64String(rsa.Decrypt(Convert.FromBase64String(textToDecrypt), true));
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
    }
}
