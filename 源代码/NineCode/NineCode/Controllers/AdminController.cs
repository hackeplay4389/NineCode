using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Security;


namespace NineCode.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        /// <summary>
        /// 控制台首页
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            ViewData["Data"] = DBHelper.GetDT("exec GetCount "+User.Identity.Name);
            System.Net.WebClient client = new System.Net.WebClient();
            ViewData["IP"] = client.DownloadString("http://app.ninecms.com/Server/GetIP");
            ViewData["OS"] = System.Environment.OSVersion.Platform.ToString();
            string name = DBHelper.GetScalar("select DB_NAME(dbid) from master.dbo.sysprocesses where status='runnable'").ToString();
            DataTable db = DBHelper.GetDT("select (select @@VERSION) as ver, size,(max_size-size)as canuse from sys.database_files where name='"+name+"'");
            ViewData["DB"] = db;
            return View();
        }

        /// <summary>
        /// 操作提示页面
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult Tip()
        {
            string[] msg;
            try
            {
                string str = System.Web.HttpUtility.UrlDecode(RouteData.Values["id"].ToString(), System.Text.Encoding.Default);
                msg = str.Split(',');
            }
            catch
            {
                return Redirect("~/Admin/Index");
            }
            string url = string.Empty;
            try
            {
                url = Request.QueryString["url"].ToString();
            }
            catch
            {
                url = string.Empty;
            }
            ViewBag.Type = msg[0];
            ViewBag.Info = msg[1];
            if (string.IsNullOrEmpty(url))
            {
                ViewBag.Before = HttpContext.Request.UrlReferrer.ToString();
            }
            else
            {
                ViewBag.Before = "http://"+HttpContext.Request.Url.Authority+url;
            }
            return View();
        }

    }
}
