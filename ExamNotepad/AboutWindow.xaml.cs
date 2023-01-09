using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;

namespace ExamNotepad
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        public AboutWindow()
        {
            InitializeComponent();
            var os = Environment.OSVersion;
            operatingSystemVersion.Content = $"OSVersion: {os.VersionString}";
            versionLabel.Content = $"Version: {assembly.GetName().Version}";
        }
    }
}
