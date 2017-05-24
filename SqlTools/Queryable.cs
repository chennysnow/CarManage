using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace  SqlTools
{

    /// <summary>
    /// ** 描述：Queryable是单表查询基类，基于拥有大量查询扩展函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class Queryable<T>
    {
        /// <summary>
        /// T的名称
        /// </summary>
        public string TName { get { return typeof(T).Name; } }
        /// <summary>
        /// 实体类型
        /// </summary>
        public Type Type { get { return typeof(T); } }
        /// <summary>
        /// 数据接口
        /// </summary>
        public MSqlClient DB = null;
        /// <summary>
        /// Where临时数据
        /// </summary>
        public List<string> Where = new List<string>();
        /// <summary>
        /// Skip临时数据
        /// </summary>
        public int? Skip { get; set; }
        /// <summary>
        /// Take临时数据
        /// </summary>
        public int? Take { get; set; }
        /// <summary>
        /// Order临时数据
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// Select临时数据
        /// </summary>
        public string Select { get; set; }
        /// <summary>
        /// SqlParameter临时数据
        /// </summary>
        public List<IDbDataParameter> Params = new List<IDbDataParameter>();
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 分组查询
        /// </summary>
        public string GroupBy { get; set; }
        /// <summary>
        /// 条件索引
        /// </summary>
        public int WhereIndex = 1;
        /// <summary>
        /// 联表查询临时数据
        /// </summary>
        public List<string> JoinTable = new List<string>();

    }

   
    public class Language
    {
        /// <summary>
        /// 数据库里面的语言后缀
        /// </summary>
        public string Suffix { get; set; }
        /// <summary>
        /// 数据库语言的VALUE
        /// </summary>
        public int LanguageValue { get; set; }
        /// <summary>
        /// 需要全局替换的字符串Key(用于替换默认语言)
        /// </summary>
        public string ReplaceViewStringKey = "LanguageId=1";
        /// <summary>
        /// 需要全局替换的字符串Value(用于替换默认语言)
        /// </summary>
        public string ReplaceViewStringValue = "LanguageId = {0}";

    }
    public class Check
    {
        /// <summary>
        /// 使用导致此异常的参数的名称初始化 System.ArgumentNullException 类的新实例。
        /// </summary>
        /// <param name="checkObj"></param>
        /// <param name="message"></param>
        public static void ArgumentNullException(object checkObj, string message)
        {
            if (checkObj == null)
                throw new ArgumentNullException(message);
        }
        /// <summary>
        /// 使用指定的错误消息初始化 System.Exception 类的新实例。
        /// </summary>
        /// <param name="isException"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Exception(bool isException, string message, params string[] args)
        {
            if (isException)
                throw new SqlSugarException(string.Format(message, args));
        }
    }
    public class SqlSugarException : Exception
    {
        public SqlSugarException(string message)
            : base(message)
        {

        }
    }
}
