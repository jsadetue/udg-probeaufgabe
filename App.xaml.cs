using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UdgChallenge
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Program.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Program.Terminate();

            base.OnExit(e);
        }
    }
}
