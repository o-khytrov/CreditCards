using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace CreditCards.UITests;

public class CreditCardWebAppShould
{
    private const string HomeUrl = "http://localhost:5258/";

    private const string AboutUrl = "http://localhost:5258/Home/About";

    private const string HomePageCreditCards = "Home Page - Credit Cards";

    [Fact]
    [Trait("Category", "Smoke")]
    public void LoadApplicationPage()
    {
        using IWebDriver driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        Assert.Equal(HomePageCreditCards, driver.Title);
        Assert.Equal(HomeUrl, driver.Url);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void ReloadHomePage()
    {
        using IWebDriver driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        var generationTokenElement = driver.FindElement(By.Id("GenerationToken"));
        Assert.Equal(HomePageCreditCards, driver.Title);
        Assert.Equal(HomeUrl, driver.Url);
        driver.Navigate().Refresh();
        Assert.Equal(HomePageCreditCards, driver.Title);
        Assert.Equal(HomeUrl, driver.Url);
        var generationTokenElementRefreshed = driver.FindElement(By.Id("GenerationToken"));
        Assert.NotEqual(generationTokenElement, generationTokenElementRefreshed);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void ReloadHomePageOnBack()
    {
        using IWebDriver driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        driver.Navigate().GoToUrl(AboutUrl);
        driver.Navigate().Back();
        Assert.Equal(HomePageCreditCards, driver.Title);
        Assert.Equal(HomeUrl, driver.Url);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void ReloadHomePageOnForward()
    {
        using IWebDriver driver = new ChromeDriver();
        driver.Navigate().GoToUrl(AboutUrl);
        driver.Navigate().GoToUrl(HomeUrl);
        driver.Navigate().Back();
        driver.Navigate().Forward();
        Assert.Equal(HomePageCreditCards, driver.Title);
        Assert.Equal(HomeUrl, driver.Url);
    }
}