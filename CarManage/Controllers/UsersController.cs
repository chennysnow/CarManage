using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlTools;
using CarManage.Model;
using Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace CarManage.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly MSqlClient mdb;
        public UsersController(MSqlClient mdao)
        {
            mdb = mdao;
        }
        public ActionResult Index()
        {
            var d = MdbProcess.List<Users>(mdb, this);
            return View("~/views/shared/_index.cshtml", d);
        }
        public ActionResult Edit(int id)
        {
            var d = MdbProcess.Edit<Users>(mdb, id, this);
            return View("~/views/shared/_manage.cshtml", d);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Users cinfo, IFormCollection collection)
        {
            try
            {
                if (cinfo.password.Length < 20)
                {
                    cinfo.password = MdbProcess.GetMd5HashStr(cinfo.password);
                }
                if (id == 0)
                {
                   cinfo.StatUs = 1;
                   cinfo.LastIP = Request.Headers["Remote_Addr"].ToString();
                   cinfo.LastDateTime = DateTime.Now;              
                }
                MdbProcess.EditPost<Users>(mdb, id, cinfo, this);
                var tmp = this.Request.Headers["Referer"].ToString();
                return Redirect(tmp);
            }
            catch(Exception ex)
            {
                return View();
            }
        }
        public JsonResult Delete(int id)
        {
            var str = MdbProcess.Delete<Users>(mdb, id);
            return Json("{\"ret\":\"" + str + "\"}");
        }
    }
}