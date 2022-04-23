using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace CreditCards.UITests;

public class JavascriptExamples
{
    [Fact]
    public void RenderAboutPage()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl("http://localhost:5000/JSOverlay.html");
        var script = "document.getElementById('HiddenLink').click()";
        var js = (IJavaScriptExecutor) driver;
        js.ExecuteScript(script);
        Assert.Equal("https://pluralsight.com/", driver.Url);
    }
}