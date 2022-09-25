using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ORMi;
using ORMi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Process = System.Diagnostics.Process;

namespace MonitorProcess
{
    public class Worker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Insert process name:");
            string processName = Console.ReadLine();

            Console.WriteLine("Insert maximum lifetime in minutes:");
            int maxLifeTime = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Insert monotoring frequency in minutes:");
            int frequency = Int32.Parse(Console.ReadLine());

            // Get Process 
            Process process = Process.GetProcesses().ToList().FirstOrDefault(x => x.ProcessName.Contains(processName) == true);

            while (!stoppingToken.IsCancellationRequested)
            {
                // If the process isn't found
                while(process == null)
                {
                    Console.WriteLine("There isn't a process with the name inputed. Insert process name:");
                    processName = Console.ReadLine();

                    // Repeat search for new processes that where opened
                    process = Process.GetProcesses().ToList().FirstOrDefault(x => x.ProcessName.Contains(processName) == true);
                }
                
                // If the process's start time has passed the lifetime
                if(DateTime.Compare(process.StartTime, DateTime.Now.AddMinutes(maxLifeTime)) < 0)
                {
                    process.Kill();

                    Console.WriteLine("The process has been closed");
                    await Task.Delay(5000, stoppingToken);
                    System.Environment.Exit(0);
                }

                await Task.Delay(frequency / 60, stoppingToken);
            }
        }

    }
}
