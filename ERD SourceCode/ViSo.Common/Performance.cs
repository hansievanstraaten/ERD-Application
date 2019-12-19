using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using GeneralExtensions;

namespace ViSo.Common
{
  public class Performance
  {
    public delegate void CounterExecutedEvent(string counterName, float counterValue);

    private static bool stopMeter = false;

    private static ManualResetEvent manualResetEvent = new ManualResetEvent(false);

    public static event CounterExecutedEvent CounterExecuted;

    private static PerformanceCounter cpuCounter; // = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
    
    private static PerformanceCounter memoryCounter; // = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);

    private static Dictionary<string, PerformanceCounter> counterList = new Dictionary<string, PerformanceCounter>();

    /// <summary>
    /// Used for ProcessCPU_Usage
    /// Tuple<string, bool> String = Process Name, bool = As Percentage
    /// </summary>
    public static List<Tuple<string, bool>> ProcessNames = new List<Tuple<string, bool>>();

    public static float CPU_Usage
    {
      get
      {
        try
        {
          if (cpuCounter == null)
          {
            cpuCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
          }

          return cpuCounter.NextValue();
        }
        catch
        {
          return -1;
        }
      }
    }

    public static float RAM_Usage
    {
      get
      {
        try
        {
          if (memoryCounter == null)
          {
            memoryCounter = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);
          }

          float rawValue = memoryCounter.NextValue();

          rawValue = (rawValue / 1024 / 1024);

          return rawValue;
        }
        catch
        {
          return -1;
        }
      }
    }

    public static float ProcessCPU_Usage(string name, bool asPercentage = true)
    {
      try
      {
        if (counterList.ContainsKey(name))
        {
          float result0 = counterList[name].NextValue();

          if (asPercentage)
          {
            result0 = result0 / Environment.ProcessorCount;
          }

          return result0;
        }

        if (!PerformanceCounterCategory.InstanceExists(name, "Process"))
        {
          return -1;
        }

        PerformanceCounter cpuCounterX = new PerformanceCounter("Process", "% Processor Time", name);

        counterList.Add(name, cpuCounterX);

        decimal result = cpuCounterX.NextValue().ToDecimal();

        if (asPercentage)
        {
          result = (result / Environment.ProcessorCount);
        }

        return result.ToFloat();
      }
      catch
      {
        return -1;
      }
    }

    public static async void StartPerformanceMetersRun(Dispatcher dispatcher)
    {
      await Task.Run(() =>
      {
        NEXT_RUN:

        try
        {
          dispatcher.Invoke(() => { CounterExecuted?.Invoke("CPU", CPU_Usage); });

          dispatcher.Invoke(() => { CounterExecuted?.Invoke("RAM", RAM_Usage); });

          foreach (Tuple<string, bool> item in ProcessNames)
          {
            dispatcher.Invoke(() => { CounterExecuted?.Invoke(item.Item1, ProcessCPU_Usage(item.Item1, item.Item2)); });
          }
        }
        catch (Exception err)
        {

        }

        manualResetEvent.WaitOne(1000);

        if (!stopMeter)
        {
          goto NEXT_RUN;
        }

        stopMeter = false;

      });
    }

    public static void StopPerformanceMeters()
    {
      stopMeter = true;
    }
  }
}
