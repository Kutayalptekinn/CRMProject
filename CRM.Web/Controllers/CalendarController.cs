using CRM.Service.Admin;
using CRM.Web.Controllers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Web.Controllers
{
    public class CalendarController : BaseController
    {
        private readonly NewRequestService _newRequestService;
        private readonly CalendarService _calendarService;
        public CalendarController(CalendarService calendarService, NewRequestService newRequestService)
        {
            _newRequestService = newRequestService;
            _calendarService = calendarService;
        }
        // GET: Calendar
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            var user = _newRequestService.GetRolesList(CurrentUser.UserId);
            var users = _newRequestService.GetUsersList(user.Select(x => x.RoleId).ToList(), CurrentUser.UserId);
            var events = _calendarService.GetCalendarListIQueryable(users);
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}