using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NineCode.Models
{
    /// <summary>
    /// 文章表
    /// </summary>
    public class Article
    {
        /// <summary>
        /// 文章编号
        /// </summary>
        [Key]
        public int AID { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        public string ATitle { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        public string AText { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime ATime { get; set; }

        /// <summary>
        /// 操作用户ID
        /// </summary>
        public int UNum { get; set; }

        /// <summary>
        /// 所属分类ID
        /// </summary>
        public int CID { get; set; }


        //导航属性
        public Users Users { get; set; }

        public Category Category { get; set; }

    }
}