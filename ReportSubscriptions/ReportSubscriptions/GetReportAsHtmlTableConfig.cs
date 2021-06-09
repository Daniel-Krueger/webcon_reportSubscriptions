using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace ReportSubscriptions
{
    public class GetReportAsHtmlTableConfig : PluginConfiguration
    {
        [ConfigEditableText(DisplayName = "App Client Id", Description = "Provide the name of the registered App see https://community.webcon.com/posts/post/examples-of-using-rest-api/109 for more details.", DescriptionAsHTML = true, IsRequired = true)]
        public string ClientId { get; set; }

        [ConfigEditableText(DisplayName = "App Client Secret", Description = "", IsPasswordField = true,IsRequired =true)]
        public string ClientSecret { get; set; }

        [ConfigEditableText(DisplayName ="Impersonation login", Description = "COS_BPSID of the user in which name the report data should be retrieved", IsRequired = true)]
        public string ImpersonationLogin{ get; set; }


        [ConfigEditableText(DisplayName = "View url", Description = "The full url of the report/view which should be queried. Examples:<ul><li>Default view <br/>https://wbc.example.com/db/1/app/22/report/56/</li><li>Specified view including private <br/>https://wbc.example.com/db/1/app/22/report/56/views/10</li>", DescriptionAsHTML =true, IsRequired = true)]
        public string ViewUrl { get; set; }

    }
}