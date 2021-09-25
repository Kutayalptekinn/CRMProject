using CRM.Data;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace CRM.ViewModels.Admin
{

    public class CurrentCustomerModel
    {
        //+
        public CurrentCustomerModel()
        {
            AuthCodes = new List<string>();
        }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }

        public IEnumerable<string> AuthCodes { get; set; }

        //++
        public bool AuthorizedForAction(string[] authCodes)
        {
            if (authCodes == null || !authCodes.Any()) return true;

            return authCodes.Any(allowedAuthAction => AuthCodes.Any(a => a == allowedAuthAction));
        }

    }
    public class CurrentUserModel
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public bool IsMainUser { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public long CustomerId { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public IEnumerable<string> AuthCodes { get; set; }

        public bool AuthorizedForAction(string[] authCodes)
        {
            if (IsMainUser || authCodes == null || !authCodes.Any()) return true;
            return authCodes.Any(allowedAuthAction => AuthCodes.Any(a => a == allowedAuthAction)
            );
        }
    }
    public class LoginViewModel
    {
        //+
        public LoginViewModel()
        {
            Roles = new List<RolesViewModel>();
        }
        public int? Id { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public List<RolesViewModel> Roles { get; set; }
        public string RoleNames { get; set; }
        public bool Checked { get; set; }

    }
    public class UserAddViewModel
    {
        public int? Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public IEnumerable<RolesViewModel> Roles { get; set; }
        public List<string> RoleNames { get; set; }
        public List<RoleModel> RoleModels { get; set; }

    }
    public class RoleModel
    {

        public string RoleName { get; set; }
        public bool Checked { get; set; }

    }
    public class RoleAddViewModel
    {
        public int? Id { get; set; }
        public string Role { get; set; }

    }

    public class UserEditViewModel
    {
        public UserEditViewModel()
        {
            Roles = new List<RolesViewModel>();
          
        }
        public int? Id { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public List<RolesViewModel> Roles { get; set; }
        public List<RoleModel> RoleNames { get; set; }
        public bool Checked { get; set; }

    }
    public class PasswordEditViewModel
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }

    }
    public class PasswordPostEditViewModel
    {
        public int Id { get; set; }
        public string Password { get; set; }

    }
    public class UsersDeleteViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<RolesViewModel> Roles { get; set; }
        public List<string> RoleNames { get; set; }

    }
    public class UserRolesViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public RolesViewModel Roles { get; set; }
        public LoginViewModel User { get; set; }
    }
    public class RolesViewModel
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        
        public bool Checked { get; set; }
        public UserRolesViewModel UserRoles { get; set; }
    }

}
