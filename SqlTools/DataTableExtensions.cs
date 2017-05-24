﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SqlTools
{
    /// <summary>
    /// 作者：sunkaixuan 
    /// 创建时间：2016/7/31
    /// 修改时间：-
    /// 说明：让.netCore支持DataTable
    /// </summary>
    public class DataTable
    {
        public DataColumnCollection Columns = new DataColumnCollection();

        public DataRowCollection Rows = new DataRowCollection();
    }

    public class DataColumn
    {
        public DataColumn()
        {

        }
        public DataColumn(string columnName)
        {
            this.ColumnName = columnName;
        }
        public DataColumn(string columnName, object dataType)
        {
            this.ColumnName = columnName;
            this.DataType = dataType;
        }
        public string ColumnName { get; internal set; }
        public object DataType { get; internal set; }
    }

    public class DataColumnCollection : IEnumerable, ICollection, IEnumerator
    {
        public DataColumn this[int thisIndex]
        {
            get
            {
                return cols[thisIndex];
            }
        }
        private int index = -1;
        private List<DataColumn> cols;
        public int Count
        {
            get
            {
                if (this.cols == null)
                {
                    this.cols = new List<DataColumn>();
                }
                return this.cols.Count;
            }
        }

        public void Add(DataColumn col)
        {
            if (this.cols == null)
            {
                this.cols = new List<DataColumn>();
            }
            this.cols.Add(col);
        }

        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        public object Current
        {
            get
            {
                return cols[index];
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        //
        // 摘要:
        //     获取该集合的 System.Collections.IEnumerator。
        //
        // 返回结果:
        //     该集合的 System.Collections.IEnumerator。
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this; ;
        }

        public bool MoveNext()
        {
            index++;
            var isNext = index < cols.Count;
            if (!isNext)
                Reset();
            return isNext;
        }

        public void Reset()
        {
            index = -1;
        }

        public bool ContainsKey(string name)
        {
            if (this.cols == null) return false;
            return (this.cols.Any(it => it.ColumnName == name));
        }
    }

    public class DataRowCollection : IEnumerable, ICollection, IEnumerator
    {

        public DataRow this[int thisIndex]
        {
            get
            {
                return Rows[thisIndex];
            }
        }

        private int index = -1;
        private List<DataRow> Rows = null;
        public int Count
        {
            get
            {
                if (this.Rows == null)
                {
                    this.Rows = new List<DataRow>();
                }
                return Rows.Count;
            }
        }

        public object Current
        {
            get
            {
                if (this.Rows == null)
                {
                    this.Rows = new List<DataRow>();
                }
                return Rows[index];
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        //
        // 摘要:
        //     获取该集合的 System.Collections.IEnumerator。
        //
        // 返回结果:
        //     该集合的 System.Collections.IEnumerator。
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this; ;
        }

        public bool MoveNext()
        {
            index++;
            var isNext = index < Rows.Count;
            if (!isNext)
                Reset();
            return isNext;
        }

        public void Reset()
        {
            index = -1;
        }

        internal void Add(DataRow daRow)
        {
            if (Rows == null)
            {
                Rows = new List<DataRow>();
            }
            Rows.Add(daRow);
        }
    }

    public class DataRow
    {
        private Dictionary<string, object> obj = new Dictionary<string, object>();

        public void Add(string key, object value)
        {
            obj.Add(key, value);
        }

        public object this[string name]
        {
            get
            {
                return obj[name];
            }
        }
        public object this[int index]
        {
            get
            {
                int i = 0;
                object reval = null;
                foreach (var item in obj)
                {
                    if (i == index)
                    {
                        reval = item.Value;
                        break;
                    }
                    i++;
                }
                return reval;
            }
        }

        public bool ContainsKey(string columnName)
        {
            if (this.obj == null) return false;
            return (this.obj.ContainsKey(columnName));
        }
    }

    public class SqlDataAdapter
    {
        private IDbCommand command;
        private string sql;
        private IDbConnection _sqlConnection;

        public SqlDataAdapter(IDbCommand command)
        {
            this.command = command;
        }

        public SqlDataAdapter(string sql, IDbConnection _sqlConnection)
        {
            this.sql = sql;
            this._sqlConnection = _sqlConnection;
        }

        public IDbCommand SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    this.command = _sqlConnection.CreateCommand();  //new IDbCommand(this.sql, this._sqlConnection);
                    this.command.CommandText = this.sql;
                }
                return this.command;
            }
        }

        public void Fill(DataTable dt)
        {
            if (dt == null)
            {
                dt = new DataTable();
            }
            var columns = dt.Columns;
            var rows = dt.Rows;
            using (IDataReader dr = command.ExecuteReader())
            {
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string name = dr.GetName(i).Trim();
                    if (!columns.ContainsKey(name))
                        columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                }

                while (dr.Read())
                {
                    DataRow daRow = new DataRow();
                    for (int i = 0; i < columns.Count; i++)
                    {
                        if (!daRow.ContainsKey(columns[i].ColumnName))
                            daRow.Add(columns[i].ColumnName, dr.GetValue(i));
                    }
                    dt.Rows.Add(daRow);
                }
            }

        }
    }
}
