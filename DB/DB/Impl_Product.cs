using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class Impl_Product:BaseDB,IProduct
    {
        private SqlConnection cnn;

        public Impl_Product(SqlConnection cnn)
        {
            this.cnn = cnn;
        }

        public DataTable GetProducts()
        {
            DataTable tt = null;
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT proID,proName,PTID,proPrice,proStock,proState,proNote,proPic FROM product", cnn))
                {
                    cnn.Open();
                    using (SqlDataReader mydr = cmd.ExecuteReader())
                    {
                        if (mydr.HasRows)
                        {
                            tt = new DataTable();
                            tt.Load(mydr);
                            mydr.Close();
                        }
                    }
                    cnn.Close();
                }
            }
            return tt;
        }

        public DataTable GetProductList()
        {
            DataTable tt = null;
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT proID, proName, PTName, proPic FROM product AS A INNER JOIN productType AS B ON A.PTID=B.PTID WHERE proState=1", cnn))
                {
                    cnn.Open();
                    using (SqlDataReader mydr = cmd.ExecuteReader())
                    {
                        if (mydr.HasRows)
                        {
                            tt = new DataTable();
                            tt.Load(mydr);
                            mydr.Close();
                        }
                    }
                    cnn.Close();
                }
            }
            return tt;
        }

        public DataTable GetProductListByType(int typeid)
        {
            DataTable tt = null;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cnn;
                if (typeid != 0)
                {
                    cmd.CommandText = "SELECT proID,proName,proPrice,proNote,B.PTName,proStock,proPic,ROW_NUMBER() OVER(ORDER BY proID) AS countID FROM product AS A INNER JOIN productType AS B ON A.PTID=B.PTID WHERE proState=1 AND A.PTID=@PTID";
                    cmd.Parameters.Add("@PTID", SqlDbType.NVarChar).Value = typeid;
                }
                else
                    cmd.CommandText = "SELECT proID,proName,proPrice,proNote,B.PTName,proStock,proPic,ROW_NUMBER() OVER(ORDER BY proID) AS countID FROM product AS A INNER JOIN productType AS B ON A.PTID=B.PTID WHERE proState=1";
                cnn.Open();
                using (SqlDataReader mydr = cmd.ExecuteReader())
                {
                    if (mydr.HasRows)
                    {
                        tt = new DataTable();
                        tt.Load(mydr);
                        mydr.Close();
                    }
                }
                cnn.Close();
            }
            return tt;
        }

        public Dictionary<string, object> GetSingleProduct(int proID)
        {
            Dictionary<string, object> result = null;

            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnn;                    
                    cmd.CommandText = "SELECT proID,proName,A.PTID,PTName,proPrice,proNote,proStock,proState,proPic FROM product AS A INNER JOIN productType AS B ON A.PTID=B.PTID WHERE proID=@proID";
                    cmd.Parameters.Add("@proID", SqlDbType.Int).Value = proID;
                                        
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
                    mydr.Close();                    
                    cnn.Close();
                }
            }

            return result;
        }

        public DataTable GetProductType()
        {
            DataTable tt = null;

            using (SqlCommand cmd = new SqlCommand("SELECT PTID,PTName,PTPic FROM productType", cnn))
            {
                cnn.Open();
                using (SqlDataReader mydr = cmd.ExecuteReader())
                {
                    if (mydr.HasRows)
                    {
                        tt = new DataTable();
                        tt.Load(mydr);
                        mydr.Close();
                    }
                }
                cnn.Close();
            }
            return tt;
        }
        public DataTable SearchProductsByName(string proName)
        {
            DataTable tt = null;
            using (SqlCommand cmd = new SqlCommand("SELECT proID,proName,proPrice,proNote,B.PTName,proStock,proPic,ROW_NUMBER() OVER(ORDER BY proID) AS countID FROM product AS A INNER JOIN productType AS B ON A.PTID=B.PTID WHERE proState=1 AND proName LIKE ('%'+@proName+'%')", cnn))
            {
                cmd.Parameters.Add("@proName", SqlDbType.NVarChar).Value = proName;
                cnn.Open();
                using (SqlDataReader mydr = cmd.ExecuteReader())
                {
                    if (mydr.HasRows)
                    {
                        tt = new DataTable();
                        tt.Load(mydr);
                        mydr.Close();
                    }
                }
                cnn.Close();
            }

            return tt;
        }

        public bool AddNewProduct(string proName,int PTID,int proPrice,string proNote,int proStock,int proState,string proPic)
        {
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO product(proName,PTID,proPrice,proNote,proStock,proState,proPic) VALUES(@proName,@PTID,@proPrice,@proNote,@proStock,@proState,@proPic)", cnn))
                {
                    cmd.Parameters.Add("@proName", SqlDbType.NVarChar).Value = proName;
                    cmd.Parameters.Add("@PTID", SqlDbType.Int).Value = PTID;
                    cmd.Parameters.Add("@proPrice", SqlDbType.Int).Value = proPrice;
                    cmd.Parameters.Add("@proNote", SqlDbType.NVarChar).Value = proNote;
                    cmd.Parameters.Add("@proStock", SqlDbType.Int).Value = proStock;
                    cmd.Parameters.Add("@proState", SqlDbType.Int).Value = proState;
                    cmd.Parameters.Add("@proPic", SqlDbType.VarChar).Value = proPic;
                                        
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
        
        public bool UpdateProduct(int proID, string proName, int PTID, int proPrice, string proNote, int proStock, int proState, string proPic)
        {
            using (cnn)
            {
                string sql= "UPDATE product SET proName=@proName,PTID=@PTID,proPrice=@proPrice,proNote=@proNote,proStock=@proStock,proState=@proState,proPic=@proPic WHERE proID=@proID";

                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    cmd.Parameters.Add("@proID", SqlDbType.Int).Value = proID;
                    cmd.Parameters.Add("@proName", SqlDbType.NVarChar).Value = proName;
                    cmd.Parameters.Add("@PTID", SqlDbType.Int).Value = PTID;
                    cmd.Parameters.Add("@proPrice", SqlDbType.Int).Value = proPrice;
                    cmd.Parameters.Add("@proNote", SqlDbType.NVarChar).Value = proNote;
                    cmd.Parameters.Add("@proStock", SqlDbType.Int).Value = proStock;
                    cmd.Parameters.Add("@proState", SqlDbType.Int).Value = proState;
                    cmd.Parameters.Add("@proPic", SqlDbType.VarChar).Value = proPic;

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
    }
}
