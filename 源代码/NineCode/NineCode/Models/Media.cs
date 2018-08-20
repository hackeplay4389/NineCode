using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NineCode.Models
{
    /// <summary>
    /// 文件表
    /// </summary>
    public class Media
    {
        /// <summary>
        /// 文件编号
        /// </summary>
        [Key]
        public int MID { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string MName { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string MUrl { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string MType { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime MTime { get; set; }

        /// <summary>
        /// 关联文章ID 0为不关联
        /// </summary>
        public int AID { get; set; }

        /// <summary>
        /// 操作用户ID
        /// </summary>
        public int UNum { get; set; }

        //导航属性
        public Article Article { get; set; }

        public Users Users { get; set; }
    }
}