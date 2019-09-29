using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class Impl_Customer:BaseDB,ICustomer
    {
        private SqlConnection cnn;

        public Impl_Customer(SqlConnection cnn)
        {
            this.cnn = cnn;
        }

        //檢查帳號是否重複
        public bool CheckPhoneDuplicate(string Phone)
        {
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM customer WHERE custPhone=@phone", cnn))
                {
                    cmd.Parameters.Add("@phone", SqlDbType.Char).Value = Phone;

                    cnn.Open();
                    int cc = (int)cmd.ExecuteScalar();
                    cmd.Dispose();
                    cnn.Close();

                    if (cc > 0)
                        return true;
                    else
                        return false;
                }
            }
        }

        public bool AddNewMember(string Phone, string PW, string Name, string Email)
        {
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO customer(custPhone,custPW,custName,custEmail) VALUES(@phone,@pw,@name,@email)", cnn))
                {
                    cmd.Parameters.Add("@phone", SqlDbType.Char).Value = Phone;
                    cmd.Parameters.Add("@pw", SqlDbType.VarChar).Value = PW;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = Name;
                    cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = Email;

                    cnn.Open();
                    int cc = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cnn.Close();

                    if (cc > 0)
                        return true;
                    else
                        return false;
                }
            }
        }

        public bool CheckLogin(string Phone, string PW)
        {
            int cc;
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM customer WHERE custPhone=@phone AND custPW=@pw", cnn))
                {
                    cmd.Parameters.Add("@phone", SqlDbType.Char).Value = Phone;
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

        public bool ChangePassword(string Phone, string PW)
        {
            int cc;
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE customer SET custPW=@pw WHERE custPhone=@phone", cnn))
                {
                    cmd.Parameters.Add("@phone", SqlDbType.Char).Value = Phone;
                    cmd.Parameters.Add("@pw", SqlDbType.VarChar).Value = PW;

                    cnn.Open();
                    cc = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cnn.Close();
                }
            }

            if (cc > 0)
                return true;
            else
                return false;
        }

        public Dictionary<string, object> CustomerInfo(string Phone)
        {
            Dictionary<string, object> result = null;
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT custPhone,custPW,custName,custEmail FROM customer WHERE custPhone=@phone", cnn))
                {
                    cmd.Parameters.Add("@phone", SqlDbType.Char).Value = Phone;

                    cnn.Open();
                    SqlDataReader mydr = cmd.ExecuteReader();
                    if (mydr.HasRows)
                    {
                        result = new Dictionary<string, object>();
                        mydr.Read();
                        for (int i = 0; i < mydr.FieldCount; i++)
                        {
                            result.Add(mydr.GetName(i), mydr[i]);
                        }
                    }
                    cmd.Dispose();
                    cnn.Close();
                }
            }
            return result;
        }

        public bool UpdateCustermorInfo(string Phone, string PW, string Name, string Email)
        {
            int cc;
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE customer SET custPW=@pw,custName=@name,custEmail=@email WHERE custPhone=@phone", cnn))
                {
                    cmd.Parameters.Add("@phone", SqlDbType.Char).Value = Phone;
                    cmd.Parameters.Add("@pw", SqlDbType.VarChar).Value = PW;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = Name;
                    cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = Email;

                    cnn.Open();
                    cc = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cnn.Close();
                }
            }

            if (cc > 0)
                return true;
            else
                return false;
        }

        public int GetCustIDByPhone(string Phone)
        {
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT custID FROM customer WHERE custPhone=@custPhone", cnn))
                {
                    cmd.Parameters.Add("@custPhone", SqlDbType.NVarChar).Value = Phone;
                    cnn.Open();
                    int custID = (int)cmd.ExecuteScalar();
                    cnn.Close();
                    return custID;
                }
            }
        }

        public DataTable SearchCustomerByName(string Name)
        {
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT custPhone,custPW,custName,custEmail FROM customer WHERE custName LIKE ('%'+@name+'%')", cnn))
                {
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = Name;
                    cnn.Open();
                    SqlDataReader mydr = cmd.ExecuteReader();
                    DataTable tt = new DataTable();
                    tt.Load(mydr);
                    cnn.Close();
                    return tt;
                }
            }
        }
    }
}
