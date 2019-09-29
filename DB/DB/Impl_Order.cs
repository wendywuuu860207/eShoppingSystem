using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class Impl_Order:BaseDB, IOrder
    {
        private SqlConnection cnn;

        public Impl_Order(SqlConnection cnn)
        {
            this.cnn = cnn;
        }

        public int AddNewOrder(int custID, string rcptName, string rcptPhone, string rcptAddr, int DTID, int Shipping, string orderNote)
        {
            int orderID;
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnn;
                    if (orderNote == null)
                        cmd.CommandText = "INSERT INTO orderInfo(custID,rcptName,rcptPhone,rcptAddr,DTID,Shipping,orderState) OUTPUT inserted.orderID VALUES(@custID,@rcptName,@rcptPhone,@rcptAddr,@DTID,@Shipping,1)";
                    else
                    {
                        cmd.CommandText = "INSERT INTO orderInfo(custID,rcptName,rcptPhone,rcptAddr,DTID,Shipping,orderState,orderNote) OUTPUT inserted.orderID VALUES(@custID,@rcptName,@rcptPhone,@rcptAddr,@DTID,@Shipping,1,@orderNote)";
                        cmd.Parameters.Add("@orderNote", SqlDbType.NVarChar).Value = orderNote;
                    }

                    cmd.Parameters.Add("@custID", SqlDbType.Int).Value = custID;
                    cmd.Parameters.Add("@rcptName", SqlDbType.NVarChar).Value = rcptName;
                    cmd.Parameters.Add("@rcptPhone", SqlDbType.VarChar).Value = rcptPhone;
                    cmd.Parameters.Add("@rcptAddr", SqlDbType.NVarChar).Value = rcptAddr;
                    cmd.Parameters.Add("@DTID", SqlDbType.Int).Value = DTID;
                    cmd.Parameters.Add("@Shipping", SqlDbType.Int).Value = Shipping;

                    cnn.Open();
                    orderID = (int)cmd.ExecuteScalar();
                    cmd.Dispose();
                    cnn.Close();
                }
            }
            return orderID;
        }

        public bool UpdateOrder(int orderID, int orderState)
        {
            int cc;
            using (cnn)
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE orderInfo SET orderState=@orderState WHERE orderID=@orderID", cnn))
                {
                    cmd.Parameters.Add("@orderID", SqlDbType.Int).Value = orderID;
                    cmd.Parameters.Add("@orderState", SqlDbType.Int).Value = orderState;

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

        public bool AddCartItems(int OrderID, List<Dictionary<string, object>> items)
        {
            SqlTransaction tx;
            this.cnn.Open();
            tx = cnn.BeginTransaction(IsolationLevel.Serializable);
            foreach (var ii in items)
            {
                if ((int)ii["Quantity"] > 0)
                    if (!this.AddNewOrderDetial(tx, OrderID, (int)ii["proID"], (int)ii["Quantity"]))
                    {
                        tx.Rollback();
                        cnn.Close();
                        return false;
                    }
            }
            tx.Commit();
            cnn.Close();
            return true;
        }

        public bool AddNewOrderDetial(SqlTransaction tx, int OrderID, int ProID, int Quantity)
        {
            int cc;
            using (SqlCommand cmd = new SqlCommand("INSERT INTO orderDetail(orderID,proID,quantity) VALUES(@orderID,@proID,@quantity)", cnn))
            {
                cmd.Parameters.Add("@orderID", SqlDbType.Int).Value = OrderID;
                cmd.Parameters.Add("@proID", SqlDbType.Int).Value = ProID;
                cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = Quantity;
                
                cmd.Transaction = tx;
                cc = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            if (cc > 0)
                return true;
            else
                return false;
        }

        public DataTable GetOrderInfoByCustPhone(string custPhone)
        {
            DataTable tt = null;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cnn;
                cmd.CommandText = "SELECT orderID,rcptName,rcptPhone,rcptAddr,C.DTName,Shipping,"
                    + "CASE orderState WHEN 0 THEN '訂單已取消' WHEN 1 THEN '訂單成立' WHEN 2 THEN '送貨中' WHEN 3 THEN '已取貨' END AS orderState,"
                    + "orderNote,orderTime FROM orderInfo AS A INNER JOIN customer AS B ON A.custID=B.custID "
                    + "INNER JOIN deliveryType AS C ON A.DTID=C.DTID WHERE B.custPhone=@custPhone";
                cmd.Parameters.Add("@custPhone", SqlDbType.VarChar).Value = custPhone;
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

        public DataTable GetOrderInfo()
        {
            DataTable tt = null;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cnn;
                cmd.CommandText = "SELECT orderID,A.custID,rcptName,rcptPhone,rcptAddr,C.DTName,Shipping,"
                    + "CASE orderState WHEN 0 THEN '訂單已取消' WHEN 1 THEN '訂單成立' WHEN 2 THEN '送貨中' WHEN 3 THEN '已取貨' END AS orderState,"
                    + "orderNote,orderTime FROM orderInfo AS A INNER JOIN customer AS B ON A.custID=B.custID "
                    + "INNER JOIN deliveryType AS C ON A.DTID=C.DTID";
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

        public DataTable GetOrderDetail(int orderID)
        {
            DataTable tt = null;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cnn;
                cmd.CommandText = "SELECT proName,proPrice,quantity FROM orderDetail AS A INNER JOIN product AS B ON A.proID=B.proID WHERE orderID=@orderID";
                cmd.Parameters.Add("@orderID", SqlDbType.Int).Value = orderID;
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
    }
}
