using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Phnyx.Web.Security.Crypto
{
    /// <summary>
    /// Encrypts strings to url-safe strings. 
    /// </summary>
    public class Encrypter
    {
        private static string ENCRYPTIONSECRET = ConfigurationManager.AppSettings["Phnyx.Web.Security.Crypto.Encrypter.EncryptionSecret"];
        private static readonly byte[] SALT = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Phnyx.Web.Security.Crypto.Encrypter.EncryptionSalt"]);
        private static ICryptoTransform ENCRYPTOR, DECRYPTOR;
        private static UTF8Encoding UTF8 = new UTF8Encoding();

        static Encrypter()
        {
            RijndaelManaged rm = new RijndaelManaged();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(ENCRYPTIONSECRET, SALT);
            var key = pdb.GetBytes(32);
            var iv = pdb.GetBytes(16);
            ENCRYPTOR = rm.CreateEncryptor(key, iv);
            DECRYPTOR = rm.CreateDecryptor(key, iv);
        }

        public static byte[] Encrypt(byte[] buffer)
        {
            MemoryStream encryptStream = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(encryptStream, ENCRYPTOR, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }
            return encryptStream.ToArray();
        }

        public static string Encrypt(string unencrypted)
        {
            return Encoder(Encrypt(UTF8.GetBytes(unencrypted)));
        }

        public static string Decrypt(string encrypted)
        {
            return UTF8.GetString(Decrypt(Decoder(encrypted)));
        }

        public static byte[] Decrypt(byte[] buffer)
        {
            MemoryStream decryptStream = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(decryptStream, DECRYPTOR, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }
            return decryptStream.ToArray();
        }

         public static Func<byte[], string> Encoder = inp => Convert.ToBase64String(inp)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace('=', '*');

        public static Func<string, byte[]> Decoder = inp =>
        {
            int paddingneeds = 4 - (inp.Length % 4);
            var str = paddingneeds == 4 || paddingneeds == 0 ? inp : inp + new String('*', paddingneeds);
            return Convert.FromBase64String(str
                    .Replace('-', '+')
                    .Replace('_', '/')
                    .Replace('*', '='));
        };
    }
}
