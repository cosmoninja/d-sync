using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace NielsEngine
{
    internal class Db
    {
        private NpgsqlConnection _conn;
        private static readonly string cs = $"Host={Common.tracker["database"]["host"]};Port={Common.tracker["database"]["port"]};Username={Common.tracker["database"]["username"]};Password={Common.tracker["database"]["password"]};Database={Common.tracker["database"]["db"]}";

        internal DataTable GetData(string sql)
        {
            try
            {
                using (_conn = new NpgsqlConnection(cs))
                {
                    NpgsqlDataAdapter dap = new NpgsqlDataAdapter(sql, _conn);
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
              Common.log(e.Message);
                return null;
            }
        }
    }
}
