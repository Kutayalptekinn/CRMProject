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
    public class CompletedRequestController : BaseController
    {
        private readonly CompletedRequestService _completedRequestService;
        private readonly NewRequestService _newRequestService;

        public CompletedRequestController(CompletedRequestService completedRequestService, NewRequestService newRequestService)
        {
            _completedRequestService = completedRequestService;
            _newRequestService = newRequestService;
        }
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorizeFilter(UserAuthCodes = "A,B")]
        [CustomAuthorizeFilter(UserAuthCodes = "B,C")]
        [CustomAuthorizeFilter(UserAuthCodes = "A,C")]
        public ActionResult CompletedRequestList()
        {
            var user = _newRequestService.GetRolesList(CurrentUser.UserId);
            var users = _newRequestService.GetUsersList(user.Select(x => x.RoleId).ToList(), CurrentUser.UserId);
            var result = _completedRequestService.GetCompletedRequestListIQueryable(users).ToList();
            
            return new ContentResult
            {
                ContentType = "application/json",
                Content = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(new
                {
                    success = true,
                    responseText = RenderPartialViewToString("~/Views/CompletedRequest/CompletedRequestList.cshtml", result)
                })
            };

        }

        public async Task<ActionResult> CompletedRequestDelete(int completedRequestId)
        {

            var model = await _completedRequestService.GetCompletedRequestDeleteViewModelAsync(completedRequestId);

            if (model != null)
            {
                ViewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = "CompletedRequestDelete" };
                return PartialView("~/Views/CompletedRequest/CompletedRequestDelete.cshtml", model);
            }

            return PartialView("~/Views/Shared/_ItemNotFoundPartial.cshtml", "İstek sistemde bulunamadı!");
        }
        [HttpPost]
        public async Task<ActionResult> CompletedRequestDelete([Bind(Prefix = "CompletedRequestDelete")] CompletedRequestDeleteViewModel model)
        {

            var callResult = await _completedRequestService.DeleteCompletedRequestAsync(model.Id, CurrentUser.UserName);
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