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
                    var consoleParser = new VepixConsoleParser();
                    if (!consoleParser.Parse(e.Args))
                    { 
                        VepixConsoleParser.AttachConsole();
                        Console.WriteLine(VepixConsoleParser.CommandLineHelp);
                        VepixConsoleParser.DetachConsole();
                        this.Shutdown();
                        return;
                    }
                }
                catch (ArgumentException ae)
                {
                    VepixConsoleParser.AttachConsole();
                    Console.WriteLine(string.Format("{0}\n{1}", ae.Message, VepixConsoleParser.CommandLineHelp));
                    VepixConsoleParser.DetachConsole();
                    this.Shutdown();
                    return;
                }
            }

            base.OnStartup(e);
        }
    }
}
