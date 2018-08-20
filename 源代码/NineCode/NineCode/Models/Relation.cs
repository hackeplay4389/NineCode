using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NineCode.Models
{
    /// <summary>
    /// 关系表
    /// </summary>
    public class Relation
    {
        /// <summary>
        /// 关系编号
        /// </summary>
        [Key]
        public int RID { get; set; }

        /// <summary>
        /// 文章编号
        /// </summary>
        public int AID { get; set; }

        /// <summary>
        /// 标签编号
        /// </summary>
        public int TNum { get; set; }

        //导航属性
        public Article Article { get; set; }

        public Tag Tag { get; set; }
    }
}