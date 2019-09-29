using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class ImplProduct:BaseDB//,IProduct
    {
        public ImplProduct(SqlConnection conn)
        {
            this.conn = conn;
        }

        public System.Data.DataTable SearchTotalProducts()
        {
            DataTable tt = null;
            using (SqlCommand cmd=new SqlCommand("SELECT 產品編號,產品,單價,單位數量,庫存量 FROM 產品資料",conn))
            {
                conn.Open();
                using (SqlDataReader mydr = cmd.ExecuteReader())
                { 
                    if(mydr.HasRows)
                    {
                        tt = new DataTable();
                        tt.Load(mydr);
                        mydr.Close();
                    }
                }
                conn.Close();
            }

            return tt;
        }

        public System.Data.DataTable ViewProductsByPage(int page)
        {
            DataTable tt = null;
            using (SqlCommand cmd = new SqlCommand("產品分頁查詢", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@page", SqlDbType.Int).Value = page;
                conn.Open();
                using (SqlDataReader mydr = cmd.ExecuteReader())
                {
                    if (mydr.HasRows)
                    {
                        tt = new DataTable();
                        tt.Load(mydr);
                        mydr.Close();
                    }
                }
                conn.Close();
            }

            return tt;
        }


        public DataTable SearchProductsByName(string productName)
        {
            DataTable tt = null;
            using (SqlCommand cmd = new SqlCommand("SELECT 產品編號,產品,單價,單位數量,庫存量 FROM 產品資料 WHERE 產品 LIKE ('%'+@pname+'%')", conn))
            {
                cmd.Parameters.Add("@pname", SqlDbType.NVarChar).Value = productName;
                conn.Open();
                using (SqlDataReader mydr = cmd.ExecuteReader())
                {
                    if (mydr.HasRows)
                    {
                        tt = new DataTable();
                        tt.Load(mydr);
                        mydr.Close();
                    }
                }
                conn.Close();
            }

            return tt;
        }


        public byte[] GetProductImageById(int productId)
        {            
            byte[] bb = null;            
                    
            using (SqlCommand cmd = new SqlCommand("SELECT 產品圖片 FROM 產品資料 WHERE 產品編號=@pid", conn))
            {
                cmd.Parameters.Add("@pid", SqlDbType.Int).Value = productId;
                conn.Open();                
                bb = (byte[])cmd.ExecuteScalar();                
                cmd.Dispose();
                conn.Close();
            }
            return bb;
        }

        public int GetTotalProductCount()
        {
            int cc = 0;

            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM 產品資料", conn))
            {            
                conn.Open();
                cc = (int)cmd.ExecuteScalar();
                cmd.Dispose();
                conn.Close();
            }
            return cc;
        }


        public Dictionary<string, object> GetProductInfoById(int productId)
        {
            Dictionary<string, object> result = null;

            using (SqlCommand cmd = new SqlCommand("SELECT 產品編號,產品,單價,單位數量,類別編號,供應商編號,庫存量 FROM 產品資料 WHERE 產品編號=@pid", conn))
            {
                cmd.Parameters.Add("@pid", SqlDbType.Int).Value = productId;
                conn.Open();
                using (SqlDataReader mydr = cmd.ExecuteReader())
                {
                    if (mydr.HasRows)
                    {
                        result = new Dictionary<string, object>();
                        mydr.Read();
                        for (int i = 0; i < mydr.FieldCount; i++)
                            result.Add(mydr.GetName(i), mydr[i]);
                    }
                }
                conn.Close();
            }
            return result;
        }


        public Dictionary<int,string> GetProductType()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();

            using (SqlCommand cmd = new SqlCommand("SELECT 類別編號,類別名稱 FROM 產品類別", conn))
            {                
                conn.Open();
                using (SqlDataReader mydr = cmd.ExecuteReader())
                {
                    while (mydr.Read())
                    {
                        result.Add(mydr.GetInt32(0), mydr.GetString(1));
                    }
                    
                    mydr.Close();
                }
                conn.Close();
            }
            return result;
        }

        public Dictionary<int, string> GetSupply()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();

            using (SqlCommand cmd = new SqlCommand("SELECT 供應商編號,供應商 FROM 供應商", conn))
            {
                conn.Open();
                using (SqlDataReader mydr = cmd.ExecuteReader())
                {
                    while (mydr.Read())
                    {
                        result.Add(mydr.GetInt32(0), mydr.GetString(1));
                    }

                    mydr.Close();
                }
                conn.Close();
            }
            return result;
        }

        public bool AddNewProduct(int id, string name, int typeId, int supplyId, string unit, double price, int stock, byte[] pic)
        {
            bool result = false;
            using (SqlCommand cmd = new SqlCommand("INSERT INTO 產品資料(產品編號,產品,類別編號,供應商編號,單位數量,單價,庫存量,產品圖片) VALUES(@id,@name,@typeId,@supplyId,@unit,@price,@stock,@pic)", conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                cmd.Parameters.Add("@typeId", SqlDbType.Int).Value = typeId;
                cmd.Parameters.Add("@supplyId", SqlDbType.Int).Value = supplyId;
                cmd.Parameters.Add("@unit", SqlDbType.NVarChar).Value = unit;
                cmd.Parameters.Add("@price", SqlDbType.Money).Value = price;
                cmd.Parameters.Add("@stock", SqlDbType.Int).Value = stock;
                cmd.Parameters.Add("@pic", SqlDbType.Image);
                if (pic == null)
                    cmd.Parameters["@pic"].Value = DBNull.Value;
                else
                    cmd.Parameters["@pic"].Value = pic;


                conn.Open();
                int cc = cmd.ExecuteNonQuery();
                cmd.Dispose();
                if (cc > 0) result = true;
            }
            conn.Close();
            return result;
        }

        public bool UpdateProduct(int id, string name, int typeId, int supplyId, string unit, double price, int stock, byte[] pic)
        {
            bool result = false;
            string sqlString = "";
            if (pic == null)
                sqlString = "UPDATE 產品資料 SET 產品=@name,類別編號=@typeId,供應商編號=@supplyId,單位數量=@unit,單價=@price,庫存量=@stock WHERE 產品編號=@id";
            else
                sqlString = "UPDATE 產品資料 SET 產品=@name,類別編號=@typeId,供應商編號=@supplyId,單位數量=@unit,單價=@price,庫存量=@stock,產品圖片=@pic WHERE 產品編號=@id";

            using (SqlCommand cmd = new SqlCommand(sqlString, conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                cmd.Parameters.Add("@typeId", SqlDbType.Int).Value = typeId;
                cmd.Parameters.Add("@supplyId", SqlDbType.Int).Value = supplyId;
                cmd.Parameters.Add("@unit", SqlDbType.NVarChar).Value = unit;
                cmd.Parameters.Add("@price", SqlDbType.Money).Value = price;
                cmd.Parameters.Add("@stock", SqlDbType.Int).Value = stock;
                cmd.Parameters.Add("@pic", SqlDbType.Image);
                if (pic == null)
                    cmd.Parameters["@pic"].Value = DBNull.Value;
                else
                    cmd.Parameters["@pic"].Value = pic;


                conn.Open();
                int cc = cmd.ExecuteNonQuery();
                cmd.Dispose();
                if (cc > 0) result = true;
            }
            conn.Close();
            return result;
        }
    }
}
