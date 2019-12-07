using System;
using System.Text;

namespace BlackJack.Utils
{
    static public class Misc
    {
        static public string ByteArrayToHexString(byte[] array, int startIndex, int count)
        {
            if (array == null)
            {
                return "null";
            }
            if (array.Length == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            for (int i = startIndex; i < startIndex + count; ++i)
            {
                sb.Append(array[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取拼接字符串
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        static public Byte[] GetBytesFromString(String dataString)
        {
            UnicodeEncoding byteConverter = new UnicodeEncoding();
            Byte[] dataBytes = byteConverter.GetBytes(dataString);
            return dataBytes;
        }

        /// <summary>
        /// 获取行号
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        static public int GetLineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = -1)
        {
            return lineNumber;
        }

        /// <summary>
        /// 获取函数名
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        static public string GetFunctionName([System.Runtime.CompilerServices.CallerMemberName] string functionName = "")
        {
            return functionName;
        }
    }
}
