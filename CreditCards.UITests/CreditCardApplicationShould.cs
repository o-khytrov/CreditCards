using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace CreditCards.UITests;

[Trait("Category", "Application")]
public class CreditCardApplicationShould
{
    private const string HomeUrl = "http://localhost:5258/";

    private const string ApplyUrl = "http://localhost:5258/Apply";

    private readonly ITestOutputHelper _output;

    public CreditCardApplicationShould(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void BeInitiatedFromPage_NewLoad()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        var applyLink = driver.FindElement(By.Name("ApplyLowRate"));
        applyLink.Click();
        Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
        Assert.Equal(ApplyUrl, driver.Url);
    }

    [Fact]
    public void BeInitiatedFromPage_RandomGreeting_UsingXPath()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        var applyLink = driver.FindElement(By.XPath("/html/body/div/div[4]/div/p/a"));
        applyLink.Click();
        Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
        Assert.Equal(ApplyUrl, driver.Url);
    }


    [Fact]
    public void BeInitiatedFromPage_EasyApplication()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        var carouselNext = driver.FindElement(By.CssSelector("[data-slide='next']"));
        carouselNext.Click();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
        var applyLink = wait.Until(webDriver => webDriver.FindElement(By.LinkText("Easy: Apply Now!")));
        applyLink.Click();
        Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
        Assert.Equal(ApplyUrl, driver.Url);
    }

    [Fact]
    public void BeInitiatedFromPage_EasyApplication_PrebuiltConditions()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(35));
        var applyLink = wait.Until(ExpectedConditions.ElementToBeClickable
            (By.ClassName("customer-service-apply-now")));
        applyLink.Click();
        Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
        Assert.Equal(ApplyUrl, driver.Url);
    }

    [Fact]
    public void DisplayProductsAndRates()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        var cell = driver.FindElement(By.TagName("td"));
        var fistProduct = cell.Text;
        Assert.Equal("Easy Credit Card", fistProduct);
    }
}