using System;
using System.Diagnostics;
using log4net;

namespace TestConsole
{
    public class MyClass
    {
        public void MyMethod(string message)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            if (String.IsNullOrEmpty(message))
            {
                log.InfoFormat("Event called at {0:yyyy-MM-dd HH:mm:ss:ff}", DateTime.Now);
            }
            else
            {
                log.InfoFormat("Event called at {0:yyyy-MM-dd HH:mm:ss:ff} {1}", DateTime.Now, message);
            }
        }
    }
}
