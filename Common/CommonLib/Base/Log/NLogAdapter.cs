using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crazy.Common
{
    public class NLogAdapter :  ILog
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public NLogAdapter()
        {
        }

        public void Trace(string message)
        {
            this.logger.Trace(message);
            
        }

        public void Warning(string message)
        {
            this.logger.Warn(message);
        }

        public void Info(string message)
        {
          
            this.logger.Info(message);
        }

        public void Debug(string message)
        {
            this.logger.Debug(message);
        }

        public void Error(string message)
        {
            this.logger.Error(message);
        }

        public void Fatal(string message)
        {
            this.logger.Fatal(message);
        }
    }
}
