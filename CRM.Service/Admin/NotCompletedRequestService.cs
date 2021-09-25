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
    public class NotCompletedRequestService
    {
        private readonly Model1Container _context;
        public NotCompletedRequestService(Model1Container context)
        {
            _context = context;
        }

        private IQueryable<NotCompletedRequestListViewModel> _getNotCompletedRequestListIQueryable(Expression<Func<Data.NotCompletedRequests, bool>> expr)
        {
            return (from b in _context.NotCompletedRequests.AsExpandable().Where(expr)

                    select new NotCompletedRequestListViewModel()
                    {

                        Id = b.Id,
                        Request = b.Request,
                        Deadline = b.Deadline,
                        Priority = b.Priority,
                        RequestorName = b.RequestorName,
                        Status = b.Status,
                        AssignTo = b.AssignTo,
                         NotDoneNote=b.NotDoneNote,
                        Step = b.Step.Select(x => new StepViewModel()
                        {
                            StepDetail = x.StepDetail,
                            Id = x.Id,
                            Checkedd = x.Checkedd

                        }).ToList()


                    });
        }


        public async Task<ServiceCallResult> AddNotCompletedRequestAsync(WorkingRequestListViewModel model1, CurrentUserModel user,int k)
        {
            var m = 0;
            var model = await _context.WorkingRequests.FirstOrDefaultAsync(a => a.Id == model1.Id).ConfigureAwait(false);
            var steps = model.Step.ToList();
            var callResult = new ServiceCallResult() { Success = false };
            foreach (var item in steps)
            {
                if (item.Checkedd == true)
                {
                    m++;
                }

            }
            var count = model.Step.Count;
            if (m == count)
            {
                callResult.ErrorMessages.Add("Bu işlemi yapmak için görev adımlarnızı tamamlamamış olmanız gerekiyor.");
                return callResult;
            }
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
            var notcompletedRq = new NotCompletedRequests()
            {
                Id = model.Id,
                RequestorName = user.UserName,
                Deadline = model.Deadline,
                Request = model.Request,
                Priority = model.Priority,
                Status = "Work is Not Done",
                AssignTo = model.AssignTo,
                StartingDate=model.StartingDate,
                 NotDoneNote=model1.NotDoneNote
            };
            foreach (var item in model.Step)
            {
                notcompletedRq.Step.Add(new Step()
                {
                    StepDetail = item.StepDetail,
                    NotCompletedRequestsId = item.NotCompletedRequestsId,
                    Id = item.Id,
                    Checkedd = item.Checkedd

                });
            }
            _context.NotCompletedRequests.Add(notcompletedRq);

            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();
                    callResult.Success = true;
                    callResult.Item = await GetNotCompletedRequestListViewAsync(notcompletedRq.Id, user.UserName).ConfigureAwait(false);
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }
        }

        public async Task<ServiceCallResult> DeleteNotCompletedRequestForDeleteUserAsync(int userId, string currentUserName)
        {
            var callResult = new ServiceCallResult() { Success = false };
            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == userId).ConfigureAwait(false);

            var NotCompletedRequestForAssignTo = await _context.NotCompletedRequests.Where(a => a.AssignTo == user.UserName).ToListAsync().ConfigureAwait(false);
            var NotCompletedRequestForRequestorName = await _context.NotCompletedRequests.Where(a => a.RequestorName == user.UserName).ToListAsync().ConfigureAwait(false);
            if (NotCompletedRequestForAssignTo.Count != 0)
            {
                foreach (var item in NotCompletedRequestForAssignTo)
                {
                    var steps = item.Step.ToList();
                    foreach (var step in steps)
                    {
                        _context.Step.Remove(step);

                    }


                    _context.NotCompletedRequests.Remove(item);
                }
            }

            if (NotCompletedRequestForRequestorName.Count != 0)
            {
                foreach (var item1 in NotCompletedRequestForRequestorName)
                {
                    var steps1 = item1.Step.ToList();
                    foreach (var step in steps1)
                    {
                        _context.Step.Remove(step);

                    }


                    _context.NotCompletedRequests.Remove(item1);
                }
            }




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
        public async Task<NotCompletedRequestListViewModel> GetNotCompletedRequestListViewAsync(int notcompletedrequestId, string currentUserName)
        {

            var predicate = PredicateBuilder.New<Data.NotCompletedRequests>(true);/*AND*/
            predicate.And(a => a.Id == notcompletedrequestId);
            var notcompletedRequest = await _getNotCompletedRequestListIQueryable(predicate).SingleOrDefaultAsync().ConfigureAwait(false);
            return notcompletedRequest;
        }

        public IQueryable<NotCompletedRequestListViewModel> GetNotCompletedRequestListIQueryable(List<LoginViewModel> users)
        {

            var predicate = PredicateBuilder.New<Data.NotCompletedRequests>(true);/*AND*/
            foreach (var item in users)
            {
                predicate.Or(a => a.AssignTo == item.UserName);
            }

            return _getNotCompletedRequestListIQueryable(predicate);
        }

        public async Task<ServiceCallResult> DeleteNotCompletedRequestAsync(int notCompletedRequestId, string currentUser)
        {
            var callResult = new ServiceCallResult() { Success = false };

            var notCompletedRequest = await _context.NotCompletedRequests.FirstOrDefaultAsync(a => a.Id == notCompletedRequestId).ConfigureAwait(false);
            if (notCompletedRequest.AssignTo == currentUser)
            {
                callResult.ErrorMessages.Add("Bu İşlemi Yapmaya Yetkiniz Yoktur.");
                return callResult;
            }
            var steps = notCompletedRequest.Step.ToList();
            foreach (var step in steps)
            {
                _context.Step.Remove(step);

            }


            if (notCompletedRequestId == null)
            {
                callResult.ErrorMessages.Add("Böyle bir SSS bulunamadı.");
                return callResult;
            }
            _context.NotCompletedRequests.Remove(notCompletedRequest);
            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {

                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();
                    callResult.SuccessMessages.Add("Silindi.");
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

        public async Task<NotCompletedRequestDeleteViewModel> GetNotCompletedRequestDeleteViewModelAsync(int notcompletedRequestId)
        {
            var notcompletedRequest = await (from b in _context.NotCompletedRequests
                                    where b.Id == notcompletedRequestId
                                    select new NotCompletedRequestDeleteViewModel()
                                    {
                                        Id = b.Id,
                                        Request = b.Request,
                                        Deadline = b.Deadline,
                                        AssignTo = b.AssignTo,
                                        Priority = b.Priority,
                                        Step = b.Step.Select(x => new StepViewModel()
                                        {
                                            StepDetail = x.StepDetail,

                                            Id = x.Id


                                        }).ToList()

                                    }).FirstOrDefaultAsync();
            return notcompletedRequest;
        }
    }
}
