using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using x86Syringe.Injection;
using x86Syringe.Workspace;

namespace x86Syringe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var proc in Process.GetProcesses())
            {
                procListView.Items.Add(proc);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedProc = (ProcessView)procListView.SelectedItem;
            Injector.InjectDll(selectedProc.PID, Path.GetFullPath("bootstrapper.dll"));            
        }
    }
}