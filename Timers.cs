using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class Timers
{
    public static async Task StartTimer(TimeSpan interval, Func<Task> timerMethod, string profileId)
    {
        // Создаем объект SemaphoreSlim с начальным значением 1
        SemaphoreSlim semaphore = new SemaphoreSlim(1);

        // Создаем таймер
        var timer = new System.Timers.Timer();
        timer.Interval = interval.TotalMilliseconds;
        timer.AutoReset = true;
        timer.Elapsed += async (sender, e) =>
        {
            // Блокируем объект semaphore
            await semaphore.WaitAsync();

            // Вызываем метод таймера
            await timerMethod();

            // Разблокируем объект semaphore
            semaphore.Release();
        };
        timer.Start();

        // останавливаем выполнение приложения, чтобы таймер продолжал работу
        await Task.Delay(-1);
    }

}

