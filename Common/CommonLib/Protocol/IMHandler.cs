using Crazy.Common;
using System;
using System.Collections.Generic;
using System.Text;
namespace Crazy.Common
{
    public interface IMHandler
    {
        void Handle(ISession sender, object message);
        Type GetMessageType();
    }


}
