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
    public class CarTypeInfoController : Controller
    {
        private readonly MSqlClient mdb;
        public CarTypeInfoController(MSqlClient mdao)
        {
            mdb = mdao;
        }
        public ActionResult Index()
        {
            var d = MdbProcess.List<CarTypeInfo>(mdb, this);
            return View("~/views/shared/_index.cshtml", d);
        }
        public ActionResult Edit(int id)
        {
            var d = MdbProcess.Edit<CarTypeInfo>(mdb, id, this);
            return View("~/views/shared/_manage.cshtml", d);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CarTypeInfo cinfo, IFormCollection collection)
        {
            try
            {
                MdbProcess.EditPost<CarTypeInfo>(mdb, id, cinfo, this);
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
            var str = MdbProcess.Delete<CarTypeInfo>(mdb, id);
            return Json("{\"ret\":\"" + str + "\"}");
        }
    }
}