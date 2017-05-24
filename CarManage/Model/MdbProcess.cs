using Entitys;
using FrameSystemInfo;
using Microsoft.AspNetCore.Mvc;
using SqlTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CarManage.Model
{
    public class MdbProcess
    {
        public static List<T> getdb<T>(MSqlClient db, string dbname, string where, int page, out int TotalRecord, out int TotalPage, string order = "id", int orderby = 1, int pagecount = 25)
        {
            List<T> ls = null;
            TotalRecord = 0;
            TotalPage = 0;
            IDbDataParameter[] dParameters = new SqlParameter[9];
            dParameters[0] = new SqlParameter("@TableName", dbname);
            dParameters[1] = new SqlParameter("@ReturnFields", "*");
            dParameters[2] = new SqlParameter("@PageSize", pagecount);
            dParameters[3] = new SqlParameter("@PageIndex", page);
            dParameters[4] = new SqlParameter("@Where", where);
            dParameters[5] = new SqlParameter("@Orderfld", order);
            dParameters[6] = new SqlParameter("@OrderType", orderby);
            dParameters[7] = new SqlParameter("@TotalRecord", SqlDbType.Int);
            dParameters[8] = new SqlParameter("@TotalPage", SqlDbType.Int);
            dParameters[7].Direction = System.Data.ParameterDirection.Output;
            dParameters[8].Direction = System.Data.ParameterDirection.Output;
            dParameters[7].Value = 0;
            dParameters[8].Value = 0;
            db.isClearParameters = false;
           
            ls = db.SqlQuery<T>("exec SupesoftPage @TableName,@ReturnFields,@PageSize,@PageIndex,@Where,@Orderfld,@OrderType,@TotalRecord  output,@TotalPage  output", dParameters);
            if (dParameters[7].Value != null)
                TotalPage = (int)dParameters[8].Value;
            TotalRecord = (int)dParameters[7].Value;
            return ls;
        }
        public static Modeldata List<T>(MSqlClient mdb, Controller _this)
        {
            var d = new Modeldata();
            d.username = _this.Request.HttpContext.User.Identity.Name;
            d.ti = PropertyManager.Instance.GetClassInfo(typeof(T));
            //d.ctl=this.
            d.ctr = _this.ControllerContext.RouteData.Values["controller"].ToString();
            d.act = _this.ControllerContext.RouteData.Values["action"].ToString();
            var page = _this.Request.Query["p"].ToString();
            if (string.IsNullOrEmpty(page)) page = "1";
            int.TryParse(page, out d.page);
            var orderby = _this.Request.Query["d"].ToString();
            if (string.IsNullOrEmpty(orderby)) orderby = "1";
            int.TryParse(orderby, out d.desc);
            d.order = _this.Request.Query["o"].ToString();
            if (string.IsNullOrEmpty(d.order)) d.order = "id";

            var data = MdbProcess.getdb<T>(mdb, d.ti.Tablename, "", d.page, out d.TotalRecord, out d.TotalPage, d.order, d.desc, 25);            
            foreach (T v in data)
            {
                d.list.Add(v);
            }
            return d;
        }
        public static Modeldata Edit<T>(MSqlClient mdb,int id, Controller _this) where T : new()
        {
            var d = new Modeldata();
            d.username = _this.Request.HttpContext.User.Identity.Name;
            d.ti = PropertyManager.Instance.GetClassInfo(typeof(T));
            d.ctr = _this.ControllerContext.RouteData.Values["controller"].ToString();
            d.act = _this.ControllerContext.RouteData.Values["action"].ToString();
            d.obj = mdb.Queryable<T>().Where("id=@id", new { id=id }).FirstOrDefault();
            if (d.obj == null)
                d.obj = new T();
            d.id = id;
            d.disptype = 1;
            d.tmp = _this.Request.Headers["Referer"].ToString();
            return d;
        }
        public static void EditPost<T>(MSqlClient mdb,int id,  T cinfo,Controller _this) where T :class, new()
        {
            TableInfo ti = PropertyManager.Instance.GetClassInfo(typeof(T));
            TableColumn tc;
            ti.tcolumn.TryGetValue(ti.keyname, out tc);
            var ids = (int)tc.GetValue(cinfo);
            if (ids == 0)
                mdb.Insert(cinfo);
            else
                mdb.Update(cinfo);
            var tmp = _this.Request.Form["returl"].ToString();
            _this.Redirect(tmp);
        }
        public static string Delete<T>(MSqlClient mdb,int id)
        {
            var str = mdb.Delete<T>(" id=" + id) ? "ok" : "no";
            return str;
        }
        /// <summary>
        /// MD5(32位加密)
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>MD5加密后的字符串</returns>
        public static string GetMd5HashStr(string str)
        {
            string pwd = string.Empty;

            //实例化一个md5对像
            MD5 md5 = MD5.Create();
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X");
            }
            return pwd;
        }
    }
}
