using CRM.Service.Admin;
using CRM.ViewModels.Admin;
using CRM.Web.Controllers.Abstract;
using CRM.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CRM.Web.Controllers
{
    public class NotCompletedRequestController : BaseController
    {
        private readonly NotCompletedRequestService _notcompletedRequestService;
        private readonly NewRequestService _newRequestService;

        public NotCompletedRequestController(NotCompletedRequestService notcompletedRequestService, NewRequestService newRequestService)
        {
            _notcompletedRequestService = notcompletedRequestService;
            _newRequestService = newRequestService;
        }
        public ActionResult Index()
        {
            return View();
        }
        [CustomAuthorizeFilter(UserAuthCodes = "A,B")]
        [CustomAuthorizeFilter(UserAuthCodes = "B,C")]
        [CustomAuthorizeFilter(UserAuthCodes = "A,C")]
        public ActionResult NotCompletedRequestList()
        {
            var user = _newRequestService.GetRolesList(CurrentUser.UserId);
            var users = _newRequestService.GetUsersList(user.Select(x => x.RoleId).ToList(), CurrentUser.UserId);
            var result = _notcompletedRequestService.GetNotCompletedRequestListIQueryable(users).ToList();
            return new ContentResult
            {
                ContentType = "application/json",
                Content = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(new
                {
                    success = true,
                    responseText = RenderPartialViewToString("~/Views/NotCompletedRequest/NotCompletedRequestList.cshtml", result)
                })
            };

        }

        public async Task<ActionResult> NotCompletedRequestDelete(int notcompletedRequestId)
        {

            var model = await _notcompletedRequestService.GetNotCompletedRequestDeleteViewModelAsync(notcompletedRequestId);

            if (model != null)
            {
                ViewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = "NotCompletedRequestDelete" };
                return PartialView("~/Views/NotCompletedRequest/NotCompletedRequestDelete.cshtml", model);
            }

            return PartialView("~/Views/Shared/_ItemNotFoundPartial.cshtml", "İstek sistemde bulunamadı!");
        }
        [HttpPost]
        public async Task<ActionResult> NotCompletedRequestDelete([Bind(Prefix = "NotCompletedRequestDelete")] NotCompletedRequestDeleteViewModel model)
        {

            var callResult = await _notcompletedRequestService.DeleteNotCompletedRequestAsync(model.Id, CurrentUser.UserName);
            if (callResult.Success)
            {

                ModelState.Clear();
                return Json(
                new
                {
                    success = true,
                    warningMessages = callResult.WarningMessages,
                    successMessages = callResult.SuccessMessages,
                });
            }
            else
            {
                ModelState.Clear();
                return Json(
                 new
                 {
                     success = false,
                     errorMessages = callResult.ErrorMessages
                 });
            }


        }
    }
}