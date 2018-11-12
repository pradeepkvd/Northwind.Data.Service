using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Enterprise.Lib.Core.Helper
{
    public class JsonHelper
    {
        public static DataTable jsonStringToTable(string jsonContent)
        {
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonContent);
            return dt;
        }

        public static string JsonToCSV(string jsonContent, string delimiter)
        {
            return string.Empty;
        }

    }
}
