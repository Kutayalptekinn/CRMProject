using CRM.Data;
using CRM.ViewModels.Admin;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Service.Admin
{
    public class CalendarService
    {

        private readonly Model1Container _context;

        public CalendarService(Model1Container context)
        {
            _context = context;
        }
        public List<CalendarListViewModel> GetCalendarList()
        {
            return (from b in _context.Calendar

                    select new CalendarListViewModel()
                    {
                        Id = b.Id,
                        Request = b.Request,
                        Deadline = b.Deadline,
                        StartingDate = b.StartingDate,
                        Worker = b.Worker



                    }).ToList();
        }

        private IQueryable<CalendarListViewModel> _getCalendarListIQueryable(Expression<Func<Data.Calendar, bool>> expr)
        {
            return (from b in _context.Calendar.AsExpandable().Where(expr)

                    select new CalendarListViewModel()
                    {


                        Id = b.Id,
                        Request = b.Request,
                        Deadline = b.Deadline,
                        StartingDate = b.StartingDate,
                        Worker = b.Worker,
                         Status=b.Status,



                    });

                   
        }
        public IQueryable<CalendarListViewModel> GetCalendarListIQueryable(List<LoginViewModel> users)
        {

            var predicate = PredicateBuilder.New<Data.Calendar>(true);/*AND*/
            foreach (var item in users)
            {
                predicate.Or(a => a.Worker == item.UserName);
            }

            return _getCalendarListIQueryable(predicate);
        }

    }
}
