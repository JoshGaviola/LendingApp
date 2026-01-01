using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.LogicClass.Cashier
{
    public  class CashierProcessLogic
    {
        public string GetTimeNow()
        {
            string time;
            return time = DateTime.Now.ToString("h:mm tt", CultureInfo.GetCultureInfo("en-US"));
        }

    }
}
