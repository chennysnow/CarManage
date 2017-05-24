using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlTools;
using CarManage.Model;
using Entitys;
using FrameSystemInfo;
using Microsoft.AspNetCore.Authorization;

namespace CarManage.Controllers
{
    [Authorize]
    public class CarDetialInfoController : Controller
    {
        private readonly MSqlClient mdb;
        public CarDetialInfoController(MSqlClient mdao)
        {
            mdb = mdao;
        }

        // GET: CarDetialInfo

        public ActionResult Index()
        {
            var d=MdbProcess.List<CarDetialInfo>(mdb, this);           
            return View("~/views/shared/_index.cshtml",d);
        }
        // GET: CarDetialInfo/Edit/5
        public ActionResult Edit(int id)
        {
             var d = MdbProcess.Edit<CarDetialInfo>(mdb, id, this);
            if (d.list != null)
            {
                var ls = FrameSysteminfo.Instance.CarTypeInfols;
                foreach (CarDetialInfo v in d.list)
                {
                    var val = ls.FirstOrDefault(x => v.CarType == v.CarType);
                    if (val != null)
                        v.CarType = val.DisplayName;
                    d.list.Add(v);
                }
            }
            return View("~/views/shared/_manage.cshtml", d);
        }

        // POST: CarDetialInfo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CarDetialInfo cinfo, IFormCollection collection)
        {
            try
            {
                MdbProcess.EditPost<CarDetialInfo>(mdb, id, cinfo, this);
                var tmp = this.Request.Headers["Referer"].ToString();
                return Redirect(tmp);
            }
            catch
            {
                return View();
            }
        }

        // GET: CarDetialInfo/Delete/5
        public JsonResult Delete(int id)
        {
            var str = MdbProcess.Delete<CarDetialInfo>(mdb, id);
            return Json("{\"ret\":\""+str+"\"}");
        }        
    }
}