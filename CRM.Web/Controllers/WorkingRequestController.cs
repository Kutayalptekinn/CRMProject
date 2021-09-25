using CRM.Core.TypeCodes;
using CRM.Data;
using CRM.Service.Admin;
using CRM.ViewModels.Admin;
using CRM.ViewModels.Common;
using CRM.Web.Controllers.Abstract;
using CRM.Web.Filters;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CRM.Web.Controllers
{
    public class WorkingRequestController : BaseController
    {
        private readonly CompletedRequestService _completedRequestService;
        private readonly NotCompletedRequestService _notcompletedRequestService;
        private readonly NewRequestService _newRequestService;
        private readonly WorkingRequestService _workingRequestService;


        public WorkingRequestController(WorkingRequestService workingRequestService, NewRequestService newRequestService, CompletedRequestService completedRequestService, NotCompletedRequestService notcompletedRequestService)
        {
            _completedRequestService = completedRequestService;
            _newRequestService = newRequestService;
            _workingRequestService = workingRequestService;
            _notcompletedRequestService = notcompletedRequestService;
        }
        // GET: Working
        public ActionResult Index(WorkingRequestSearchViewModel model)
        {
            return View(model);
        }

        public ActionResult Description(int id)
        {
            var model = _workingRequestService.GetDescription(id);
            return View(model);
        }
       


        public ActionResult WorkingRequestList(WorkingRequestSearchViewModel model, int? page)
        {
            ViewBag.SearchModel = model;
            var currentPageIndex = page ?? 1;
            if(model.DoneAndNotDoneList=="Completed List")
            {
                return RedirectToAction("CompletedRequestList", "CompletedRequest");
            }
            if (model.DoneAndNotDoneList == "Not Completed List")
            {
                return RedirectToAction("NotCompletedRequestList", "NotCompletedRequest");
            }
            var resultQuery = _workingRequestService.GetWorkingRequestListIQueryable(model, CurrentUser).OrderBy(a => a.Id);
            if (model.SortList == ListEnum.Status)
            {
                resultQuery = resultQuery.OrderBy(p => p.Status);
            }
            if (model.SortList == ListEnum.RequestorName)
            {
                resultQuery = resultQuery.OrderBy(p => p.RequestorName);
            }
            if (model.SortList == ListEnum.AssignTo)
            {
                resultQuery = resultQuery.OrderBy(p => p.AssignTo);
            }
            if (model.SortList == ListEnum.Priority)
            {
                resultQuery = resultQuery.OrderBy(p => p.Priority);
            }
            var result = resultQuery.ToPagedList(currentPageIndex - 1, 5);
            return new ContentResult
            {
                ContentType = "application/json",
                Content = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(new
                {
                    success = true,
                    responseText = RenderPartialViewToString("~/Views/WorkingRequest/WorkingRequestList.cshtml", result)
                })
            };

        }
        [CustomAuthorizeFilter(UserAuthCodes = "A,B")]
        [CustomAuthorizeFilter(UserAuthCodes = "B,C")]
        [CustomAuthorizeFilter(UserAuthCodes = "A,C")]
        public ActionResult WorkingRequestDetailList()
        {
            var user = _newRequestService.GetRolesList(CurrentUser.UserId);
            var users = _newRequestService.GetUsersList(user.Select(x => x.RoleId).ToList(), CurrentUser.UserId);

            var result = _workingRequestService.GetWorkingRequestDetailListIQueryable(users).ToList();

            return PartialView("~/Views/WorkingRequest/WorkingRequestDetailList.cshtml", result);

        }

       
        
        [HttpPost]
        public async Task<ActionResult> check(int id)
        {
            var callResult = await _workingRequestService.GetCheckAsync(id,CurrentUser);
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

            return Json(
             new
             {
                 success = false,
                 errorMessages = callResult.ErrorMessages
             });
        }

        public async Task<ActionResult> WorkingRequestDelete(int newRequestId)
        {
            var model = await _workingRequestService.GetWorkingRequestDeleteViewModelAsync(newRequestId);
            if (model != null)
            {
                ViewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = "NewRequestDelete" };
                return PartialView("~/Views/WorkingRequest/WorkingRequestDelete.cshtml", model);
            }

            return PartialView("~/Views/Shared/_ItemNotFoundPartial.cshtml", "İstek sistemde bulunamadı!");
        }
        [HttpPost]
        public async Task<ActionResult> WorkingRequestDelete([Bind(Prefix = "NewRequestDelete")] WorkingRequestDeleteViewModel model)
        {
            
            var callResult = await _workingRequestService.DeleteWorkingRequestAsync(model.Id,CurrentUser.UserName);
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

            return Json(
             new
             {
                 success = false,
                 errorMessages = callResult.ErrorMessages
             });

        }
        public async Task<ActionResult> DeleteAndAddDone(int id)
        {
            var model = await _workingRequestService.FindUserById(id);
            return PartialView("~/Views/CompletedRequest/DoneNote.cshtml",model);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteAndAddDone(WorkingRequestListViewModel model)
        {
            int done = 1;
            var callResult = await _completedRequestService.AddCompletedRequestAsync(model, CurrentUser, done);
            if (callResult.ErrorMessages.Count != 0)
            {
                ModelState.Clear();
                return Json(
                new
                {
                    success = false,
                    errorMessages = callResult.ErrorMessages,

                }, JsonRequestBehavior.AllowGet) ;
            }
            var callResult1 = await _workingRequestService.DeleteWorkingRequestAsync(model.Id,CurrentUser.UserName);
            if (callResult1.Success)
            {
                ModelState.Clear();
                return Json(
                new
                {
                    success = true,
                    warningMessages = callResult1.WarningMessages,
                    successMessages = callResult1.SuccessMessages,

                }, JsonRequestBehavior.AllowGet);

            }

            return Json(
             new
             {
                 success = false,
                 errorMessages = callResult1.ErrorMessages,

             }, JsonRequestBehavior.AllowGet);

        }
        public async Task<ActionResult> DeleteAndAddNotDone(int id)
        {
            var model = await _workingRequestService.FindUserById(id);
            return PartialView("~/Views/NotCompletedRequest/NotDoneNote.cshtml", model);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteAndAddNotDone(WorkingRequestListViewModel model)
        {
            int notdone = 1;
            var callResult = await _notcompletedRequestService.AddNotCompletedRequestAsync(model, CurrentUser,notdone);
            if (callResult.ErrorMessages.Count != 0)
            {
                ModelState.Clear();
                return Json(
                new
                {
                    success = false,
                    errorMessages = callResult.ErrorMessages,

                }, JsonRequestBehavior.AllowGet);
            }
            var callResult1 = await _workingRequestService.DeleteWorkingRequestAsync(model.Id, CurrentUser.UserName);
            if (callResult1.Success)
            {
                ModelState.Clear();
                return Json(
                new
                {
                    success = true,
                    warningMessages = callResult1.WarningMessages,
                    successMessages = callResult1.SuccessMessages,

                }, JsonRequestBehavior.AllowGet);

            }

            return Json(
             new
             {
                 success = false,
                 errorMessages = callResult1.ErrorMessages,

             }, JsonRequestBehavior.AllowGet);

        }

    }
}