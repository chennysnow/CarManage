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
    public class ShopInfoController : Controller
    {
        private readonly MSqlClient mdb;
        public ShopInfoController(MSqlClient mdao)
        {
            mdb = mdao;
        }
        public ActionResult Index()
        {
            var d = MdbProcess.List<ShopInfo>(mdb, this);
            return View("~/views/shared/_index.cshtml", d);
        }
        public ActionResult Edit(int id)
        {
            var d = MdbProcess.Edit<ShopInfo>(mdb, id, this);
            return View("~/views/shared/_manage.cshtml", d);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ShopInfo cinfo, IFormCollection collection)
        {
            try
            {
                MdbProcess.EditPost<ShopInfo>(mdb, id, cinfo, this);
                var tmp = this.Request.Headers["Referer"].ToString();
                return Redirect(tmp);
            }
            catch
            {
                return View();
            }
        }
        public JsonResult Delete(int id)
        {
            var str = MdbProcess.Delete<ShopInfo>(mdb, id);
            return Json("{\"ret\":\"" + str + "\"}");
        }
    }
}