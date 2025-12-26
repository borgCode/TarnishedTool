using System.Threading;
using System.Windows;

namespace SilkyRing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        
        private static Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "TarnishedTool";

            _mutex = new Mutex(true, appName, out var createdNew);

            if (!createdNew)
            {
                Current.Shutdown();
            }

            base.OnStartup(e);
        }    
    }
}