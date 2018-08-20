using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NineCode.Models
{
    /// <summary>
    /// 站点配置信息
    /// </summary>
    public class SiteConfig
    {
        /// <summary>
        /// 编号ID
        /// </summary>
        public int CfgID { get; set; }

        /// <summary>
        /// SMTP服务器
        /// </summary>
        public string CfgServer { get; set; }

        /// <summary>
        /// SMTP端口
        /// </summary>
        public string CfgPort { get; set; }

        /// <summary>
        /// SMTP账户
        /// </summary>
        public string CfgUser { get; set; }

        /// <summary>
        /// SMTP密码
        /// </summary>
        public string CfgPass { get; set; }

        /// <summary>
        /// SMTP发件人地址
        /// </summary>
        public string CfgFrom { get; set; }

    }
}