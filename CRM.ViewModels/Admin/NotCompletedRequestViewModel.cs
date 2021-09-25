using CRM.Core.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.ViewModels.Admin
{
    public class NotCompletedRequestListViewModel
    {
        public int Id { get; set; }
        public string Request { get; set; }
        public string Status { get; set; }
        public PriorityType? Priority { get; set; }
        public string RequestorName { get; set; }
        public string AssignTo { get; set; }
        public Nullable<System.DateTime> StartingDate { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }
        public string WorkSteps { get; set; }
        public string RoleName { get; set; }
        public Nullable<bool> Checked { get; set; }
        public List<StepViewModel> Step { get; set; }
        public string NotDoneNote { get; set; }

    }

    public class NotCompletedRequestDeleteViewModel
    {
        public int Id { get; set; }
        public string Request { get; set; }
        public string Status { get; set; }
        public PriorityType? Priority { get; set; }
        public string RequestorName { get; set; }
        public string AssignTo { get; set; }
        public Nullable<System.DateTime> StartingDate { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }
        public string WorkSteps { get; set; }
        public string RoleName { get; set; }
        public Nullable<bool> Checked { get; set; }
        public List<StepViewModel> Step { get; set; }
        public string NotDoneNote { get; set; }

    }
}
