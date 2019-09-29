using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public interface ICustomer
    {
        bool CheckPhoneDuplicate(string Phone);

        bool AddNewMember(string Phone, string PW, string Name,string EMail);

        bool CheckLogin(string Phone, string PW);

        bool ChangePassword(string Phone, string PW);

        Dictionary<string, object> CustomerInfo(string Phone);

        bool UpdateCustermorInfo(string Phone, string PW, string Name, string Email);
        int GetCustIDByPhone(string Phone);
        DataTable SearchCustomerByName(string name);
    }
}

