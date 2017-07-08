using J.Vepix.Wpf.Utilities;
using MahApps.Metro;
using System;
using System.Linq;
using System.Windows;

namespace J.Vepix.Wpf
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
                    var consoleParser = new VepixCommandLineParser();
                    if (!consoleParser.Parse(e.Args))
                    {
                        consoleParser.DisplayHelp();
                        this.Shutdown();
                        return;
                    }
                }
                catch (ArgumentException ae)
                {
                    VepixCommandLineParser.AttachConsole();
                    Console.WriteLine(string.Format("{0}\n{1}", ae.Message, VepixCommandLineParser.CommandLineHelp));
                    VepixCommandLineParser.DetachConsole();
                    this.Shutdown();
                    return;
                }
            }

            ThemeManager.AddAccent("CustomAccent", new Uri("/Resources/CustomAccent.xaml", UriKind.Relative));

            // get the current app style (theme and accent) from the application
            Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(Application.Current);

            // now change app style to the custom accent and current theme
            ThemeManager.ChangeAppStyle(Application.Current,
                                        ThemeManager.GetAccent("CustomAccent"),
                                        theme.Item1);

            base.OnStartup(e);
        }
    }
}
