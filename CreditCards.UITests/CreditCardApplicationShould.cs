using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace CreditCards.UITests;

[Trait("Category", "Application")]
public class CreditCardApplicationShould
{
    private const string HomeUrl = "http://localhost:5000/";

    private const string ApplyUrl = "http://localhost:5000/Apply";
    private const string AboutUrl = "http://localhost:5000/Home/About";
    private const string ApplyLowRateTitle = "Credit Card Application - Credit Cards";
    private const string HomeTitle = "Home Page - Credit Cards";

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
        Assert.Equal(ApplyLowRateTitle, driver.Title);
        Assert.Equal(ApplyUrl, driver.Url);
    }

    [Fact]
    public void BeInitiatedFromPage_RandomGreeting_UsingXPath()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        var applyLink = driver.FindElement(By.XPath("/html/body/div/div[4]/div/p/a"));
        applyLink.Click();
        Assert.Equal(ApplyLowRateTitle, driver.Title);
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
        Assert.Equal(ApplyLowRateTitle, driver.Title);
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
        Assert.Equal(ApplyLowRateTitle, driver.Title);
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

    [Fact]
    public void BeSubmittedWhenValid()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(ApplyUrl);
        var firstNameField = driver.FindElementById("FirstName");
        firstNameField.SendKeys("Sarah");
        var lastNameField = driver.FindElementById("LastName");
        lastNameField.SendKeys("Smith");
        var frequentFlyerNumber = driver.FindElementById("FrequentFlyerNumber");
        frequentFlyerNumber.SendKeys("123456-A");
        driver.FindElementById("Age").SendKeys("18");
        driver.FindElementById("GrossAnnualIncome").SendKeys("50000");
        var singleRadio = driver.FindElementById("Single");
        singleRadio.Click();
        var businessSourceElement = driver.FindElementById("BusinessSource");
        var businessSource = new SelectElement(businessSourceElement);
        Assert.Equal("I'd Rather Not Say", businessSource.SelectedOption.Text);
        Assert.Equal(5, businessSource.Options.Count);
        businessSource.SelectByValue("Email");
        driver.FindElementById("TermsAccepted").Click();
        firstNameField.Submit();
        Assert.StartsWith("Application Complete", driver.Title);
        Assert.NotEmpty(driver.FindElementById("ReferenceNumber").Text);
        Assert.Equal("Sarah Smith", driver.FindElementById("FullName").Text);
    }

    [Fact]
    public void BeSubmittedWhenValidationErrorsCorrected()
    {
        const string firstName = "Sarah";
        const string lastName = "Smith";
        const string age = "18";
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(ApplyUrl);
        var firstNameField = driver.FindElementById("FirstName");
        firstNameField.SendKeys(firstName);
        var frequentFlyerNumber = driver.FindElementById("FrequentFlyerNumber");
        frequentFlyerNumber.SendKeys("123456-A");
        driver.FindElementById("Age").SendKeys("5");
        driver.FindElementById("GrossAnnualIncome").SendKeys("50000");
        var singleRadio = driver.FindElementById("Single");
        singleRadio.Click();
        var businessSourceElement = driver.FindElementById("BusinessSource");
        var businessSource = new SelectElement(businessSourceElement);
        Assert.Equal("I'd Rather Not Say", businessSource.SelectedOption.Text);
        Assert.Equal(5, businessSource.Options.Count);
        businessSource.SelectByValue("Email");
        driver.FindElementById("TermsAccepted").Click();
        firstNameField.Submit();

        //Assert that validation failed 
        var validationErrors = driver.FindElementsByCssSelector(".validation-summary-errors > ul > li");
        Assert.Equal(2, validationErrors.Count);
        Assert.Equal("Please provide a last name", validationErrors[0].Text);
        Assert.Equal("You must be at least 18 years old", validationErrors[1].Text);

        //Fix Validation Errors
        driver.FindElementById("Age").Clear();
        driver.FindElementById("Age").SendKeys(age);
        var lastNameField = driver.FindElementById("LastName");
        lastNameField.SendKeys(lastName);

        //Re-submit the form 
        driver.FindElementById("FirstName").Submit();

        //Check form submitted
        Assert.StartsWith("Application Complete", driver.Title);
        Assert.NotEmpty(driver.FindElementById("ReferenceNumber").Text);
        Assert.Equal("Sarah Smith", driver.FindElementById("FullName").Text);
    }

    [Fact]
    public void OpenContactFooterLinkInNewTab()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        driver.FindElementById("ContactFooter").Click();
        var allTabs = driver.WindowHandles;
        var homepaPageTab = allTabs[0];
        var contactPageTab = allTabs[1];
        driver.SwitchTo().Window(contactPageTab);
        Assert.EndsWith("/Home/Contact", driver.Url);
    }

    [Fact]
    public void AlertIfLiveChatClosed()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        driver.FindElementById("LiveChat").Click();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        var alert = wait.Until(ExpectedConditions.AlertIsPresent());
        Assert.Equal("Live chat is currently closed.", alert.Text);
        alert.Accept();
    }

    [Fact]
    public void NotNavigateToAboutUsPageWhenCancelClicked()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        driver.FindElementById("LearnAboutUs").Click();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        var alert = wait.Until(ExpectedConditions.AlertIsPresent());
        alert.Dismiss();
        Assert.Equal(HomeTitle, driver.Title);
    }

    [Fact]
    public void NotDisplayCookieUseMessage()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(HomeUrl);
        driver.Manage().Cookies.AddCookie(new Cookie("acceptedCookies", "true"));
        driver.Navigate().Refresh();
        var message = driver.FindElementsById("CookiesBeingUsed");
        Assert.Empty(message);
    }

    [Fact]
    [UseReporter(typeof(BeyondCompareReporter))]
    public void RenderAboutPage()
    {
        using var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(AboutUrl);
        var screenshotDriver = (ITakesScreenshot) driver;
        var screenshot = screenshotDriver.GetScreenshot();
        screenshot.SaveAsFile("aboutPage.png", ScreenshotImageFormat.Png);
        var file = new FileInfo("aboutPage.png");
        Approvals.Verify(file);
    }
}