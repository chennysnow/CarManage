using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CarManage.Model;
using Entitys;
using Microsoft.AspNetCore.Http;
using SqlTools;
using Microsoft.AspNetCore.Authorization;

namespace CarManage.Controllers
{
    [Authorize]
    public class BandInfoController : Controller
    {
        private readonly MSqlClient mdb;
        public BandInfoController(MSqlClient mdao)
        {
            mdb = mdao;
        }
        public ActionResult Index()
        {
            var d = MdbProcess.List<BandInfo>(mdb, this);
            return View("~/views/shared/_index.cshtml", d);
        }
        public ActionResult Edit(int id)
        {
            var d = MdbProcess.Edit<BandInfo>(mdb, id, this);
            return View("~/views/shared/_manage.cshtml", d);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, BandInfo cinfo, IFormCollection collection)
        {
            try
            {
                MdbProcess.EditPost<BandInfo>(mdb, id, cinfo, this);
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
            var str = MdbProcess.Delete<BandInfo>(mdb, id);
            return Json("{\"ret\":\"" + str + "\"}");
        }
    }
}