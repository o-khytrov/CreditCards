using System.Collections.ObjectModel;
using ApprovalTests;
using ApprovalTests.Reporters;
using CreditCards.UITests.PageObjectModels;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace CreditCards.UITests;

[Trait("Category", "Application")]
public class CreditCardApplicationShould : IClassFixture<ChromeDriverFixture>
{
    private readonly ChromeDriverFixture _chromeDriverFixture;
    private const string ApplyUrl = "http://localhost:5000/Apply";
    private const string AboutUrl = "http://localhost:5000/Home/About";
    private const string ApplyLowRateTitle = "Credit Card Application - Credit Cards";

    public CreditCardApplicationShould(ChromeDriverFixture chromeDriverFixture)
    {
        _chromeDriverFixture = chromeDriverFixture;
        _chromeDriverFixture.Driver.Manage().Cookies.DeleteAllCookies();
        _chromeDriverFixture.Driver.Navigate().GoToUrl("about:blank");
    }

    [Fact]
    public void BeInitiatedFromPage_NewLowRate()
    {
        var homePage = new HomePage(_chromeDriverFixture.Driver);
        homePage.NavigateTo();
        var applicationPage = homePage.ClickApplyLowRateLink();
        applicationPage.EnsurePageLoaded();
    }

    [Fact]
    public void BeInitiatedFromPage_RandomGreeting_UsingXPath()
    {
        var driver = _chromeDriverFixture.Driver;
        driver.Navigate().GoToUrl(HomePage.Url);
        var applyLink = driver.FindElement(By.XPath("/html/body/div/div[4]/div/p/a"));
        applyLink.Click();
        Assert.Equal(ApplyLowRateTitle, driver.Title);
        Assert.Equal(ApplyUrl, driver.Url);
    }


    [Fact]
    public void BeInitiatedFromPage_EasyApplication()
    {
        var driver = _chromeDriverFixture.Driver;
        var homePage = new HomePage(driver);
        homePage.NavigateTo();
        var applicationPage = homePage.WaitForEasyApplicationCarouselPage();
        applicationPage.EnsurePageLoaded();
    }

    [Fact]
    public void DisplayProductsAndRates()
    {
        var driver = _chromeDriverFixture.Driver;
        var homePage = new HomePage(driver);
        homePage.NavigateTo();
        Assert.Equal("Easy Credit Card", homePage.ProductCells[0].name);
        Assert.Equal("20% APR", homePage.ProductCells[0].interestRate);
    }

    [Fact]
    public void BeSubmittedWhenValid()
    {
        var driver = _chromeDriverFixture.Driver;
        var applicationPage = new ApplicationPage(driver);
        applicationPage.NavigateTo();
        applicationPage.EnterFirstName("Sarah");
        applicationPage.EnterLastName("Smith");
        applicationPage.EnterFrequentFlyerNumber("123456-A");
        applicationPage.EnterAge("18");
        applicationPage.SelectMaritalStatusSingle();
        applicationPage.SelectBusinessSourceTv();
        applicationPage.AcceptTerms();
        applicationPage.SetGrossAnnualIncome("50000");
        var applicationCompletePage = applicationPage.SubmitForm();
        applicationCompletePage.EnsurePageLoaded();

        Assert.Equal("ReferredToHuman", applicationPage.Decision);
        Assert.NotEmpty(applicationPage.ReferenceNumber);
        Assert.Equal("Sarah Smith", driver.FindElement(By.Id("FullName")).Text);
    }

    [Fact]
    public void BeSubmittedWhenValidationErrorsCorrected()
    {
        const string firstName = "Sarah";
        const string lastName = "Smith";
        const string age = "18";
        var driver = _chromeDriverFixture.Driver;
        var applicationPage = new ApplicationPage(driver);
        applicationPage.NavigateTo();
        applicationPage.EnterFirstName("Sarah");
        applicationPage.EnterFrequentFlyerNumber("123456-A");
        applicationPage.EnterAge("5");
        applicationPage.SelectMaritalStatusSingle();
        applicationPage.SelectBusinessSourceTv();
        applicationPage.AcceptTerms();
        applicationPage.SetGrossAnnualIncome("50000");
        applicationPage.SubmitForm();


        var validationErrors = applicationPage.ValidationErrors;
        //Assert that validation failed 
        Assert.Equal(2, validationErrors.Count);
        Assert.Equal("Please provide a last name", validationErrors[0].Text);
        Assert.Equal("You must be at least 18 years old", validationErrors[1].Text);

        //Fix Validation Errors
        applicationPage.ClearAge();
        applicationPage.EnterAge("18");
        applicationPage.EnterLastName("Smith");

        //Re-submit the form 
        driver.FindElement(By.Id("FirstName")).Submit();

        //Check form submitted
        Assert.StartsWith("Application Complete", driver.Title);
        Assert.NotEmpty(driver.FindElement(By.Id("ReferenceNumber")).Text);
        Assert.Equal("Sarah Smith", driver.FindElement(By.Id("FullName")).Text);
    }

    [Fact]
    public void OpenContactFooterLinkInNewTab()
    {
        var driver = _chromeDriverFixture.Driver;
        var homePage = new HomePage(driver);
        homePage.NavigateTo();
        homePage.ClickContactFooterLink();
        var allTabs = driver.WindowHandles;
        var contactPageTab = allTabs[1];
        driver.SwitchTo().Window(contactPageTab);
        Assert.EndsWith("/Home/Contact", driver.Url);
    }

    [Fact]
    public void AlertIfLiveChatClosed()
    {
        var driver = _chromeDriverFixture.Driver;
        var homePage = new HomePage(driver);
        homePage.NavigateTo();
        homePage.ClickLiveChatLink();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        var alert = wait.Until(ExpectedConditions.AlertIsPresent());
        Assert.Equal("Live chat is currently closed.", alert.Text);
        alert.Accept();
    }

    [Fact]
    public void NotNavigateToAboutUsPageWhenCancelClicked()
    {
        var driver = _chromeDriverFixture.Driver;
        var homePage = new HomePage(driver);
        homePage.NavigateTo();
        homePage.ClickContactLearnAboutUsLink();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        var alert = wait.Until(ExpectedConditions.AlertIsPresent());
        alert.Dismiss();
        Assert.Equal(HomePage.Title, driver.Title);
    }

    [Fact]
    public void NotDisplayCookieUseMessage()
    {
        var driver = _chromeDriverFixture.Driver;
        var homePage = new HomePage(driver);
        homePage.NavigateTo();
        driver.Manage().Cookies.AddCookie(new Cookie("acceptedCookies", "true"));
        homePage.Refresh();
        Assert.False(homePage.IsCookieMessagePresent());
    }

    [Fact]
    [UseReporter(typeof(BeyondCompareReporter))]
    public void RenderAboutPage()
    {
        var driver = _chromeDriverFixture.Driver;
        driver.Navigate().GoToUrl(AboutUrl);
        var screenshotDriver = (ITakesScreenshot) driver;
        var screenshot = screenshotDriver.GetScreenshot();
        screenshot.SaveAsFile("aboutPage.png", ScreenshotImageFormat.Png);
        var file = new FileInfo("aboutPage.png");
        //Approvals.Verify(file);
    }
}