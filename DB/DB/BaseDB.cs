using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public abstract class BaseDB
    {
        protected SqlConnection conn;
        public SqlConnection Connection {
            get { return this.conn; }
        }
    }
}
