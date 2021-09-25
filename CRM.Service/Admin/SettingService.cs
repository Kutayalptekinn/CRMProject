using CRM.Data;
using CRM.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CRM.ViewModels.Admin.LoginViewModel;

namespace CRM.Service.Admin
{
    public class SettingService
    {
        private readonly Model1Container _context;

        public SettingService(Model1Container context)
        {
            _context = context;
        }
        public bool Login(LoginViewModel login)
        {
            return _context.Users.Any(x => x.Password == login.Password && x.UserName == login.UserName);
        }
        public long? LoginWithRole(LoginViewModel login)
        {
            return _context.Users.FirstOrDefault(x => x.Password == login.Password && x.UserName == login.UserName)?.Id;
        }

    }
}
