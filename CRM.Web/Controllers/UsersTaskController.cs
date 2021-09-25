using CRM.Service.Admin;
using CRM.ViewModels.Admin;
using CRM.Web.Controllers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Web.Controllers
{
    public class UsersTaskController : BaseController
    {
        private readonly NewRequestService _newRequestService;

        public UsersTaskController(NewRequestService newRequestService)
        {
            _newRequestService = newRequestService;
        }
        // GET: UsersTask
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChooseUser()
        {
           
           
            
            
                var user = _newRequestService.GetRolesList(CurrentUser.UserId);
                var users = _newRequestService.GetUsersList(user.Select(x => x.RoleId).ToList(), CurrentUser.UserId);
                List<string> ListUser = new List<string>();
                foreach (var item in users)
                {
                    ListUser.Add(item.UserName);
                }
                SelectList list = new SelectList(ListUser);
                ViewBag.Users = list;
            
                return PartialView("~/Views/UsersTask/ChooseUser.cshtml");
        }
        [HttpPost]
        public ActionResult ChooseUser(ChooseUserViewModel model)
        {
            var result = _newRequestService.GetNewRequestsListByChoosen(model, CurrentUser.UserName).ToList();
            return PartialView("~/Views/UsersTask/UsersList.cshtml", result);
        }


    }
}