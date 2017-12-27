﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MarkdownMonster.Annotations;
using Westwind.Utilities;

namespace MarkdownMonster.Windows
{
    public class TableParser
    {


        public string TableHeaders { get; set; }

        /// <summary>
        /// Maximum column width that's to be padded.
        /// If greater values are rendering ragged
        /// </summary>
        public int MaxColumnWidth { get; set; } = 40;



        private ObservableCollection<ObservableCollection<CellContent>> _tableData;

        public ObservableCollection<ObservableCollection<CellContent>> TableData
        {
            get
            {
                if (_tableData == null)
                    _tableData = new ObservableCollection<ObservableCollection<CellContent>>();
                return _tableData;
            }
            set { _tableData = value; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableData"></param>
        /// <returns></returns>
        public string ToPipeTableMarkdown(ObservableCollection<ObservableCollection<CellContent>> tableData = null)
        {
            if (tableData == null)
                tableData = TableData;

            if (tableData == null || tableData.Count < 1)
                return string.Empty;

            for (int i = tableData.Count-1; i > -1; i--)
            {
                if (tableData[i] == null || tableData[i].Count == 0)
                    tableData.Remove(tableData[i]);
            }

            var columnInfo = GetColumnInfo(tableData);         

            StringBuilder sb = new StringBuilder();                        
            sb.Clear();

            string line = "\n| ";
            for (int i = 0; i < columnInfo.Count; i++)
            {
                var colInfo = columnInfo[i];                
                line += $"{colInfo.Title.PadRight(colInfo.MaxWidth)} | ";
            }
            sb.AppendLine(line.TrimEnd());

            
            sb.Append("|");
            for (int i = 0; i < line.Length-4; i++)
                sb.Append("-");
            sb.AppendLine("|");

            foreach (var row in tableData.Skip(1))
            {
                line = "| ";
                for (int i = 0; i < row.Count; i++)
                {
                    var col = row[i];
                    col.Text = col.Text.Replace("\n", "<br>").Replace("\r", "");

                    var colInfo = columnInfo[i];
                    line += col.Text.PadRight(colInfo.MaxWidth) + " | ";
                }

                sb.AppendLine(line.Trim());
            }

            return sb + "\n";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableData"></param>
        /// <returns></returns>
        public string ToGridTableMarkdown(ObservableCollection<ObservableCollection<CellContent>> tableData = null)
        {
            if (tableData == null)
                tableData = TableData;

            if (tableData == null || tableData.Count < 1)
                return string.Empty;

            for (int i = tableData.Count - 1; i > -1; i--)
            {
                if (tableData[i] == null || tableData[i].Count == 0)
                    tableData.Remove(tableData[i]);
            }

            var columnInfo = GetColumnInfo(tableData);
            for (int i = 0; i < columnInfo.Count; i++)
            {
                var colInf = columnInfo[i];
                for (int j = 0; j < tableData.Count; j++)
                {                    
                    var col = tableData[j][i];                    
                    col.Lines = col.Text.Split('\n');
                    var maxWidth = col.Lines.Max(c => c.Length);
                    if (maxWidth > columnInfo[i].MaxWidth)
                        columnInfo[i].MaxWidth = maxWidth;
                }
            }
            
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            string separatorLine = "+-";
            string line = "| ";
            for (int i = 0; i < columnInfo.Count; i++)
            {
                var colInfo = columnInfo[i];
                line += $"{colInfo.Title.PadRight(colInfo.MaxWidth)} | ";
                separatorLine += "-".PadRight(colInfo.MaxWidth,'-') + "-+-";
            }
            separatorLine = separatorLine.TrimEnd('-');

            sb.AppendLine(separatorLine);
            sb.AppendLine(line.TrimEnd());
            sb.AppendLine(separatorLine.Replace("-", "="));

            foreach (var row in tableData.Skip(1))
            {
                int rowLines = row.Max(s => StringUtils.GetLines(s.Text).Length);

                
                foreach (var col in row)
                {
                    var list = new List<string>();
                    list.AddRange(col.Lines);
                    for (int i = col.Lines.Length; i < rowLines; i++)
                        list.Add(string.Empty);
                    col.Lines = list.ToArray();
                }
            }

            
            foreach (var row in tableData.Skip(1))
            {
                for (int j = 0; j < row[0].Lines.Length; j++)
                {
                    line = "| ";
                    for (int i = 0; i < row.Count; i++)
                    {
                        var col = row[i];
                        line += col.Lines[j].PadRight(columnInfo[i].MaxWidth) + " | ";
                    }

                    sb.AppendLine(line.Trim());
                }

                sb.AppendLine(separatorLine);
            }


            return sb + "\n";
        }

        /// <summary>
        /// Takes the input collection and parses it into an HTML string. First row is considered to be the
        /// header of the table.
        /// </summary>
        /// <param name="tableData"></param>
        /// <returns></returns>
        public string ToTableHtml(ObservableCollection<ObservableCollection<CellContent>> tableData = null)
        {
            if (tableData == null)
                tableData = TableData;

            if (tableData == null || tableData.Count < 1)
                return string.Empty;

            for (int i = tableData.Count - 1; i > -1; i--)
            {
                if (tableData[i] == null || tableData[i].Count == 0)
                    tableData.Remove(tableData[i]);
            }

            var columnInfo = GetColumnInfo(tableData);

            StringBuilder sb = new StringBuilder();
            sb.Clear();
            
            sb.AppendLine("\n<table>");
            sb.AppendLine("<thead>");
            sb.AppendLine("\t<tr>");

            for (int i = 0; i < columnInfo.Count; i++)
            {
                var colInfo = columnInfo[i];
                sb.AppendLine($"\t\t<th>{HtmlUtils.HtmlEncode(colInfo.Title.Trim())}</th>");
            }

            sb.AppendLine("\t</tr>");
            sb.AppendLine("</thead>");

            sb.AppendLine("<tbody>");
            foreach (var row in tableData.Skip(1))
            {
                sb.AppendLine("\t<tr>");
                for (int i = 0; i < row.Count; i++)
                {
                    var col = row[i];
                    col.Text = col.Text.Replace("\n", "<br>").Replace("\r", "");                    
                    sb.AppendLine($"\t\t<td>{HtmlUtils.HtmlEncode(col.Text.Trim())}</td>");
                }

                sb.AppendLine("\t</tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>\n");

            return sb.ToString();
        }


        /// <summary>
            /// Parses a table represented as Markdown into an Observable collection
            /// </summary>
            /// <param name="tableMarkdown"></param>
            /// <returns></returns>
            public ObservableCollection<ObservableCollection<CellContent>> ParseMarkdownToData(string tableMarkdown)
        {
            
            var data = new ObservableCollection<ObservableCollection<CellContent>>();
            if (string.IsNullOrEmpty(tableMarkdown))
                return data;

            if (tableMarkdown.Trim().StartsWith("+-") && tableMarkdown.Trim().EndsWith("-+"))
                return ParseMarkdownGridTableToData(tableMarkdown);

            var lines = StringUtils.GetLines(tableMarkdown.Trim());
            foreach (var row in lines)
            {
                if (row.Length == 0)
                    continue;
                if (row.StartsWith("|---"))
                    continue;

                var cols = row.Trim('|').Split('|');
                var columnData = new ObservableCollection<CellContent>();
                foreach (var col in cols)
                    columnData.Add(new CellContent(col.Trim()));

                data.Add(columnData);
            }


            return data;
        }


        public ObservableCollection<ObservableCollection<CellContent>> ParseMarkdownGridTableToData(string tableMarkdown)
        {
            var data = new ObservableCollection<ObservableCollection<CellContent>>();
            if (string.IsNullOrEmpty(tableMarkdown))
                return data;

            var lines = StringUtils.GetLines(tableMarkdown.Trim());
            
            // loop through rows
            for (var index = 0; index < lines.Length; index++)
            {
             
                var rowText = lines[index];
                if (rowText.Length == 0)
                    continue;                

                if (rowText.StartsWith("+--") || rowText.StartsWith("+=="))
                {
                    var columnData = new ObservableCollection<CellContent>();

                    // goto next 'column line'
                    index++;
                    if (index >= lines.Length)
                        break;
                    rowText = lines[index];

                    var cellText = new List<StringBuilder>();
                    string[] cols = new string[0];

                    while (true)
                    {
                        cols = rowText.Trim('|').Split('|');
                        
                        for (var i = 0; i < cols.Length; i++)
                            cellText.Add(new StringBuilder());
                        
                        for (var i = 0; i < cols.Length; i++)
                        {
                            var col = cols[i];
                            cellText[i].AppendLine(col.Trim());
                        }

                        // get the next line of this column
                        if (lines[index + 1].StartsWith("|"))
                        {
                            index++;
                            rowText = lines[index];
                        }
                        else
                            break;
                    }

                    
                    if (cols.Length == 0)
                        continue;

                    // collect multiple lines per column
                    for (var i = 0; i < cols.Length; i++)
                    {
                        cellText[i].Length -= 2; // strip off trailing \r\n
                        var ctext = cellText[i].ToString().Replace("\r","");                        
                        columnData.Add(new CellContent(ctext));
                    }

                    data.Add(columnData);
                }                
            }

            return data;
        }

        /// <summary>
        /// Retrieves information about each of the columns in the table including
        /// max width and title. Looks at the first row of the table data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tableHeaders"></param>
        /// <returns></returns>
        public List<ColumnInfo> GetColumnInfo(ObservableCollection<ObservableCollection<CellContent>> data)
        {
            var headers = new List<ColumnInfo>();
            if (data == null || data.Count < 1)
                return headers;

            var cols = data[0];

            for (int i = 0; i < cols.Count; i++)
            {
                var header = cols[i].Text;
                var colInfo = new ColumnInfo
                {
                    Title = header,
                    MaxWidth = header.Length
                };
               
                var maxWidth = data.Max(d => d[i].Text.Length);
                if (maxWidth > colInfo.MaxWidth)
                    colInfo.MaxWidth = maxWidth;
                if (colInfo.MaxWidth > MaxColumnWidth)
                    colInfo.MaxWidth = MaxColumnWidth;

                headers.Add(colInfo);
            }

            return headers;
        }
    }

    [DebuggerDisplay("{Title} - {MaxWidth}")]
    public class ColumnInfo
    {
        public string Title;
        public int MaxWidth;
    }



}
