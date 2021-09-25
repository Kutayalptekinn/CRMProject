using CRM.Core.TypeCodes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CRM.ViewModels.Admin
{
    public class NewRequestListViewModel
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
        public ListEnum SortList { get; set; }
        [AllowHtml]
        public string Description  { get; set; }
    }
    public class NewRequestSearchViewModel
    {
        public PriorityType? PriorityList { get; set; }
        public ListEnum? SortList { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }


    public class EmptyAssignToNewRequestListViewModel
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
        [AllowHtml]
        public string Description { get; set; }
    }
    public class NewRequestDeleteViewModel
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
        public string Description { get; set; }
    }

    public class NewRequestAddViewModel
    {
        public int? Id { get; set; }
        public string Request { get; set; }
        public string Status { get; set; }
        public PriorityType Priority { get; set; }
        public string RequestorName { get; set; }
        public string AssignTo { get; set; }
        public Nullable<System.DateTime> StartingDate { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }
        public string WorkSteps { get; set; }
        public Nullable<bool> Checked { get; set; }
        public List<StepViewModel> Step { get; set; }
        [AllowHtml]
        public string Description { get; set; }


    }

    public class ChooseUserViewModel
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
        public List<WorkingRequestListViewModel> WorkingList { get; set; }
        [AllowHtml]
        public string Description { get; set; }

    }

    public class StepViewModel
    {
        public int Id { get; set; }
        public int NewRequestsId { get; set; }
        public int WorkingRequestId { get; set; }
        public string Name { get; set; }
        public string StepDetail { get; set; }
        public Nullable<bool> Checkedd { get; set; }
        public NewRequestAddViewModel NewRequests { get; set; }
    }
    public class StepAddViewModel
    {
        public int Id { get; set; }
        public int NewRequestsId { get; set; }
        public string Name { get; set; }
        public string StepDetail { get; set; }
        public Nullable<bool> Checkedd { get; set; }
        public NewRequestAddViewModel NewRequests { get; set; }
        [AllowHtml]
        public string Description { get; set; }
    }
    
    

    public class NewRequestEditViewModel
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
        [AllowHtml]
        public string Description { get; set; }
    }

        public class UserDeleteViewModel
        {

        public int Id { get; set; }
        public string Request { get; set; }
        public string Status { get; set; }
        public PriorityType Priority { get; set; }
        public string RequestorName { get; set; }
        public string AssignTo { get; set; }
        public Nullable<System.DateTime> StartingDate { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }
        public string WorkSteps { get; set; }
        public List<StepViewModel> Step { get; set; }
        [AllowHtml]
        public string Description { get; set; }
    }
    




}
