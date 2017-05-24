using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SqlTools
{
    public class SqlBase
    {
        public static string ConnectionString = "";
        public static string DBType = "";


        public static string ColumnsByTableNamesql = " SELECT Name FROM SysColumns WHERE id=Object_Id('{0}')";

        /// <summary>
        ///根据表名获取自添列 keyTableName Value columnName
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static void GetIdentitiesKeyByTableName(Dictionary<string, string> keydic)
        {
            var ss = "";
            string sql = string.Format(@"  
                            Select so.name tableName,                   --表名字
                                   sc.name keyName,             --自增字段名字
                                   ident_current(so.name) curr_value,    --自增字段当前值
                                   ident_incr(so.name) incr_value,       --自增字段增长值
                                   ident_seed(so.name) seed_value        --自增字段种子值
                              from sysobjects so 
                            Inner Join syscolumns sc
                                on so.id = sc.id
                                and columnproperty(sc.id, sc.name, 'IsIdentity') = 1
         ");
            try
            {
                using (IDbConnection conn = CreateConnection())
                {
                    conn.Open();
                    IDbCommand command = conn.CreateCommand();
                    command.CommandText = sql;
                    IDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        var v = dr.GetValue(0).ToString();
                        var v1 = dr.GetValue(1).ToString();
                        if (keydic.TryGetValue(v, out ss) == false)
                            keydic.Add(v, v1);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 根据表获取主键
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static void GetPrimaryKeyByTableName(Dictionary<string, List<string>> Primarykey)
        {
            List<string> ss;
            string sql = @"  				SELECT a.name as keyName ,d.name as tableName
              FROM   syscolumns a 
              inner  join sysobjects d on a.id=d.id       
              where  exists(SELECT 1 FROM sysobjects where xtype='PK' and  parent_obj=a.id and name in (  
              SELECT name  FROM sysindexes   WHERE indid in(  
              SELECT indid FROM sysindexkeys WHERE id = a.id AND colid=a.colid  
            )))"; try
            {
                using (IDbConnection conn = CreateConnection())
                {
                    conn.Open();
                    IDbCommand command = conn.CreateCommand();
                    command.CommandText = sql;
                    IDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        var v1 = dr.GetValue(0).ToString();
                        var v = dr.GetValue(1).ToString();
                        if (Primarykey.TryGetValue(v, out ss) == false)
                        {
                            ss = new List<string>();
                            ss.Add(v1);
                            Primarykey.Add(v, ss);
                        }
                        else
                        {
                            if (ss.IndexOf(v1) < 0)
                                ss.Add(v1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static string GetSelectData(string syszd)
        {
            var sb = new StringBuilder();
            try
            {
                var strs = syszd.Split(':');
                if (strs.Length != 2) return "";
                using (IDbConnection conn = CreateConnection())
                {
                    conn.Open();
                    IDbCommand command = conn.CreateCommand();
                    command.CommandText = strs[1];
                    IDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr.FieldCount > 1)
                        {
                            sb.AppendFormat("{0}:{1};", dr.GetValue(0).ToString(), dr.GetValue(1).ToString());
                        }
                    }
                    if (sb.Length > 0)
                        sb.Remove(sb.Length - 1, 1);
                }
            }
            catch (Exception ex)
            {

            }
            return sb.ToString();
        }


        public static void GetZD(Dictionary<string, SysZd> syszd)
        {
            var ss = "";
            SysZd zd, zd1;
            try
            {
                using (IDbConnection conn = CreateConnection())
                {
                    conn.Open();
                    IDbCommand command = conn.CreateCommand();
                    command.CommandText = "select * from zd";
                    IDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        zd = new SysZd();
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            string name = dr.GetName(i).Trim();
                            var v = dr.GetValue(i).ToString();
                            zd.setdata(name, v);
                        }
                        if (syszd.TryGetValue(zd.field, out zd1) == false)
                            syszd.Add(zd.field, zd);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static IDbDataParameter CreateParameter()
        {
            IDbDataParameter idap = new SqlParameter();

            return idap;
        }
        public static IDbConnection CreateConnection()
        {
            IDbConnection idap = new SqlConnection(SqlBase.ConnectionString);
            return idap;

        }

        public static void ishierarchyid(IDbDataParameter parc)
        {
            var par1 = parc as SqlParameter;
            if (par1.SqlDbType == SqlDbType.Udt)
            {
                par1.TypeName = "HIERARCHYID";
            }
            SqlTool.SetParSize(parc);
        }
    }
}
