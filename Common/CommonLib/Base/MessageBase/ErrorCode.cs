using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.Common
{
    public class ErrorCode
    {
        public const Int32 MessageOk = 0;
        public const Int32 RpcResponseError = 1;
        public const Int32 ERR_RpcFail = 2;
        public static bool IsRpcNeedThrowException(int errorCode)
        {
            if (errorCode == RpcResponseError)
                return true;
            return false;
        }
    }
}
