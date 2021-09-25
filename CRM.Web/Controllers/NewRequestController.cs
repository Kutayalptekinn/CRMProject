using CRM.Data;
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
using MvcPaging;
using CRM.Utils.Constants;
using CRM.Core.TypeCodes;
using System.Drawing;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using ImageResizer;


namespace CRM.Web.Controllers
{
    public class NewRequestController : BaseController
    {
        private readonly NewRequestService _newRequestService;
        private readonly WorkingRequestService _workingRequestService;


        public NewRequestController(NewRequestService newRequestService, WorkingRequestService workingRequestService)
        {
            _newRequestService = newRequestService;
            _workingRequestService = workingRequestService;
        }
        // GET: NewRequest
        public ActionResult Index()
        {

            return View();
        }
        [CustomAuthorizeFilter(UserAuthCodes = "A,B")]
        [CustomAuthorizeFilter(UserAuthCodes = "B,C")]
        [CustomAuthorizeFilter(UserAuthCodes = "A,C")]
        public ActionResult NewRequestAdd()
        {

            var M = _newRequestService.GetEmptyStep();
            var user = _newRequestService.GetRolesList(CurrentUser.UserId);


            var users = _newRequestService.GetUsersList(user.Select(x => x.RoleId).ToList(), CurrentUser.UserId);
            List<string> ListUser = new List<string>();
            foreach (var item in users)
            {
                ListUser.Add(item.UserName);
            }
            SelectList list = new SelectList(ListUser);
            ViewBag.Users = list;

            return PartialView("~/Views/NewRequest/NewRequestAdd.cshtml", M);
        }
        [HttpPost]
        public async Task<ActionResult> NewRequestAdd(NewRequestAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                var callResult = await _newRequestService.AddNewRequestAsync(model, CurrentUser);
                if (callResult.Success)
                {

                    
                        ModelState.Clear();
                        var viewModel = (NewRequestListViewModel)callResult.Item;
                        var jsonResult = Json(
                            new
                            {
                                //response text mantığı
                                success = true,
                                responseText = RenderPartialViewToString("~/Views/NewRequest/DisplayTemplates/NewRequestListViewModel.cshtml", viewModel),
                                item = viewModel
                            });
                        jsonResult.MaxJsonLength = int.MaxValue;
                        return jsonResult;
                    

                }
            }
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> EmptyAssignToNewRequestAdd(List<EmptyAssignToNewRequestListViewModel> model)
        {
            var responseText = string.Empty;
            foreach (var item in model)
            {
                if (item.Checked)
                {
                    var model1 = await _newRequestService.GetEmptyAssignToNewRequestListViewModelAsync(item.Id);
                    model1.AssignTo = CurrentUser.UserName;
                    var callResult = await _newRequestService.EditEmptyNewRequestAsync(model1, CurrentUser.UserName);
                    if (callResult.Success)
                    {
                        ModelState.Clear();
                        var viewModel = (NewRequestListViewModel)callResult.Item;
                        responseText += RenderPartialViewToString("~/Views/NewRequest/DisplayTemplates/NewRequestListViewModel.cshtml", viewModel);

                    }
                }
            }
            var json = Json(
                       new
                       {
                           success = true,
                           responseText = responseText
                       });
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public ActionResult AddStep()
        {
            var model = new CRM.ViewModels.Admin.StepViewModel();
            return PartialView("AddStepPartial", model);
        }
        public ActionResult EditStep()
        {
            var model = new CRM.ViewModels.Admin.StepViewModel();
            return PartialView("EditStepPartial", model);
        }
        public ActionResult ChooseTask(EmptyAssignToNewRequestListViewModel model)
        {
            if (CurrentUser.AuthCodes.Contains("A") && CurrentUser.AuthCodes.Contains("B") && CurrentUser.AuthCodes.Contains("C"))
            {

                var result1 = _newRequestService.GetNewRequestsList();
                return PartialView("~/Views/NewRequest/ChooseTask.cshtml", result1);
            }
            var user = _newRequestService.GetRolesList(CurrentUser.UserId);
            var users = _newRequestService.GetUsersListByEmptyAssignTo(user.Select(x => x.RoleId).ToList(), CurrentUser.UserId);
            var result = _newRequestService.GetEmptyAssignToNewRequestsList(model, users).ToList();
            return PartialView("~/Views/NewRequest/ChooseTask.cshtml", result);
        }
        public ActionResult NewRequestList(NewRequestSearchViewModel model, int? page)
        {
            //++
            ViewBag.SearchModel = model;
            //+
            var currentPageIndex = page ?? 1;
            //if (CurrentUser.AuthCodes.Contains("A") && CurrentUser.AuthCodes.Contains("B") && CurrentUser.AuthCodes.Contains("C"))
            //{
            //    var result = _newRequestService.GetNewRequestsList().ToPagedList(currentPageIndex - 1, 5);
            //    return new ContentResult
            //    {
            //        ContentType = "application/json",
            //        Content = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(new
            //        {
            //            success = true,
            //            responseText = RenderPartialViewToString("~/Views/NewRequest/NewRequestList.cshtml", result)
            //        })
            //    };
            //}
            //else
            //{
            var resultQuery = _newRequestService.GetNewRequestListIQueryable(model, CurrentUser).OrderBy(a => a.Id);
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
            var result = resultQuery.ToPagedList(currentPageIndex - 1, 4);
            return new ContentResult
            {
                ContentType = "application/json",
                Content = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(new
                {
                    success = true,
                    responseText = RenderPartialViewToString("~/Views/NewRequest/NewRequestList.cshtml", result)
                })
            };
            //}



        }


        public ActionResult Description(int id)
        {
            var model = _newRequestService.GetDescription(id);
            return View(model);
        }
        public async Task<ActionResult> NewRequestEdit(int newRequestId)
        {
            var model = await _newRequestService.GetNewRequestEditViewModelAsync(newRequestId);

            var user = _newRequestService.GetRolesList(CurrentUser.UserId);


            var users = _newRequestService.GetUsersList(user.Select(x => x.RoleId).ToList(), CurrentUser.UserId);
            List<string> ListUser = new List<string>();
            foreach (var item in users)
            {
                ListUser.Add(item.UserName);
            }
            SelectList list = new SelectList(ListUser);
            ViewBag.Users = list;

            if (model.AssignTo == CurrentUser.UserName)
            {
                return PartialView("~/Views/Shared/_ItemNotFoundPartial.cshtml", "İşlem için yetkiniz yoktur.!");
            }



            if (model != null)
            {
                ViewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = "NewRequestEdit" };
                return PartialView("~/Views/NewRequest/NewRequestEdit.cshtml", model);
            }

            return PartialView("~/Views/Shared/_ItemNotFoundPartial.cshtml", "İstek sistemde bulunamadı!");
        }
        [HttpPost]
        public async Task<ActionResult> NewRequestEdit([Bind(Prefix = "NewRequestEdit")] NewRequestEditViewModel model)
        {
            var f = 0;
            if (ModelState.IsValid)
            {
                model.Description = ParseDescriptionBase64Images(model.Description);
                //var webClient = new WebClient();
                //byte[] imageBytes = webClient.DownloadData(model.Description);

                //byte[] bytes = Convert.FromBase64String(model.Description);

                //Image image;
                //using (MemoryStream ms = new MemoryStream(bytes))
                //{
                //    image = Image.FromStream(ms);
                //}

                //string filePath = "dsa.jpg";
                //image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);


                var callResult = await _newRequestService.EditNewRequestAsync(model, CurrentUser, f);
                if (callResult.Success)
                {

                    ModelState.Clear();
                    var viewModel = (NewRequestListViewModel)callResult.Item;
                    var jsonResult = Json(
                        new
                        {
                            success = true,
                            responseText = RenderPartialViewToString("~/Views/NewRequest/DisplayTemplates/NewRequestListViewModel.cshtml", viewModel),
                            item = viewModel
                        });
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                foreach (var error in callResult.ErrorMessages)
                {
                    ModelState.AddModelError("", error);
                }
            }
            ViewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = "NewRequestEdit" };
            return Json(
                new
                {
                    success = false,
                    responseText = RenderPartialViewToString("~/Views/NewRequest/NewRequestEdit.cshtml", model)
                });

        }
        //+
        public static string ParseDescriptionBase64Images(string description)
        {
            if (String.IsNullOrWhiteSpace(description)) return description;
            var imagesDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + "/uploads");
            var imageUrl = String.Format("https://localhost:44390/uploads/");

            if (!System.IO.Directory.Exists(imagesDirectory))
                System.IO.Directory.CreateDirectory(imagesDirectory);


            HtmlDocument document = new HtmlDocument();
            //descriptionun içindeki imgyi buluyo
            document.LoadHtml(description);
            document.DocumentNode.Descendants("img")
                .Where(e =>
                {
                    string src = e.GetAttributeValue("src", null) ?? "";
                    return !string.IsNullOrEmpty(src) && src.StartsWith("data:image");
                })
                .ToList()
                .ForEach(x =>
                {
                    string currentSrcValue = x.GetAttributeValue("src", null);
                    currentSrcValue = currentSrcValue.Split(',')[1];//Base64 part of string
                    if ((currentSrcValue.Length % 4) == 0) // good for Base64
                    {
                        byte[] imageData = Convert.FromBase64String(currentSrcValue);
                        //
                        string newFileName = $"{Guid.NewGuid():N}.jpg";
                        //slashları kendi kombine
                        string pathImage = System.IO.Path.Combine(imagesDirectory, newFileName);
                        //kaydetme işlemi
                        ImageBuilder.Current.Build(imageData, pathImage, new ResizeSettings("maxwidth=120;format=jpg;quality=90;autorotate=true"));
                        x.SetAttributeValue("src", imageUrl + newFileName);
                    }

                });

            return document.DocumentNode.OuterHtml;
        }
        public async Task<ActionResult> NewRequestDelete(int newRequestId)
        {

            var model = await _newRequestService.GetNewRequestDeleteViewModelAsync(newRequestId);

            if (model != null)
            {
                //+
                ViewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = "NewRequestDelete" };
                return PartialView("~/Views/NewRequest/NewRequestDelete.cshtml", model);
            }

            return PartialView("~/Views/Shared/_ItemNotFoundPartial.cshtml", "İstek sistemde bulunamadı!");
        }
        [HttpPost]
        //+
        public async Task<ActionResult> NewRequestDelete([Bind(Prefix = "NewRequestDelete")] UserDeleteViewModel model)
        {
            int n = 0;
            var callResult = await _newRequestService.DeleteNewRequestAsync(model.Id, CurrentUser.UserName, n);
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

            ModelState.Clear();
            return Json(
             new
             {
                 success = false,
                 errorMessages = callResult.ErrorMessages
             });



        }
        [HttpPost]
        public async Task<ActionResult> StartWorking(int id)
        {
            int start = 1;
            var callResult = await _workingRequestService.AddWorkingRequestAsync(id, CurrentUser, start);
            if (callResult.ErrorMessages.Count != 0)
            {
                ModelState.Clear();
                return Json(
                new
                {
                    ssuccess = false,
                    errorMessages = callResult.ErrorMessages,

                }, JsonRequestBehavior.AllowGet);
            }
            var callResult1 = await _newRequestService.DeleteNewRequestAsync(id, CurrentUser.UserName, start);
            if (callResult1.Success)
            {
                ModelState.Clear();
                return Json(
                new
                {
                    success = true,
                    warningMessages = callResult1.WarningMessages,
                    successMessages = callResult1.SuccessMessages,

                });

            }
            ModelState.Clear();
            return Json(
             new
             {
                 success = false,
                 errorMessages = callResult1.ErrorMessages,

             }, JsonRequestBehavior.AllowGet);

        }
    }

}



























//[HttpPost]
//public async Task<ActionResult> check(int id)
//{
//    var callResult = await _newRequestService.GetUserAsync(id);
//    if (callResult.Success)
//    {

//        ModelState.Clear();
//        return Json(
//        new
//        {
//            success = true,
//            warningMessages = callResult.WarningMessages,
//            successMessages = callResult.SuccessMessages,
//        });
//    }

//    return Json(
//     new
//     {
//         success = false,
//         errorMessages = callResult.ErrorMessages
//     });
//}








//[HttpPost]
//public async Task<ActionResult> Delete(int newRequestId)
//{
//    var callResult = await _newRequestService.DeleteNewRequestAsync(newRequestId);
//    if (callResult.Success)
//    {

//        ModelState.Clear();

//        return Json(
//            new
//            {
//                success = true,
//                warningMessages = callResult.WarningMessages,
//                successMessages = callResult.SuccessMessages,
//            });
//    }

//    return Json(
//        new
//        {
//            success = false,
//            errorMessages = callResult.ErrorMessages
//        });

//}