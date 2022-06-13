using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NielsEngine
{
    internal class Utils
    {
        internal static bool ExportDatatoCsv(DataTable dt, string fileName)
        {
            try
            {
                var columnCount = 0;
                if (dt != null)
                    columnCount = dt.Columns.Count;
                var columnNames = "";
                var rC = 0;
                if (dt != null)
                    rC = dt.Rows.Count;
                var outputCsv = new string[rC+ 1];


                for (var i = 0; i < columnCount; i++)
                    columnNames += dt.Columns[i].ColumnName + " | ";
                outputCsv[0] += $"﻿Stockcode | Date Range | Br Code | Store Name | Barcode | Description | D/Code | Department | C/Code | Category | Supplier | Qty. Sold | Price | Discount | Sales Value | ";

                if (dt != null)
                    for (var i = 1; i - 1 < dt.Rows.Count; i++)
                    for (var j = 0; j < columnCount; j++)
                    {
                        if (j == 4)
                            dt.Rows[i - 1][j] = RemoveSpecialCharacters(dt.Rows[i - 1][j].ToString().Trim());

                        outputCsv[i] += dt.Rows[i - 1][j].ToString().Trim() + " | ";
                    }

                File.WriteAllLines(fileName, outputCsv, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                Common.log(ex);
                return false;
            }
        }
        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }
    }
}