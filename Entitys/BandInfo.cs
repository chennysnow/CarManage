using SqlTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Entitys
{
    public class BandInfo
    {
        [Id(Name = "Id")]
        public int Id { get; set; }
        /// <summary>
        /// 首字母
        /// </summary>
        [Column(Cn = "首字母", disp = 1, width = 6, dispsore = 1)]
        public string FirstChart { get; set; }

        [Column(Cn = "品牌名称", disp = 2, width = 20, dispsore = 2)]
        public string BrandName { get; set; }
        [Column(Cn = "品牌编号", disp = 3, width = 10, dispsore = 3)]
        public string BrandNum { get; set; }
        [Column(Cn = "父编号", disp = 4, width = 10, dispsore = 4)]
        public string ParentBrandNum { get; set; }
        [Column(Cn = "显示名称", disp = 5, width = 10, dispsore = 5)]
        public string DisplayName { get; set; }
        [Column(Cn = "创建时间", disp = 6, width = 10, dispsore = 6)]
        public DateTime CreateTime { get; set; }
    }
}
