using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlTools
{

    /// <summary>
    /// 
    /// </summary>
    public class SysZd
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SysZd()
        {
        }
        public int id
        {
            get;
            set;
        }
        public string field
        {
            get;
            set;
        }
        public string name
        {
            get;
            set;
        }
        public string TYPE
        {
            get;
            set;
        }
        public int isdisp
        {
            get;
            set;
        }
        public int leng
        {
            get;
            set;
        }
        public int dispsort
        {
            get;
            set;
        }
        public void setdata(string k, string n)
        {
            switch (k)
            {
                case "id":
                    id = int.Parse(n);
                    break;
                case "field":
                    field = n;
                    break;
                case "name":
                    name = n;
                    break;
                case "TYPE":
                    TYPE = n;
                    break;
                case "isdisp":
                    isdisp = Convert.ToInt32(n);
                    break;
                case "leng":
                    leng = Convert.ToInt32(n);
                    break;
                case "dispsort":
                    dispsort = Convert.ToInt32(n);
                    break;
            }
        }
    }
}

