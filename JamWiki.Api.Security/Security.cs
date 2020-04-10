using System;
using System.Security.Cryptography;
using System.Text;

namespace JamWiki.Api.Helpers
{
    public class Security
    {
        public static string Encrypt(string key, string toEncrypt, bool useHashing = true)
        {
            byte[] resultArray = null;
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

                if (useHashing)
                {
                    using (MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider())
                    {
                        keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                    }
                }
                else
                {
                    keyArray = Encoding.UTF8.GetBytes(key);
                }


                using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = keyArray;
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;
                    ICryptoTransform cTransform = tdes.CreateEncryptor();
                    resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                }
                
            }
            catch (Exception)
            {
                // log
            }

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string key, string cipherString, bool useHashing = true)
        {
            byte[] resultArray = null;
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);

                if (useHashing)
                {
                    using (MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider())
                    {
                        keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                    }
                }
                else
                    keyArray = Encoding.UTF8.GetBytes(key);

                
                using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = keyArray;
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    var cTransform = tdes.CreateDecryptor();
                    resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                }
            }
            catch (Exception)
            {
                // log
            }
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}