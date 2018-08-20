using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;

namespace NineCode.Controllers
{
    public class ArticleController : Controller
    {
        /// <summary>
        /// 添加、编辑文章
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult Editor()
        {
            Models.Article a = new Models.Article();
            string id;
            try
            {
                id = RouteData.Values["id"].ToString();
            }
            catch
            {
                id = "";
            }
            ViewData["Category"] = DBHelper.GetDT("exec GetCategory");
            int aid;
            if (int.TryParse(id, out aid))
            {
                //修改文章
                DataTable dt = null;
                try
                {
                    dt = DBHelper.GetDT("exec GetArticleById " + id);
                    a.AID = int.Parse(dt.Rows[0]["AID"].ToString());
                    a.ATitle = dt.Rows[0]["ATitle"].ToString();
                    a.AText = dt.Rows[0]["AText"].ToString();
                    a.ATime = DateTime.Now;
                    a.UNum = int.Parse(User.Identity.Name.ToString());
                    a.Category = new Models.Category();
                    a.Category.CName = dt.Rows[0]["CName"].ToString();
                }
                catch
                {
                    return Content("false");
                }
                DataTable tags = DBHelper.GetDT("exec GetTagById " + a.AID);
                string tag = "";
                for (int i = 0; i < tags.Rows.Count; i++)
                {
                    tag += ("@" + tags.Rows[i]["TName"].ToString() + " ");
                }
                ViewData["Tag"] = tag;
                ViewData["Type"] = "Up";
            }
            else
            {
                //添加文章
                string res = DBHelper.GetScalar("exec NewArticel " + User.Identity.Name.ToString()).ToString();
                a.AID = int.Parse(res);
                a.ATime = DateTime.Now;
                a.ATitle = "";
                a.AText = "";
                a.UNum = int.Parse(User.Identity.Name.ToString());
                a.Category = new Models.Category();
                a.Category.CName = "";
                ViewData["Tag"] = "";
                ViewData["Type"] = "New";
            }
            return View(a);
        }

        /// <summary>
        /// 保存文章修改
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult UpArticle(Models.Article a)
        {
            string tag = "";
            try {
                tag = RouteData.Values["id"].ToString();
            }
            catch{
                tag="";
            }
            a.ATitle = string.IsNullOrEmpty(a.ATitle.Trim()) ? "Null_Title" : a.ATitle;
            a.AText = string.IsNullOrEmpty(a.AText.Trim()) ? "NineCode" : a.AText;
            string sql = "exec UpArticle " + a.AID +
                ",'" + a.ATitle +
                "','" + a.AText +
                "'," + a.CID +
                "," + a.UNum + "";
            UpTag(tag, a.AID);
            string res = DBHelper.GetScalar(sql).ToString();
            if (res == "true")
                res = "tip,恭喜您，文章操作成功！";
            else
                res = "warning,抱歉，文章操作失败！";
            res = "/Admin/Tip/" + System.Web.HttpUtility.UrlEncode(res, System.Text.Encoding.Default);
            return Json(new { url = res }, JsonRequestBehavior.DenyGet);
        }


        /// <summary>
        /// 删除文章
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult DelArticle()
        {
            try
            {
                int aid = int.Parse(RouteData.Values["id"].ToString());
                string sql = "exec DelArticle " + aid;
                string res = DBHelper.GetScalar(sql).ToString();
                return Content(res);
            }
            catch
            {
                return Content("false");
            }
        }


        /// <summary>
        /// 更新文章标签
        /// </summary>
        private void UpTag(string tags, int aid)
        {
            if (!string.IsNullOrEmpty(tags))
            {
                DBHelper.GetScalar("exec ClearTag " + aid);
                string[] arr;
                try
                {
                    arr = tags.Trim().Split('@');
                }
                catch
                {
                    arr = null;
                }
                for (int i = 1; i < arr.Length; i++)
                {
                    string tag = arr[i].Trim();
                    DBHelper.GetScalar("exec AddTag '" + tag + "'," + aid);
                }
            }
        }


        /// <summary>
        /// 文章列表
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult List()
        {
            //清除冗余
            DBHelper.GetLine("exec ClearNullArticle");
            string id;
            int i;
            try
            {
                id = RouteData.Values["id"].ToString();
            }
            catch
            {
                id = "";

            }
            try
            {
                i = int.Parse(Request.QueryString["nav"].ToString());
            }
            catch
            {
                i = 0;
            }
            DataTable dt = DBHelper.GetDT("exec GetArticleByCID '" + id + "'");
            PageData pd = new PageData(dt, i, 15);
            ViewData["Category"] = DBHelper.GetDT("exec GetCategory");
            ViewData["List"] = pd.Result;
            ViewData["PageInfo"] = pd.PageInfo;
            ViewData["Key"] = id;
            return View();
        }

    }
}
