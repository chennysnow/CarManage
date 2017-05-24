using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Data;
using System.Data.SqlClient;
using SqlTools;
namespace SqlTools
{
    public class PropertyManager
    {     
        private Dictionary<string, TableInfo> dic = new Dictionary<string, TableInfo>();
        public Dictionary<string, string> keydic = new Dictionary<string, string>();
        public Dictionary<string, List<string>> Primarykey = new Dictionary<string, List<string>>();
        public Dictionary<string, SysZd> syszd = new Dictionary<string, SysZd>();
        private static PropertyManager Sqldata = null;
        public bool islock = false;
        public string Dbtype = "";
        private static readonly Lazy<PropertyManager> instance = new Lazy<PropertyManager>(() => new PropertyManager());
        public PropertyManager()
        {
            SqlBase.GetIdentitiesKeyByTableName(keydic);
            SqlBase.GetPrimaryKeyByTableName(Primarykey);            
        }     
        public static PropertyManager Instance
        {
            get
            {
                return instance.Value;
            }
        }

        public TableInfo GetTinfo(string name)
        {
            TableInfo ti = null;
            dic.TryGetValue(name, out ti);
            return ti;
        }

        public void AddPrimaryKey( string key ,string val)
        {
            List<string> ls;
            string s="";
            lock (Primarykey)
            {
                if(Primarykey.TryGetValue(key,out ls))
                {
                    if (ls.IndexOf(val) < 0)
                        ls.Add(val);
                }else
                {
                    ls = new List<string>();
                    ls.Add(val);
                    Primarykey.Add(key, ls);
                }
            }
        }
        public string GetPrimaryKey(string key)
        {
            List<string> ls;
            if (string.IsNullOrEmpty(key)) return "";
            string s = "";
            lock (Primarykey)
            {
                if (Primarykey.TryGetValue(key, out ls))
                {
                    if (ls.Count > 0)
                        s=ls[0];
                }               
            }
            return s;
        }
        public void AddKey(string k,string v)
        {
            string s = "";
            lock (keydic)
            {
                if(keydic.TryGetValue(k , out s)==false)
                {
                    keydic.Add(k, v);
                }
            }
        }
        public string GetIdentitiesKey(string key)
        {
            string s = "";
            if (string.IsNullOrEmpty(key)) return s;
            keydic.TryGetValue(key, out s);
            return s;                
        }


        #region 反射类

        public static string GetTableName(Type classType, out string nameview, out string order, out int OrderType)
        {
            string strTableName = string.Empty;
            string strEntityName = string.Empty;
            nameview = "";
            strEntityName = classType.FullName;
            order = " id ";
            OrderType = 1;
            var ObjAttr = classType.GetTypeInfo().GetCustomAttributes(false);
            object classAttr;
            if (ObjAttr.Count()> 0)
            {
                classAttr = ObjAttr.First();
                if (classAttr is TableAttribute)
                {
                    TableAttribute tableAttr = classAttr as TableAttribute;
                    strTableName = tableAttr.Name;
                    nameview = tableAttr.Nameview;
                    order = tableAttr.order;
                    OrderType = tableAttr.OrderType;
                }
                if (string.IsNullOrEmpty(strTableName))
                {
                   // throw new Exception("实体类:" + strEntityName + "的属性配置[Table(name=\"tablename\")]错误或未配置");
                }
            }
            else
            {
                strTableName = classType.Name;
                nameview = classType.Name;
                order = "";
                OrderType = 0;
            }

            return strTableName;
        }

        public string GetColumnName(object attribute)
        {
            string columnName = string.Empty;
            if (attribute is ColumnAttribute)
            {
                ColumnAttribute columnAttr = attribute as ColumnAttribute;
                columnName = columnAttr.Name;
            }
            if (attribute is IdAttribute)
            {
                IdAttribute idAttr = attribute as IdAttribute;
                columnName = idAttr.Name;
            }

            return columnName;
        }
        public TableInfo SetTableInfo(Type type)
        {
            string columnName = string.Empty;
            TableInfo dt = new TableInfo();
            TableColumn tc = null;
            //  WebDispAttribute wd = null;
            SysZd szd;
            dt.type = type;
            int itp = 0;
            string keyname = "";
            string nameview, order;
            int ordert = 1;
            // PropertyInfo remark = null;
            //获取属性信息数组
            dt.tablename = GetTableName(type, out nameview, out order, out ordert);//获取表名
            dt.tableview = nameview;
            PropertyInfo[] properties1 = null;
            List<PropertyInfo> propList = new List<PropertyInfo>();
            List<PropertyInfo> dispList = new List<PropertyInfo>();
            //////////////////获取基类////////////////////////////
            if (type.GetTypeInfo().BaseType != null)
            {
                properties1 = type.GetTypeInfo().BaseType.GetProperties();
                foreach (PropertyInfo info in properties1)
                {
                    propList.Add(info);
                }
            }

            PropertyInfo[] properties = type.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo info in properties)
            {
                if (info.DeclaringType == type)
                    propList.Add(info);
            }

            foreach (PropertyInfo property in propList)
            {
                tc = new TableColumn();
                //  tc.CName = property.Name.ToLower();
                tc.property = property;
                // tc.type=property.              
                IdAttribute idAttr = null;
                ColumnAttribute columnattr = null;
                tc.Name = property.Name;
                if(syszd.TryGetValue(tc.Name,out szd))
                {
                    tc.CName = szd.name;
                    tc.Name = property.Name;
                    tc.TName= szd.field;
                    tc.Iscolumn = true;
                    tc.Ctype = szd.TYPE;
                    tc.Lx = szd.isdisp.ToString();
                    tc.Size = szd.leng;
                    tc.dispsore = szd.dispsort;
                }
                else
                {
                    tc.CName = property.Name;
                    tc.Name = property.Name;
                    tc.Iscolumn = true;
                    tc.Ctype = Ctotype(property, out itp);
                    tc.Lx = itp.ToString();
                    tc.Size = 10;
                }

                tc.SetValue= DynamicMethodFactory.CreatePropertySetter(property);
                tc.GetValue= DynamicMethodFactory.CreatePropertyGetter(property);
                //获取实体对象属性自定义属性数组(如Column、Id、Table)   
                var propertyAttrs = property.GetCustomAttributes(false);
                foreach (var propertyAttr in propertyAttrs)
                {
                    // object propertyAttr = propertyAttrs[i];
                    //获取Column自定义属性中配置的Name值(表的列名)
                    string tempName = GetColumnName(propertyAttr);
                    if (propertyAttr is IdAttribute)
                    {
                        idAttr = propertyAttr as IdAttribute;
                        if(!string.IsNullOrEmpty(idAttr.Name))
                            tc.Name = idAttr.Name;
                        tc.IsKey = true;
                        if (idAttr.Ctype != "")
                            tc.Ctype = idAttr.Ctype;
                        keyname = tc.Name;
                    }
                    if (propertyAttr is ColumnAttribute)
                    {
                        columnattr = propertyAttr as ColumnAttribute;
                        if (columnattr.Tname != "")
                            tc.TName = columnattr.Tname;
                        tc.Iscolumn = columnattr.Iscolumn;
                        if (columnattr.Ctype != "")
                            tc.Ctype = columnattr.Ctype;
                        tc.IsUnique = columnattr.IsUnique;
                        if (columnattr.width > 0)
                            tc.width = columnattr.width;
                        if (!string.IsNullOrEmpty(columnattr.Tmp))
                            tc.Tmp = columnattr.Tmp;
                        if (!string.IsNullOrEmpty(columnattr.Cfname))
                            tc.Cfame = columnattr.Cfname;
                        if (!string.IsNullOrEmpty(columnattr.Lx))
                            tc.Lx = columnattr.Lx;
                        if (!string.IsNullOrEmpty(columnattr.Cn))
                            tc.CName = columnattr.Cn;
                        if (!string.IsNullOrEmpty(columnattr.Tname))
                            tc.SelectData = columnattr.Tname;
                        tc.dispsore = columnattr.dispsore;
                        tc.findzd = columnattr.findzd;
                        tc.lists = columnattr.lists;
                        tc.acturl = columnattr.acturl;
                        tc.disp = columnattr.disp;
                        tc.isread = columnattr.isread;
                        tc.text = columnattr.text;
                        tc.zurl = columnattr.zurl;
                        tc.Size = columnattr.Size;
                        tc.tmplx = columnattr.tmplx;
                        
                    }
                    if(propertyAttr is SelectDataAttribute)
                    {
                       var Selumnattr = propertyAttr as SelectDataAttribute;
                        if (Selumnattr.Name.IndexOf("DB:") >= 0)
                            tc.SelectData = SqlBase.GetSelectData(Selumnattr.Name);
                        else
                            tc.SelectData = Selumnattr.Text;
                    }
                }
                if (tc.Name != "")
                {
                    dt.tcolumn.Add(tc.Name, tc);
                    dt.keyname = keyname;                   
                }
            }
            if (string.IsNullOrEmpty(dt.keyname))
            {
                keyname=GetIdentitiesKey(dt.tablename);
                if(!string.IsNullOrEmpty(keyname))
                    dt.keyname = keyname;               
            }
            if (string.IsNullOrEmpty(dt.PrimaryKey))
            {
                keyname = GetPrimaryKey(dt.tablename);
                if (!string.IsNullOrEmpty(keyname))
                    dt.PrimaryKey = keyname;
            }
            return dt;
        }

        private string Ctotype(PropertyInfo p, out int l)
        {
            string s = "";
            l = 0;
            switch (p.PropertyType.Name.ToLower())
            {
                case "sbyte":
                case "byte":
                case "short":
                case "ushort":
                case "int":
                case "int32":
                case "int16":
                case "int64":
                case "uint":
                case "long":
                case "ulong":
                    s = "int";
                    l = 2;
                    break;
                case "string":
                    s = "str";
                    l = 1;
                    break;
                case "float":
                case "double":
                case "decimal":
                    s = "dec";
                    l = 1;
                    break;
                case "datetime":
                    s = "date";
                    l = 3;
                    break;
            }

            return s;
        }

        public TableInfo GetClassInfo(Type type)
        {
            TableInfo tinfo,tfi;
            lock (dic)
            {
                if (dic.TryGetValue(type.Name, out tinfo) == false)
                {
                    tinfo = SetTableInfo(type);
                    if (string.IsNullOrEmpty(type.Name) == false) 
                    {
                        if (dic.TryGetValue(type.Name, out tfi) == false)
                            dic.Add(type.Name, tinfo);
                    }
                }
            }
            return tinfo;
        }
        public TableInfo GetClassInfo(object dbbase)
        {
            Type type = dbbase.GetType();
            TableInfo tinfo= GetClassInfo(type);
           
            return tinfo;
        }
        /// <summary>
        /// 返回一个类的实例中的成员的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public string Getobjdata<T>(T obj,string str)
        {
            string res = "";
            Type type = typeof(T);
            TableInfo ti = GetClassInfo(type);
            TableColumn tc;
            if (ti.tcolumn.TryGetValue(str,out tc) == true)
            {
                res = tc.GetValue(obj).ToString();
            }
            return res;
        } 
        #endregion



        public IDbDataParameter CreateParameter()
        {
            IDbDataParameter idap = null;
            switch (Dbtype)
            {
                case "mysql":
                    break;
                default:
                    idap = new SqlParameter();
                    return idap;
            }
            return null;
        }

    }


    public class TableInfo
    {
        public Dictionary<string, TableColumn> tcolumn = new Dictionary<string, TableColumn>();
        public object Idreb = null;
        private string _tablename = string.Empty;
        private string _tableview = string.Empty;
        private string _keyname = string.Empty;
        public string PrimaryKey { set; get; } = "";
        public TableInfo()
        {
            type = null;
        }

        public string tablename
        {
            get { return _tablename; }
            set { _tablename = value; }
        }

        public string tableview
        {
            get { return _tableview; }
            set { _tableview = value; }
        }
        public string keyname
        {
            get { return _keyname; }
            set { _keyname = value; }
        }
        public string Tablename
        {
            get { return string.IsNullOrEmpty(_tableview) == false ? _tableview : _tablename; }
        }

        public Type type
        {
            get; set;
        }


    }
    public class TableColumn
    {
        private string _Name = string.Empty; //列名
        private bool _IsKey = false; //是否ID
        private bool _Iscolumn = true; //是否是表中的列,false 在读取时有用,在写时不写入表.
        private string _CName = string.Empty;
        private string _TName = string.Empty;//字段名
        private string _Ctype = string.Empty;
        private string _lx = string.Empty;
        private string _type = "";
        private string _Cfname = string.Empty;

        public TableColumn() { }

        public SetValueDelegate SetValue = null;
        public GetValueDelegate GetValue = null;
        public PropertyInfo property = null;

        //   GetPropertyValue<T> result;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Cfame
        {
            get { return _Cfname; }
            set { _Cfname = value; }
        }
        public bool Iscolumn
        {
            get { return _Iscolumn; }
            set { _Iscolumn = value; }
        }
        public bool IsKey
        {
            get { return _IsKey; }
            set { _IsKey = value; }
        }
        public string CName
        {
            get { return _CName; }
            set { _CName = value; }
        }
        public string TName
        {
            get { return _TName; }
            set { _TName = value; }
        }
        public string type
        {
            get { return _type; }
            set { _type = value; }
        }
        public string Ctype
        {
            get { return _Ctype; }
            set { _Ctype = value; }
        }
        private bool _IsUnique = false; //是否唯一
        public bool IsUnique
        {
            get { return _IsUnique; }
            set { _IsUnique = value; }
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
        public string Lx
        {
            get { return _lx; }
            set { _lx = value; }
        }
        public int Size { get; set; } = 0;

        public string SelectData { get; set; } = "";
      
        public int dispsore { get; set; } = 0;
        public int findzd { get; set; } = 0;
        public int lists { get; set; } = 0;
        public string acturl { get; set; } = "";
        public int disp { get; set; } = 0;
        public int isread { get; set; } = 0;
        public string  text { get; set; } = "";
        public string  zurl { get; set; } = "";
        public int  tmplx { get; set; } = 0;

        public void Copydata(TableColumn tc)
        {
            this._Name = tc.Name;
            this._IsKey = tc.IsKey;
            this._Iscolumn = tc.Iscolumn;
            this._CName = tc._CName;
            this._Ctype = tc._Ctype;
            this._lx = tc._lx;
        }      
       
    }
  

}
