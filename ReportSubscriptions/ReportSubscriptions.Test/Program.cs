using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportSubscriptions.Logic;
using ReportSubscriptions.Model;
namespace ReportSubscriptions.Test
{
    class Program
    {
        private static string tableStyle = @"
            
<style>
    #bps-reportDataTable{
        border:  black 1px solid;
        border-collapse: collapse;
    }
    #bps-reportDataTable td{
        border:  black 1px solid;
        padding: 5px;
    }
    #bps-reportDataTable > tbody > tr > td.Int {
        text-align: right;
    }
</style>
    ";
        static void Main(string[] args)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
            string configContent;
            using (System.IO.StreamReader file = new System.IO.StreamReader($@"{System.IO.Path.GetTempPath()}\report.subscription\configuration"))
            {
                configContent = file.ReadToEnd();
            }
            Configuration config = null;
            using (var contentReader = new System.IO.StringReader(configContent))
            {
                config = serializer.Deserialize(contentReader) as Configuration;
            }
            config.Page = 1;
            config.Size = 1000;

            var clientProvider = new AutenticatedClientProvider(config);
            var client = clientProvider.GetClientAsync().Result;
            ApiManager apiManager = new ApiManager(client, config);
            ReportData reportData;
            try
            {
                reportData = apiManager.GetReportDataAsync().Result;                
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Equals("InternalError: Object reference not set to an instance of an object."))
                {
                    throw new ApplicationException("There was an error retrieving the data, check whether the report url is valid, that the user has access to the view and there are only supported columns in the view.");
                }
                throw;
            }
            string tableHtml = (new TableGenerator(reportData, config)).Generate();
            string htmlFile = System.IO.Path.GetTempPath() + "report.subscription\\report.html";
            System.IO.File.WriteAllText(htmlFile, tableStyle + tableHtml);
            var fileopener = new System.Diagnostics.Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + htmlFile + "\"";
            fileopener.Start();
        }
    }
}
