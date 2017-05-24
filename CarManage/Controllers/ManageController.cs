using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlTools;
using CarManage.Model;
using Entitys;
using Microsoft.AspNetCore.Authorization;

namespace CarManage.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {

        private readonly MSqlClient mdb;
        public ManageController(MSqlClient mdao)
        {
            mdb = mdao;
        }
        // GET: Manage
        public ActionResult Index()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb1 = new StringBuilder();
            var d = new Modeldata();
            d.username =Request.HttpContext.User.Identity.Name;
            //select convert(varchar(10),CreateTime,120) as cdate ,count(*) as sl from CarDetialInfo where CreateTime >dateadd(DAY,-30,getdate()) group by convert(varchar(10),CreateTime,120)
            var ls = mdb.SqlQuery<chart>("select convert(varchar(10),CreateTime,120) as cdate ,count(*) as sl from CarDetialInfo where CreateTime >dateadd(DAY,-30,getdate()) group by convert(varchar(10),CreateTime,120)");
            if (ls != null)
            {
                foreach (var v in ls)
                {
                    sb.Append($"\"{v.cdate}\",");
                    sb1.Append($"{v.sl},");
                }
                if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
                if (sb1.Length > 0) sb1.Remove(sb1.Length - 1, 1);
            }
            d.tmp ="["+ sb.ToString()+"]";
            d.tmp1 ="["+ sb1.ToString()+"]";
           
            return View(d);
        }

        public JsonResult getuserdata(string id)
        {
            string ids = id == null ? "" : id;
            int i;
            int.TryParse(ids, out i);
            StringBuilder sb = new StringBuilder();
            DateTime date = DateTime.Now.AddDays(0 - i);
            DateTime cdate = new DateTime(date.Year, date.Month, date.Day);
            var lss = mdb.Queryable<ShopInfo>().Where(x => x.CreateTime > cdate).ToList();
            foreach (var v in lss)
            {
                sb.Append($"{{\"CompanyName\":\"{v.CompanyName}\",\"ShopNum\":\"{v.ShopNum}\",\"PhoneShopNum\":\"{v.PhoneShopNum}\",   \"PhoneNumber\":\"{v.PhoneNumber}\", \"CompanyAddress\":\"{v.CompanyAddress}\"}},");               
            }
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return Json("["+sb.ToString()+"]");
        }




    }
}