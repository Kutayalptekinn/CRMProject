using CRM.Data;
using CRM.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Service.Common
{
    public class CommonService
    {
        private readonly Model1Container _context;
        public CommonService(Model1Container context)
        {
            _context = context;
        }
        public async Task<CurrentUserModel> GetCurrentUserModelAsync(long userId)
        {
            var userQuery = from u in _context.Users
                            where
                                u.Id == userId                              
                            select new CurrentUserModel
                            {
                                UserId = u.Id,
                                UserName = u.UserName,                               
                                AuthCodes = u.UserRoles.Select(x=>x.Roles.RoleName)
                            };

            return await userQuery.SingleOrDefaultAsync().ConfigureAwait(false);
        }
    }
}

//expression predicatenin oluşturduğu şey 
