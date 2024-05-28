using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;

namespace MR_Automation.Tests
{
    public class ExcelReader
    {
        public static Dictionary<string, int> ReadResponseCountsFromExcel(string filePath)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var responseCounts = new Dictionary<string, int>();

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var table = result.Tables[0];

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        string key = table.Rows[i][0].ToString();
                        int value = int.Parse(table.Rows[i][1].ToString());
                        responseCounts[key] = value;
                    }
                }
            }

            return responseCounts;
        }
    }
}
