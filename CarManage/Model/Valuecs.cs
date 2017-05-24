using Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSystem.Model
{
    public class Valuecs
    {
        public string title = "";
        public List<CarTypeInfo> cartype = new List<CarTypeInfo>();
        public List<CarTypeInfo> bsqtype = new List<CarTypeInfo>();
        public List<BandInfo>  bandinfo = new List<BandInfo>();
        public List<List<CarDetialInfo>> Cardetialinfo = new List<List<CarDetialInfo>>();
        public string dicval ="";
        public string order = "";
        public string displist = "0";
    }
    public class Jsonvlaue
    {
        public string Name = "";
        public string Value = "";
    }
}
