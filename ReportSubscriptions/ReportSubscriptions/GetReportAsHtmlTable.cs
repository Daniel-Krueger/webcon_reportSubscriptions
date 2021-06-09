using System;
using WebCon.WorkFlow.SDK.BusinessRulePlugins;
using WebCon.WorkFlow.SDK.BusinessRulePlugins.Model;
using ReportSubscriptions.Logic;
using ReportSubscriptions.Model;

namespace ReportSubscriptions
{
    public class GetReportAsHtmlTable : CustomBusinessRule<GetReportAsHtmlTableConfig>
    {
        readonly IndentTextLogger logger = new IndentTextLogger();

        public override EvaluationResult Evaluate(CustomBusinessRuleParams args)
        {

            try
            {
                logger.Log("Building configuration");
                var config = new Configuration();
                config.ClientId = Configuration.ClientId;
                config.ClientSecret = Configuration.ClientSecret;
                config.ImpersonationLogin = Configuration.ImpersonationLogin;
                config.ViewUrl = Configuration.ViewUrl;

                var clientProvider = new AutenticatedClientProvider(config);
                logger.Log($"Retrieving report data from view '{config.ViewUrl}' as '{config.ImpersonationLogin}'");
                logger.Indent();
                logger.Log($"Retrieving token");
                var client = clientProvider.GetClientAsync().Result;
                logger.Log($"Creating ApiManager");
                ApiManager apiManager = new ApiManager(client, config);
                logger.Log($"Retrieving report data");
                var reportData = apiManager.GetReportDataAsync().Result;
                logger.Outdent();
                logger.Log($"Generating html table for report data");
                string tableHtml = (new TableGenerator(reportData, config)).Generate();
                logger.Log($"Generating html table for report data");
                args.Context.PluginLogger.AppendInfo(logger.ToString());
                return EvaluationResult.CreateStringResult(tableHtml);
            }
            catch (Exception ex)
            {
                logger.Log("Error building report data", ex, args.Context.CurrentProcessID);
                args.Context.PluginLogger.AppendInfo(logger.ToString());
                if (ex.InnerException != null && ex.InnerException.Message.Equals("InternalError: Object reference not set to an instance of an object."))
                {
                    throw new ApplicationException("There was an error retrieving the data, check whether the report url is valid, that the user has access to the view and there are only supported columns in the view.");
                }                
                throw;
                
            }
        }
    }
}