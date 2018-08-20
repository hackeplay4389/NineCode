using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NineCode.Models
{
    /// <summary>
    /// 分类表
    /// </summary>
    public class Category
    {
        /// <summary>
        /// 分类编号
        /// </summary>
        public int CID { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CName { get; set; }

    }
}