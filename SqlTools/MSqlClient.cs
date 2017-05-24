using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SqlTools
{
    public class MSqlClient: SqlHelper
    {


        public MSqlClient(string connectionString, string dbtype = "mssql") : base(connectionString)
        {
            ConnectionString = connectionString;
            DBtype = dbtype;
            _sqlConnection = SqlBase.CreateConnection();
            _sqlConnection.Open();
        }
        public MSqlClient() : base(SqlBase.ConnectionString)
        {
            ConnectionString = SqlBase.ConnectionString;
            DBtype = SqlBase.DBType;
            _sqlConnection = SqlBase.CreateConnection();
            _sqlConnection.Open();
        }
        public SqlTransaction _tran = null;
        public string CurrentFilterKey = null;
        public Sqlable Sqlable()
        {
            var sqlable = new Sqlable() { DB = this };
            //全局过滤器
            if (CurrentFilterKey.IsValuable())
            {
                //if (_filterFuns.IsValuable() && _filterFuns.ContainsKey(CurrentFilterKey))
                //{
                //    var filterInfo = _filterFuns[CurrentFilterKey];
                //    var filterVlue = filterInfo();
                //    string whereStr = string.Format(" AND {0} ", filterVlue.Key);
                //    sqlable.Where.Add(whereStr);
                //    if (filterVlue.Value != null)
                //        sqlable.Params.AddRange(SqlSugarTool.GetParameters(filterVlue.Value));
                //    return sqlable;
                //}
            }
            return sqlable;
        }



        public List<T> SqlQuery<T>(string sql, IDbDataParameter[] pars)
        {
            return SqlQuery<T>(sql, pars.ToList());
        }
        public List<T> SqlQuery<T>(string sql, object whereObj = null)
        {
           
            var pars = SqlTool.GetParameters(whereObj).ToList();
            return SqlQuery<T>(sql, pars);
        }
        public List<T> SqlQuery<T>(string sql, List<IDbDataParameter> pars)
        {
            IDataReader reader = null;
            //全局过滤器            
            var type = typeof(T);
            sql = string.Format(@"--{0}
{1}", type.Name, sql);
            reader = GetReader(sql,  pars.ToArray());
            string fields = sql;
            if (sql.Length > 101)
            {
                fields = sql.Substring(0, 100);
            }
            var reval = SqlTool.DataReaderToList<T>(type, reader, fields);
            fields = null;
            sql = null;
            return reval;
        }

        public string ConnectionString { get; set; } = "";
        /// <summary>
        /// 查询是否允许脏读，（默认为:true）
        /// </summary>
        public bool IsNoLock { get; set; }

        /// <summary>
        /// 忽略非数据库列，如果非特殊需求不建议启用
        /// </summary>
        public bool IsIgnoreErrorColumns = false;
       // public int CommandTimeOut { get; private set; }
       // public bool IsGetPageParas { get; private set; }
        public PageModel PageModel { get; internal set; }
        public Language Language = null;
        internal int GetInt(string v, SqlParameter[] sqlParameter)
        {
            return 0;
        } 
        internal object GetDataTable(string v, SqlParameter[] sqlParameter)
        {
            throw new NotImplementedException();
        }
        public Queryable<T> Queryable<T>() where T : new()
        {
            var queryable = new Queryable<T>() { DB = this };
            TableInfo ti = PropertyManager.Instance.GetClassInfo(typeof(T));
            queryable.TableName = ti.tablename;
            //别名表}
            //全局过滤器          
            return queryable;
        }
        /// <summary>
        /// 创建单表查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Queryable<T> Queryable<T>(string tableName) where T : new()
        {
            return new Queryable<T>() { DB = this, TableName = tableName };
        }
       
        /// <summary>
        /// 数据库修改，插入，删除操作
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">实体类</param>
        /// <param name="op">操作，1，11插入；2修改，3 删除</param>
        /// <param name="CommTxt">返回的sql</param>
        /// <returns>@部分的参数</returns>
        private List<IDbDataParameter> _DataIuD<T>(T entity, int op, out string CommTxt )
        {
            TableInfo tinfo=null;
            return _DataIuD<T>(entity, op, out CommTxt,ref tinfo);
        }


        private List<IDbDataParameter> _DataIuD<T>(T entity, int op,out string CommTxt ,ref TableInfo tinfo)
        {
            object obj;
            Type type = entity.GetType();
            string typeName = type.Name;
             tinfo = PropertyManager.Instance.GetClassInfo(type);
            var name = string.IsNullOrEmpty(tinfo.tableview) == true ? tinfo.tablename : tinfo.tableview;
            string col = "";
            var sb = new StringBuilder(null);
            var sb1 = new StringBuilder(null);
            TableColumn tc = null;
            CommTxt = "";
            List<IDbDataParameter> pars = new List<IDbDataParameter>();
            IDbDataParameter par;
            foreach (KeyValuePair<string, TableColumn> kvp in tinfo.tcolumn)
            {
                if (kvp.Value.IsKey == false)
                {
                    if (kvp.Value.Iscolumn == true)
                    {
                        switch (op)
                        {
                            case 11:
                            case 1:
                                obj = kvp.Value.GetValue(entity);
                                if (obj != null)
                                {
                                    sb.AppendFormat(",{0}", kvp.Value.Name);
                                    sb1.AppendFormat(",@{0}", kvp.Value.Name);
                                    par = SqlBase.CreateParameter();
                                    par.ParameterName = string.Format("@{0}", kvp.Value.Name);
                                    par.Value = obj;// kvp.Value.GetValue(entity);
                                    pars.Add(par);
                                }
                                break;
                            case 22:
                            case 2:
                                obj=kvp.Value.GetValue(entity);
                                if (obj != null)
                                {
                                    sb1.AppendFormat(",{0} = @{1}", kvp.Value.Name, kvp.Value.Name);
                                    par = SqlBase.CreateParameter();
                                    par.ParameterName = string.Format("@{0}", kvp.Value.Name);
                                    par.Value = obj;// kvp.Value.GetValue(entity);
                                    pars.Add(par);
                                }                         
                                break;
                        }
                    }
                }
                else
                {
                    col = kvp.Value.Name;
                    tc = kvp.Value;
                }
            }
            if (sb.Length > 0)
                sb.Remove(0, 1);
            if (sb1.Length > 0)
                sb1.Remove(0, 1);
            switch (op)
            {
                case 0:
                    if (sb1.Length > 1)
                    {
                        CommTxt = string.Format("select  * from {0} {1}=@{2}", name, col, col);
                        par = SqlBase.CreateParameter();
                        par.ParameterName = string.Format("@{0}", tc.Name);
                        par.Value = tc.GetValue(entity);
                        pars.Add(par);                       
                    }
                    break;
                case 1:
                    if (sb1.Length > 1)
                        CommTxt = string.Format("Insert into [{0}] ( {1} ) VALUES ( {2})", name, sb.ToString(), sb1.ToString());                   
                    break;
                case 22:
                case 2:
                    if (sb1.Length > 1)
                    {
                        if (op == 2)
                        {
                            CommTxt = string.Format("UPDATE [{0}] SET	{1} WHERE {2}=@{3}", name, sb1.ToString(), col, col);
                            par = SqlBase.CreateParameter();
                            par.ParameterName = string.Format("@{0}", col);
                            par.Value = tc.GetValue(entity);
                            pars.Add(par);
                        }  else
                            CommTxt = string.Format("UPDATE [{0}] SET   {1} WHERE 1=1", name, sb1.ToString(), col, col);
                    }
                    break;
                case 3:
                    CommTxt = string.Format("Delete from  [{0}]  WHERE ({1} = @{2})", name, col, col);
                    par = SqlBase.CreateParameter();
                    par.ParameterName = string.Format("@{0}", tc.Name);
                    par.Value = tc.GetValue(entity);
                    pars.Add(par);                
                    break;
                case 11:
                    if (sb1.Length > 1)
                    {
                        CommTxt = string.Format("Insert into {0} ( {1} ) select  {2} ", name, sb.ToString(), sb1.ToString());
                    }
                    break;
            }          
            return pars;
        }

        /// <summary>
        /// 批量插入
        /// 使用说明:sqlSugar.Insert(List《entity》);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">插入对象</param>
        /// <param name="isIdentity">主键是否为自增长,true可以不填,false必填</param>
        /// <returns></returns>
        public List<object> InsertRange<T>(List<T> entities, bool isIdentity = true) where T : class
        {
            List<object> reval = new List<object>();
            foreach (var it in entities)
            {
                reval.Add(Insert<T>(it, isIdentity));
            }
            return reval;
        }

        /// <summary>
        /// 插入
        /// 使用说明:sqlSugar.Insert(entity);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">插入对象</param>
        /// <param name="isIdentity">该属性已经作废可以不填，主键是否为自增长,true可以不填,false必填</param>
        /// <returns></returns>
        public object Insert<T>(T entity, bool isIdentity = true) where T : class
        {
            string sql = "";
            List<IDbDataParameter> pars = _DataIuD(entity, 1, out sql);
            try
            {
                var lastInsertRowId = GetScalar(sql, pars.ToArray());
                return lastInsertRowId;
            }
            catch (Exception ex)
            {
                throw new Exception("sql:" + sql + "\n" + ex.Message);
            }

        }

        /// <summary>
        /// 更新
        /// 注意：rowObj为T类型将更新该实体的非主键所有列，如果rowObj类型为匿名类将更新指定列
        /// 使用说明:sqlSugar.Update《T》(rowObj,whereObj);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowObj">new T(){name="张三",sex="男"}或者new {name="张三",sex="男"}</param>
        /// <param name="expression">it.id=100</param>
        /// <returns></returns>
        public bool Update<T>(object rowObj, Expression<Func<T, bool>> expression) where T : class
        {
            if (rowObj == null) { throw new ArgumentNullException("SqlSugarClient.Update.rowObj"); }
            if (expression == null) { throw new ArgumentNullException("SqlSugarClient.Update.expression"); }
            string sql;
            StringBuilder sb1 = null;
            TableColumn tc;
            IDbDataParameter par;
            List<IDbDataParameter> pars = null;
            Type t1 = typeof(T);
            Type t2 = rowObj.GetType();
            if (t1.Name != t2.Name)
            {
                sb1 = new StringBuilder();
                pars = new List<IDbDataParameter>();
                TableInfo ti = PropertyManager.Instance.GetClassInfo(t1);
                TableInfo ti1 = PropertyManager.Instance.GetClassInfo(t2);
                foreach (var k in ti1.tcolumn)
                {
                    if (k.Value.IsKey) continue;
                    if (ti.tcolumn.TryGetValue(k.Key, out tc))
                    {
                        sb1.AppendFormat(",{0} = @{1}", k.Value.Name, k.Value.Name);
                        par = SqlBase.CreateParameter();
                        par.ParameterName = string.Format("@{0}", k.Value.Name);
                        par.Value = k.Value.GetValue(rowObj);
                        pars.Add(par);
                    }
                }
                if (sb1.Length > 0)
                    sb1.Remove(0, 1);
                sql = string.Format("UPDATE [{0}] SET   {1} WHERE 1=1", ti.Tablename, sb1.ToString());
            }
            else
                pars = _DataIuD(rowObj, 22, out sql);
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression);

            List<IDbDataParameter> parsList = new List<IDbDataParameter>();
            foreach (var v in re.Paras)
                parsList.Add(v);
            if (pars != null)
            {
                foreach (var parc in pars)
                {
                    SqlBase.ishierarchyid(parc);
                    parsList.Add(parc);
                }
            }
            try
            {
                var updateRowCount = ExecuteCommand(sql+ re.SqlWhere, parsList.ToArray());
                return updateRowCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("sql:" + sql + re.SqlWhere + "\n" + ex.Message);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="rowObj">实体必须包含主键</param>
        /// <returns></returns>
        public bool Update<T>(T rowObj) where T : class
        {
            var reval = Update<T, object>(rowObj);
            return reval;
        }

        /// <summary>
        /// 更新
        /// 注意：rowObj为T类型将更新该实体的非主键所有列，如果rowObj类型为匿名类将更新指定列
        /// 使用说明:sqlSugar.Update《T》(rowObj,whereObj);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowObj">new T(){name="张三",sex="男"}或者new {name="张三",sex="男"}</param>
        /// <param name="whereIn">new int[]{1,2,3}</param>
        /// <returns></returns>
        public bool Update<T, FiledType>(object rowObj, params FiledType[] whereIn) where T : class
        {
            if (rowObj == null) { throw new ArgumentNullException("SqlSugarClient.Update.rowObj"); }
            int op = 22;
            StringBuilder sbSql = new StringBuilder();
            Type type = rowObj.GetType();
            
            TableInfo tinfo = null;          
            if (whereIn.Count() == 0)
            {
                op = 2;
            }
            string sql;
            List<IDbDataParameter> pars = _DataIuD(rowObj, op, out sql,ref tinfo);
            string key = string.IsNullOrEmpty(tinfo.PrimaryKey) ? tinfo.keyname : tinfo.PrimaryKey;
            if (op==22)           
                sbSql.AppendFormat("AND {0} IN ({1})",  key, whereIn.ToJoinSqlInVal());            
            List<IDbDataParameter> parsList = new List<IDbDataParameter>();           
            if (pars != null)
            {
                foreach (var par in pars)
                {
                    SqlBase.ishierarchyid(par);
                    parsList.Add(par);
                }
            }
            try
            {
                var updateRowCount = ExecuteCommand(sql, parsList.ToArray());
                return updateRowCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("sql:" + sql  + "\n" + ex.Message);
            }
        }

        /// <summary>
        /// 删除,根据表达示
        /// 使用说明:
        /// Delete《T》(it=>it.id=100) 或者Delete《T》(3)
        /// </summary>
        /// <param name="expression">筛选表达示</param>
        public bool Delete<T>(T obj)
        {
            Type type = typeof(T);
            TableInfo tinfo = PropertyManager.Instance.GetClassInfo(type);
            string typeName = tinfo.Tablename;
            TableColumn tc;
            IDbDataParameter para = PropertyManager.Instance.CreateParameter();
            para.ParameterName = tinfo.keyname;
            if (tinfo.tcolumn.TryGetValue(tinfo.keyname, out tc) == false) return false;
            para.Value = tc.GetValue(obj);
            string sql = string.Format("DELETE FROM [{0}] WHERE {1}=@{1}", typeName, tinfo.keyname);
            bool isSuccess = ExecuteCommand(sql, new IDbDataParameter[] { para }) > 0;
            return isSuccess;
        }
        /// <summary>
        /// 删除,根据表达示
        /// 使用说明:
        /// Delete《T》(it=>it.id=100) 或者Delete《T》(3)
        /// </summary>
        /// <param name="expression">筛选表达示</param>
        public bool Delete<T>(string where)
        {
            Type type = typeof(T);
            TableInfo tinfo = PropertyManager.Instance.GetClassInfo(type);
            string typeName = tinfo.Tablename;
            string sql = string.Format("DELETE FROM [{0}] WHERE {1}", typeName, where);
            bool isSuccess = ExecuteCommand(sql, new IDbDataParameter[] { }) > 0;
            return isSuccess;
        }
        /// <summary>
        /// 删除,根据表达示
        /// 使用说明:
        /// Delete《T》(it=>it.id=100) 或者Delete《T》(3)
        /// </summary>
        /// <param name="expression">筛选表达示</param>
        public bool Delete<T>(Expression<Func<T, bool>> expression)
        {
            Type type = typeof(T);
            TableInfo tinfo = PropertyManager.Instance.GetClassInfo(type);
            string typeName = tinfo.Tablename;
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression);
            string sql = string.Format("DELETE FROM [{0}] WHERE 1=1 {1}", typeName, re.SqlWhere);
            bool isSuccess = ExecuteCommand(sql, re.Paras.ToArray()) > 0;
            return isSuccess;
        }


        /// <summary>
        /// 批量删除
        /// 注意：whereIn 主键集合  
        /// 使用说明:Delete《T》(new int[]{1,2,3}) 或者  Delete《T》(3)
        /// </summary>
        /// <param name="whereIn"> delete ids </param>
        public bool Delete<T, FiledType>(params FiledType[] whereIn)
        {
            Type type = typeof(T);
            TableInfo tinfo = PropertyManager.Instance.GetClassInfo(type);
            string typeName = tinfo.Tablename;
            bool isSuccess = false;
            if (whereIn != null && whereIn.Length > 0)
            {
                string sql = string.Format("DELETE FROM [{0}] WHERE {1} IN ({2})", typeName, tinfo.keyname, whereIn.ToJoinSqlInVal());
                int deleteRowCount = ExecuteCommand(sql);
                isSuccess = deleteRowCount > 0;
            }
            return isSuccess;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="FiledType">whereIn里面元素的类型</typeparam>
        /// <param name="expression">in 的字段名称</param>
        /// <param name="whereIn">需要删除条件值的数组集合</param>
        /// <returns></returns>
        public bool Delete<T, FiledType>(Expression<Func<T, object>> expression, List<FiledType> whereIn)
        {
            if (whereIn == null) return false;
            return Delete<T, FiledType>(expression, whereIn.ToArray());
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="FiledType">whereIn里面元素的类型</typeparam>
        /// <param name="expression">in 的字段名称</param>
        /// <param name="whereIn">需要删除条件值的数组集合</param>
        /// <returns></returns>
        public bool Delete<T, FiledType>(Expression<Func<T, object>> expression, params FiledType[] whereIn)
        {
            ResolveExpress re = new ResolveExpress();
            var fieldName = re.GetExpressionRightField(expression);
            Type type = typeof(T);
            TableInfo tinfo = PropertyManager.Instance.GetClassInfo(type);
            string typeName = tinfo.Tablename;
            bool isSuccess = false;
            if (whereIn != null && whereIn.Length > 0)
            {
                string sql = string.Format("DELETE FROM [{0}] WHERE {1} IN ({2})", typeName, fieldName, whereIn.ToJoinSqlInVal());
                int deleteRowCount = ExecuteCommand(sql);
                isSuccess = deleteRowCount > 0;
            }
            return isSuccess;
        }

       
    }
    //public class MDao : IDisposable
    //{

    //    public MSqlClient db;

    //    //禁止实例化
    //    public MDao()
    //    {
         
    //        this.db = new MSqlClient(Connstring);
    //    }


    //    void IDisposable.Dispose()
    //    {
    //        if (db != null)
    //        {
    //            db.Dispose();
    //        }
    //    }
    //}
}
