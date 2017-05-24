using SqlTools;
using System;

namespace Entitys
{
    public class ShopInfo
    {
        [Id(Name = "Id")]
        public int Id { get; set; }

        [Column(Cn = "公司名称", disp = 2, width = 20, dispsore = 1)]
        public string CompanyName { get; set; }

        [Column(Cn = "公司地址",  width = 15, dispsore = 2)]
        public string CompanyAddress { get; set; }

        [Column(Cn = "电话号码", disp = 3, width = 6, dispsore = 3)]
        public string PhoneNumber { get; set; }

        [Column(Cn = "身份证", disp = 4, width = 4, dispsore = 5,tmplx =1)]//tmplx =1是图片
        public string IdCart { get; set; }

        [Column(Cn = "营业执照", disp = 5, width = 4, dispsore = 6, tmplx = 1)]
        public string BusinessLicense { get; set; }

        [Column(Cn = "用户名", disp = 1, width = 6, dispsore = 7)]
        public string ShopNum { get; set; }

        [Column(Cn = "密码",  width = 10, dispsore = 8)]
        public string ShopPwd { get; set; }

        [Column(Cn = "短号", disp = 7, width = 10, dispsore = 9)]
        public string PhoneShopNum { get; set; }

        [Column(Cn = "是否校验", disp = 8, width = 10, dispsore = 10,Tmp ="1有效;0无效")]  
       // [SelectData(Text ="0:否;1:是")]
        public int start { get; set; }

        [Column(Cn = "积分", disp = 5, width = 10, dispsore = 11)]      
        public int jifen { get; set; }

        [Column(Cn = "创建时间", disp = 6, width = 10, dispsore = 12)]
        public DateTime CreateTime { get; set; }
    }
}
