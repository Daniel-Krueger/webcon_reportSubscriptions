using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ReportSubscriptions.Model;

namespace ReportSubscriptions.Logic
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Based on https://github.com/WEBCON-BPS/RestApi-DataImporter
    /// </remarks>
    public class ApiManager
    {
        private readonly HttpClient _client;
        private readonly Configuration _config;
        private readonly UrlBuilder _urlBuilder;

        public ApiManager(HttpClient client, Configuration config)
        {
            _client = client;
            _config = config;
            _urlBuilder = new UrlBuilder(config);
        }

        #region Reports

        public async Task<ReportData> GetReportDataAsync()
        {
            dynamic result = await SendRequestDynamicAsync(_urlBuilder.ReportsViewParams(), HttpMethod.Get);
            IEnumerable<Column> cols = ParseColumns(result);
            IEnumerable<Row> rows = ParseReportRows(result, cols);
            return new ReportData()
            {
                Columns = cols.ToList(),
                Rows = rows.ToList()
            };
        }

        private IEnumerable<Column> ParseColumns(dynamic result)
        {
            var cols = (IEnumerable<dynamic>)result.columns;

            foreach (var col in cols)
                yield return new Column(col.guid?.ToString(), col.name?.ToString(), col.type?.ToString());
        }

        private IEnumerable<Row> ParseReportRows(dynamic result, IEnumerable<Column> columns)
        {
            var rows = (IEnumerable<dynamic>)result.rows;

            foreach (var row in rows)
            {
                var cells = (IEnumerable<dynamic>)row.cells;
                var id = row.id;
                yield return new Row() { ElementId = id, Values = GetCellsObjectValues(cells, columns).ToArray() };
            }
        }

        private IEnumerable<object> GetCellsObjectValues(IEnumerable<dynamic> cells, IEnumerable<Column> columns)
        {
            for (int i = 0; i < columns.Count(); i++)
            {
                switch (columns.ElementAt(i).Type)
                {
                    case "ChoicePicker":
                    case "Autocomplete":
                    case "ChoiceList":
                    case "People":
                        {
                            var value = cells.ElementAt(i).value;

                            if (value is JValue jVal)
                            {
                                yield return jVal.ToString();
                                break;
                            }

                            if (value is JArray jArray)
                            {
                                yield return string.Join(";", jArray.Select(x => (dynamic)x).Select(x => $"{x.id}#{x.name}"));
                                break;
                            }
                            break;
                        }

                    case "SurveyChoose":
                        {
                            var value = cells.ElementAt(i).value;

                            if (value is JValue jVal)
                            {
                                yield return jVal.ToString();
                                break;
                            }

                            if (value is JArray jArray)
                            {
                                yield return string.Join("|;", jArray.Select(x => (dynamic)x).Select(x => $"{x.id}#{x.name}"));
                                break;
                            }
                            break;
                        }

                    case "LocalAttachments":
                    case "RelativeAttachments":
                        {
                            //unsupported in api update
                            yield return string.Empty;
                            break;
                        }

                    case "Int":
                        {
                            var value = cells.ElementAt(i).value;
                            yield return value != null ? (int?)Convert.ToInt32(value) : null;

                            break;
                        }

                    case "Decimal":
                        {
                            var value = cells.ElementAt(i).value;
                            yield return value != null ? (double?)Convert.ToDouble(value) : null;

                            break;
                        }

                    case "Date":
                        {
                            var value = cells.ElementAt(i).value;
                            yield return value != null ? (DateTime?)Convert.ToDateTime(value) : null;

                            break;
                        }

                    default:
                        {
                            yield return (string)Convert.ToString(cells.ElementAt(i).value);
                            break;
                        }
                }
            }
        }

        #endregion


        private async Task<dynamic> SendRequestDynamicAsync(string link, HttpMethod method, HttpContent content = null)
        {
            var request = await _client.SendAsync(new HttpRequestMessage(method, link) { Content = content });
            var response = await request.Content.ReadAsStringAsync();

            if (!request.IsSuccessStatusCode)
                throw new ApiException().CreateEx(response);

            dynamic result = JsonConvert.DeserializeObject(response);
            return result;
        }

        private async Task<(bool ok, string response)> SendRequestAsync(string link, HttpMethod method, HttpContent content = null)
        {
            var request = await _client.SendAsync(new HttpRequestMessage(method, link) { Content = content });
            var response = await request.Content.ReadAsStringAsync();

            return (request.IsSuccessStatusCode, response);
        }
    }
}
