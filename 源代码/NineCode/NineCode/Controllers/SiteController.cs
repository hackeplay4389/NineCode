using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace NineCode.Controllers
{
    public class SiteController : Controller
    {
        /// <summary>
        /// 权限控制
        /// </summary>
        private void Can()
        {
            if (User.Identity.Name != "10000")
            {
                string url = url = "/Admin/Tip/" + System.Web.HttpUtility.UrlEncode("warning,抱歉，您无权操作此功能！", System.Text.Encoding.Default) + "?url=/Admin/Index";
                Response.Redirect(url);
                Response.End();
            }
        }

        /// <summary>
        /// 基本信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Base()
        {
            DataTable info = DBHelper.GetDT("select * from SiteInfo");
            Models.SiteInfo si = new Models.SiteInfo();
            si.ITitle = info.Rows[0]["ITitle"].ToString();
            si.ISmall = info.Rows[0]["ISmall"].ToString();
            si.ISEO = info.Rows[0]["ISEO"].ToString();
            si.IICP = info.Rows[0]["IICP"].ToString();
            si.IBei = info.Rows[0]["IBei"].ToString();
            si.ICount = info.Rows[0]["ICount"].ToString();
            return View(si);
        }

        /// <summary>
        /// 更新基本信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult UpInfo(Models.SiteInfo si)
        {
            string sql = "update SiteInfo set ITitle='" + si.ITitle +
               "',ISmall='" + si.ISmall +
               "',ISEO='" + si.ISEO +
               "',IICP='" + si.IICP +
               "',IBei='" + si.IBei +
               "',ICount='" + si.ICount + "' where IID=1";
            if (DBHelper.GetLine(sql) == 1)
            {
                return Json(new { res = "true", msg = "网站信息更新成功！" });
            }
            else
            {
                return Json(new { res = "false", msg = "网站信息更新失败！" });
            }
        }

        /// <summary>
        /// 邮件服务器设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Mail()
        {
            Can();
            DataTable cfg = DBHelper.GetDT("select * from SiteConfig");
            Models.SiteConfig sc = new Models.SiteConfig();
            sc.CfgServer = cfg.Rows[0]["CfgServer"].ToString();
            sc.CfgPort = cfg.Rows[0]["CfgPort"].ToString();
            sc.CfgUser = cfg.Rows[0]["CfgUser"].ToString();
            sc.CfgPass = cfg.Rows[0]["CfgPass"].ToString();
            sc.CfgFrom = cfg.Rows[0]["CfgFrom"].ToString();
            return View(sc);
        }


        /// <summary>
        /// 发送测试邮件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult TestSend()
        {
            Can();
            string to;
            try
            {
                to = Request.QueryString["to"].ToString();
            }
            catch
            {
                return Json(new { res = "false", msg = "未发现收件人地址！" }, JsonRequestBehavior.DenyGet);
            }
            MailHelper mail = new MailHelper();
            mail.To = to;
            mail.Title = "系统测试邮件";
            mail.Body = "<h3>当您看到这封邮件时，说明您的SMTP服务配置成功！</h3>";
            if (mail.Send())
            {
                return Json(new { res = "true", msg = "测试邮件已发送至您的邮箱！" }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(new { res = "false", msg = "测试邮件发送失败！" }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 更新配置信息
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult ChangeMail(Models.SiteConfig sc)
        {
            string sql = "update SiteConfig set CfgServer='" + sc.CfgServer +
                "',CfgPort=" + sc.CfgPort +
                ",CfgUser='" + sc.CfgUser +
                "',CfgPass='" + sc.CfgPass +
                "',CfgFrom='" + sc.CfgFrom + "' where CfgID=1";
            if (DBHelper.GetLine(sql) == 1)
            {
                return Json(new { res = "true", msg = "配置信息更新成功！" });
            }
            else
            {
                return Json(new { res = "false", msg = "配置信息更新失败！" });
            }
        }

        /// <summary>
        /// 网站开关设置
        /// </summary>
        /// <returns></returns>
        public ActionResult Off()
        {
            DataTable info = DBHelper.GetDT("select IIsOff,IWhyOff from SiteInfo");
            Models.SiteInfo si = new Models.SiteInfo();
            si.IIsOff = info.Rows[0]["IIsOff"].ToString();
            si.IWhyOff = info.Rows[0]["IWhyOff"].ToString();
            return View(si);
        }

        /// <summary>
        /// 更新网站状态
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult UpState(Models.SiteInfo si)
        {
            string sql = "update SiteInfo set IIsOff='" + si.IIsOff +
                "',IWhyOff='" + si.IWhyOff + "' where IID=1";
            if (DBHelper.GetLine(sql) == 1)
            {
                return Json(new { res = "true", msg = "操作执行成功！" });
            }
            else
            {
                return Json(new { res = "false", msg = "操作执行失败！" });
            }
        }


        /// <summary>
        /// SQL执行页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SQL()
        {
            Can();
            return View();
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult ExeSql(Models.SiteConfig sc)
        {
            Can();
            int lines = -1;
            try
            {
                lines = DBHelper.GetLine(sc.CfgServer);
            }
            catch
            {
                return Json(new { res = "false", msg = "SQL语句执行失败！" });
            }

            if (lines <= 0)
            {
                return Json(new { res = "false", msg = "SQL语句执行失败！" });
            }
            else
            {
                return Json(new { res = "true", msg = "语句执行成功，受影响行数：" + lines });
            }
        }

    }
}
