using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class Impl_Admin:BaseDB,IAdmin
    {
        private SqlConnection cnn;

        public Impl_Admin(SqlConnection cnn)
        {
            this.cnn = cnn;
        }

        public bool CheckLogin(string Act, string PW)
        {
            int cc;
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM administrator WHERE adminAct=@act AND adminPW=@pw", cnn))
                {
                    cmd.Parameters.Add("@act", SqlDbType.VarChar).Value = Act;
                    cmd.Parameters.Add("@pw", SqlDbType.VarChar).Value = PW;

                    cnn.Open();
                    cc = (int)cmd.ExecuteScalar();
                    cmd.Dispose();
                    cnn.Close();
                }
            }

            if (cc > 0)
                return true;
            else
                return false;
        }
    }
}
