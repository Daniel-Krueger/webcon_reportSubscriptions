namespace ReportSubscriptions.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Copied from https://github.com/WEBCON-BPS/RestApi-DataImporter
    /// </remarks>
    public class ApiFailureResponse
    {
        public string type { get; set; }
        public string description { get; set; }
        public string errorGuid { get; set; }
    }
}