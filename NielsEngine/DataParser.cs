using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NielsEngine
{
    class DataParser:Db
    {
        internal void PrepareSalesData()
        {
            var week = 0;
            if (Common.tracker.ContainsKey("last_week"))
            {

                try
                {
                    week = (int) Common.tracker["last_week"];
                    week = week + 1;
                }
                catch (Exception e)
                {
                    
                    week = GetCurrWkNum(); ;
                }
            }
            else
            {
                week = 1;
            }
            var curWk = GetCurrWkNum();
            if (week >= curWk) return;
            var fDay = FirstDateOfWeek(DateTime.Now.Year, week);
            var monday = fDay.AddDays(-(int)fDay.DayOfWeek + (int)DayOfWeek.Monday);
            var sunday = monday.AddDays(7);
            var str = GetGridQuery(monday, sunday);
            var dt = GetData(str);
            var finalFileName = $"{Common._commonAppDataFolder}" + $"\\week_{week}.txt";
            if (dt == null)
                return;
            if (Utils.ExportDatatoCsv(dt, finalFileName))
                Common.tracker["last_week"] = week;
        }

        private static int GetCurrWkNum()
        {
            int week;
            CultureInfo cul = CultureInfo.CurrentCulture;
            int weekNum = cul.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay,
                DayOfWeek.Monday);
            week = weekNum;
            return week;
        }

        private string GetGridQuery(DateTime startDate, DateTime endDate)
        {
            
            var sql = $"SELECT pos_trans_details.code AS stockcode,null as date,null as brcode,null as company, stock.scancode,	pos_trans_details.description,dept_code,department.NAME AS dept,stock.category AS cat_code,category.NAME AS category,supplier.name as supplier, SUM(qty::numeric) AS qty, unit_incl as price,sum(discount_amount) as discount,SUM(total_incl) AS svalue FROM pos_trans_details LEFT JOIN department ON department.code = pos_trans_details.dept_code left JOIN stock on stock.stockcode = pos_trans_details.code JOIN category on category.code = stock.category left join supplier on supplier.accno = pos_trans_details.supp_accno  ";//stock.category as cat_code,

            sql += $" where pos_trans_details.run_date BETWEEN '{startDate:d}' AND '{endDate:d}'";

         

            sql += " GROUP BY dept_code,department.NAME,stock.category,stock.scancode, category.name,pos_trans_details.code,pos_trans_details.description,supplier.name,unit_incl ORDER BY pos_trans_details.code,	unit_incl  ";

            var sql2 = sql.Replace("pos_", "till_");

            var fsql =
                $"SELECT stockcode,'{startDate:dd/MM/yyyy} - {endDate.AddDays(-1):dd/MM/yyyy}'  as date,'{Common.tracker["branch"]["code"]}' as brcode,'{Common.tracker["branch"]["name"]}'  as company, scancode,	description,dept_code,dept,cat_code,category,supplier, SUM(qty::numeric) AS qty, price,sum(discount) as discount,SUM(svalue) AS svalue FROM ";
            fsql += $"(({sql}) UNION ALL ({sql2})) as a ";
            fsql += $"GROUP BY stockcode,scancode,	description,dept_code,dept,cat_code,category,supplier, price;";

            return fsql;
        }
        static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);

            int daysOffset = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset);

            int firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(jan1, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);

            if (firstWeek <= 1)
            {
                weekOfYear -= 1;
            }

            return firstMonday.AddDays(weekOfYear * 7);
        }


    }
}
