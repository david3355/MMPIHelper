using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

public enum Keys2Send { UP, DOWN, RIGHT, LEFT, ENTER, BACKSPACE, TAB, DELETE, ESC, END, HOME, F5 }

public class KeystrokeSimulator
{
    [DllImport("user32.dll")]
    public static extern int SetForegroundWindow(IntPtr hWnd); // Brings the interface of selected process to the front

    protected KeystrokeSimulator() { }

    private static void SendKey(string Key)
    {
        SendKeys.SendWait('{' + Key + '}');
    }

    protected static void SetForegroundWindow(Process Process)
    {
        SetForegroundWindow(Process.MainWindowHandle);
    }

    protected static void SetForegroundWindow(String ProcessName)
    {
        Process[] procs = GetProcessByName(ProcessName);
        if (procs.Length > 0) SetForegroundWindow(procs[0]);
    }


    public static void Send(Process Process, string Key)
    {
        SetForegroundWindow(Process);
        SendKey(Key);
    }

    public static void Send(Process[] Processes, string Key)
    {
        foreach (Process proc in Processes)
        {
            Send(proc, Key);
        }
    }

    public static void Send(string ProcessName, string Key)
    {
        Process[] procs = GetProcessByName(ProcessName);
        if (procs.Length > 0) Send(procs, Key);
    }

    public static void Send(Process Process, string[] Keys)
    {
        Send(Process, Keys, 0);
    }

    public static void Send(Process Process, string[] Keys, int DelayMillisec)
    {
        SetForegroundWindow(Process);
        foreach (string key in Keys)
        {
            SendKey(key);
            if (DelayMillisec > 0) Thread.Sleep(DelayMillisec);
        }
    }

    public static void Send(Process[] Processes, string[] Keys)
    {
        foreach (Process proc in Processes)
        {
            foreach (string key in Keys) Send(proc, key);
        }
    }

    public static Process[] GetProcessByName(string ProcessName)
    {
        return Process.GetProcessesByName(ProcessName);
    }

    public static Process[] GetAllProcess()
    {
        return Process.GetProcesses();
    }
}
