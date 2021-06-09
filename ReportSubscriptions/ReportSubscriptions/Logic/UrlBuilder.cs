using ReportSubscriptions.Model;

namespace ReportSubscriptions.Logic
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Copied from https://github.com/WEBCON-BPS/RestApi-DataImporter
    /// </remarks>
    public sealed class UrlBuilder
    {
        private readonly Configuration _config;
        private const string Version = "v3.0";

        public UrlBuilder(Configuration config)
        {
            _config = config;
        }


        public string ReportsViewParams()
        {
            var view = _config.ViewId.HasValue ? $"/views/{_config.ViewId}" : string.Empty;
            return $"api/data/{Version}/db/{_config.DbId}/applications/{_config.AppId}/reports/{_config.ReportId}{view}?page={_config.Page}&size={_config.Size}";
        }
  }
}
