using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlTools
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TableAttribute : Attribute
    {
        private string _Name = string.Empty;
        private string _Nameview = string.Empty;
        private int _OrderType = 1;
        private string _order = " id ";
        public TableAttribute() { }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Nameview
        {
            get { return _Nameview; }
            set { _Nameview = value; }
        }
        public string order
        {
            get { return _order; }
            set { _order = value; }
        }
        public int OrderType
        {
            get { return _OrderType; }
            set { _OrderType = value; }
        }
    }


   
    /// <summary>
    /// Text="A:A/B:B/C:C",lx大于0 ，则在Mtxt中id=lx的记录中找相应的数据name为db:sql
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
      AllowMultiple = false, Inherited = false)]
    public class SelectDataAttribute : Attribute
    {
        private int lx { get; set; } = 0;

        public string Text { get; set; } = "";
        
        public string Name { get; set; } = "";
        
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false, Inherited = false)]
    public class IdAttribute : Attribute
    {
        private string _Name = string.Empty;
        private int _Strategy = GenerationType.INDENTITY;
        private string _Ctype = string.Empty; //列类型
        private string _Cn = string.Empty; //列名
        private int _disp = 0;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Cn
        {
            get { return _Cn; }
            set { _Cn = value; }
        }
        public int Strategy
        {
            get { return _Strategy; }
            set { _Strategy = value; }
        }
        public string Ctype
        {
            get { return _Ctype; }
            set { _Ctype = value; }
        }
        public int disp
        {
            get { return _disp; }
            set { _disp = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
       AllowMultiple = false, Inherited = false)]
    public class ColumnAttribute : Attribute
    {
        private string _Name = string.Empty; //列名
        private string _Cfname = string.Empty; //字段别名
        private string _Ctype = string.Empty; //类型
        private string _Cn = string.Empty; //列中文
        private string _Tname = string.Empty; //字段名
        private bool _IsUnique = false; //是否唯一
        private bool _Iscolumn = true; //是否是表中的列,false 在读取时有用,在写时不写入表.
        private string _lx = string.Empty; //列类型

        public ColumnAttribute() { }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public int Size { get; set; } = 0;
        public string Tname
        {
            get { return _Tname; }
            set { _Tname = value; }
        }
        public string Cfname
        {
            get { return _Cfname; }
            set { _Cfname = value; }
        }
        public string Ctype
        {
            get { return _Ctype; }
            set { _Ctype = value; }
        }
        public string Cn
        {
            get { return _Cn; }
            set { _Cn = value; }
        }
       
        public bool IsUnique
        {
            get { return _IsUnique; }
            set { _IsUnique = value; }
        }
        public bool Iscolumn
        {
            get { return _Iscolumn; }
            set { _Iscolumn = value; }
        }
        private int _tmplx = 0;//临时用的参数
        public int tmplx
        {
            get { return _tmplx; }
            set { _tmplx = value; }
        }

        public string Lx
        {
            get { return _lx; }
            set { _lx = value; }
        }
        private int _width = 50; //列类型
        public int width
        {
            get { return _width; }
            set { _width = value; }
        }
        private string _tmp = ""; 
        public string Tmp
        {
            get { return _tmp; }
            set { _tmp = value; }
        }

        public int dispsore { get; set; } = 0;
        public int findzd { get; set; } = 0;
        public int lists { get; set; } = 0;
        public string acturl { get; set; } = "";
        public int disp { get; set; } = 0;
        public int   isread { get; set; } = 0;
        public string text { get; set; } = "";
        public string zurl { get; set; } = "";
    }


    public class GenerationType
    {
        public const int INDENTITY = 1;//自动增长
        public const int SEQUENCE = 2;//序列
        public const int TABLE = 3;//TABLE

        private GenerationType() { }//私有构造函数，不可被实例化对象
    }

}
