using Jw.Vepix.Wpf.Utilities;
using MahApps.Metro;
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

            //todo: need to come up with a way where this can always be referenced
            // for debugging/release and installed
            try
            {
                ThemeManager.AddAccent("CustomAccent", new Uri(@"C:\Users\Jon\Documents\Visual Studio 2015\Projects\Vepix\Jw.Vepix.Wpf\Resources\CustomAccent.xaml"));

                // get the current app style (theme and accent) from the application
                Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(Application.Current);

                // now change app style to the custom accent and current theme
                ThemeManager.ChangeAppStyle(Application.Current,
                                            ThemeManager.GetAccent("CustomAccent"),
                                            theme.Item1);
            }
            catch (Exception ex) { }

            base.OnStartup(e);
        }
    }
}
