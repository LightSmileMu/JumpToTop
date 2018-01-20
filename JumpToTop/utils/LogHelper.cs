using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JumpToTop.utils
{
    
    public static class LogHelper
    {
        private static ILog _logger = null;
        
        static LogHelper()
        {
            _logger = LogManager.GetLogger("LogToFile");
        }

        public static void Debug(string message)
        {
            _logger.Debug(message);
        }

        public static void Info(string message)
        {
            _logger.Info(message);
        }

        public static void Warn(string message)
        {
            _logger.Warn(message);
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Fatal(string message)
        {
            _logger.Fatal(message);
        }
    }
}
