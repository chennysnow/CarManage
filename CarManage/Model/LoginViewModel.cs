using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Marep.Web
{
    public class LoginViewModel
    {
        [Required( ErrorMessage = "用户名不能为空!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "用户名不能为空!")]
        public string Password { get; set; }
    }
    public class UserPwdModel
    {
        [Required( ErrorMessage = "密码不能为空!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "新密码不能为空!")]
        public string NewPassword { get; set; }

       [Required(ErrorMessage = "校验密码不能为空")]
        public string ChkPassword { get; set; }

    }


}
