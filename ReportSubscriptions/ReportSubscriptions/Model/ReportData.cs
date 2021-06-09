using System.Collections.Generic;

namespace ReportSubscriptions.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Based on https://github.com/WEBCON-BPS/RestApi-DataImporter
    /// </remarks>
    public class ReportData
    {
        public IList<Column> Columns { get; set; }
        public IList<Row> Rows { get; set; }
    }

    public class Row
    {
        public int? ElementId { get; set; }
        public int? RowId { get; set; }
        public object[] Values { get; set; }
    }
    public class Column
    {
        public Column(string guid, string name, string type)
        {
            Guid = guid;
            Name = name;
            Type = type;
        }

        public string Guid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
