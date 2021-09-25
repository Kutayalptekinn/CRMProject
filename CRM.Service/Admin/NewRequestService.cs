using CRM.Data;
using CRM.ViewModels.Admin;
using CRM.ViewModels.Common;
using LinqKit;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CRM.Service.Admin
{
    public class NewRequestService
    {
        private readonly Model1Container _context;

        public NewRequestService(Model1Container context)
        {
            _context = context;
        }

        //asexpandable
        //step alım metodu
        //212 237
        //task async anlamı
        private IQueryable<NewRequestListViewModel> _getNewRequestsListIQueryable(Expression<Func<Data.NewRequests, bool>> expr)
        {
            return (from b in _context.NewRequests.AsExpandable().Where(expr)

                    select new NewRequestListViewModel()
                    {

                        Id = b.Id,
                        Request = b.Request,
                        Deadline = b.Deadline,
                        Priority = b.Priority,
                        RequestorName = b.RequestorName,
                        Status = b.Status,
                        AssignTo = b.AssignTo,
                        Step = b.Step.Select(x => new StepViewModel()
                        {
                            StepDetail = x.StepDetail
                        }).ToList()


                    });
        }

        public List<EmptyAssignToNewRequestListViewModel> GetNewRequestsList()
        {
            return (from b in _context.NewRequests.Where(x => x.AssignTo == null).Include(x => x.Step)

                    select new EmptyAssignToNewRequestListViewModel()
                    {
                        Id = b.Id,
                        Request = b.Request,
                        Deadline = b.Deadline,
                        Priority = b.Priority,
                        Status = b.Status,
                        RequestorName = b.RequestorName,
                        AssignTo = b.AssignTo,
                        Description = b.Description,
                        Step = b.Step.Select(x => new StepViewModel()
                        {
                            Id = x.Id,
                            NewRequestsId = (int)x.NewRequestsId,
                            StepDetail = x.StepDetail
                        }).ToList()
                        // (from a in b.Step
                        //  select new StepViewModel()
                        //  {
                        //      StepDetail = a.StepDetail,
                        //  }
                        //).ToList()
                    }).ToList();
        }
        public List<ChooseUserViewModel> GetNewRequestsListByChoosen(ChooseUserViewModel model, string currentUserName)
        {
            var newReqests = (from b in _context.NewRequests.Where(x => x.AssignTo == model.AssignTo)

                              select new ChooseUserViewModel()
                              {
                                  Id = b.Id,
                                  Request = b.Request,
                                  Deadline = b.Deadline,
                                  Priority = b.Priority,
                                  Status = b.Status,
                                  RequestorName = b.RequestorName,
                                  AssignTo = b.AssignTo,
                                  Step = b.Step.Select(x => new StepViewModel()
                                  {
                                      StepDetail = x.StepDetail
                                  }).ToList()

                              }).ToList();

            var workingList = (from a in _context.WorkingRequests
                               where a.AssignTo == model.AssignTo
                               select new ChooseUserViewModel()
                               {
                                   Id = a.Id,
                                   Request = a.Request,
                                   Deadline = a.Deadline,
                                   Priority = a.Priority,
                                   Status = a.Status,
                                   RequestorName = a.RequestorName,
                                   AssignTo = a.AssignTo

                               }
                                     ).ToList();

            var completedList = (from a in _context.CompletedRequests
                                 where a.AssignTo == model.AssignTo
                                 select new ChooseUserViewModel()
                                 {
                                     Id = a.Id,
                                     Request = a.Request,
                                     Deadline = a.Deadline,
                                     Priority = a.Priority,
                                     Status = a.Status,
                                     RequestorName = a.RequestorName,
                                     AssignTo = a.AssignTo

                                 }
                                    ).ToList();

            var notcompletedList = (from a in _context.NotCompletedRequests
                                    where a.AssignTo == model.AssignTo
                                    select new ChooseUserViewModel()
                                    {
                                        Id = a.Id,
                                        Request = a.Request,
                                        Deadline = a.Deadline,
                                        Priority = a.Priority,
                                        Status = a.Status,
                                        RequestorName = a.RequestorName,
                                        AssignTo = a.AssignTo

                                    }
                                   ).ToList();


            newReqests.AddRange(workingList);
            newReqests.AddRange(completedList);
            newReqests.AddRange(notcompletedList);
            return newReqests;

        }

        public NewRequestListViewModel GetDescription(int id)
        {
            return (from b in _context.NewRequests
                    where b.Id == id

                    select new NewRequestListViewModel()
                    {
                        Description = b.Description


                    }).FirstOrDefault();
        }
        public List<EmptyAssignToNewRequestListViewModel> GetEmptyAssignToNewRequestsList(EmptyAssignToNewRequestListViewModel model, List<LoginViewModel> users)
        {
            var userNames = users.Select(t => t.UserName).ToList();
            return (from b in _context.NewRequests.Where(x => x.AssignTo == null && userNames.Contains(x.RequestorName))

                    select new EmptyAssignToNewRequestListViewModel()
                    {
                        Id = b.Id,
                        Request = b.Request,
                        Deadline = b.Deadline,
                        Priority = b.Priority,
                        Status = b.Status,
                        RequestorName = b.RequestorName,
                        AssignTo = b.AssignTo,
                        Description = b.Description,
                        Checked = b.Checked ?? false,
                        Step = b.Step.Select(x => new StepViewModel()
                        {
                            StepDetail = x.StepDetail
                        }).ToList()



                    }).ToList();
        }
        public List<UserRolesViewModel> GetRolesList(int id)
        {

            return (from b in _context.UserRoles
                    where b.Users.UserRoles.Any(x => x.UserId == id)
                    select new UserRolesViewModel()
                    {
                        Id = b.Id,
                        UserId = b.UserId,
                        RoleId = b.RoleId,
                        Roles = new RolesViewModel()
                        {
                            Id = b.Id,
                            RoleName = b.Roles.RoleName,
                        },
                        User = new LoginViewModel()
                        {
                            UserName = b.Users.UserName,
                            Password = b.Users.Password
                        }
                    }).ToList();
        }
        public List<LoginViewModel> GetUsersList(List<int> roleIds, int excludeUserId)
        {

            return (from b in _context.Users
                    where b.UserRoles.All(y => roleIds.Any(x => y.RoleId == x)) && b.Id != excludeUserId
                    select new LoginViewModel()
                    {

                        UserName = b.UserName

                    }).ToList();
        }

        public NewRequestAddViewModel GetEmptyStep()
        {

            var model = new NewRequestAddViewModel();
            model.Step = new List<StepViewModel>();
            if (!model.Step.Any())
            {
                model.Step.Add(new StepViewModel());
            }

            return model;
        }
        public List<LoginViewModel> GetUsersListByEmptyAssignTo(List<int> roleIds, int excludeUserId)
        {
            return (from b in _context.Users
                    where roleIds.All(x => b.UserRoles.Select(t => t.RoleId).Contains(x)) && b.Id != excludeUserId
                    select new LoginViewModel()
                    {
                        UserName = b.UserName

                    }).ToList();
        }

        public async Task<NewRequestEditViewModel> GetNewRequestEditViewModelAsync(int newRequestId)
        {
            var newRequest = await (from b in _context.NewRequests
                                    where b.Id == newRequestId
                                    select new NewRequestEditViewModel()
                                    {
                                        Id = b.Id,
                                        Request = b.Request,
                                        Deadline = b.Deadline,
                                        AssignTo = b.AssignTo,
                                        Priority = b.Priority,
                                        RequestorName=b.RequestorName,
                                        Description=b.Description,
                                        Step = b.Step.Select(x => new StepViewModel()
                                        {
                                            StepDetail = x.StepDetail,
                                            NewRequestsId = (int)x.NewRequestsId,
                                            Id = x.Id


                                        }).ToList()



                                    }).FirstOrDefaultAsync();
            return newRequest;
        }
        public async Task<NewRequestEditViewModel> GetEmptyAssignToNewRequestListViewModelAsync(int newRequestId)
        {
            var newRequest = await (from b in _context.NewRequests
                                    where b.Id == newRequestId
                                    select new NewRequestEditViewModel()
                                    {
                                        Id = b.Id,
                                        Request = b.Request,
                                        Deadline = b.Deadline,
                                        AssignTo = b.AssignTo,
                                        Priority = b.Priority,
                                        RequestorName = b.RequestorName,
                                        Description = b.Description,
                                        Step = b.Step.Select(x => new StepViewModel()
                                        {
                                            StepDetail = x.StepDetail,
                                            NewRequestsId = (int)x.NewRequestsId,
                                            Id = x.Id


                                        }).ToList()


                                    }).FirstOrDefaultAsync();
            return newRequest;
        }
        public async Task<NewRequestDeleteViewModel> GetNewRequestDeleteViewModelAsync(int newRequestId)
        {
            var newRequest = await (from b in _context.NewRequests
                                    where b.Id == newRequestId
                                    select new NewRequestDeleteViewModel()
                                    {
                                        Id = b.Id,
                                        Request = b.Request,
                                        Deadline = b.Deadline,
                                        AssignTo = b.AssignTo,
                                        Priority = b.Priority,
                                        Step = b.Step.Select(x => new StepViewModel()
                                        {
                                            StepDetail = x.StepDetail,
                                            NewRequestsId = (int)x.NewRequestsId,
                                            Id = x.Id


                                        }).ToList()

                                    }).FirstOrDefaultAsync();
            return newRequest;
        }

        public async Task<ServiceCallResult> EditNewRequestAsync(NewRequestEditViewModel model, CurrentUserModel userModel,int f)
        {
            var callResult = new ServiceCallResult() { Success = false };

            var newRequest = await _context.NewRequests.FirstOrDefaultAsync(a => a.Id == model.Id).ConfigureAwait(false);

            if (newRequest == null)
            {
                callResult.ErrorMessages.Add("Böyle bir newRequest bulunamadı.");
                return callResult;
            }
            //var step = newRequest.Step.Where(a => model.Step.All(z => a.NewRequestsId == z.NewRequestsId)).ToList();
            if(f==1)
            {
                newRequest.Id = model.Id;
                newRequest.Request = model.Request;
                newRequest.Priority = model.Priority;
                newRequest.Deadline = model.Deadline;
                newRequest.AssignTo = userModel.UserName;
                newRequest.Description = model.Description;
            }
            else
            {
                newRequest.Id = model.Id;
                newRequest.Request = model.Request;
                newRequest.Priority = model.Priority;
                newRequest.Deadline = model.Deadline;
                newRequest.AssignTo = model.AssignTo;
                newRequest.Description = model.Description;
            }
           
            var steps = newRequest.Step.ToList();
            foreach (var step in steps)
            {
                var callresult = await DeleteStepAsync(step.Id).ConfigureAwait(false);
            }
            foreach (var step in model.Step)
            {
                if (step.Id == 0)
                {
                    step.NewRequestsId = newRequest.Id;
                }
                var callresult = await AddStepAsync(step).ConfigureAwait(false);
            }







            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();

                    callResult.Success = true;
                    callResult.Item = await GetNewRequestListViewAsync(newRequest.Id, userModel.FirstName).ConfigureAwait(false);
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }

        }


        public async Task<ServiceCallResult> EditEmptyNewRequestAsync(NewRequestEditViewModel model, string currentUserName)
        {
            var callResult = new ServiceCallResult() { Success = false };

            var newRequest = await _context.NewRequests.FirstOrDefaultAsync(a => a.Id == model.Id).ConfigureAwait(false);

            if (newRequest == null)
            {
                callResult.ErrorMessages.Add("Böyle bir newRequest bulunamadı.");
                return callResult;
            }
            //var step = newRequest.Step.Where(a => model.Step.All(z => a.NewRequestsId == z.NewRequestsId)).ToList();
            newRequest.Id = model.Id;
            newRequest.Request = model.Request;
            newRequest.Priority = model.Priority;
            newRequest.Deadline = model.Deadline;
            newRequest.AssignTo = model.AssignTo;

            foreach (var step in model.Step)
            {
                var dbStep = newRequest.Step.FirstOrDefault(a => a.Id == step.Id);
                if (dbStep != null)
                {
                    dbStep.StepDetail = step.StepDetail;
                }
            }
            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();

                    callResult.Success = true;
                    callResult.Item = await GetNewRequestListViewAsync(newRequest.Id, currentUserName).ConfigureAwait(false);
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }

        }
        public async Task<ServiceCallResult> AddStepAsync(StepViewModel model)
        {
            var callResult = new ServiceCallResult() { Success = false };
            var step = new Step()
            {
                Id = model.Id,
                StepDetail = model.StepDetail,
                NewRequestsId = model.NewRequestsId,
            };
            _context.Step.Add(step);

            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();
                    callResult.Success = true;
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }
        }
        public async Task<NewRequestListViewModel> GetNewRequestListViewAsync(int newrequestId, string currentUserName)
        {

            var predicate = PredicateBuilder.New<Data.NewRequests>(true);/*AND*/
            predicate.And(a => a.Id == newrequestId);
            var newRequest = await _getNewRequestsListIQueryable(predicate).SingleOrDefaultAsync().ConfigureAwait(false);
            return newRequest;
        }


        public IQueryable<NewRequestListViewModel> GetNewRequestListIQueryable(NewRequestSearchViewModel model, CurrentUserModel currentUser)
        {

            var predicate = PredicateBuilder.New<Data.NewRequests>(true);
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                var predicateOr = PredicateBuilder.New<Data.NewRequests>(false);
                predicateOr.Or(a => a.RequestorName.Contains(model.Name));
                predicateOr.Or(a => a.AssignTo.Contains(model.Name));
                predicateOr.Or(a => a.Request.Contains(model.Name));
                predicateOr.Or(a => a.Status.Contains(model.Name));
                predicate = predicate.And(predicateOr);

            }
            if (model.PriorityList != null)
            {
                predicate.And(a => a.Priority == model.PriorityList);
            }
            if (currentUser.AuthCodes.Contains("A") && currentUser.AuthCodes.Contains("B") && currentUser.AuthCodes.Contains("C"))
            {
                predicate.And(a => a.AssignTo != null);
            }
            else
            {
                predicate.And(a => a.AssignTo == currentUser.UserName || a.RequestorName == currentUser.UserName);
            }

            return _getNewRequestsListIQueryable(predicate);
        }

        public async Task<ServiceCallResult> AddNewRequestAsync(NewRequestAddViewModel model, CurrentUserModel user)
        {
            var callResult = new ServiceCallResult() { Success = false };
            var newRq = new NewRequests()
            {

                RequestorName = user.UserName,
                Deadline = model.Deadline,
                Request = model.Request,
                Priority = model.Priority,
                Status = "New",
                AssignTo = model.AssignTo,
                Description = model.Description,



            };
            foreach (var item in model.Step)
            {
                newRq.Step.Add(new Step()
                {
                    StepDetail = item.StepDetail,
                    NewRequestsId = item.NewRequestsId
                });
            }


            _context.NewRequests.Add(newRq);

            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();
                    callResult.Success = true;
                    callResult.Item = await GetNewRequestListViewAsync(newRq.Id, user.UserName).ConfigureAwait(false);
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }
        }
        public async Task<ServiceCallResult> DeleteNewRequestAsync(int newRequestId, string currentUser, int n)
        {

            var callResult = new ServiceCallResult() { Success = false };

            var newRequest = await _context.NewRequests.FirstOrDefaultAsync(a => a.Id == newRequestId).ConfigureAwait(false);
            if (newRequest.AssignTo == currentUser && n == 0)
            {
                callResult.ErrorMessages.Add("Bu İşlemi Yapmaya Yetkiniz Yoktur.");
                return callResult;
            }

            var steps = newRequest.Step.ToList();
            foreach (var step in steps)
            {
                _context.Step.Remove(step);

            }


            if (newRequestId == null)
            {
                callResult.ErrorMessages.Add("Böyle bir SSS bulunamadı.");
                return callResult;
            }
            _context.NewRequests.Remove(newRequest);
            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {

                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();
                    callResult.SuccessMessages.Add("İşlem Gerçekleşti.");
                    callResult.Success = true;
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }


        }


        public async Task<ServiceCallResult> DeleteStepAsync(int stepId)
        {
            var callResult = new ServiceCallResult() { Success = false };


            var step = await _context.Step.FirstOrDefaultAsync(a => a.Id == stepId).ConfigureAwait(false);
            if (step == null)
            {
                callResult.ErrorMessages.Add("step doesnt exist");
                return callResult;
            }
            _context.Step.Remove(step);
            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();
                    callResult.Success = true;
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }


        }
        public async Task<ServiceCallResult> GetUserAsync(int id)
        {
            var callResult = new ServiceCallResult() { Success = false };


            var newRequest = await _context.NewRequests.FirstOrDefaultAsync(a => a.Id == id).ConfigureAwait(false);
            if (id == null)
            {
                callResult.ErrorMessages.Add("Böyle bir SSS bulunamadı.");
                return callResult;
            }
            newRequest.Checked = !newRequest.Checked;

            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();
                    callResult.Success = true;
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }


        }


    }
}

