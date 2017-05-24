using SqlTools;
using System;

namespace Entitys
{
    public class CarTypeInfo
    {
        [Id(Name = "Id")]
        public int Id { get; set; }

        [Column(Cn = "显示名称", disp = 1, width = 20, dispsore = 1)]
        public string DisplayName { get; set; }

        [Column(Cn = "汽车类型", disp = 2, width = 20, dispsore = 2)]
        public string CarTypeKey { get; set; }

        [Column(Cn = "父类型", disp = 3, width = 10, dispsore = 3)]
        public string ParentDisplayName { get; set; }

        [Column(Cn = "父类型编码", disp = 4, width = 10, dispsore = 4)]
        public string ParentCarTypeKey { get; set; }
    }
}
