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


    public class UsersService
    {
        private readonly Model1Container _context;

        public UsersService(Model1Container context)
        {
            _context = context;
        }

        public List<LoginViewModel> GetUsersList(List<int> roleIds, int excludeUserId)
        {
            return (from b in _context.Users
                    where b.UserRoles.All(y => roleIds.Any(x => y.RoleId == x))
                    select new LoginViewModel()
                    {
                        Id = b.Id,
                        UserName = b.UserName,
                        Password = b.Password,
                        Roles = (from a in b.UserRoles
                                 select new RolesViewModel()
                                 {
                                     RoleName = a.Roles.RoleName,
                                 }).ToList()
                    }).ToList();
        }

        public async Task<UsersDeleteViewModel> GetUserDeleteViewModelAsync(int userId)
        {
            var user = await (from b in _context.Users
                              where b.Id == userId
                              select new UsersDeleteViewModel()
                              {
                                  Id = b.Id,
                                  Password = b.Password,
                                  UserName = b.UserName

                              }).FirstOrDefaultAsync();

            return user;
        }
        public async Task<PasswordEditViewModel> GetUserPasswordEditViewModelAsync(int userId)
        {
            var user = await (from b in _context.Users
                              where b.Id == userId
                              select new PasswordEditViewModel()
                              {
                                  Id = b.Id,



                              }).FirstOrDefaultAsync();

            return user;
        }

        public async Task<ServiceCallResult> PasswordEditUserAsync(PasswordEditViewModel model, CurrentUserModel currentUser)
        {

            var callResult = new ServiceCallResult() { Success = false };

            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == model.Id).ConfigureAwait(false);



            if (user.Password != model.Password)
            {
                callResult.ErrorMessages.Add("Yanlış Parola");
                return callResult;
            }
            user.Password = model.NewPassword;
            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();
                    callResult.SuccessMessages.Add("Parola Başarıyla Değiştirildi.");
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



        public async Task<ServiceCallResult> DeleteUserAsync(int userId)
        {

            var callResult = new ServiceCallResult() { Success = false };

            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == userId).ConfigureAwait(false);
            var userRoles = user.UserRoles.ToList();
            foreach (var item in userRoles)
            {
                _context.UserRoles.Remove(item);
            }


            if (userId == null)
            {
                callResult.ErrorMessages.Add("Böyle bir user bulunamadı.");
                return callResult;
            }
            _context.Users.Remove(user);
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


        public async Task<ServiceCallResult> CheckOnGoingRequests(int userId)
        {

            var callResult = new ServiceCallResult() { Success = false };

            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == userId).ConfigureAwait(false);


            var NewRequestForAssignTo = await _context.NewRequests.Where(a => a.AssignTo == user.UserName).ToListAsync().ConfigureAwait(false);
            var WorkingRequestForAssignTo=await _context.WorkingRequests.Where(a => a.AssignTo == user.UserName).ToListAsync().ConfigureAwait(false);
            if (NewRequestForAssignTo.Count!=0 || WorkingRequestForAssignTo.Count!=0)
            {
                
                callResult.ErrorMessages.Add("Kullanıcının Devam Eden Görevi Bulunmaktadır");
                
                return callResult;
            }

                    callResult.SuccessMessages.Add("İşlem Gerçekleşti.");
                    callResult.Success = true;
                    return callResult;
                

        }


        
        public async Task<UserEditViewModel> GetUserEditViewModelAsync(int userId)
        {
            var user = await (from b in _context.Users
                              where b.Id == userId
                              select new UserEditViewModel()
                              {
                                  Id = b.Id,
                                  UserName = b.UserName,



                                  RoleNames = (from c in b.UserRoles
                                               select new RoleModel()
                                               {

                                                   RoleName = c.Roles.RoleName
                                               }).ToList(),

                                  Roles = (from a in _context.Roles
                                           select new RolesViewModel()
                                           {
                                               Id = a.Id,
                                               RoleName = a.RoleName

                                           }).ToList()

                              }).FirstOrDefaultAsync();


            return user;
        }



        public async Task<ServiceCallResult> AddNewUserAsync(UserAddViewModel model)
        {
            var k = 0;
            var callResult = new ServiceCallResult() { Success = false };
            var user1 = new Users()
            {

                Password = model.Password,
                UserName = model.UserName,
            };
            _context.Users.Add(user1);


            foreach (var item in model.Roles)
            {
                if (item.Checked)
                {
                    var m = _context.Roles.FirstOrDefault(x => x.Id == item.Id);
                    var user2 = new UserRoles()
                    {
                        UserId = user1.Id,
                        RoleId = m.Id
                         
                    };
                    _context.UserRoles.Add(user2);
                }

            }




            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();
                    callResult.Success = true;
                    callResult.Item = await GetUserListViewAsync(user1.Id).ConfigureAwait(false);
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }
        }

        public async Task<LoginViewModel> GetUserListViewAsync(int userId)
        {

            var predicate = PredicateBuilder.New<Data.Users>(true);/*AND*/
            predicate.And(a => a.Id == userId);
            var user = await _getUsersListIQueryable(predicate).SingleOrDefaultAsync().ConfigureAwait(false);
            return user;
        }
        private IQueryable<LoginViewModel> _getUsersListIQueryable(Expression<Func<Data.Users, bool>> expr)
        {
            return (from b in _context.Users.AsExpandable().Where(expr)

                    select new LoginViewModel()
                    {
                        Id = b.Id,
                        Password = b.Password,
                        Roles = (from a in b.UserRoles
                                 select new RolesViewModel()
                                 {
                                     RoleName = a.Roles.RoleName,
                                 }).ToList(),
                        UserName = b.UserName,




                    });
        }

        public LoginViewModel GetRolesList()
        {

            return new LoginViewModel
            {
                Roles =
                (from b in _context.Roles
                 select new RolesViewModel()
                 {

                     Id = b.Id,
                     RoleName = b.RoleName

                 }).ToList()
            };
        }

        public async Task<ServiceCallResult> EditUserAsync(UserEditViewModel model, CurrentUserModel currentUser)
        {
            var callResult = new ServiceCallResult() { Success = false };

            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == model.Id).ConfigureAwait(false);

            if (user == null)
            {
                callResult.ErrorMessages.Add("Böyle bir user bulunamadı.");
                return callResult;
            }

            user.Id = (int)model.Id;
            user.UserName = model.UserName;
            var userRoles = user.UserRoles.ToList();
            foreach (var item in userRoles)
            {
                _context.UserRoles.Remove(item);
            }
            var roleNames = model.Roles.ToList();
            foreach (var item1 in roleNames)
            {
                if(item1.Checked==true)
                {
                    var m = _context.Roles.FirstOrDefault(x => x.Id == item1.Id);
                    var userRoles1 = new UserRoles
                    {
                        RoleId = m.Id,
                        UserId = (int)model.Id

                    };
                    _context.UserRoles.Add(userRoles1);
                }
               

            }


            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();

                    callResult.Success = true;
                    callResult.Item = await GetUserListViewAsync(user.Id).ConfigureAwait(false);
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }

        }

        //public async Task<ServiceCallResult> AddNewRoleAsync(RoleAddViewModel model)
        //{
        //    var callResult = new ServiceCallResult() { Success = false };

        //    var roles =await _context.Roles.ToListAsync().ConfigureAwait(false);
        //    var roleName = model.Role;
        //    foreach (var item in roles)
        //    {
        //        if(item.RoleName==roleName)
        //        {
        //            callResult.ErrorMessages.Add("Bu Adda rol bulunmaktadır");
        //            return callResult;
        //        }
        //    }
        //    var role = new Roles()
        //    {

        //        RoleName = model.Role
        //    };
        //    _context.Roles.Add(role);

        //    using (var dbtransaction = _context.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            await _context.SaveChangesAsync().ConfigureAwait(false);
        //            dbtransaction.Commit();
        //            callResult.Success = true;
        //            callResult.Item = await GetRoleListViewAsync(role.Id).ConfigureAwait(false);
        //            return callResult;
        //        }
        //        catch (Exception exc)
        //        {
        //            callResult.ErrorMessages.Add(exc.GetBaseException().Message);
        //            return callResult;
        //        }
        //    }
        //}

        public async Task<LoginViewModel> GetRoleListViewAsync(int roleId)
        {

            var predicate = PredicateBuilder.New<Data.Roles>(true);/*AND*/
            predicate.And(a => a.Id == roleId);
            var user = await _getRolesListIQueryable(predicate).SingleOrDefaultAsync().ConfigureAwait(false);
            return user;
        }

        private IQueryable<LoginViewModel> _getRolesListIQueryable(Expression<Func<Data.Roles, bool>> expr)
        {
            return (from b in _context.Roles.AsExpandable().Where(expr)

                    select new LoginViewModel()
                    {
                        Id = b.Id,
                        RoleNames = b.RoleName




                    });
        }

    }
}
