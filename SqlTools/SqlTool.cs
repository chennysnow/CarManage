using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace SqlTools
{
    /// <summary>
    /// ** 描述：SqlSugar工具类
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class SqlTool
    {
        internal static Type StringType = typeof(string);
        internal static Type IntType = typeof(int);
        internal static Type DecType = typeof(decimal);
        internal static Type GuidType = typeof(Guid);
        internal static Type DateType = typeof(DateTime);
        internal static Type ByteType = typeof(Byte);
        internal static Type BoolType = typeof(bool);
        internal static Type ObjType = typeof(object);
        internal static Type Dob = typeof(double);
        internal static Type DicSS = typeof(KeyValuePair<string, string>);
        internal static Type DicSi = typeof(KeyValuePair<string, int>);
        internal static Type Dicii = typeof(KeyValuePair<int, int>);
        internal static Type DicOO = typeof(KeyValuePair<object, object>);
        internal static Type DicSo = typeof(KeyValuePair<string, object>);
        internal static Type DicIS = typeof(KeyValuePair<int, string>);
        internal static Type DicArraySS = typeof(Dictionary<string, string>);
        internal static Type DicArraySO = typeof(Dictionary<string, object>);

      
        private static void FillValueTypeToDr<T>(Type type, IDataReader dr, List<T> strReval)
        {
            using (IDataReader re = dr)
            {
                while (re.Read())
                {
                    strReval.Add((T)Convert.ChangeType(re.GetValue(0), type));
                }
            }
        }       

        public static void SetParSize(IDbDataParameter par)
        {
            int size = par.Size;
            if (size < 4000)
            {
                par.Size = 4000;
            }
        }

        /// <summary>
        /// 将实体对象转换成SqlParameter[] 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDbDataParameter[] GetParameters(object obj)
        {
            List<IDbDataParameter> listParams =   new List<IDbDataParameter>();
            if (obj != null)
            {
                var type = obj.GetType();
                var isDic = type.IsIn(SqlTool.DicArraySO, SqlTool.DicArraySS);
                if (isDic)
                {
                   /* if (type == SqlTool.DicArraySO)
                    {
                        var newObj = (Dictionary<string, object>)obj;
                        var pars = newObj.Select(it => new IDbDataParameter("@" + it.Key, it.Value));
                        foreach (var par in pars)
                        {
                            SetParSize(par);
                        }
                        listParams.AddRange(pars);
                    }
                    else
                    {

                        var newObj = (Dictionary<string, string>)obj;
                        var pars = newObj.Select(it => new IDbDataParameter("@" + it.Key, it.Value));
                        foreach (var par in pars)
                        {
                            SetParSize(par);
                        }
                        listParams.AddRange(pars); ;
                    }*/
                }
                else
                {
                    TableInfo ti = PropertyManager.Instance.GetClassInfo(obj);
                    TableColumn tc;
                    IDbDataParameter idp = null;
                    foreach (var k in ti.tcolumn)
                    {
                        if (k.Value.IsKey == false)
                        {
                            if (k.Value.Iscolumn == true)
                            {
                                idp = PropertyManager.Instance.CreateParameter();
                                idp.ParameterName = "@"+k.Value.Name;
                                idp.Value = k.Value.GetValue(obj);
                                listParams.Add(idp);
                            }
                        }
                    }                    
                }
            }
            return listParams.ToArray();
        }



        /// <summary>
        /// 获取type属性cache
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cachePropertiesKey"></param>
        /// <param name="cachePropertiesManager"></param>
        /// <returns></returns>
        internal static PropertyInfo[] GetGetPropertiesByCache(Type type, string cachePropertiesKey, CacheManager<PropertyInfo[]> cachePropertiesManager)
        {
            PropertyInfo[] props = null;
            if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
            {
                props = cachePropertiesManager[cachePropertiesKey];
            }
            else
            {
                props = type.GetProperties();
                cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
            }
            return props;
        }        
       
        /// <summary>
        /// 根据表名获取列名
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal static List<string> GetColumnsByTableName(MSqlClient db, string tableName)
        {
            string key = "GetColumnNamesByTableName" + tableName;
            var cm = CacheManager<List<string>>.GetInstance();
            if (cm.ContainsKey(key))
            {
                return cm[key];
            }
            else
            {
                string sql = string.Format(SqlBase.ColumnsByTableNamesql, tableName);  // " SELECT Name FROM SysColumns WHERE id=Object_Id('" + tableName + "')";
                var reval = db.SqlQuery<string>(sql);
                cm.Add(key, reval, cm.Day);
                return reval;
            }
        }

       

        /// <summary>
        /// 处理like条件的通配符
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string SqlLikeWordEncode(string word)
        {
            if (word == null) return word;
            return Regex.Replace(word, @"(\[|\%)", "[$1]");
        }

        public static string GetLockString(bool isNoLock)
        {
            return isNoLock ? " WITH(NOLOCK) " : "";
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static Guid GetPropertyValue(object obj, string property)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return (Guid)propertyInfo.GetValue(obj, null);
        }
        /// <summary>
        /// 包装SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        internal static string PackagingSQL(string sql, string shortName)
        {
            return string.Format(" SELECT * FROM ({0}) {1} ", sql, shortName);
        }

        /// <summary>
        /// 使用页面自动填充sqlParameter时 Request.Form出现特殊字符时可以重写Request.Form方法，使用时注意加锁并且用到将该值设为null
        /// </summary>
        public static Func<string, string> SpecialRequestForm = null;

        /// <summary>
        /// 获取参数到键值集合根据页面Request参数
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetParameterDictionary(bool isNotNullAndEmpty = false)
        {
            throw new Exception("未实现");
        }

        /// <summary>
        /// 获取参数到键值集合根据页面Request参数
        /// </summary>
        /// <returns></returns>
        public static IDbDataParameter[] GetParameterArray(bool isNotNullAndEmpty = false)
        {
            throw new Exception("未实现");
        }

        internal static StringBuilder GetQueryableSql<T>(Queryable<T> queryable)
        {
            string joinInfo = string.Join(" ", queryable.JoinTable);
            StringBuilder sbSql = new StringBuilder();
            string tableName = queryable.TableName.IsNullOrEmpty() ? queryable.TName : queryable.TableName;
            if (queryable.DB.Language.IsValuable() && queryable.DB.Language.Suffix.IsValuable())
            {
                var viewNameList = LanguageHelper.GetLanguageViewNameList(queryable.DB);
                var isLanView = viewNameList.IsValuable() && viewNameList.Any(it => it == tableName);
                if (!queryable.DB.Language.Suffix.StartsWith(LanguageHelper.PreSuffix))
                {
                    queryable.DB.Language.Suffix = LanguageHelper.PreSuffix + queryable.DB.Language.Suffix;
                }

                //将视图变更为多语言的视图
                if (isLanView)
                    tableName = typeof(T).Name + queryable.DB.Language.Suffix;
            }
            if (queryable.DB.PageModel == PageModel.RowNumber)
            {
                #region  rowNumber
                string withNoLock = queryable.DB.IsNoLock ? "WITH(NOLOCK)" : null;
                var order = queryable.OrderBy.IsValuable() ? (",row_index=ROW_NUMBER() OVER(ORDER BY " + queryable.OrderBy + " )") : null;

                sbSql.AppendFormat("SELECT " + queryable.Select.GetSelectFiles() + " {1} FROM [{0}] {5} {2} WHERE 1=1 {3} {4} ", tableName, order, withNoLock, string.Join("", queryable.Where), queryable.GroupBy.GetGroupBy(), joinInfo);
                if (queryable.Skip == null && queryable.Take != null)
                {
                    if (joinInfo.IsValuable())
                    {
                        sbSql.Insert(0, "SELECT * FROM ( ");
                    }
                    else
                    {
                        sbSql.Insert(0, "SELECT " + queryable.Select.GetSelectFiles() + " FROM ( ");
                    }
                    sbSql.Append(") t WHERE t.row_index<=" + queryable.Take);
                }
                else if (queryable.Skip != null && queryable.Take == null)
                {
                    if (joinInfo.IsValuable())
                    {
                        sbSql.Insert(0, "SELECT * FROM ( ");
                    }
                    else
                    {
                        sbSql.Insert(0, "SELECT " + queryable.Select.GetSelectFiles() + " FROM ( ");
                    }
                    sbSql.Append(") t WHERE t.row_index>" + (queryable.Skip));
                }
                else if (queryable.Skip != null && queryable.Take != null)
                {
                    if (joinInfo.IsValuable())
                    {
                        sbSql.Insert(0, "SELECT * FROM ( ");
                    }
                    else
                    {
                        sbSql.Insert(0, "SELECT " + queryable.Select.GetSelectFiles() + " FROM ( ");
                    }
                    sbSql.Append(") t WHERE t.row_index BETWEEN " + (queryable.Skip + 1) + " AND " + (queryable.Skip + queryable.Take));
                }
                #endregion
            }
            else
            {

                #region offset
                string withNoLock = queryable.DB.IsNoLock ? "WITH(NOLOCK)" : null;
                var order = queryable.OrderBy.IsValuable() ? ("ORDER BY " + queryable.OrderBy + " ") : null;
                sbSql.AppendFormat("SELECT " + queryable.Select.GetSelectFiles() + " {1} FROM [{0}] {5} {2} WHERE 1=1 {3} {4} ", tableName, "", withNoLock, string.Join("", queryable.Where), queryable.GroupBy.GetGroupBy(), joinInfo);
                sbSql.Append(order);
                if (queryable.Skip != null || queryable.Take != null)
                {
                    sbSql.AppendFormat("OFFSET {0} ROW FETCH NEXT {1} ROWS ONLY", Convert.ToInt32(queryable.Skip), Convert.ToInt32(queryable.Take));
                }
                #endregion
            }
            return sbSql;
        }
        internal static void GetSqlableSql(Sqlable sqlable, string fileds, string orderByFiled, int pageIndex, int pageSize, StringBuilder sbSql)
        {
            if (sqlable.DB.PageModel == PageModel.RowNumber)
            {
                sbSql.Insert(0, string.Format("SELECT {0},row_index=ROW_NUMBER() OVER(ORDER BY {1} )", fileds, orderByFiled));
                sbSql.Append(" WHERE 1=1 ").Append(string.Join(" ", sqlable.Where));
                sbSql.Append(sqlable.OrderBy);
                sbSql.Append(sqlable.GroupBy);
                int skip = (pageIndex - 1) * pageSize + 1;
                int take = pageSize;
                sbSql.Insert(0, "SELECT * FROM ( ");
                sbSql.AppendFormat(") t WHERE  t.row_index BETWEEN {0}  AND {1}   ", skip, skip + take - 1);
            }
            else
            {
                sbSql.Insert(0, string.Format("SELECT {0}", fileds));
                sbSql.Append(" WHERE 1=1 ").Append(string.Join(" ", sqlable.Where));
                sbSql.Append(sqlable.GroupBy);
                sbSql.AppendFormat(" ORDER BY {0} ", orderByFiled);
                int skip = (pageIndex - 1) * pageSize;
                int take = pageSize;
                sbSql.AppendFormat("OFFSET {0} ROW FETCH NEXT {1} ROWS ONLY", skip, take);
            }
        }
        /// <summary>
        /// 获取最底层类型
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        internal static Type GetUnderType(PropertyInfo propertyInfo, ref bool isNullable)
        {
            Type unType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            isNullable = unType != null;
            unType = unType ?? propertyInfo.PropertyType;
            return unType;
        }
        /// <summary>
        /// Reader转成List《T》
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dr"></param>
        /// <param name="isClose"></param>
        /// <returns></returns>
        internal static List<T> DataReaderToList<T>(Type type, IDataReader dr, string fields, bool isClose = true, bool isTry = true)
        {
            if (type.Name.Contains("KeyValuePair"))
            {
                List<T> strReval = new List<T>();
                FillValueTypeToDictionary(type, dr, strReval);
                return strReval;
            }
            //值类型
            else if (type.GetTypeInfo().IsValueType || type == SqlTool.StringType)
            {
                List<T> strReval = new List<T>();
                FillValueTypeToDr<T>(type, dr, strReval);
                return strReval;
            }
            //数组类型
            else if (type.IsArray)
            {
                List<T> strReval = new List<T>();
                FillValueTypeToArray(type, dr, strReval);
                return strReval;
            }


         /* */  var cacheManager = CacheManager<IDataReaderEntityBuilder<T>>.GetInstance();
            string key = "DataReaderToList." + fields + type.FullName;
            IDataReaderEntityBuilder<T> eblist = null;
            if (cacheManager.ContainsKey(key))
            {
                eblist = cacheManager[key];
            }
            else
            {
                eblist = IDataReaderEntityBuilder<T>.CreateBuilder(type, dr);
                eblist = IDataReaderEntityBuilder<T>.CreateBuilder(type, dr);
                cacheManager.Add(key, eblist, cacheManager.Day);
            }
            List<T> list = new List<T>();
            try
            {
                if (dr == null) return list;
                while (dr.Read())
                {
                    list.Add(eblist.Build(dr));
                }
                if (isClose) { dr.Close(); dr.Dispose(); dr = null; }
            }
            catch (Exception ex)
            {
                if (isClose) { dr.Close(); dr.Dispose(); dr = null; }
                throw ex;
            }
            return list;
        }
        private static void FillValueTypeToDictionary<T>(Type type, IDataReader dr, List<T> strReval)
        {
            using (IDataReader re = dr)
            {
                Dictionary<string, string> reval = new Dictionary<string, string>();
                while (re.Read())
                {
                    if (SqlTool.DicOO == type)
                    {
                        var kv = new KeyValuePair<object, object>(dr.GetValue(0), re.GetValue(1));
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<object, object>)));
                    }
                    else if (SqlTool.Dicii == type)
                    {
                        var kv = new KeyValuePair<int, int>(dr.GetValue(0).TryToInt(), re.GetValue(1).TryToInt());
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, int>)));
                    }
                    else if (SqlTool.DicSi == type)
                    {
                        var kv = new KeyValuePair<string, int>(dr.GetValue(0).TryToString(), re.GetValue(1).TryToInt());
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, int>)));
                    }
                    else if (SqlTool.DicSo == type)
                    {
                        var kv = new KeyValuePair<string, object>(dr.GetValue(0).TryToString(), re.GetValue(1));
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, object>)));
                    }
                    else if (SqlTool.DicSS == type)
                    {
                        var kv = new KeyValuePair<string, string>(dr.GetValue(0).TryToString(), dr.GetValue(1).TryToString());
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, string>)));
                    }
                    else
                    {
                        Check.Exception(true, "暂时不支持该类型的Dictionary 你可以试试 Dictionary<string ,string>或者联系作者！！");
                    }
                }
            }
        }
        private static void FillValueTypeToArray<T>(Type type, IDataReader dr, List<T> strReval)
        {
            using (IDataReader re = dr)
            {
                int count = dr.FieldCount;
                var childType = type.GetElementType();
                while (re.Read())
                {
                    object[] array = new object[count];

                    if (childType == SqlTool.StringType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => it.TryToString()).ToArray(), type));
                    else if (childType == SqlTool.ObjType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => it).ToArray(), type));
                    else if (childType == SqlTool.BoolType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => it.TryToBoolean()).ToArray(), type));
                    else if (childType == SqlTool.ByteType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (byte)it).ToArray(), type));
                    else if (childType == SqlTool.DecType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => it.TryToDecimal()).ToArray(), type));
                    else if (childType == SqlTool.GuidType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => it.TryToGuid()).ToArray(), type));
                    else if (childType == SqlTool.DateType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => it.TryToDate()).ToArray(), type));
                    else if (childType == SqlTool.IntType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => it.TryToInt()).ToArray(), type));
                    else
                        Check.Exception(true, "暂时不支持该类型的Array 你可以试试 object[] 或者联系作者！！");
                }
            }
        }
       
    }
}
