using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace EncryptionWPF.Encryption
{
    class MyAes
    {
        public string IV_Setup;
        AesCryptoServiceProvider crypto;
        public MyAes()
        {
            crypto = new AesCryptoServiceProvider();
            crypto.KeySize = 256;
            crypto.Mode = CipherMode.CBC;
            crypto.Padding = PaddingMode.PKCS7;
        }
        public String Encrypt(String clear_text)
        {
            ICryptoTransform transform = crypto.CreateEncryptor();
            byte[] encrypted_bytes = transform.TransformFinalBlock(
                UTF8Encoding.UTF8.GetBytes(clear_text),
                0,
                clear_text.Length);
            string str = Convert.ToBase64String(encrypted_bytes);
            return str;
        }
        public String Decrypt(String cipher_text)
        {
            ICryptoTransform transform = crypto.CreateDecryptor();
            byte[] enc_bytes = Convert.FromBase64String(cipher_text);
            byte[] decrypted_bytes = transform.TransformFinalBlock(enc_bytes, 0, enc_bytes.Length);
            string str = UTF8Encoding.UTF8.GetString(decrypted_bytes);
            return str;
        }
        public String Generator(String key)
        {
            byte[] Key_bytes = UTF8Encoding.UTF8.GetBytes(key);
            crypto.Key = Key_bytes;
            string IV_Str = GetIV();
            byte[] IV = UTF8Encoding.UTF8.GetBytes(IV_Str);
            crypto.IV = IV;
            string str = key;
            return str;
        }
        public void SetIVfromPW(string pw)
        {
            IV_Setup = pw;
        }
        public string GetIV()
        {
            return IV_Setup;
        }
    }
}