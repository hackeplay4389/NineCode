using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Collections;

namespace NineCode.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /Main/
        /// <summary>
        /// 网站首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            IsOff();
            return View();
        }

        /// <summary>
        /// 搜索结果
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult List()
        {
            IsOff();
            string key = "";
            int index;
            try
            {
                key = RouteData.Values["id"].ToString();
                index = int.Parse(Request.QueryString["nav"].ToString());
            }
            catch
            {
                if (string.IsNullOrEmpty(key))
                {
                    return Redirect("~");
                }
                else
                {
                    index = 0;
                }
            }
            if (key.Length > 50)
            {
                key = key.Substring(0, 50);
            }
            if (index < 0)
            {
                index = Math.Abs(index);
            }
            DataTable dt = DBHelper.GetDT("exec SearchArticle '" + key + "'");
            PageData pd = new PageData(dt, index, 10);
            ViewData["List"] = pd.Result;
            ViewData["Info"] = pd.PageInfo;
            ViewData["Key"] = key;
            return View();
        }


        /// <summary>
        /// 文章详情页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Details()
        {
            IsOff();
            int id;
            try
            {
                id = int.Parse(RouteData.Values["id"].ToString());
            }
            catch
            {
                return Redirect("~");
            }
            DataTable dt = DBHelper.GetDT("exec GetArticleById " + id + "");
            Models.Article a = new Models.Article();
            a.AID = int.Parse(dt.Rows[0]["AID"].ToString());
            a.ATitle = dt.Rows[0]["ATitle"].ToString();
            a.AText = dt.Rows[0]["AText"].ToString();
            a.ATime = DateTime.Parse(dt.Rows[0]["ATime"].ToString());
            a.Users = new Models.Users();
            a.Users.UName = dt.Rows[0]["UName"].ToString();
            a.Category = new Models.Category();
            a.Category.CName = dt.Rows[0]["CName"].ToString();
            dt = DBHelper.GetDT("exec GetBrother " + id + "");
            ViewData["Prev"] = dt.Rows[0]["Prev"].ToString();
            ViewData["Next"] = dt.Rows[0]["Next"].ToString();
            ViewData["Tags"] = DBHelper.GetDT("exec GetTagById " + id + "");
            return View(a);
        }


        /// <summary>
        /// 网站暂停服务
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Warning()
        {
            DataTable info = DBHelper.GetDT("select IIsOff,IWhyOff from SiteInfo");
            Models.SiteInfo si = new Models.SiteInfo();
            si.IIsOff = info.Rows[0]["IIsOff"].ToString();
            si.IWhyOff = info.Rows[0]["IWhyOff"].ToString();
            return View(si);
        }

        /// <summary>
        /// 是否关站
        /// </summary>
        private void IsOff()
        {
            string res = DBHelper.GetScalar("select IIsOff from SiteInfo where IID=1").ToString();
            if (res == "true")
            {
                Response.Redirect("/Main/Warning");
                Response.End();
            }
        }


        

    }
}
