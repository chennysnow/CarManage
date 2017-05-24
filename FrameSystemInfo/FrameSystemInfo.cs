using Entitys;
using SqlTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FrameSystemInfo
{

    public class FrameSysteminfo
    {
        public CacheOnline online = null;
        private static readonly object _instanceLock = new object();
        private static FrameSysteminfo instance = null;
        public List<CarTypeInfo> CarTypeInfols = null;
        public Dictionary<string, string> menudic = new Dictionary<string, string>{
            { "BandInfo", "汽车品牌表" },
            { "CarDetialInfo", "汽车详细信息" },
            { "CarTypeInfo", "汽车类型表" },
            { "ShopInfo", "商户信息表" },
            { "Users", "用户管理" }
        };
        List<Role> Rolelist = new List<Role>();
        public FrameSysteminfo()
        {
            online = new CacheOnline();
            init();

        }

        public static FrameSysteminfo Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_instanceLock)
                    {
                        instance = new FrameSysteminfo();
                    }
                }
                return instance;
            }
        }
        public void init()
        {
            using (MSqlClient mdb = new MSqlClient(SqlBase.ConnectionString, SqlBase.DBType))
            {
                CarTypeInfols = mdb.Queryable<CarTypeInfo>().Where(x => x.Id>0).ToList();
            }

            Rolelist.Add(new Role
            {
                role_id = "manage",
                remark = "管理员"
            });
            Rolelist.Add(new Role
            {
                role_id = "system",
                remark = "系统管理员"
            });
            Rolelist.Add(new Role
            {
                role_id = "user",
                remark = "用户"
            });
        }

        public Users GetUserAsync(string username, string password)
        {
            using (MSqlClient mdb = new MSqlClient(SqlBase.ConnectionString, SqlBase.DBType))
            {
                var results = mdb.Queryable<Users>().Where(x => x.user_id == username);
                var result = results.SingleOrDefault();
                return result;
            }          

        }

        //获取用户角色信息
        public List<Role> GetUserRolesAsync(Users user)
        {
            var role = Rolelist.Where(x => x.role_id == user.role).ToList();
            if (role.Count == 0)
            {
                role.Add(new Role
                {
                    role_id = "user",
                    remark = "用户"
                });
            }
            return role;
        }
        public void AddOnline(Users user)
        {
            if (online == null)
                online = new CacheOnline();
            OnlineUser ou = online.GetValues(user.user_id);
            if (ou == null)
            {
                ou = new OnlineUser();
                ou.Timer = 0;
                ou.U_StartTime = DateTime.Now;
                ou.us = user;
                online.InsertUser(ou);
            }
        }

        
    }
}
  
