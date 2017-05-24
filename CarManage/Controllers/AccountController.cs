using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarSystem;
using Marep.Web;
using SqlTools;
using Microsoft.Extensions.Logging;
using Entitys;
using CarManage.Model;

namespace CarManage.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly IdentityService _identityService;
        private readonly MSqlClient mdb;
        public AccountController(
          ILoggerFactory loggerFactory,
          IdentityService identityService, MSqlClient mdao)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _identityService = identityService;
            this.mdb = mdao;
        }
        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            //使用用户名和密码获取用户信息
            var user = _identityService.CheckUserAsync(model.UserName, model.Password);
            if (user == null)
            {
                //无用户返回错误
                ModelState.AddModelError(string.Empty, "用户名或密码错误!");
                return View(model);
            }
            //登录，在Response中设置Cookie
            await HttpContext.Authentication.SignInAsync(IdentityService.AuthenticationScheme, user);
            return RedirectToLocal(returnUrl);
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(ManageController.Index), "Manage");
            }
        }

        public async Task<IActionResult> logout()
        {
            await HttpContext.Authentication.SignOutAsync(IdentityService.AuthenticationScheme);
            return RedirectToAction(nameof(AccountController.Login), "Account");
            //return RedirectToLocal("/account/login");
        }

        public  IActionResult Userpwd()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public   IActionResult Userpwd(UserPwdModel pwd)
        {
            
            if (pwd.ChkPassword == pwd.NewPassword)
            {
                var username = Request.HttpContext.User.Identity.Name;
                var user= mdb.Queryable<Users>().FirstOrDefault(x => x.user_id == username);
                if (user != null)
                {
                    user.password = MdbProcess.GetMd5HashStr(pwd.NewPassword);
                    mdb.Update(user);
                    ViewBag.ret=1;
                }
            }
            return View();
        }

    }
}