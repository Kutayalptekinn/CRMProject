using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.TypeCodes
{
    public enum ListEnum : byte
    {

        Status = 1,
        Priority = 2,
        RequestorName = 3,
        AssignTo = 4,


    }
    public enum PriorityType : byte
    {

        Low = 3,
        Medium = 2,
        High = 1,



    }

 
}
