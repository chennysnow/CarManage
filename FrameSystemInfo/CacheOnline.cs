using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using Entitys;

namespace FrameSystemInfo
{
    public class CacheOnline
    {


        protected List<OnlineUser> Luser = new List<OnlineUser>();
        protected static object _CacheDataRwl = new object();
        //定时器
        Timer _UpdateTimer;
        //用户登陆超时设置(毫秒)
        int _TimeOut=30*60*1000;//30
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="LoginTimeOutMinute">用户登陆超时设置(分钟)</param>
        public CacheOnline(int LoginTimeOutMinute)
        {
           Timer timer = new System.Threading.Timer(new TimerCallback(ClearTimeOutUser), null, 0, 60000);
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        public CacheOnline() : this(30) { }

        /// <summary>
        /// 清除到期在线用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearTimeOutUser(object sender)
        {
            lock (_CacheDataRwl) {
                Luser.ForEach(x => x.Timer++);
                var ls = Luser.Where(x => x.Timer > _TimeOut).ToList();
                foreach(var s in ls)
                {
                    Luser.Remove(s);
                }
            }
        }

        public void Remove(string Username)
        {
            lock (_CacheDataRwl)
            {
                var ls = Luser.Where(x => x.U_Name == Username);
                foreach (var s in ls)
                {
                    Luser.Remove(s);
                }
            }
        }
        /// <summary>
        /// 更新用户请求信息
        /// </summary>
        /// <param name="key"></param>
        public void Access(string name)
        {
            lock (_CacheDataRwl)
            {
                var ls = Luser.FirstOrDefault(x => x.U_Name == name);
                if (ls != null)
                    ls.Timer = 0;
            }
        }
      

        /// <summary>
        /// 检测Key是否在线
        /// </summary>
        /// <param name="key">用户标识</param>
        /// <returns>True在线 False不在线</returns>
        public bool CheckKeyOnline(string name)
        {
            lock (_CacheDataRwl)
            {
                var ls = Luser.FirstOrDefault(x => x.U_Name == name);
                if (ls != null)
                    return true;
            }
            return false;
        }



        #region "插入用户"

        /// <summary>
        /// 插入用户
        /// </summary>
        /// <param name="key">sessionid</param>
        /// <param name="value"></param>
        public void InsertUser(OnlineUser value)
        {
            lock (_CacheDataRwl)
            {
                var ls = Luser.FirstOrDefault(x => x.U_Name == value.U_Name);
                if (ls == null)
                    Luser.Add(value);
            }
        }
        #endregion

        /// <summary>
        /// 所有用户总数
        /// </summary>
        public int AllCount
        {
            get
            {
                return Luser.Count;
            }
        }

        

        /// <summary>
        /// 清除所有在线人数
        /// </summary>
        public void Clear()
        {
            lock (_CacheDataRwl)
            {
                Luser.Clear();
            }
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="U_Name">用户名</param>
        /// <returns></returns>
        public OnlineUser GetValues(string user)
        {
            lock (_CacheDataRwl)
            {
                var ls = Luser.FirstOrDefault(x => x.U_Name ==user);
                return ls;
            }

        }
    }



    /// <summary>
    /// 用户基础类
    /// </summary>
    public class OnlineUser
    {
        #region "Private Variables"
        private object _syn = new object();
        private DateTime _U_StartTime = DateTime.Now;
        public int Timer = 0;
        public Users us;
        public Dictionary<string, int> _qxs = null;
        public int _info = 0;
        #endregion
        #region "Public Variables"
        /// <summary>
        /// 用户标识值
        /// </summary>
        public int info
        {
            get
            {
                lock (_syn)
                {
                    if (_info > 0)
                        _info--;
                    return _info;
                }
            }
            set
            {
                lock (_syn)
                {
                    _info = value;
                }

            }
        }

      

        /// <summary>
        /// 用户名
        /// </summary>
        public string U_Name
        {
            get
            {
                return us.user_id;// _U_Name;
            }
            //set
            //{
            //   us.user_id = value;// _U_Name 
            //}
        }

        
        /// <summary>
        /// 开始访问时间
        /// </summary>
        public DateTime U_StartTime
        {
            get
            {
                return _U_StartTime;
            }
            set
            {
                _U_StartTime = value;
            }
        }
      
      
        /// <summary>
        /// 用户IP
        /// </summary>
        public string U_LastIP
        {
            get
            {
                return us.LastIP;
            }
            set
            {
                us.LastIP = value;
            }
        }       
       
        #endregion

        #region "Public Variables"

        public int getqxdata(string key)
        {
            int val = 0;
            if (!_qxs.TryGetValue(key, out val))
                val = 0;
            return val;

        }

        #endregion


    }



}
