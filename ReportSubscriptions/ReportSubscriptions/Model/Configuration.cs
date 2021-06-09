using System.Text.RegularExpressions;
namespace ReportSubscriptions.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Copied from https://github.com/WEBCON-BPS/RestApi-DataImporter
    /// </remarks>
    public class Configuration
    {
        private readonly Regex segmentPattern = new Regex(@"/([\w]*)/([\d]*)", System.Text.RegularExpressions.RegexOptions.Compiled);
        public string PortalUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ImpersonationLogin { get; set; }
        public int DbId { get; set; }

        public int AppId { get; set; }

        public int ReportId { get; set; }
        public int? ViewId { get; set; }
        public int? Page { get; set; }
        public int? Size { get; set; }

        private string viewUrl;
        public string ViewUrl
        {
            get { return viewUrl; }
            set
            {
                viewUrl = value;
                var uri = new System.Uri(viewUrl.ToLower());
                PortalUrl = uri.AbsoluteUri.Substring(0, uri.AbsoluteUri.Length - uri.AbsolutePath.Length);
                var matches = segmentPattern.Matches(uri.AbsolutePath);
                DbId = 0;
                AppId = 0;
                ReportId = 0;
                ViewId = null;

                foreach (Match match in matches)
                {
                    if (match.Groups[1].Value == "db")
                    {
                        DbId = int.Parse(match.Groups[2].Value);
                    }
                    if (match.Groups[1].Value == "app")
                    {
                        AppId = int.Parse(match.Groups[2].Value);
                    }
                    if (match.Groups[1].Value == "report")
                    {
                        ReportId = int.Parse(match.Groups[2].Value);
                    }
                    if (match.Groups[1].Value == "view")
                    {
                        ViewId = int.Parse(match.Groups[2].Value);
                    }
                }
            }
        }
    }
}
