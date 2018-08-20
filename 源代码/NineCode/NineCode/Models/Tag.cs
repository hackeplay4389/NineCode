using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NineCode.Models
{
    /// <summary>
    /// 标签表
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// 标签ID
        /// </summary>
        [Key]
        public int TNum { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string TName { get; set; }

    }
}