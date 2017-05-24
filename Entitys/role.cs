using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlTools;
namespace Entitys
{
    /// <summary>
    /// 
    /// </summary>
    [Table(Name = "role")]
    public class Role 
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public Role()
        {
            ///Todo
        }
         [Id(Name = "ID")]
        public int ID
        {
            get;
            set;
        }

        public string role_id
        {
            get;
            set;
        }
        public string remark
        {
            get;
            set;
        }
    }
}
