using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class VkFriendAdder
{
    private readonly RemoteWebDriver _driver;
    private readonly string url = "https://vk.com/friends?act=find";
    private readonly string? _profileId = "";
    private readonly string? _logFileName = "";
    private readonly string? _profileName = "";



    public VkFriendAdder(RemoteWebDriver driver, string profileId, string logFileName, string? profileName)
    {
        this._driver = driver;
        this._profileId = profileId;
        this._logFileName = logFileName;
        this._profileName = profileName;
    }

    public async Task AddFriends()
    {
        try
        {
            if(_driver.Url != url)
                _driver.Navigate().GoToUrl(url);

            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            await Task.Run(() =>
            {
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            });
        }
        catch (Exception)
        {
            string message1 = $"Не удалось дождаться загрузки страницы профиля: {_profileName}";
            LogManager.LogMessage(message1, _logFileName);

            Console.WriteLine(message1);

            _driver.Dispose();
            await Task.Delay(1000);
            return;
        }

        await Processes.CheckRunningChromeAsync();

        // Проверяем Url на предмет блокировки
        if (_driver.Url.Contains("blocked"))
        {
            string message2 = $"Этот аккаунт заблокирован: {_profileName}";
            LogManager.LogMessage(message2, _logFileName);

            _driver.Dispose();
            await Task.Delay(1000); ;
            return;
        }

        if (String.IsNullOrEmpty(_driver.Url))
        {
            _driver.Dispose();
            await Task.Delay(1000);
            return;
        }

        // Проверяем страницу на предмет popup с предложением о красивом имени
        try
        {
            IWebElement element = _driver.FindElement(By.XPath("//div[@class='box_layout' and @onclick='boxQueue.skip=true;']"));
            if (element != null)
            {
                IWebElement closeButton = _driver.FindElement(By.XPath("//div[@class='box_x_button']"));
                closeButton.Click();
            }
        }
        catch (NoSuchElementException) { }


        int errorCount = 0;
        int addedCount = 0;

        IList<IWebElement> addButtonList = null;
        try
        {
            addButtonList = await GetAddButtonsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не удалось получить список возможных друзей или произошла другая ошибка: {ex.Message}");

            _driver.Dispose();
            await Task.Delay(1000);

            return;
        }

        if (addButtonList.Count == 0)
        {
            Console.WriteLine("Кнопки не найдены добавить в друзья не найдены");

            _driver.Dispose();
            await Task.Delay(1000);

            return;
        }

        Random random = new Random();

        for (int i = 0; i < addButtonList.Count && addedCount < 100; i++)
        {
            // Ищем элемент с классом "box_title" и текстом "Ошибка"
            bool pageHaveError = await IsPageHaveErrorsAsync();
            if (pageHaveError)
            {
                _driver.Dispose();
                await Task.Delay(1000);

                break;
            }

            //try
            //{
            //    var errorTitle = _driver.FindElement(By.ClassName("box_title"));

            //    if (errorTitle != null || errorTitle?.Text == "Ошибка")
            //    {

            //        string? textlimitForAddFriendPerDay = "К сожалению, вы не можете добавлять больше друзей за один день. Пожалуйста, попробуйте завтра.";
            //        var limitForAddFriendPerDay = _driver.FindElements(By.ClassName("box_body"))
            //                                 .FirstOrDefault(e => e.Text.Contains(textlimitForAddFriendPerDay));


            //        string filePath = "sleepingProfiles.json";
            //        List<SleepingProfiles>? sleepingProfiles;

            //        if (File.Exists(filePath))
            //        {
            //            sleepingProfiles = JsonConvert.DeserializeObject<List<SleepingProfiles>>(File.ReadAllText(filePath));
            //        }
            //        else
            //        {
            //            sleepingProfiles = new List<SleepingProfiles>();
            //            File.WriteAllText(filePath, JsonConvert.SerializeObject(sleepingProfiles));
            //        }

            //        if (limitForAddFriendPerDay != null)
            //        {

            //            // Записываем в объект SleepingProfiles текущее время
            //            SleepingProfiles currentProfile = new SleepingProfiles
            //            {
            //                FellAsleepProfile = DateTime.Now,
            //                ProfilesId = _profileId
            //            };

            //            sleepingProfiles?.Add(currentProfile);
            //            File.WriteAllText(filePath, JsonConvert.SerializeObject(sleepingProfiles));

            //            string message3 = $"В профиле {_profileName} достигнут лимит на сегодня, пойду посплю.";
            //            LogManager.LogMessage(message3, _logFileName);

            //            await Task.Delay(1000);

            //            return;
            //        }


            //        string? textLimit10000AddFriend = "К сожалению, вы не можете добавлять больше 10";
            //        var limit10000AddFriend = _driver.FindElements(By.ClassName("box_body"))
            //                                 .FirstOrDefault(e => e.Text.Contains(textLimit10000AddFriend));

            //        if (limit10000AddFriend != null)
            //        {
            //            // Записываем в объект SleepingProfiles текущее время
            //            SleepingProfiles currentProfile = new SleepingProfiles
            //            {
            //                LimitProfile = true,
            //                ProfilesId = _profileId
            //            };

            //            sleepingProfiles?.Add(currentProfile);
            //            File.WriteAllText(filePath, JsonConvert.SerializeObject(sleepingProfiles));

            //            string message4 = $"Профиль {_profileName} заполнен под горлышко, 10 000 лимит";
            //            LogManager.LogMessage(message4, _logFileName);

            //            await Task.Delay(1000);

            //            return;
            //        }

            //        return;
            //    }
            //}
            //catch (Exception) { }

            try
            {
                var addButton = addButtonList[i];
                int delay = random.Next(500, 1000);
                await Task.Delay(delay);
                ((IJavaScriptExecutor)_driver).ExecuteScript("return arguments[0].click()", addButton);
                addedCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось кликнуть кнопку: {ex.Message}");
            }

            await Task.Delay(1000);

            if (addedCount >= 100 || addedCount > addButtonList.Count)
            {
                string message5 = $"В профиль {_profileName} добавлено {addedCount} друзей";
                LogManager.LogMessage(message5, _logFileName);

                await Task.Delay(500);

                return;
            }
        }

        await Task.Delay(1000);
    }

    private async Task<bool> IsPageHaveErrorsAsync()
    {
        try
        {
            var errorTitle = await Task.Run(() => _driver.FindElement(By.ClassName("box_title")));

            if (errorTitle != null || errorTitle?.Text == "Ошибка")
            {
                string? textlimitForAddFriendPerDay = "К сожалению, вы не можете добавлять больше друзей за один день. Пожалуйста, попробуйте завтра.";
                var limitForAddFriendPerDay = await Task.Run(() => _driver.FindElements(By.ClassName("box_body"))
                    .FirstOrDefault(e => e.Text.Contains(textlimitForAddFriendPerDay)));

                string filePath = "sleepingProfiles.json";
                List<SleepingProfiles>? sleepingProfiles;

                if (File.Exists(filePath))
                {
                    sleepingProfiles = JsonConvert.DeserializeObject<List<SleepingProfiles>>(File.ReadAllText(filePath));
                }
                else
                {
                    await Task.Delay(500);
                    sleepingProfiles = new List<SleepingProfiles>();
                    File.WriteAllText(filePath, JsonConvert.SerializeObject(sleepingProfiles));
                }

                if (limitForAddFriendPerDay != null)
                {
                    SleepingProfiles currentProfile = new SleepingProfiles
                    {
                        FellAsleepProfile = DateTime.Now,
                        ProfilesId = _profileId
                    };

                    await Task.Delay(500);
                    sleepingProfiles?.Add(currentProfile);
                    File.WriteAllText(filePath, JsonConvert.SerializeObject(sleepingProfiles));

                    //_driver.Dispose();
                    //await Task.Delay(1000);

                    return true;
                }


                string? textLimit10000AddFriend = "К сожалению, вы не можете добавлять больше 10";
                var limit10000AddFriend = await Task.Run(() => _driver.FindElements(By.ClassName("box_body"))
                    .FirstOrDefault(e => e.Text.Contains(textLimit10000AddFriend)));

                if (limit10000AddFriend != null)
                {
                    SleepingProfiles currentProfile = new SleepingProfiles
                    {
                        LimitProfile = true,
                        ProfilesId = _profileId
                    };

                    await Task.Delay(500);
                    sleepingProfiles?.Add(currentProfile);
                    File.WriteAllText(filePath, JsonConvert.SerializeObject(sleepingProfiles));

                    string message1 = $"Профиль {_profileName} заполнен под горлышко, 10 000 лимит";
                    LogManager.LogMessage(message1, _logFileName);

                    //_driver.Dispose();
                   // await Task.Delay(1000);

                    return true;
                }
            }
        }
        catch (Exception)
        {
            //string message = $"Профиль {_profileName}: страница содержит ошибки";
            //LogManager.LogMessage(message, _logFileName);
            //_driver.Dispose();
            await Task.Delay(1000);
            return false;
        }

        return false;
    }



    private async Task<IList<IWebElement>> GetAddButtonsAsync()
    {
        IList<IWebElement> addButtonList = new List<IWebElement>();
        int count = 0;
        while (addButtonList.Count < 100 && count < 4)
        {
            count++;
            try
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript($"window.scrollTo(0, document.body.scrollHeight * {count} / 4)");
                await Task.Delay(1000);

                IReadOnlyCollection<IWebElement> currentButtons = _driver.FindElements(By.CssSelector("a.friends_find_user_add"));
                foreach (var addButton in currentButtons)
                {
                    if (addButtonList.Count < 100 && !addButtonList.Contains(addButton))
                    {
                        addButtonList.Add(addButton);
                    }
                }
            }
            catch (StaleElementReferenceException)
            {
                continue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не получилось получить кнопку: {ex.Message}");
                break;
            }
        }
        return addButtonList;
    }

}
