using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlTools;
using System.Collections;
using FrameSystemInfo;

namespace CarManage.Model
{
    public class Modeldata
    {
        public int id = 0;
        public string ctr="";
        public string act = "";
        public string order = "";
        public int desc = 1;
        public int page = 1;
        public TableInfo ti=null;
        public List<TableColumn> _tabcollist = null;    
        public ArrayList list = new System.Collections.ArrayList();
        public object obj = null;
        public int TotalRecord = 0;
        public int TotalPage = 0;
        public string tmp = "";
        public string tmp1 = "";
        public int disptype = 0;//0列表 disp,1编辑 dispsore
        public string username = "";

        public List<TableColumn> tabcollist {
            get
            {
                if (_tabcollist == null)
                {
                    List<TableColumn> _list = new List<TableColumn>();
                    if (ti != null)
                    {
                        foreach (var k in ti.tcolumn)
                        {
                            _list.Add(k.Value);
                        }
                        if(disptype==0)
                        _tabcollist = _list.OrderBy(x => x.disp).ToList();
                        else
                            _tabcollist = _list.OrderBy(x => x.dispsore).ToList();
                    }
                }

                return _tabcollist;
            }
            set
            {
                _tabcollist = value;
            }
        } 
        public string Convert(object obj1)
        {
            string str="";
            if (obj1 == null)
                return "";
            switch(obj1.GetType().Name)
            {
                case "DateTime":
                    DateTime dat = (DateTime)obj1;
                    str = dat.Year.ToString()+"-"+dat.Month.ToString()+"-"+dat.Day.ToString();
                    break;
               case "Decimal":
                    decimal num=(decimal)obj1;
                    str = num.ToString("#0.00");
                    break;
                default:
                    str = obj1.ToString()??"";
                    break;

            }
            return str;
        }
        public string Cname
        {
            get
            {
                string s;
                FrameSysteminfo.Instance.menudic.TryGetValue(ctr, out s);
                return s;
            }
        }

        
    }
    class chart
    {
        public string cdate { get; set; } = "";
        public int sl  { get; set; } = 0;
    }
}
