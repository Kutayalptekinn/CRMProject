using CRM.Data;
using CRM.ViewModels.Admin;
using CRM.ViewModels.Common;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Service.Admin
{
    public class WorkingRequestService
    {
        private readonly Model1Container _context;

        public WorkingRequestService(Model1Container context)
        {
            _context = context;
        }

 

        private IQueryable<WorkingRequestListViewModel> _getWorkingRequestsListIQueryable(Expression<Func<Data.WorkingRequests, bool>> expr)
        {
            return (from b in _context.WorkingRequests.AsExpandable().Where(expr)

                    select new WorkingRequestListViewModel()
                    {

                        Id = b.Id,
                        Request = b.Request,
                        Deadline = b.Deadline,
                        Priority = b.Priority,
                        RequestorName = b.RequestorName,
                        Status = b.Status,
                        AssignTo = b.AssignTo,
                         DiffDay= (int)DbFunctions.DiffDays(DateTime.Now, b.Deadline) ,
                        Step = b.Step.Select(x => new StepViewModel()
                        {
                            StepDetail = x.StepDetail,
                            Id = x.Id,
                            Checkedd = x.Checkedd

                        }).ToList()


                    });
        }

        public WorkingRequestListViewModel GetDescription(int id)
        {
            return (from b in _context.WorkingRequests
                    where b.Id == id

                    select new WorkingRequestListViewModel()
                    {
                        Description = b.Description


                    }).FirstOrDefault();
        }

        private IQueryable<WorkingRequestDetailListViewModel> _getWorkingRequestDetailsListIQueryable(Expression<Func<Data.WorkingRequests, bool>> expr)
        {
            return (from b in _context.WorkingRequests.AsExpandable().Where(expr)

                    select new WorkingRequestDetailListViewModel()
                    {

                        Id = b.Id,
                        Request = b.Request,
                        Deadline = b.Deadline,
                        Priority = b.Priority,
                        RequestorName = b.RequestorName,
                        Status = "Working on it",
                        AssignTo = b.AssignTo,
                        StartingDate = b.StartingDate,
                        Step = b.Step.Select(x => new StepViewModel()
                        {
                            StepDetail = x.StepDetail,
                            Id = x.Id

                        }).ToList()


                    });
        }
        public List<WorkingRequestListViewModel> GetWorkingRequestsList()
        {
            return (from b in _context.WorkingRequests.Where(x => x.AssignTo != null).Include(x => x.Step)

                    select new WorkingRequestListViewModel()
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
                            Id = x.Id,
                            WorkingRequestId = (int)x.WorkingRequestId,
                            StepDetail = x.StepDetail
                        }).ToList()

                    }).ToList();
        }
        public async Task<ServiceCallResult> GetCheckAsync(int id,CurrentUserModel currentUser)
        {
            var callResult = new ServiceCallResult() { Success = false };


            var workingRequestChecked = await _context.Step.FirstOrDefaultAsync(a => a.Id == id).ConfigureAwait(false);
            if (id == null)
            {
                callResult.ErrorMessages.Add("Böyle bir istek bulunamadı.");
                return callResult;
            }
            if (workingRequestChecked.WorkingRequests.RequestorName == currentUser.UserName)
            {
                callResult.ErrorMessages.Add("Sadece atanan kişi görev adımlarını seçebilir");
                return callResult;
            }

            workingRequestChecked.Checkedd = !workingRequestChecked.Checkedd;

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
        public IQueryable<WorkingRequestListViewModel> GetWorkingRequestListIQueryable(WorkingRequestSearchViewModel model, CurrentUserModel currentUser)
        {

            var predicate = PredicateBuilder.New<Data.WorkingRequests>(false);/*AND*/
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                predicate.Or(a => a.RequestorName.Contains(model.Name));
                predicate.Or(a => a.AssignTo.Contains(model.Name));
                predicate.Or(a => a.Request.Contains(model.Name));
                predicate.Or(a => a.Status.Contains(model.Name));
            }
           
            if (currentUser.AuthCodes.Contains("A") && currentUser.AuthCodes.Contains("B") && currentUser.AuthCodes.Contains("C"))
            {
                predicate.And(a => a.AssignTo != null);
            }
            else
            {
                predicate.And(a => a.AssignTo == currentUser.UserName || a.RequestorName == currentUser.UserName);
            }

            if (model.LastMonth)
            {
                predicate.And(x => DbFunctions.DiffDays(DateTime.Now, x.Deadline) > 2 && DbFunctions.DiffDays(DateTime.Now, x.Deadline) < 31 && x.Deadline > DateTime.Now);

            }
            if (model.LastWeek)
            {
                predicate.And(x => DbFunctions.DiffDays(DateTime.Now, x.Deadline) > 2 &&  DbFunctions.DiffDays(DateTime.Now, x.Deadline) < 8 && x.Deadline > DateTime.Now);

            }
            if (model.LastDay)
            {
                predicate.And(x => DbFunctions.DiffDays(DateTime.Now, x.Deadline) < 2 && x.Deadline > DateTime.Now);

            }
            if (model.Expired)
            {
                predicate.And(x => DbFunctions.DiffDays(DateTime.Now, x.Deadline) < 0);

            }

            return _getWorkingRequestsListIQueryable(predicate);
        }

        public IQueryable<WorkingRequestDetailListViewModel> GetWorkingRequestDetailListIQueryable(List<LoginViewModel> users)
        {

            var predicate = PredicateBuilder.New<Data.WorkingRequests>(true);/*AND*/
            foreach (var item in users)
            {
                predicate.Or(a => a.AssignTo == item.UserName);
            }

            return _getWorkingRequestDetailsListIQueryable(predicate);
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

        public List<WorkingRequestListViewModel> FilterByMonth()
        {

            return (from b in _context.WorkingRequests.Where(x => DbFunctions.DiffDays(DateTime.Now,x.Deadline ) < 30 && x.Deadline>DateTime.Now)
                        //return (from b in _context.WorkingRequests.Where(x => (x.Deadline - DateTime.Now).Value.Days < 30)

                    select new WorkingRequestListViewModel()
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
                                Id = x.Id,
                                WorkingRequestId = (int)x.WorkingRequestId,
                                StepDetail = x.StepDetail
                            }).ToList()

                        

                        //DateDiff = DbFunctions.DiffDays(DateTime.Now, b.Deadline).Value,

                    }).ToList();
        }

        public async Task<WorkingRequestDeleteViewModel> GetWorkingRequestDeleteViewModelAsync(int workingRequestId)
        {
            var workingRequest = await (from b in _context.WorkingRequests
                                        where b.Id == workingRequestId
                                        select new WorkingRequestDeleteViewModel()
                                        {
                                            Id = b.Id,
                                            Request = b.Request,
                                            Deadline = b.Deadline,
                                            AssignTo = b.AssignTo,
                                            Priority = b.Priority,
                                            Step = b.Step.Select(x => new StepViewModel()
                                            {
                                                StepDetail = x.StepDetail,
                                                WorkingRequestId = (int)x.WorkingRequestId,
                                                Id = x.Id
                                            }).ToList()

                                        }).FirstOrDefaultAsync();
            return workingRequest;
        }
        public async Task<WorkingRequestListViewModel> FindUserById(int workingRequestId)
        {
            {
                var workingRequest = await (from b in _context.WorkingRequests
                                            where b.Id == workingRequestId
                                            select new WorkingRequestListViewModel()
                                            {
                                                Id = b.Id,
                                                Request = b.Request,
                                                Deadline = b.Deadline,
                                                AssignTo = b.AssignTo,
                                                Priority = b.Priority,
                                                Step = b.Step.Select(x => new StepViewModel()
                                                {
                                                    StepDetail = x.StepDetail,
                                                    WorkingRequestId = (int)x.WorkingRequestId,
                                                    Id = x.Id


                                                }).ToList()



                                            }).FirstOrDefaultAsync();
                return workingRequest;
            }
        }
        public async Task<ServiceCallResult> DeleteWorkingRequestAsync(int workingRequestId, string currentUser)
        {
            var callResult = new ServiceCallResult() { Success = false };
            var workingRequest = await _context.WorkingRequests.FirstOrDefaultAsync(a => a.Id == workingRequestId).ConfigureAwait(false);



            var steps = workingRequest.Step.ToList();
            foreach (var step in steps)
            {
                _context.Step.Remove(step);

            }


            if (workingRequestId == null)
            {
                callResult.ErrorMessages.Add("Böyle bir istek bulunamadı.");
                return callResult;
            }
            _context.WorkingRequests.Remove(workingRequest);
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
        

        public async Task<ServiceCallResult> AddWorkingRequestAsync(int id, CurrentUserModel user, int k)
        {
            var model = await _context.NewRequests.FirstOrDefaultAsync(a => a.Id == id).ConfigureAwait(false);
            var callResult = new ServiceCallResult() { Success = false };

            if (model.AssignTo == user.UserName && k == 0)
            {
                callResult.ErrorMessages.Add("Bu İşlemi Yapmaya Yetkiniz Yoktur.");
                return callResult;
            }
            else if (model.RequestorName == user.UserName && k == 1)
            {
                callResult.ErrorMessages.Add("Sadece atanan kişi göreve başlayabilir.");
                return callResult;
            }
            var workingRq = new WorkingRequests()
            {
                Id = model.Id,
                RequestorName = model.RequestorName,
                Deadline = model.Deadline,
                Request = model.Request,
                Priority = model.Priority,
                Status = "Work Started",
                AssignTo = model.AssignTo,
                StartingDate = DateTime.Now
            };
            foreach (var item in model.Step)
            {
                workingRq.Step.Add(new Step()
                {
                    StepDetail = item.StepDetail,
                    WorkingRequestId = item.WorkingRequestId,
                    Id = item.Id,
                    Checkedd = false

                });
            }

            var calendar = new Calendar()
            {
                Id = model.Id,
                Worker = model.AssignTo,
                Deadline = model.Deadline,
                Request = model.Request,
                StartingDate = DateTime.Now,
                Status = "Work Started"

            };

            _context.Calendar.Add(calendar);
            _context.WorkingRequests.Add(workingRq);

            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();
                    callResult.Success = true;
                    callResult.Item = await GetWorkingRequestListViewAsync(workingRq.Id, user.UserName).ConfigureAwait(false);
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }
        }

        public async Task<WorkingRequestListViewModel> GetWorkingRequestListViewAsync(int workingrequestId, string currentUserName)
        {

            var predicate = PredicateBuilder.New<Data.WorkingRequests>(true);/*AND*/
            predicate.And(a => a.Id == workingrequestId);
            var workingRequest = await _getWorkingRequestsListIQueryable(predicate).SingleOrDefaultAsync().ConfigureAwait(false);
            return workingRequest;
        }

    }
}
