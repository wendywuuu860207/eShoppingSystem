using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public interface IProduct
    {
        DataTable GetProducts();
        DataTable GetProductList();

        DataTable GetProductListByType(int typeid);

        Dictionary<string, object> GetSingleProduct(int productId);

        DataTable GetProductType();

        DataTable SearchProductsByName(string proName);

        bool AddNewProduct(string proName, int PTID, int proPrice, string proNote, int proStock, int proState, string proPic);

        bool UpdateProduct(int proID, string proName, int PTID, int proPrice, string proNote, int proStock, int proState, string proPic);
    }
}
