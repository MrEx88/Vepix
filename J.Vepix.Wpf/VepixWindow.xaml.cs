using J.Vepix.Wpf.Services;
using MahApps.Metro.Controls;
using System.Windows;

namespace J.Vepix.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class VepixWindow : MetroWindow
    {
        public VepixWindow()
        {
            InitializeComponent();
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            var message = new System.Text.StringBuilder()
                .Append("Version: ")
                .AppendLine(System.Diagnostics.FileVersionInfo.GetVersionInfo(
                    System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion)
                .AppendLine()
                .AppendLine("Author: Jon Wesneski").ToString();

            new MessageDialogService().ShowMessage("vepix - About", message);
        }
    }
}
