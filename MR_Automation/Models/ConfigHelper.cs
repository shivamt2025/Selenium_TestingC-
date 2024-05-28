using System.Configuration;

namespace YourNamespace.Helpers
{
    public static class ConfigHelper
    {
        public static string GetExcelFilePath()
        {
            return ConfigurationManager.AppSettings["ExcelFilePath"];
        }
    }
}
