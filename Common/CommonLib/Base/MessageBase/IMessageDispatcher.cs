namespace Crazy.Common
{
    public interface IMessageDispatcher
    {
        void Dispatch(object session, ushort opcode, object message);
    }
}
