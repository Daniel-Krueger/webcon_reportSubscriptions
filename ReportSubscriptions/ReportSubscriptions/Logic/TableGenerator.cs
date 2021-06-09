using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
using ReportSubscriptions.Model;
namespace ReportSubscriptions.Logic
{
    public class TableGenerator
    {
        private ReportData data;
        private Configuration config;
        private StringBuilder tableBuilder = new StringBuilder();
        private Regex bpsUrlPattern = new Regex(@"/.*/db/\d*/");
        private const string choiceIdGroup = "choiceId";
        private const string choiceNameGroup = "choiceName";
        private Regex choicePattern = new Regex(@";{0,1}(?<" + choiceIdGroup + ">[^#]*)#(?<" + choiceNameGroup + ">[^;]*)", RegexOptions.Compiled);
        public TableGenerator(ReportData reportData, Configuration configuration)
        {
            data = reportData;
            config = configuration;
        }


        public string Generate()
        {
            using (TableHelper.Table table = new TableHelper.Table(tableBuilder, id: "bps-reportDataTable"))
            {
                table.StartHead();
                using (var thead = table.AddRow())
                {
                    thead.AddCell("Instance");
                    foreach (var column in data.Columns)
                    {
                        thead.AddCell(column.Name, id: column.Type);
                    }
                }
                table.EndHead();
                table.StartBody();
                int rowNumber = 0;
                int columnCounter = 0;
                foreach (var row in data.Rows)
                {
                    rowNumber++;
                    using (var tr = table.AddRow(classAttributes: (rowNumber % 2 == 0) ? "even" : "odd"))
                    {
                        tr.AddCell($"<a href='{config.PortalUrl}/db/{config.DbId}/app/{config.AppId}/element/{row.ElementId}'>{row.ElementId}</a>");
                        columnCounter = -1;
                        foreach (var value in row.Values)
                        {
                            columnCounter++;
                            if (value == null)
                            {
                                tr.AddCell(null);
                                continue;
                            }
                            string rowValue = value.ToString();
                            string cellValue = null;
                            // Handle links
                            if (bpsUrlPattern.IsMatch(rowValue))
                            {
                                cellValue = config.PortalUrl + rowValue;
                                if (rowValue.Contains("/images/db/"))
                                {
                                    cellValue = $"<img src='{config.PortalUrl + rowValue}'/>";
                                }
                                else
                                {
                                    cellValue = $"<a href='{config.PortalUrl + rowValue}'>{config.PortalUrl + rowValue}</a>";
                                }
                            }
                            // Handle picker columns
                            else if (data.Columns[columnCounter].Type != null &&
                                (data.Columns[columnCounter].Type.Contains("Choice") || data.Columns[columnCounter].Type.Equals("People")))
                            {
                                var matches = choicePattern.Matches(rowValue);

                                for (int i = 0; i < matches.Count; i++)
                                {
                                    Match match = matches[i];
                                    string[] cellValues = new string[matches.Count];
                                    if (match.Success)
                                    {
                                        cellValues[i] = match.Groups[choiceNameGroup].Value;

                                    }
                                    cellValue = string.Join(";", cellValues);
                                }
                            }
                            // Handle date columns
                            else if (data.Columns[columnCounter].Type != null && data.Columns[columnCounter].Type.Contains("Date"))
                            {
                                // We don't know the format of date or time but a space divides them.
                                cellValue = rowValue.Substring(0, rowValue.IndexOf(" "));
                            }
                            else
                            {
                                cellValue = rowValue;
                            }
                            tr.AddCell(cellValue, classAttributes : data.Columns[columnCounter].Type);

                        }
                    }
                }
                table.EndBody();
            }
            return tableBuilder.ToString();
        }
    }
}
