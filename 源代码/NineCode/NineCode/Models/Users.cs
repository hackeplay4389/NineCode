using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NineCode.Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class Users
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [Key]
        public int UNum { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        //[RegularExpression(@"^[A-Za-z0-9]{4,20}$")]
        public string UName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string UPass { get; set; }

        /// <summary>
        /// 安全邮箱
        /// </summary>
        public string UMail { get; set; }

        /// <summary>
        /// 账户状态
        /// </summary>
        public string UState { get; set; }

        /// <summary>
        /// 活跃时间
        /// </summary>
        public DateTime ULogin { get; set; }
    }
}