using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.ViewModels.Admin
{
    public class CalendarListViewModel
    {
        public int Id { get; set; }
        public string Request { get; set; }
        public string Worker { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> StartingDate { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }


    }
}
