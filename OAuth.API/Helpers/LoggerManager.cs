using log4net;
using log4net.Config;
using System.Reflection;
using System.Xml;

namespace OAuthAPI.Helpers
{
    public class LoggerManager : ILoggerManager
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LoggerManager));

        public LoggerManager()
        {
            try
            {
                XmlDocument log4netConfig = new XmlDocument();

                using (var fs = File.OpenRead("log4net.config"))
                {
                    log4netConfig.Load(fs);

                    var repo = LogManager.CreateRepository(
                            Assembly.GetEntryAssembly(),
                            typeof(log4net.Repository.Hierarchy.Hierarchy));

                    XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

                    // The first log to be written 
                    _logger.Info("Log System Initialized");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error", ex);
            }
        }

        public void LogInformation(string message)
        {
            _logger.Info(message);
        }

        public void Log(string message, LogType type)
        {
            switch (type)
            {
                case LogType.Info:
                    _logger.Info(message);
                    return;
                case LogType.Debug:
                    _logger.Debug(message);
                    return;
                case LogType.Warn:
                    _logger.Warn(message);
                    return;
                case LogType.Error:
                    _logger.Error(message);
                    return;
                case LogType.Fatal:
                    _logger.Fatal(message);
                    return;
                default:
                    _logger.Info(message);
                    return;
            }
        }
    }
}
