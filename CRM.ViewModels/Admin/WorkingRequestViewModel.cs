using CRM.Core.TypeCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.ViewModels.Admin
{

    public class WorkingRequestListViewModel
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
        public string DoneNote { get; set; }
        public string NotDoneNote { get; set; }
        public int DiffDay { get; set; }
        public string Description { get; set; }
    }
    public class WorkingRequestListByDateViewModel
    {
        public int DateDiff { get; set; }
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
        public string DoneNote { get; set; }
        public string NotDoneNote { get; set; }
        public string Description { get; set; }
    }

    public class WorkingRequestDetailListViewModel
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
    }

    public class WorkingRequestCompleteListViewModel
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
    }

    public class EmptyAssignToWorkingRequestListViewModel
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
        public bool Checked { get; set; }
        public List<StepViewModel> Step { get; set; }

    }


    public class WorkingRequestAddViewModel
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
        public Nullable<bool> Checked { get; set; }
        public List<StepViewModel> Step { get; set; }


    }




    public class WorkingRequestSearchViewModel
    {
        public int DateDiff { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public ListEnum? SortList { get; set; }
        public string DoneAndNotDoneList { get; set; }
        public bool LastMonth { get; set; }
        public bool LastWeek { get; set; }
        public bool LastDay { get; set; }
        public bool Expired { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }
    }

    public class WorkingRequestEditViewModel
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
        public List<StepViewModel> Step { get; set; }
    }

    public class WorkingRequestDeleteViewModel
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
        public List<StepViewModel> Step { get; set; }
    }
}
