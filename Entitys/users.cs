using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlTools;
namespace Entitys
{
    [Table(Name = "users")]
    public class Users 
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public Users()
        {
            ///Todo
        }
         [Id(Name = "ID")]
        public int ID
        {
            get;
            set;
        }
        [Column(Cn = "用户名", disp = 1, width = 10, dispsore = 1)]
        public string user_id
        {
            get;
            set;
        }

        [Column(Cn = "密码", disp = 2, width = 10, dispsore = 2)]
        public string password
        {
            get;
            set;
        }
        [Column(Cn = "部门", disp = 3, width = 10, dispsore = 3)]
        public string userbm
        {
            get;
            set;
        } = "";
        [Column(Cn = "使用状态", disp = 5, width = 10, dispsore = 5)]
        public int StatUs
        {
            get;
            set;
        } = 1;
        [Column(Cn = "最后IP", disp = 6, width = 10, dispsore = 6)]
        public string LastIP
        {
            get;
            set;
        }
        [Column(Cn = "最后访问时间", disp =7, width = 10, dispsore = 7)]
        public DateTime LastDateTime
        {
            get;
            set;
        }
        public string ExtendField
        {
            get;
            set;
        }
        public string remark
        {
            get;
            set;
        }
        [Column(Cn = "角色", disp = 4, width = 10, dispsore = 4)]
        [SelectData(Text ="manage:管理员;system:系统管理员;user:用户")]
        public string role { get; set; } = "manage";
    }
}
