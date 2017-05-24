using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace SqlTools
{
    /// <summary>
    /// ** 描述：底层SQL辅助函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class SqlHelper : IDisposable
    {
        //SqlConnection _sqlConnection;
        //SqlTransaction _tran = null;
        protected string DBtype = "";
        protected IDbConnection _sqlConnection= null;
        protected IDbTransaction _tran = null;
        /// <summary>
        /// 是否清空SqlParameters
        /// </summary>
        public bool isClearParameters = true;
        public int CommandTimeOut = 30000;
        public bool IsStoredProcedure = false;
        /// <summary>
        /// 将页面参数自动填充到SqlParameter []，无需在程序中指定，这种情况需要注意是否有重复参数
        /// 例如：
        ///     var list = db.Queryable《Student》().Where("id=@id").ToList();
        ///     以前写法
        ///     var list = db.Queryable《Student》().Where("id=@id", new { id=Request["id"] }).ToList();
        /// </summary>
        public bool IsGetPageParas = false;
        public SqlHelper(string connectionString)
        {
           // _sqlConnection = IDbConnection.  new DbConnection(connectionString);
           // _sqlConnection.Open();
        }
        public IDbConnection GetConnection()
        {
            return _sqlConnection;
        }
        public void BeginTran()
        {
            _tran = _sqlConnection.BeginTransaction();
        }

        public void BeginTran(IsolationLevel iso)
        {
            _tran = _sqlConnection.BeginTransaction(iso);
        }

        //public void BeginTran(string transactionName)
        //{
        //    _tran = _sqlConnection.BeginTransaction(transactionName);
        //}

        //public void BeginTran(IsolationLevel iso, string transactionName)
        //{
        //    _tran = _sqlConnection.BeginTransaction(iso, transactionName);
        //}

        public void RollbackTran()
        {
            if (_tran != null)
            {
                _tran.Rollback();
                _tran = null;
            }
        }
        public void CommitTran()
        {
            if (_tran != null)
            {
                _tran.Commit();
                _tran = null;
            }
        }
        public string GetString(string sql, object pars)
        {
            return GetString(sql, SqlTool.GetParameters(pars));
        }
        public string GetString(string sql, params IDbDataParameter[] pars)
        {
            return Convert.ToString(GetScalar(sql, pars));
        }
        public int GetInt(string sql, object pars)
        {
            return GetInt(sql, SqlTool.GetParameters(pars));
        }
        public int GetInt(string sql, params IDbDataParameter[] pars)
        {
            return Convert.ToInt32(GetScalar(sql, pars));
        }
        public Double GetDouble(string sql, params IDbDataParameter[] pars)
        {
            return Convert.ToDouble(GetScalar(sql, pars));
        }
        public decimal GetDecimal(string sql, params IDbDataParameter[] pars)
        {
            return Convert.ToDecimal(GetScalar(sql, pars));
        }
        public DateTime GetDateTime(string sql, params IDbDataParameter[] pars)
        {
            return Convert.ToDateTime(GetScalar(sql, pars));
        }
        public object GetScalar(string sql, object pars)
        {
            return GetScalar(sql, SqlTool.GetParameters(pars));
        }
        public object GetScalar(string sql, params IDbDataParameter[] pars)
        {
            IDbCommand sqlCommand = _sqlConnection.CreateCommand();
            //IDbCommand sqlCommand = new IDbCommand(sql, _sqlConnection);
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            sqlCommand.CommandText = sql;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (pars != null)
            {
                //sqlCommand.Parameters.AddRange(pars);
                foreach (var par in pars)
                    sqlCommand.Parameters.Add(par);
            }
            if (IsGetPageParas)
            {
                SqlToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }
            object scalar = sqlCommand.ExecuteScalar();
            scalar = (scalar == null ? 0 : scalar);
            sqlCommand.Parameters.Clear();
            return scalar;
        }
        public int ExecuteCommand(string sql, object pars)
        {
            return ExecuteCommand(sql, SqlTool.GetParameters(pars));
        }
        public int ExecuteCommand(string sql, params IDbDataParameter[] pars)
        {
            // SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            IDbCommand sqlCommand = _sqlConnection.CreateCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            if (pars != null)
            {
                // sqlCommand.Parameters.AddRange(pars);
                foreach (var par in pars)
                    sqlCommand.Parameters.Add(par);
            }
            if (IsGetPageParas)
            {
                SqlToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }
            int count = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();
            return count;
        }
        public IDataReader GetReader(string sql, object pars)
        {
            return GetReader(sql, SqlTool.GetParameters(pars));
        }
        public IDataReader GetReader(string sql, params IDbDataParameter[] pars)
        {
            IDbCommand sqlCommand = _sqlConnection.CreateCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            sqlCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            if (pars != null)
            {
                // sqlCommand.Parameters.AddRange(pars);
                foreach (var par in pars)
                    sqlCommand.Parameters.Add(par);
            }
            if (IsGetPageParas)
            {
                SqlToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }
            IDataReader sqlDataReader = sqlCommand.ExecuteReader();
            if (isClearParameters)
                sqlCommand.Parameters.Clear();
            IsStoredProcedure = false;
            return sqlDataReader;
        }
        public List<T> GetList<T>(string sql, object pars)
        {
            return GetList<T>(sql, SqlTool.GetParameters(pars));
        }
        public List<T> GetList<T>(string sql, params IDbDataParameter[] pars)
        {
            var reval = SqlTool.DataReaderToList<T>(typeof(T), GetReader(sql, pars), null);
            return reval;
        }
        public T GetSingle<T>(string sql, object pars)
        {
            return GetSingle<T>(sql, SqlTool.GetParameters(pars));
        }
        public T GetSingle<T>(string sql, params IDbDataParameter[] pars)
        {
            var reval = SqlTool.DataReaderToList<T>(typeof(T), GetReader(sql, pars), null).Single();
            return reval;
        }
        public DataTable GetDataTable(string sql, object pars)
        {
            return GetDataTable(sql, SqlTool.GetParameters(pars));
        }
        public DataTable GetDataTable(string sql, params IDbDataParameter[] pars)
        {
            SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(sql, _sqlConnection);
            IDbCommand sqlCommand = _sqlConnection.CreateCommand();
            sqlCommand.CommandText = sql;
            // _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            foreach (var par in pars)
                _sqlDataAdapter.SelectCommand.Parameters.Add(par);
            if (IsGetPageParas)
            {
                SqlToolExtensions.RequestParasToSqlParameters(_sqlDataAdapter.SelectCommand.Parameters);
            }
            _sqlDataAdapter.SelectCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            DataTable dt = new DataTable();
            _sqlDataAdapter.Fill(dt);
            _sqlDataAdapter.SelectCommand.Parameters.Clear();
            return dt;
        }
        public void Dispose()
        {
            if (_sqlConnection != null)
            {
                if (_sqlConnection.State != ConnectionState.Closed)
                {
                    if (_tran != null)
                        _tran.Commit();
                    _sqlConnection.Close();
                }
            }
        }      
       
    }
}
