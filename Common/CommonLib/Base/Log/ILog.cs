namespace Crazy.Common
{
    internal interface ILog
    {
        void Trace(string message);
        void Warning(string message);
        void Info(string message);
        void Error(string v);
        void Debug(string message);
        void Fatal(string v);
    }
}