using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NineCode.Models
{
    /// <summary>
    /// 站点信息
    /// </summary>
    public class SiteInfo
    {
        /// <summary>
        /// 编号ID
        /// </summary>
        [Key]
        public int IID { get; set; }

        /// <summary>
        /// 站点标题
        /// </summary>
        public string ITitle { get; set; }

        /// <summary>
        /// 站点副标题
        /// </summary>
        public string ISmall { get; set; }

        /// <summary>
        /// 站点SEO信息
        /// </summary>
        public string ISEO { get; set; }

        /// <summary>
        /// ICP备案信息
        /// </summary>
        public string IICP { get; set; }

        /// <summary>
        /// 公安部备案信息
        /// </summary>
        public string IBei { get; set; }

        /// <summary>
        /// 第三方统计脚本
        /// </summary>
        public string ICount { get; set; }

        /// <summary>
        /// 是否关闭站点
        /// </summary>
        public string IIsOff { get; set; }

        /// <summary>
        /// 关闭站点原因
        /// </summary>
        public string IWhyOff { get; set; }
    }
}