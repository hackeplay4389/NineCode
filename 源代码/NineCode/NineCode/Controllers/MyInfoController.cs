using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Security;

namespace NineCode.Controllers
{
    public class MyInfoController : Controller
    {
        //
        // GET: /MyInfo/

        /// <summary>
        /// 邮件验证码全局变量
        /// </summary>
        private static string mailCode;
        private static string mail;

        /// <summary>
        /// 获取当前用户的安全邮箱
        /// </summary>
        /// <returns></returns>
        private string GetMail()
        {
            return DBHelper.GetScalar("select UMail from Users where UNum=" + User.Identity.Name + "").ToString();
        }

        /// <summary>
        /// 发送邮件验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult SendMail()
        {
            string to = GetMail();
            if (string.IsNullOrEmpty(to))
            {
                return Json(new { msg = "未设置安全邮箱", res = "false" }, JsonRequestBehavior.DenyGet);
            }
            MailHelper mail = new MailHelper();
            mail.To = to;
            mail.Title = "系统授权码";
            mailCode = MailHelper.RandomCode(12);
            mail.Body = "<h3>此次操作的授权码为：【<b>" + mailCode + "</b>】</h3>";
            bool result = mail.Send();
            return Json(new { msg = to, res = result.ToString().ToLower() }, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 个人基本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Base()
        {
            DataTable info = DBHelper.GetDT("exec GetUserInfoById " + User.Identity.Name + "");
            Models.Users u = new Models.Users();
            u.UNum = int.Parse(info.Rows[0]["UNum"].ToString());
            u.UName = info.Rows[0]["UName"].ToString();
            u.UMail = info.Rows[0]["UMail"].ToString();
            u.ULogin = DateTime.Parse(info.Rows[0]["ULogin"].ToString());
            u.UState = info.Rows[0]["UState"].ToString() == "true" ? "正常" : "禁用";
            ViewData["ACount"] = info.Rows[0]["ACount"].ToString();
            return View(u);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Change()
        {
            GetMail();
            return View();
        }

        /// <summary>
        /// 重置密码操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult ResetPass(Models.Users u)
        {
            string pass, newpass, code;
            try
            {
                ///借助Users实体传递所需值
                pass = u.UPass;
                newpass = u.UName;
                code = u.UState;
            }
            catch
            {
                return Json(new { res = "false", msg = "数据解析异常" }, JsonRequestBehavior.DenyGet);
            }
            pass = FormsAuthentication.HashPasswordForStoringInConfigFile(pass, "md5");
            string nowpass = DBHelper.GetScalar("select UPass from Users where UNum=" + User.Identity.Name + "").ToString();
            if (pass != nowpass)
            {
                return Json(new { res = "false", msg = "登录密码错误！" }, JsonRequestBehavior.DenyGet);
            }
            if (!string.IsNullOrEmpty(mail))
            {
                if (code != mailCode)
                {
                    return Json(new { res = "false", msg = "授权码校验失败！" }, JsonRequestBehavior.DenyGet);
                }
            }
            newpass = FormsAuthentication.HashPasswordForStoringInConfigFile(newpass, "md5");
            string sql = "update Users set UPass='" + newpass + "' where UNum=" + User.Identity.Name + "";
            if (DBHelper.GetLine(sql) > 0)
            {
                return Json(new { res = "true", msg = "密码修改成功！" }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(new { res = "false", msg = "密码修改失败！" }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 安全邮箱页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Mail()
        {
            mail = GetMail();
            ViewData["Mail"] = mail;
            return View();
        }


        /// <summary>
        /// 更改安全邮箱
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult ChangeMail(Models.Users u)
        {
            string newmail, pass, code, nowpass=string.Empty;
            newmail = u.UMail;
            pass = u.UPass;
            code = u.UState;
            if (!string.IsNullOrEmpty(mail) && string.IsNullOrEmpty(code))
            {
                return Json(new { res = "false", msg = "请输入操作授权码！" }, JsonRequestBehavior.DenyGet);
            }
            if (mailCode != code)
            {
                return Json(new { res = "false", msg = "授权码校验失败！" }, JsonRequestBehavior.DenyGet);
            }
            pass = FormsAuthentication.HashPasswordForStoringInConfigFile(nowpass, "md5");
            nowpass = DBHelper.GetScalar("select UPass from Users where UNum=" + User.Identity.Name + "").ToString();
            if (pass != nowpass)
            {
                return Json(new { res = "false", msg = "登录密码输入错误！" }, JsonRequestBehavior.DenyGet);
            }
            string sql = "update Users set UMail='" + newmail + "' where UNum=" + User.Identity.Name + "";
            if (DBHelper.GetLine(sql) > 0)
            {
                return Json(new { res = "true", msg = "安全邮箱修改成功！" }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(new { res = "false", msg = "安全邮箱修改失败！" }, JsonRequestBehavior.DenyGet);
            }
        }

    }
}
