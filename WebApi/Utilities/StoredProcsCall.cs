using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebApi.Models;

namespace WebApi.Utilities
{
    public class StoredProcsCall
    {
        public static bool AddManagers(string employeeid)
        {
            PsiogEntities PE = new PsiogEntities();
            
            var result= PE.ManagersSetUpProcess(employeeid);
            return Convert.ToBoolean(result);

        }

        public static bool AllocateLeave(string employeeid, int month)
        {
            PsiogEntities PE = new PsiogEntities();

            var result = PE.SetUpEmployeeLeaveAvailability(month,employeeid);
            return Convert.ToBoolean(result);
        }

    }
}