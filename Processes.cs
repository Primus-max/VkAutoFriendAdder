using System.Diagnostics;


public class Processes
{
    public static string processNameChrome = "chrome";
    public static string processNameChromeDriver = "chromedriver";


    public static async Task CheckRunningChromeAsync()
    {
        await Task.Run(() =>
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                try
                {
                    if (process.ProcessName.ToLower() == processNameChrome.ToLower())
                    {
                        if (process.MainWindowTitle == "Новая вкладка – Chromium")
                        {
                            process.Kill();
                            Console.WriteLine($"Процесс {processNameChrome} был закрыт форсированно");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при попытке закрыть процесс {processNameChrome}: {ex.Message}");
                }
            }
        });
    }


    //public static void CheckRunningChrome()
    //{
    //    Process[] processes = Process.GetProcesses();
    //    foreach (Process process in processes)
    //    {
    //        try
    //        {
    //            if (process.ProcessName.ToLower() == processNameChrome.ToLower())
    //            {
    //                process.Kill();
    //                Console.WriteLine($"Процесс {processNameChrome} был закрыт форсированно");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Ошибка при попытке закрыть процесс {processNameChrome}: {ex.Message}");
    //        }
    //    }
    //}

    public static void CheckRunningChromeDriver()
    {
        Process[] processes = Process.GetProcesses();
        foreach (Process process in processes)
        {
            try
            {
                if (process.ProcessName.ToLower() == processNameChromeDriver.ToLower())
                {
                    process.Kill();
                    Console.WriteLine($"Процесс {processNameChromeDriver} был закрыт форсированно");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при попытке закрыть процесс {processNameChromeDriver}: {ex.Message}");
            }
        }
    }
}




