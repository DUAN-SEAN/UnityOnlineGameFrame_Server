using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.NetSharp
{
    public interface IServiceEventHandler
    {
        /// <summary>
        /// Invoke when a new client connected.
        /// </summary>
        /// <param name="client">Client proxy object for outgoing message.</param>
        /// <returns>The handler for incoming message. Return null to discard the connection.</returns>
        Task<IClientEventHandler> OnConnect(IClient client);

        /// <summary>
        /// 当endpoint有exception产生
        /// </summary>
        /// <param name="ex"></param>
        void OnException(Exception ex);
    }
}
