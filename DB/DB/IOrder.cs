using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public interface IOrder
    {
        int AddNewOrder(int custID, string rcptName, string rcptPhone, string rcptAddr, int DTID, int Shipping, string orderNote);
        bool AddNewOrderDetial(SqlTransaction tx, int OrderID, int ProID, int Quantity);
        bool UpdateOrder(int orderID, int orderState);
        bool AddCartItems(int OrderID, List<Dictionary<string, object>> items);
        DataTable GetOrderInfoByCustPhone(string custPhone);
        DataTable GetOrderInfo();
        DataTable GetOrderDetail(int orderID);

    }
}
