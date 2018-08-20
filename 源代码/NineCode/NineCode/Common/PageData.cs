/*
 * 功能：DataTable数据分页处理类
 * 时间：2018年1月14日
 * 版本：1.2
 * 作者：Jiuone
 * 描述：
 *      传入DataTable类型数据源、当前页码、单页显示数量
 *      以DataTable类型返回当前页的数据
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace NineCode
{
    /// <summary>
    /// DataTable数据分页处理类
    /// </summary>
    public class PageData
    {
        //成员属性
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// 每页显示数据数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 数据源总页数
        /// </summary>
        private int _PageCount;
        public int PageCount 
        {
            get
            {
                return _PageCount;
            }
        }

        /// <summary>
        /// 数据源 DataTable类型
        /// </summary>
        public DataTable DataSource { get; set; }

        /// <summary>
        /// 分页类返回的分页数据 DataTable类型
        /// </summary>
        private DataTable _Result;
        public DataTable Result 
        {
            get 
            {
                return _Result;
            }
        }

        /// <summary>
        /// 分页相关信息 HashTable类型
        /// </summary>
        private Hashtable _PageInfo;
        public Hashtable PageInfo 
        {
            get 
            {
                return _PageInfo;
            }
        }

        //重载方法
        /// <summary>
        /// DataTable类型数据分页类
        /// </summary>
        /// <param name="dt">DataTable类型的数据源</param>
        /// <param name="cs">int类型的当前页面码</param>
        /// <param name="ps">int类型的每页显示数据数量</param>
        public PageData(DataTable dt, int cs, int ps)
        {
            DataSource = dt;
            CurrentIndex = cs;
            PageSize = ps;
            CutData();
        }
        /// <summary>
        /// DataTable数据分页类
        /// </summary>
        /// <param name="dt">DataTable类型的数据源,需额外设置</param>
        public PageData(DataTable dt)
        {
            DataSource = dt;
        }
        /// <summary>
        /// DataTable数据分页类
        /// </summary>
        public PageData()
        {
        }


        //成员方法
        /// <summary>
        /// 数据分页操作方法
        /// </summary>
        private void CutData()
        {
            _Result = DataSource.Clone();
            int min = (CurrentIndex * PageSize);
            int max = min + PageSize;
            if (max > DataSource.Rows.Count)
            {
                max = DataSource.Rows.Count;
            }
            for (int i = min; i < max; i++)
            {

                Result.ImportRow(DataSource.Rows[i]);
            }
            ViewInfo();
        }

        /// <summary>
        /// 返回分页数据相关信息
        /// </summary>
        private void ViewInfo()
        {
            double pCount;
            try
            {
                pCount = DataSource.Rows.Count / int.Parse(PageSize.ToString());
            }
            catch
            {
                pCount = 0;
            }
            _PageCount = int.Parse((Math.Floor(pCount+1)).ToString());
            Hashtable ht = new Hashtable();
            ht.Add("Count",DataSource.Rows.Count);  //数据源数据总数
            ht.Add("Current",CurrentIndex);  //当前页索引，【0】 开始
            ht.Add("Size", PageSize);  //每页所展示的数据行数
            ht.Add("Pages",_PageCount);  //所需页数
            _PageInfo=ht;
        }
    }
}