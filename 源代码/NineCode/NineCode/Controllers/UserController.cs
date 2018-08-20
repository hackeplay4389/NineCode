using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Security;

namespace NineCode.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        private static string url = url = "/Admin/Tip/" + System.Web.HttpUtility.UrlEncode("warning,抱歉，您无权操作此功能！", System.Text.Encoding.Default) + "?url=/Admin/Index";

        /// <summary>
        /// 网站管理员列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult AdminList()
        {
            if (User.Identity.Name != "10000")
            {
                return Redirect(url);
            }
            DataTable data = DBHelper.GetDT("exec GetUserList");
            ViewData["Data"] = data;
            return View();
        }

        /// <summary>
        /// 操作网站管理员账户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult MgrAdmin()
        {
            if (User.Identity.Name != "10000")
            {
                return Redirect(url);
            }
            string val = string.Empty;
            try
            {
                val = RouteData.Values["id"].ToString();
            }
            catch
            {
                return Json(new { msg = "发送指令错误！" }, JsonRequestBehavior.DenyGet);
            }
            string type = val.Substring(0, 1);
            switch (type)
            {
                case "S":
                case "D":
                    type = DBHelper.GetScalar("exec MgrAdmin '" + type + "'," + val.Substring(1) + "").ToString();
                    break;
                case "R":
                    //默认新密码
                    string newPass = FormsAuthentication.HashPasswordForStoringInConfigFile("123456", "md5");
                    type = DBHelper.GetScalar("exec MgrAdmin '" + type + "'," + val.Substring(1) + ",'"+newPass+"'").ToString();
                    break;
                default:
                    type = "发送指令错误！";
                    break;
            }
            return Json(new { msg=type},JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 添加会员
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult AddUser()
        {
            if (User.Identity.Name != "10000")
            {
                return Redirect(url);
            }
            return View();
        }

        /// <summary>
        /// 添加会员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult AddAdminUser(Models.Users u)
        {
            if (User.Identity.Name != "10000")
            {
                return Redirect(url);
            }
            if (!ModelState.IsValid)
            {
                return Json(new { msg="信息验证失败！"},JsonRequestBehavior.DenyGet);
            }
            u.UPass=FormsAuthentication.HashPasswordForStoringInConfigFile(u.UPass, "md5");
            string res = DBHelper.GetScalar("exec AddAdminUser '"+u.UName+"','"+u.UPass+"','"+u.UMail+"'").ToString();
            return Json(new { msg = res }, JsonRequestBehavior.DenyGet);
        }
    }
}
