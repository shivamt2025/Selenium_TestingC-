using AventStack.ExtentReports;
using MR_Automation.Tests;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace MR_Automation
{
    [TestFixture, Order(2)]
    public class AnalysisTest : AnalysisRepository
    {
        private string _responseFilePath;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Read the response file path from the configuration file
            _responseFilePath = ConfigurationSettings.AppSettings["response_sheet"];
        }

        [Test, Order(1)]
        public void SearchAndAnalyzeTopics()
        {
            TestConstants.LogTest = TestConstants.Extent.CreateTest("Search and Analyze Topics").Info("Starting test for searching and analyzing topics.");

            // Perform search for "Snacking"
            PerformSearch("Snacking");

            WebDriverWait wait = new WebDriverWait(TestConstants.Driver, TimeSpan.FromSeconds(50));

            try
            {
                // Wait for the project to be clickable and select it
                IWebElement projectElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(_xPathForProject)));
                projectElement.Click();
                TestConstants.LogTest.Log(Status.Info, "Project selected.");

                // Wait for the review button and click it
                IWebElement reviewButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(_xPathForReviewButton)));
                reviewButton.Click();
                TestConstants.LogTest.Log(Status.Info, "Review button clicked.");

                // Wait for the "Review" message in the top bar
                IWebElement reviewMessageElement = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(_xPathForReviewMessage)));
                TestConstants.LogTest.Log(Status.Info, "Review message found in the top bar.");

                // Wait for the analysis topics element to be clickable and click it to open the dropdown menu
                IWebElement analysisTopicsElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(_xPathForAnalysisTopics)));
                analysisTopicsElement.Click();
                TestConstants.LogTest.Log(Status.Info, "Analysis topics text clicked.");

                // Add a wait after clicking on the analysis topics text to ensure the dropdown menu opens
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(_xPathForDropdownItems)));
                TestConstants.LogTest.Log(Status.Info, "Dropdown menu opened.");

                // Wait for the dropdown items to be visible
                var topics = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(cssSelectorForDropdownItems)));

                // Count the number of dropdown items
                int actualTopicsCount = topics.Count;
                TestConstants.LogTest.Log(Status.Info, $"Dropdown contains {actualTopicsCount} topics.");
                }

            
            catch (Exception ex)
            {
                TestConstants.LogTest.Log(Status.Fail, "An error occurred: " + ex.Message);
            }
        }

        [Test, Order(2)]
        public void VerifyResponseCount()
        {
            TestConstants.LogTest = TestConstants.Extent.CreateTest("Verify Response Count").Info("Starting test to verify response count.");

            try
            {
                // Ensure "Top Picks" button is switched off
                IWebElement topPicksButton = TestConstants.Driver.FindElement(By.XPath(_xPathForTopPicksButton));
                topPicksButton.Click();
                TestConstants.LogTest.Log(Status.Info, "Top Picks button switched off.");

                // Fetch the digit in front of word "response"
                int x = FetchResponseCount();

               
                // Click on "View By" element to open the dropdown menu
                IWebElement viewByElement = TestConstants.Driver.FindElement(By.XPath(_xPathForViewByElement));
                viewByElement.Click();
                TestConstants.LogTest.Log(Status.Info, "View By element clicked.");

                // Wait for the "Transcripts" option to be clickable
                WebDriverWait wait = new WebDriverWait(TestConstants.Driver, TimeSpan.FromSeconds(10));
                IWebElement transcriptsOption = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(_xPathForTranscriptsOption)));

                // Click on "Transcripts" from the dropdown menu
                transcriptsOption.Click();
                TestConstants.LogTest.Log(Status.Info, "Transcripts option selected.");

                // Wait for some time to ensure the UI updates
                System.Threading.Thread.Sleep(2000); // Adjust this time as needed

                // Click on "Go To" element
                IWebElement goToElement = TestConstants.Driver.FindElement(By.XPath(_xPathForGoToElement));
                goToElement.Click();
                TestConstants.LogTest.Log(Status.Info, "Go To element clicked.");





                // Fetch and sum up response counts for each option except "All"
                int sumOfResponseCounts = 0;
                var optionsCount = _xPathsForOptions.Count();
                for (int i = 0; i < optionsCount; i++)
                {
                    IWebElement optionElement = TestConstants.Driver.FindElement(By.XPath(_xPathsForOptions[i]));
                    optionElement.Click();
                    // Wait for some time
                    System.Threading.Thread.Sleep(2000); // Adjust this time as needed
                    int responseCount = FetchResponseCount();
                    sumOfResponseCounts += responseCount;
                    TestConstants.LogTest.Log(Status.Info, $"Response count for option selected: {responseCount}");






                    if (i < optionsCount - 1)
                    {
                        goToElement.Click();
                    }






                }

                // Verify the response count for each option with the data from Excel
                for (int i = 0; i < optionsCount; i++)
                {
                    string optionKey = $"Transcript Response {i + 1}";
                    int expectedResponseCount = FetchResponseCountFromExcel(_responseFilePath, optionKey);
                    int actualResponseCount = FetchResponseCountForOption(i);

                    if (actualResponseCount == expectedResponseCount)
                    {
                        TestConstants.LogTest.Log(Status.Pass, $"Response count for {optionKey} matches the expected count: {actualResponseCount}");
                    }
                    else
                    {
                        TestConstants.LogTest.Log(Status.Fail, $"Response count for {optionKey} ({actualResponseCount}) does not match the expected count: {expectedResponseCount}");
                    }
                }


                // Check if sum of response counts equals to x
                if (sumOfResponseCounts == x)
                    TestConstants.LogTest.Log(Status.Pass, "Sum of response counts matches the initial count.");
                else
                    TestConstants.LogTest.Log(Status.Fail, "Sum of response counts does not match the initial count.");


                // Verify the total responses (x) with the data from Excel
                if (x == FetchResponseCountFromExcel(_responseFilePath, "Responses"))
                {
                    TestConstants.LogTest.Log(Status.Pass, $"Total responses count matches the expected count: {x}");
                }
                else
                {
                    TestConstants.LogTest.Log(Status.Fail, $"Total responses count ({x}) does not match the expected count: {FetchResponseCountFromExcel(_responseFilePath, "Responses")}");
                }



                 int FetchResponseCountForOption(int index)
                {
                    IWebElement optionElement = TestConstants.Driver.FindElement(By.XPath(_xPathsForOptions[index]));
                    optionElement.Click();
                    // Wait for some time
                    System.Threading.Thread.Sleep(2000); // Adjust this time as needed
                    return FetchResponseCount();
                }




            }
            catch (Exception ex)
            {
                TestConstants.LogTest.Log(Status.Fail, "An error occurred: " + ex.Message);
            }
        }
    }
}