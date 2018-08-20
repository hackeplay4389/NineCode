using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace NineCode.Controllers
{
    public class ManagerController : Controller
    {
        //
        // GET: /Manager/

        /// <summary>
        /// 分类管理页
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult Category()
        {
            ViewData["Category"] = DBHelper.GetDT("exec GetCategoryList");
            return View();
        }

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult AddClass()
        {
            string value;
            try
            {
                value = RouteData.Values["id"].ToString();
            }
            catch
            {
                return Content("false");
            }
            return Content(DBHelper.GetScalar("exec MgrCategory 'I',default,'" + value + "'").ToString());
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult DelClass()
        {
            int id;
            try
            {
                id = int.Parse(RouteData.Values["id"].ToString());
            }
            catch
            {
                return Content("false");
            }
            return Content(DBHelper.GetScalar("exec MgrCategory 'D'," + id ).ToString());
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult ChgClass()
        {
            int id;
            string name;
            try
            {
                id =int.Parse(RouteData.Values["id"].ToString());
                name =Request.QueryString["name"].ToString();
            }
            catch
            {
                return Content("false");
            }
            return Content(DBHelper.GetScalar("exec MgrCategory 'U'," + id + ",'" + name + "'").ToString());
        }


        /// <summary>
        /// 标签管理页
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult Tags()
        {
            int i;
            try
            {
                i = int.Parse(Request.QueryString["nav"].ToString());
            }
            catch
            {
                i = 0;
            }
            DataTable dt = DBHelper.GetDT("exec GetTagList");
            PageData pd = new PageData(dt, i, 15);
            ViewData["Tags"] = pd.Result;
            ViewData["PageInfo"] = pd.PageInfo;
            return View();
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult DelTag()
        {
            int id;
            try
            {
                id = int.Parse(RouteData.Values["id"].ToString());
            }
            catch
            {
                return Content("false");
            }
            return Content(DBHelper.GetScalar("exec MgrTag 'D'," + id ).ToString());
        }

        /// <summary>
        /// 修改标签
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult ChgTag()
        {
            int id;
            string name;
            try
            {
                id = int.Parse(RouteData.Values["id"].ToString());
                name = Request.QueryString["name"].ToString();
            }
            catch
            {
                return Content("false");
            }
            return Content(DBHelper.GetScalar("exec MgrTag 'U'," + id + ",'" + name + "'").ToString());
        }


    }
}
