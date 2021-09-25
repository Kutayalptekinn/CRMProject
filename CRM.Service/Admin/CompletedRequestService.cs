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
    public class CompletedRequestService
    {
        private readonly Model1Container _context;
        public CompletedRequestService(Model1Container context)
        {
            _context = context;
        }

        private IQueryable<CompletedRequestListViewModel> _getCompletedRequestListIQueryable(Expression<Func<Data.CompletedRequests, bool>> expr)
        {
            return (from b in _context.CompletedRequests.AsExpandable().Where(expr)

                    select new CompletedRequestListViewModel()
                    {
                        StartingDate = b.StartingDate,
                        Id = b.Id,
                        Request = b.Request,
                        Deadline = b.Deadline,
                        Priority = b.Priority,
                        RequestorName = b.RequestorName,
                        Status = "Done",
                        AssignTo = b.AssignTo,
                        DoneNote = b.DoneNote,
                        Step = b.Step.Select(x => new StepViewModel()
                        {
                            StepDetail = x.StepDetail,
                            Id = x.Id,
                            Checkedd = x.Checkedd

                        }).ToList()


                    }) ;
        }

        

        public IQueryable<CompletedRequestListViewModel> GetCompletedRequestListIQueryable(List<LoginViewModel> users)
        {

            var predicate = PredicateBuilder.New<Data.CompletedRequests>(true);/*AND*/
            foreach (var item in users)
            {
                predicate.Or(a => a.AssignTo == item.UserName);
            }

            return _getCompletedRequestListIQueryable(predicate);
        }


        public async Task<ServiceCallResult> AddCompletedRequestAsync(WorkingRequestListViewModel model1, CurrentUserModel user,int k)
        {

            var m = 0;
            var model = await _context.WorkingRequests.FirstOrDefaultAsync(a => a.Id == model1.Id).ConfigureAwait(false);
            var steps = model.Step.ToList();
            var callResult = new ServiceCallResult() { Success = false };
            foreach (var item in steps)
            {
                if(item.Checkedd==true)
                {
                    m++;
                }
                
            }
            var count = model.Step.Count;
            if (m != count)
            {
                callResult.ErrorMessages.Add("Bu işlemi yapmak için görev adımlarnızı tamamlamanız gerekiyor.");
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

            var completedRq = new CompletedRequests()
            {
                Id = model.Id,
                RequestorName = user.UserName,
                Deadline = model.Deadline,
                Request = model.Request,
                Priority = model.Priority,
                Status = "Done",
                AssignTo = model.AssignTo,
                StartingDate=model.StartingDate,
                DoneNote=model1.DoneNote,
                
            };
            foreach (var item in model.Step)
            {
                completedRq.Step.Add(new Step()
                {
                    StepDetail = item.StepDetail,
                     CompletedRequestsId = item.CompletedRequestsId,
                    Id = item.Id,
                    Checkedd = item.Checkedd

                });
            }
            _context.CompletedRequests.Add(completedRq);

            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();
                    callResult.Success = true;
                    callResult.Item = await GetCompletedRequestListViewAsync(completedRq.Id, user.UserName).ConfigureAwait(false);
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }
        }

        public async Task<ServiceCallResult> DeleteCompletedRequestForDeleteUserAsync(int userId, string currentUserName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == userId).ConfigureAwait(false);
            var callResult = new ServiceCallResult() { Success = false };

            var completedRequestForAssignTo = await _context.CompletedRequests.Where(a => a.AssignTo == user.UserName).ToListAsync().ConfigureAwait(false);
            var completedRequestForRequestorName = await _context.CompletedRequests.Where(a => a.RequestorName == user.UserName).ToListAsync().ConfigureAwait(false);
            if (completedRequestForRequestorName.Count != 0)
            {
                foreach (var item in completedRequestForRequestorName)
                {
                    var steps = item.Step.ToList();
                    foreach (var step in steps)
                    {
                        _context.Step.Remove(step);

                    }


                    _context.CompletedRequests.Remove(item);
                }
            }
            if (completedRequestForAssignTo.Count != 0)

            {
                foreach (var item in completedRequestForAssignTo)
                {
                    var steps1 = item.Step.ToList();
                    foreach (var step in steps1)
                    {
                        _context.Step.Remove(step);

                    }


                    _context.CompletedRequests.Remove(item);
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

        public async Task<CompletedRequestListViewModel> GetCompletedRequestListViewAsync(int completedrequestId, string currentUserName)
        {

            var predicate = PredicateBuilder.New<Data.CompletedRequests>(true);/*AND*/
            predicate.And(a => a.Id == completedrequestId);
            var completedRequest = await _getCompletedRequestListIQueryable(predicate).SingleOrDefaultAsync().ConfigureAwait(false);
            return completedRequest;
        }

        public async Task<ServiceCallResult> DeleteCompletedRequestAsync(int completedRequestId, string currentUser)
        {
            var callResult = new ServiceCallResult() { Success = false };

            var completedRequest = await _context.CompletedRequests.FirstOrDefaultAsync(a => a.Id == completedRequestId).ConfigureAwait(false);
            if (completedRequest.AssignTo == currentUser)
            {
                callResult.ErrorMessages.Add("Bu İşlemi Yapmaya Yetkiniz Yoktur.");
                return callResult;
            }
            var steps = completedRequest.Step.ToList();
            foreach (var step in steps)
            {
                _context.Step.Remove(step);

            }


            if (completedRequestId == null)
            {
                callResult.ErrorMessages.Add("Böyle bir SSS bulunamadı.");
                return callResult;
            }
            _context.CompletedRequests.Remove(completedRequest);
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
        public async Task<CompletedRequestDeleteViewModel> GetCompletedRequestDeleteViewModelAsync(int completedRequestId)
        {
            var newRequest = await (from b in _context.CompletedRequests
                                    where b.Id == completedRequestId
                                    select new CompletedRequestDeleteViewModel()
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
            return newRequest;
        }
    }
}
