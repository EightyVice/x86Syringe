using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Management;
using System.Timers;
using System.Windows.Media.Imaging;
using x86Syringe.Processes;

namespace x86Syringe;

public class ProcessViewModel : INotifyPropertyChanged
{
    private ObservableCollection<ProcessView> _processes;
    private System.Timers.Timer _timer;

    public ObservableCollection<ProcessView> Processes
    {
        get => _processes;
        set
        {
            _processes = value;
            OnPropertyChanged(nameof(Processes));
        }
    }

    public ProcessViewModel()
    {
        Processes = new ObservableCollection<ProcessView>();
        _timer = new System.Timers.Timer(5000); // Refresh every 5 seconds
        _timer.Elapsed += (s, e) => RefreshProcesses();
        _timer.Start();
        RefreshProcesses();
    }


    void GetProcess()
    {
        var wmiQueryString = "SELECT ProcessId, ExecutablePath, Name, CommandLine FROM Win32_Process";
        using (var searcher = new ManagementObjectSearcher(wmiQueryString))
        using (var results = searcher.Get())
        {
            var query = from p in Process.GetProcesses()
                        join mo in results.Cast<ManagementObject>()
                        on p.Id equals (int)(uint)mo["ProcessId"]
                        select new
                        {
                            Process = p,
                            Path = (string)mo["ExecutablePath"],
                            CommandLine = (string)mo["CommandLine"],
                        };
            foreach (var item in query)
            {
                // Do what you want with the Process, Path, and CommandLine
            }
        }
    }
    private void RefreshProcesses()
    {
        var currentProcesses = Process.GetProcesses();
        var processInfos = new ObservableCollection<ProcessView>();

        foreach (var procInfo in ProcessHelper.GetProcessInfos())
        {

            if (procInfo.Location == null)
                continue;

            if (!File.Exists(procInfo.Location))
                continue;

            processInfos.Add(new ProcessView
            {
                PID = procInfo.Id,
                Icon = ToBitmapImage(Icon.ExtractAssociatedIcon(procInfo.Location)?.ToBitmap()),
                Name = procInfo.Name,
                Location = procInfo.Location
            });
        }

        Processes = processInfos;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    private static BitmapImage ToBitmapImage(Bitmap bitmap)
    {
        using (var memory = new MemoryStream())
        {
            bitmap.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }
    }

}

public class ProcessView
{
    public BitmapImage Icon { get; set; } 
    public int PID { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
}
