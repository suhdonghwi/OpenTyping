using NLog;
using System.Windows;
using System.Windows.Threading;

namespace OpenTyping
{
    /// <summary>
    ///     App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;          // get exception
            logger.Error(exception.ToString()); ; // leave the log to file
            e.Handled = true;                     // prevent the application from crashing
            Shutdown();                           // quit the application in a controlled way
        }
    }
}