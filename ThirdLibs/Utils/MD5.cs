using System.Text;
using System.Security.Cryptography;
using System;

namespace BlackJack.Utils
{
    static public class MD5
    {
        public static string GetMd5Hash(string pathName)
        {
            string strResult = "";
            string strHashData = "";
            byte[] arrbytHashValue;

            System.IO.FileStream oFileStream = null;

            System.Security.Cryptography.MD5CryptoServiceProvider oMD5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();

            try
            {
                oFileStream = new System.IO.FileStream(pathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);

                arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream); //计算指定Stream 对象的哈希值

                oFileStream.Close();
                oMD5Hasher.Clear();

                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”

                strHashData = System.BitConverter.ToString(arrbytHashValue);

                //替换-
                strHashData = strHashData.Replace("-", "").ToLower();

                strResult = strHashData;
            }

            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
                //DEBUG.WriteLine(ex.Message);
            }
            return strResult;
        }


        public static string CreateMD5FromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return "";
            }
            var oMD5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var arrbytHashValue = oMD5Hasher.ComputeHash(bytes); //计算指定byte[] 对象的哈希值
            string strMD5 = System.BitConverter.ToString(arrbytHashValue).Replace("-", "");
            return strMD5;
        }

        public static string GetStringMD5(string sDataIn)
        {
            byte[] bytValue;
            bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn);
            using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                byte[] bytHash = md5.ComputeHash(bytValue);
                StringBuilder sTemp = new StringBuilder();
                for (int i = 0; i < bytHash.Length; i++)
                {
                    sTemp.Append(bytHash[i].ToString("x2"));
                }
                return sTemp.ToString();
            }
        }

        /// <summary>
        /// 3des的加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DES3Encrypt(string data, string key)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = ASCIIEncoding.ASCII.GetBytes(key);
            DES.Mode = CipherMode.ECB;
            //DES.Padding = PaddingMode.PKCS7;

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            byte[] Buffer = UTF8Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        /// <summary>
        /// 3des的解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DES3Decrypt(string data, string key)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = System.Text.UTF8Encoding.UTF8.GetBytes(key);
            DES.Mode = CipherMode.ECB;
            DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

            ICryptoTransform DESDecrypt = DES.CreateDecryptor();

            string result = "";
            try
            {
                byte[] Buffer = Convert.FromBase64String(data);
                result = UTF8Encoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (Exception e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
            }
            return result;
        }
    }
}
