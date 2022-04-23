using CreditCards.UITests.PageObjectModels;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace CreditCards.UITests;

public class CreditCardWebAppShould
{
    private const string AboutUrl = "http://localhost:5000/Home/About"; 
    private const string HomePageCreditCards = "Home Page - Credit Cards";

    [Fact]
    [Trait("Category", "Smoke")]
    public void LoadApplicationPage()
    {
        using IWebDriver driver = new ChromeDriver();
        var homePage = new HomePage(driver);
        homePage.NavigateTo();
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void ReloadHomePageOnBack()
    {
        using IWebDriver driver = new ChromeDriver();
        var homePage = new HomePage(driver);
        homePage.NavigateTo();
        var initialToken = homePage.GenerationToken;

        driver.Navigate().GoToUrl(AboutUrl);
        driver.Navigate().Back();

        homePage.EnsurePageLoaded();

        Assert.NotEqual(initialToken, homePage.GenerationToken);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void ReloadHomePageOnForward()
    {
        using IWebDriver driver = new ChromeDriver();
        driver.Navigate().GoToUrl(AboutUrl);
        driver.Navigate().GoToUrl(HomePage.Url);
        driver.Navigate().Back();
        driver.Navigate().Forward();
        Assert.Equal(HomePageCreditCards, driver.Title);
        Assert.Equal(HomePage.Url, driver.Url);
    }
}