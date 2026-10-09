using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Automation;

internal static class Program
{
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [STAThread]
    private static int Main(string[] args)
    {
        var timer = Stopwatch.StartNew();

        string target = args.Length > 0 ? args[0] : "";
        string fallbackUrl = args.Length > 1 ? args[1] : "";

        if (string.IsNullOrWhiteSpace(target))
        {
            Console.WriteLine("Usage: TaPi.exe <tab-title> [fallback-url]");
            return 1;
        }

        // Prefer Brave; fall back to Chrome.
        var browserProcesses = Process.GetProcessesByName("brave");

        if (browserProcesses.Length == 0) browserProcesses = Process.GetProcessesByName("chrome");

        if (browserProcesses.Length == 0)
        {
            Console.WriteLine("Neither Brave nor Chrome is running.");
            return OpenFallback(fallbackUrl);
        }

        var browserPids = browserProcesses
            .Select(p => p.Id)
            .ToHashSet();

        //Deallocating memory taken by browserProcesses 
        foreach (var process in browserProcesses) process.Dispose();

        try
        {
            var condition = new PropertyCondition(
                AutomationElement.ControlTypeProperty,
                ControlType.TabItem
            );

            var allTabs = AutomationElement.RootElement.FindAll(
                TreeScope.Descendants,
                condition
            );

            foreach (AutomationElement tab in allTabs)
            {
                string name;

                try
                {
                    name = tab.Current.Name;
                    if (string.IsNullOrEmpty(name) || name.IndexOf(target, StringComparison.OrdinalIgnoreCase) < 0) continue;
                }
                catch (ElementNotAvailableException)
                {
                    continue;
                }

                // Walk upward to find the containing browser window.
                AutomationElement? parent = tab;
                IntPtr hwnd = IntPtr.Zero;
                bool isBrowserWindow = false;

                while (parent != null)
                {
                    try
                    {
                        var current = parent.Current;

                        if (current.ControlType == ControlType.Window)
                        {
                            hwnd = new IntPtr(current.NativeWindowHandle);
                            isBrowserWindow = browserPids.Contains(current.ProcessId);
                            break;
                        }

                        parent = TreeWalker.ControlViewWalker.GetParent(parent);
                    }
                    catch (ElementNotAvailableException)
                    {
                        parent = null;
                    }
                }

                if (!isBrowserWindow || hwnd == IntPtr.Zero)
                    continue;

                try
                {
                    if (!tab.TryGetCurrentPattern( SelectionItemPattern.Pattern, out object pattern))
                    {
                        continue;
                    }

                    ((SelectionItemPattern)pattern).Select();
                    SetForegroundWindow(hwnd);

                    Console.WriteLine($"Activated tab: {name}");
                    timer.Stop();
                    Console.WriteLine($"Execution time: {timer.ElapsedMilliseconds} ms");
                    return 0;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return OpenFallback(fallbackUrl);
        }
        catch (ElementNotAvailableException)
        {
            return OpenFallback(fallbackUrl);
        }
        finally
        {
            timer.Stop();
        }
    }

    private static int OpenFallback(string url)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });

                Console.WriteLine($"Opened fallback URL: {url}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to open fallback URL: {ex.Message}");
                return 1;
            }
        }

        Console.WriteLine("No matching tab found.");
        return 1;
    }
}