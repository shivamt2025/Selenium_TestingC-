using AventStack.ExtentReports;
using MR_Automation.Tests;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using OfficeOpenXml;


namespace MR_Automation
{
    public class AnalysisRepository
    {
        internal string _searchBarXPath = "//*[@id=\"root\"]/div/div[2]/div[1]/div/input";
        internal string _xPathForProject = "//*[@id=\"root\"]/div/div[2]/div[2]/div/div[2]/div";
        internal string _xPathForReviewButton = "//*[@id=\"root\"]/div/div[2]/div[2]/div/div[2]/div/div[5]/div/div";
        internal string _xPathForReviewMessage = "//*[@id=\"root\"]/div/div[2]/div[1]";
        internal string _xPathForAnalysisTopics = "//*[@id=\"panel1a-header\"]/div[1]/span";
        internal string _xPathForDropdownArrow = "//*[@id=\"panel1a-header\"]/div[2]";
        internal string _xPathForDropdownItems = "//*[@id=\"root\"]/div/div[2]/div[2]/div/div[1]/div/div[2]/div/div[1]/div[2]/div/div/div/div";

        internal string cssSelectorForDropdownItems = "div.MuiAccordionDetails-root.css-u7qq7e > div.innerdivedit > div\r\n";




        internal string _xPathForTopPicksButton = "//*[@id=\"root\"]/div/div[2]/div[2]/div/div[2]/div[2]/div[2]/div/div/div[1]/div[2]/div[1]/div/span/input";
        internal string _xPathForViewByElement = "//*[@id=\"root\"]/div/div[2]/div[1]/div[2]/div/div[1]/div";
        internal string _xPathForTranscriptsOption = "//li[@data-value='Transcripts']\r\n";
        internal string _xPathForGoToElement = "//*[@id=\"root\"]/div/div[2]/div[1]/div[2]/div/div[2]/div/div/div";
        internal string[] _xPathsForOptions = {
            "//li[@data-value='47128a42-3b48-4cf8-a9db-349cd163d8f0']\r\n", // Option 1
            "//li[@data-value='cc5f37cf-6df8-4ed4-a297-a73554e9a42d']\r\n", // Option 2
            // Add more XPath for additional options as needed
        };
        internal string _xPathForResponseCount = "//*[@id=\"root\"]/div/div[2]/div[2]/div/div[2]/div[2]/div[2]/div/div/div[1]/div[1]/span";

        public void PerformSearch(string searchQuery)
        {
            TestConstants.LogTest = TestConstants.Extent.CreateTest("Perform Search").Info("Starting search operation.");

            // Find search bar element and enter search query
            IWebElement searchBar = TestConstants.Driver.FindElement(By.XPath(_searchBarXPath));
            searchBar.SendKeys(searchQuery);
            searchBar.SendKeys(Keys.Enter);
            TestConstants.LogTest.Log(Status.Info, $"Search query '{searchQuery}' entered and search initiated.");
        }

        internal int FetchResponseCount()
        {
            // Find the element containing the text "response" and fetch the digit in front of it
            IWebElement responseElement = TestConstants.Driver.FindElement(By.XPath(_xPathForResponseCount));
            string responseText = responseElement.Text;
            int responseCount = int.Parse(new string(responseText.Where(char.IsDigit).ToArray()));
            return responseCount;
        }




        protected int FetchResponseCountFromExcel(string filePath, string key)
        {
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;
                for (int row = 1; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value.ToString().Trim() == key)
                    {
                        return int.Parse(worksheet.Cells[row, 2].Value.ToString().Trim());
                    }
                }
            }
            throw new ArgumentException($"Key '{key}' not found in the Excel file.");
        }
        public Dictionary<string, int> GetExcelData()
        {
            // Get the file path from app.config
            string relativeFilePath = ConfigurationManager.AppSettings["response_sheet"];
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(currentDirectory, relativeFilePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.", filePath);
            }

            return ExcelReader.ReadResponseCountsFromExcel(filePath);
        }
    }
}
