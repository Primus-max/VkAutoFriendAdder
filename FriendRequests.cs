using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class FriendRequests
{
    private readonly IWebDriver driver;
    private readonly string url = "https://vk.com/friends?section=all_requests";
    private string _profileId = "";

    public FriendRequests(IWebDriver driver, string profileId)
    {
        this.driver = driver;
        this._profileId = profileId;
    }

    public void Navigate()
    {
        driver.Navigate().GoToUrl(url);
    }

    public IList<IWebElement> GetRequestButtons()
    {
        try
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

            var buttons = driver.FindElements(By.CssSelector("button[id^='accept_request_']"));
            if (buttons.Count == 0)
            {
                driver.Quit();
                throw new Exception($"Нет активных заявок в профиле: {_profileId}");
            }

            return buttons;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
            return new List<IWebElement>();
        }
    }


    public async Task ClickRequestButton(IWebElement button)
    {
        var jsExecutor = (IJavaScriptExecutor)driver;
        await Task.Delay(1000); // добавляем задержку вместо Thread.Sleep
        jsExecutor.ExecuteScript("arguments[0].focus();", button);
        button.Click();
    }

    public void AcceptRequest(IWebElement button)
    {
        var onclick = button.GetAttribute("onclick");
        var startIndex = onclick.IndexOf('(') + 1;
        var endIndex = onclick.IndexOf(',', startIndex);
        var userId = onclick.Substring(startIndex, endIndex - startIndex);
        var startIndex2 = onclick.IndexOf('\'', endIndex) + 1;
        var endIndex2 = onclick.IndexOf('\'', startIndex2);
        var requestId = onclick.Substring(startIndex2, endIndex2 - startIndex2);

        var jsExecutor = (IJavaScriptExecutor)driver;
        jsExecutor.ExecuteScript($"Friends.acceptRequest({userId}, '{requestId}', arguments[0])", button);
    }


    public static Task ProcessFriendRequests(RemoteWebDriver driver, string profileId)
    {
        FriendRequests friendRequests = new FriendRequests(driver, profileId);
        friendRequests.Navigate();
        var requestButtons = friendRequests.GetRequestButtons();
        if (requestButtons.Count != 0)
        {
            foreach (var button in requestButtons)
            {
                friendRequests.AcceptRequest(button);
                Thread.Sleep(500);
            }
        }
        else
        {
            driver.Quit();
        }
        return Task.CompletedTask;
    }
}

