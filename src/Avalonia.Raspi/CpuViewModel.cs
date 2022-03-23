using Iot.Device.CpuTemperature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.Raspi
{
    public class CpuViewModel
    {
        public bool IsRunning { get; set; }
        public double CpuTemp { get; set; }
        public string Processor { get; set; }
        public string MachineName { get; set; }
        public int ProcessorCore { get; set; }

        public string OSName { get; set; }
        public bool Is64Bit { get; set; }

        void GetInfo()
        {
            if(System.OperatingSystem.IsWindows())
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                foreach (ManagementObject mo in mos.Get())
                {
                    Processor = mo["Name"]?.ToString();
                }
            }
            else
            {
                var linux = new LinuxCpuInfo();
                linux.GetValues();
                Processor = linux.ModelName;
            }
            OSName = Environment.OSVersion.ToString();
            Is64Bit = Environment.Is64BitOperatingSystem;
          
            ProcessorCore = Environment.ProcessorCount;

            MachineName = Environment.MachineName;
           
           
        }
        public CpuViewModel()
        {
            GetInfo();
            IsRunning = false;
        }

        public void Stop()
        {
            if (Loop != null && !IsRunning)
            {
                LoopToken.Cancel();
                IsRunning = false;  
            }
        }

        Thread Loop;
        CancellationTokenSource LoopToken;
        public void Start()
        {

            if (!IsRunning)
            {
                LoopToken = new CancellationTokenSource();
                var token = LoopToken.Token;


                Loop = new Thread(new ThreadStart(() =>
                {
                    CpuTemperature cpuTemperature = new CpuTemperature();
                    while (true)
                    {
                        if (token.IsCancellationRequested) break;
                        if (cpuTemperature.IsAvailable)
                        {
                            var temperature = cpuTemperature.ReadTemperatures();
                            foreach (var entry in temperature)
                            {
                                if (!double.IsNaN(entry.Temperature.DegreesCelsius))
                                {
                                    CpuTemp = entry.Temperature.DegreesCelsius;
                                    //Console.WriteLine($"Temperature from {entry.Sensor.ToString()}: {entry.Temperature.DegreesCelsius} °C");
                                }
                                else
                                {
                                    Console.WriteLine("Unable to read Temperature.");
                                    //Console.WriteLine("Unable to read Temperature.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"CPU temperature is not available");
                        }

                        Thread.Sleep(1000);
                    }
                }));
                Loop.Start();
                IsRunning=true; 
            }
        }
    }



}
