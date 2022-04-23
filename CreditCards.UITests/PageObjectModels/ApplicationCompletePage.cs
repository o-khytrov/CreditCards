using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CreditCards.UITests.PageObjectModels;

public class ApplicationCompletePage
{
    private readonly IWebDriver _driver;

    public static readonly string Url = "http://localhost:5000/Apply";
    public static readonly string Title = "Application Complete - Credit Cards";

    public ApplicationCompletePage(IWebDriver driver)
    {
        _driver = driver;
    }

    public ReadOnlyCollection<(string name, string interestRate)> ProductCells
    {
        get
        {
            var productCells = _driver.FindElements(By.TagName("td"));
            var products = new List<(string, string)>();
            for (var i = 0; i < productCells.Count - 1; i += 2)
            {
                var name = productCells[i].Text;
                var interestRate = productCells[i + 1].Text;
                products.Add((name, interestRate));
            }

            return products.AsReadOnly();
        }
    }

    public string GenerationToken => _driver.FindElement(By.Id("GenerationToken")).Text;

    public void ClickContactFooterLink() => _driver.FindElement(By.Id("ContactFooter")).Click();
    public void ClickContactLearnAboutUsLink() => _driver.FindElement(By.Id("LearnAboutUs")).Click();
    public bool IsCookieMessagePresent() => _driver.FindElements(By.Id("CookiesBeingUsed")).Any();

    public void NavigateTo()
    {
        _driver.Navigate().GoToUrl(Url);
        EnsurePageLoaded();
    }

    public void Refresh()
    {
        _driver.Navigate().Refresh();
    }

    public void EnsurePageLoaded()
    {
        var pageLoaded = _driver.Url == Url && _driver.Title == Title;
        if (!pageLoaded)
        {
            throw new Exception(
                $"Failed to load the page. Page Url = '{_driver.Url}' Page Source = '{_driver.PageSource}'");
        }
    }

    public void ClickLiveChatLink()
    {
        _driver.FindElement(By.Id("LiveChat")).Click();
    }

    public void EnterFirstName(string firstName)
    {
        _driver.FindElement(By.Id("FirstName")).SendKeys(firstName);
    }

    public void EnterLastName(string lastName)
    {
        _driver.FindElement(By.Id("LastName")).SendKeys(lastName);
    }

    public void EnterFrequentFlyerNumber(string frequentFlyerNumber)
    {
        _driver.FindElement(By.Id("FrequentFlyerNumber")).SendKeys(frequentFlyerNumber);
    }

    public void EnterAge(string age)
    {
        _driver.FindElement(By.Id("Age")).SendKeys(age);
    }

    public void SelectMaritalStatusSingle()
    {
        _driver.FindElement(By.Id("Single")).Click();
    }

    public void SelectBusinessSourceTv()
    {
        var businessSourceElement = _driver.FindElement(By.Id("BusinessSource"));
        var businessSource = new SelectElement(businessSourceElement);
        businessSource.SelectByValue("TV");

        _driver.FindElement(By.Id("Single")).Click();
    }

    public void AcceptTerms()
    {
        _driver.FindElement(By.Id("TermsAccepted")).Click();
    }

    public void SetGrossAnnualIncome(string grossIncome)
    {
        _driver.FindElement(By.Id("GrossAnnualIncome")).SendKeys(grossIncome);
    }

    public void SubmitForm()
    {
        _driver.FindElement(By.Id("TermsAccepted")).Submit();
    }
}