using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CRM.Data;
using CRM.Service.Admin;
using CRM.ViewModels.Admin;
using CRM.Web.Controllers.Abstract;

namespace CRM.Web.Controllers
{
    public class UsersController : BaseController
    {
        private readonly UsersService _usersService;
        private readonly NotCompletedRequestService _notCompletedRequestService;
        private readonly CompletedRequestService _completedRequestService;
        private readonly NewRequestService _newRequestService;
        public UsersController(UsersService usersService,CompletedRequestService completedRequestService, NotCompletedRequestService notCompletedRequestService, NewRequestService newRequestService)
        {
            _completedRequestService = completedRequestService;
            _usersService = usersService;
            _notCompletedRequestService = notCompletedRequestService;
            _newRequestService = newRequestService;
        }

        public ActionResult Index()
        {

            return View();

        }
        // GET: Users
        public ActionResult UsersList()
        {
            var user = _newRequestService.GetRolesList(CurrentUser.UserId);
            var result = _usersService.GetUsersList(user.Select(x => x.RoleId).ToList(), CurrentUser.UserId);
            return View("~/Views/Users/UsersList.cshtml", result);

        }

        // GET: Users/Create
        public ActionResult Create()
        {
            var model = _usersService.GetRolesList();

            return View(model);
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Create(UserAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                var callResult = await _usersService.AddNewUserAsync(model);
                if (callResult.Success)
                {
                    ModelState.Clear();
                    var viewModel = (LoginViewModel)callResult.Item;
                    var jsonResult = Json(
                        new
                        {
                            success = true,
                            responseText = RenderPartialViewToString("~/Views/Users/DisplayTemplates/LoginViewModel.cshtml", viewModel),
                            item = viewModel
                        });
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }

            }
            return View();
        }

        public async Task<ActionResult> UserEdit(int userId)
        {

            var model = await _usersService.GetUserEditViewModelAsync(userId);
           if(model.UserName==CurrentUser.UserName)
            {
                return PartialView("~/Views/Shared/_ItemNotFoundPartial.cshtml", "Kullanıcı Kendisini Düzenleyemez");
            }
            foreach (var item in model.RoleNames)
            {
                foreach (var item1 in model.Roles)
                {
                    if (item.RoleName == item1.RoleName)
                    {
                        item1.Checked = true;
                    }
                }

            }
            if (model != null)
            {
                ViewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = "UserEdit" };
                return PartialView("~/Views/Users/Edit.cshtml", model);
            }

            return PartialView("~/Views/Shared/_ItemNotFoundPartial.cshtml", "İstek sistemde bulunamadı!");
        }
        [HttpPost]
        public async Task<ActionResult> UserEdit([Bind(Prefix = "UserEdit")] UserEditViewModel model)
        {

            if (ModelState.IsValid)
            {
                var callResult = await _usersService.EditUserAsync(model, CurrentUser);
                if (callResult.Success)
                {

                    ModelState.Clear();
                    var viewModel = (LoginViewModel)callResult.Item;
                    var jsonResult = Json(
                        new
                        {
                            success = true,
                            responseText = RenderPartialViewToString("~/Views/Users/DisplayTemplates/LoginViewModel.cshtml", viewModel),
                            item = viewModel
                        });
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                return Json(
             new
             {
                 success = false,
                 errorMessages = callResult.ErrorMessages
             });
            }
            return View();

        }
        //// POST: Users/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,UserName,Password")] Users users)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(users).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(users);
        //}

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(int userId)
        {

            var model = await _usersService.GetUserDeleteViewModelAsync(userId);
            if (model.UserName == CurrentUser.UserName)
            {
                return PartialView("~/Views/Shared/_ItemNotFoundPartial.cshtml", "Kullanıcı Kendisini Silemez");
            }
            if (model != null)
            {
                ViewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = "UsersDelete" };
                return PartialView("~/Views/Users/UsersDelete.cshtml", model);
            }

            return PartialView("~/Views/Shared/_ItemNotFoundPartial.cshtml", "İstek sistemde bulunamadı!");
        }

        // POST: Users/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete([Bind(Prefix = "UsersDelete")] UsersDeleteViewModel model)
        {
            var callResultForOnGoingRequests=await _usersService.CheckOnGoingRequests(model.Id);

           

            if (callResultForOnGoingRequests.Success)
            {
                var callResultForDeleteCompletedRequests = await _completedRequestService.DeleteCompletedRequestForDeleteUserAsync(model.Id, CurrentUser.UserName);
                var callResultForDeleteNotCompletedRequests = await _notCompletedRequestService.DeleteNotCompletedRequestForDeleteUserAsync(model.Id, CurrentUser.UserName);
                var callResult = await _usersService.DeleteUserAsync(model.Id);
                ModelState.Clear();

                return Json(
                new
                {
                    success = true,
                    warningMessages = callResultForOnGoingRequests.WarningMessages,
                    successMessages = callResultForOnGoingRequests.SuccessMessages,
                });
            }

            ModelState.Clear();
            return Json(
             new
             {
                 success = false,
                 errorMessages = callResultForOnGoingRequests.ErrorMessages
             });
        }

        public async Task<ActionResult> PasswordEdit(int userId)
        {
            var model = await _usersService.GetUserPasswordEditViewModelAsync(userId);

            return PartialView("~/Views/Users/PasswordEdit.cshtml", model);
        }
        [HttpPost]
        public async Task<ActionResult> PasswordEdit(PasswordEditViewModel model)
        {

            if (ModelState.IsValid)
            {
                var callResult = await _usersService.PasswordEditUserAsync(model, CurrentUser);
                if (callResult.Success)
                {

                    ModelState.Clear();
                    return Json(
                 new
                 {
                     success = true,
                     successMessages = callResult.SuccessMessages
                 });
                }
                ModelState.Clear();
                return Json(
                 new
                 {
                     success = false,
                     errorMessages = callResult.ErrorMessages
                 });

            }
            return View();
        }
        //public ActionResult CreateRole()
        //{
        //    List<string> Alphabet = new List<string>();
        //    for (int i = 65; i <= 90; i++)
        //    {
        //        Alphabet.Add(Convert.ToChar(i).ToString());
        //    }

        //    SelectList list = new SelectList(Alphabet);
        //    ViewBag.Alphabet = list;
        //    return View();
        //}

        //[HttpPost]
        //public async Task<ActionResult> CreateRole(RoleAddViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var callResult = await _usersService.AddNewRoleAsync(model);
        //        if (callResult.Success)
        //        {
        //            return Json(
        //        new
        //        {
        //            success = true,
        //            successMessages = callResult.SuccessMessages,

        //        });
        //        }

        //        ModelState.Clear();
        //        return Json(
        //         new
        //         {
        //             success = false,
        //             errorMessages = callResult.ErrorMessages
        //         });

        //    }
        //    return View();
        //}
    }
}

