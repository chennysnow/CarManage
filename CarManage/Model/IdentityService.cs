using Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FrameSystemInfo;
namespace CarSystem
{
    public class IdentityService
    {
        public const string AuthenticationScheme = "912SCAuthCookie";
       
        public IdentityService()
        {

        }
        //使用用户名和密码获取用户
        public ClaimsPrincipal CheckUserAsync(string username, string password)
        {
            var user = FrameSysteminfo.Instance.GetUserAsync(username, password);
            if (user == null) return null;

            var ci = CreateClaimsIdentity(user);
            var roles = FrameSysteminfo.Instance.GetUserRolesAsync(user);
            AddRoleClaims(ci, roles.ToList());
            FrameSysteminfo.Instance.AddOnline(user);
            return new ClaimsPrincipal(ci);
        }        
       
        #region 辅助方法

        private ClaimsIdentity CreateClaimsIdentity(Users user)
        {
            //用当前用户信息创建一个ClaimsIdentity
            //AuthenticationScheme需要和Cookie中间件中AuthenticationScheme一致
            //如果添加的角色时使用的类型不是ClaimTypes.Role，则需要在此处指定类型
            //var result = new ClaimsIdentity(AuthenticationScheme,NameType,RoleType);
            var result = new ClaimsIdentity(AuthenticationScheme);
            //NameType使用自带的ClaimTypes.Name
            result.AddClaim(new Claim(ClaimTypes.Name, user.user_id));
            return result;
        }

        private void AddRoleClaims(ClaimsIdentity claimsIdentity, IList<Role> roles)
        {
            foreach (var role in roles)
            {
                //添加角色时使用自带的ClaimTypes.Role就不需要在新建ClaimsIdentity时指定角色验证类型
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.role_id));
            }
        }

        #endregion
    }

    public class IdentityResult
    {
        public bool IsSuccess { get; }
        public string ErrorString { get; }
        public ClaimsPrincipal User { get; }

        public IdentityResult(string error)
        {
            IsSuccess = false;
            ErrorString = error;
        }

        public IdentityResult(ClaimsPrincipal user)
        {
            IsSuccess = true;
            User = user;
        }
    }
}
