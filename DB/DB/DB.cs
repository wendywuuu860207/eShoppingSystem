using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class DB:BaseDB
    {
        public DB()
        {
            this.conn = new SqlConnection("server=TINA\\SQLEXPRESS;database=ShopSystem;uid=SQLUser;pwd=user1234");
        }
        public DB(string username,string password)
        {
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
            scsb.DataSource = "TINA\\SQLEXPRESS";
            scsb.InitialCatalog = "ShopSystem";
            scsb.IntegratedSecurity = false;
            scsb.UserID = username;
            scsb.Password = password;
            this.conn = new SqlConnection(scsb.ConnectionString);
        }
    }
}
