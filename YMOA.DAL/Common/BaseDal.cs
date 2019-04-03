using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMOA.DAL
{
    public class BaseDal
    {
        public string ConnString { get; set; } = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        public IDbConnection GetConnection()
        {
            return new SqlConnection(ConnString);
        }
    }
}
