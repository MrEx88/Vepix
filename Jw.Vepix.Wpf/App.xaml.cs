using Jw.Vepix.Wpf.Utilities;
using System;
using System.Linq;
using System.Windows;

namespace Jw.Vepix.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Count() > 0)
            {
                try
                {
                    var v = VepixConsole.Instance();
                    if (!v.Parse(e.Args))
                    { 
                        VepixConsole.AttachConsole();
                        Console.WriteLine(VepixConsole.CommandLineHelp);
                        VepixConsole.DetachConsole();
                        this.Shutdown();
                        return;
                    }
                }
                catch (ArgumentException ae)
                {
                    VepixConsole.AttachConsole();
                    Console.WriteLine(string.Format("{0}\n{1}", ae.Message, VepixConsole.CommandLineHelp));
                    VepixConsole.DetachConsole();
                    this.Shutdown();
                    return;
                }
            }

            //new Bootstrapper().Bootstrap().Resolve<VepixWindow>().Show();
            base.OnStartup(e);
        }
    }
}
