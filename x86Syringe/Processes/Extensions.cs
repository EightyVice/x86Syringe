using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace x86Syringe.Processes
{
    internal record ProcessInfo(int Id, string Name, string Location);
    internal static class ProcessHelper
    {
        public static ProcessInfo[] GetProcessInfos()
        {
            var processes = new List<ProcessInfo>();

            var query = "SELECT ProcessId, Name, ExecutablePath FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(query))
            {
                using (var results = searcher.Get())
                {
                    foreach (ManagementObject obj in results)
                    {
                        var id = Convert.ToInt32(obj["ProcessId"]);
                        var name = obj["Name"]?.ToString();
                        var location = obj["ExecutablePath"]?.ToString();

                        processes.Add(new ProcessInfo(id, name, location));
                    }
                }
            }

            return processes.ToArray();
        }
    }
}
