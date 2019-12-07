using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.Common
{
    /// <summary>
    /// RPC异常,带ErrorCode
    /// </summary>
    [Serializable]
    public class RpcException : Exception
    {
        public int Error { get; private set; }

        public RpcException(int error, string message) : base($"Error: {error} Message: {message}")
        {
            this.Error = error;
        }

        public RpcException(int error, string message, Exception e) : base($"Error: {error} Message: {message}", e)
        {
            this.Error = error;
        }
    }
}
