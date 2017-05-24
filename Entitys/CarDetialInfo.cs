using System;
using SqlTools;
namespace Entitys
{//编号 品牌型号    车型 上牌日期    颜色 交易报价    变速器 发布日期    编码
    public class CarDetialInfo
    {
        [Id(Name = "Id")]

        public int Id { get; set; } = 0;

        [Column(Cn = "品牌型号", disp = 2, width = 20, dispsore = 1)]
        public string ProTitle { get; set; } = "";

        [Column(Cn = "编码",disp =9, Lx = "d", width = 13, dispsore = 2,Tmp ="输入短号编码")]
        public string ProNum { get; set; } = "";

        [Column(Cn = "报价(万)",disp =6, Lx = "d", width = 8, dispsore = 3)]
        public decimal baojia { get; set; } = 0;
        [Column(Cn = "车型", disp =4, Lx = "d", width = 6, dispsore = 4)]
        [SelectData(Name = "DB:select CarTypeKey,DisplayName from CarTypeInfo where ParentCarTypeKey='0'")]
        public string CarType { get; set; } = "";
        [Column(Cn = "品牌信息", dispsore = 5)]
        public string BrandInfo { get; set; } = "";
        [Column(Cn = "品牌信息key", dispsore = 6)]
        public string BrandInfoKey { get; set; } = "";
        [Column(Cn = "品牌类别", dispsore = 7)]
        public string BrandType { get; set; } = "";
        [Column(Cn = "品牌类别Key", dispsore = 8)]
        public string BrandTypeKey { get; set; } = "";
        [Column(Cn = "其它数据", dispsore = 9)]
        public string OtherParam { get; set; } = "";
        [Column(Cn = "颜色", disp = 5, width = 10, dispsore = 10)]
        public string CarColor { get; set; } = "";
        [Column(Cn = "排量", dispsore = 11)]
        public string PaiLiang { get; set; } = "";
        [Column(Cn = "国产进口", dispsore = 12)]

        public string country { get; set; } = "";
        [Column(Cn = "变速器", disp = 7, Lx = "d", width = 8, dispsore = 13)]
        [SelectData(Name = "DB:select CarTypeKey,DisplayName from CarTypeInfo where ParentCarTypeKey='1'")]
        public string BianShuQi { get; set; } = "";
        [Column(Cn = "里程", dispsore = 14)]
        public string LiCheng { get; set; } = "";
        [Column(Cn = "排放标准", dispsore = 15)]
        public string PaiFangBiaoZhun { get; set; } = "";
        [Column(Cn = "燃油", dispsore = 16)]
        public string RanYou { get; set; } = "";
        [Column(Cn = "备注", dispsore = 27,Size =100)]
        public string Remark { get; set; } = "";
        [Column(Cn = "图片", dispsore = 18,Size =100)]//大于等于100用textarea
        public string Images { get; set; } = "";
        [Column(Cn = "主图", dispsore = 19)]
        public string mianimg { get; set; } = "";
        [Column(Cn = "上牌日期", disp = 4, Lx = "d", width = 8, dispsore = 20)]
        public string ShangPaiTime { get; set; }
        [Column(Cn = "上牌年份", dispsore = 21)]
        public int ShangPaiYear { get; set; } = 0;
        [Column(Cn = "上牌月份", dispsore = 22)]
        public int ShangPaiMonth { get; set; } = 0;
        [Column(Cn = "发布日期", disp = 8, Lx = "d",Ctype ="dat", width = 8, dispsore = 23, Tmp = "输入格式yyyy-mm-dd")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public string SellerName { get; set; } = "";
        [Column(Cn = "编号", disp =1, Lx = "d", width = 4, dispsore = 24)]
        public string SellerNumber { get; set; } = "";
        [Column(Cn = "联系电话", dispsore = 25)]
        public string SellerPhone { get; set; } = "";
        [Column(Cn = "公司地址", dispsore = 26)]
        public string CarSellAddress { get; set; } = "";

    }
}
