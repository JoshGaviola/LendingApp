using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendingApp.Data;

namespace LendingApp.Class.Services
{
    public static class DataGetter
    {
        public static ApplicantsData Data { get; } = new ApplicantsData();
    }
}
