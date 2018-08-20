using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;

namespace NineCode.Controllers
{
    public class MediaController : Controller
    {
        //
        // GET: /Media/

        /// <summary>
        /// 文章图片列表页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Image()
        {
            DataTable dt = null;
            int i;
            try
            {
                i = int.Parse(Request.QueryString["nav"].ToString());
            }
            catch
            {
                i = 0;
            }
            string key = "";
            try
            {
                key = RouteData.Values["id"].ToString();
            }
            catch
            {
                key = "";
            }
            if (string.IsNullOrEmpty(key))
            {
                dt = DBHelper.GetDT("exec GetArticileImg");
                PageData pd = new PageData(dt, i, 18);
                ViewData["PageInfo"] = pd.PageInfo;
                ViewData["List"] = pd.Result;
            }
            else
            {
                int id;
                string sql;
                if (int.TryParse(key, out id))
                    sql = "exec GetArticileImg 'N','" + id + "'";
                else
                    sql = "exec GetArticileImg 'T','" + key + "'";
                dt = DBHelper.GetDT(sql);
                PageData pd = new PageData(dt, i, 18);
                ViewData["PageInfo"] = pd.PageInfo;
                ViewData["List"] = pd.Result;
            }
            ViewData["Key"] = key;
            return View();
        }

        /// <summary>
        /// 删除文章图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult DelImage()
        {
            string url = "";
            try
            {
                url = Request.QueryString["url"].ToString();
            }
            catch
            {
                url = "";
            }
            string res = "";
            //删除文件
            try
            {
                string path = Server.MapPath("~" + url);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch
            {
                res = "false";
            }
            if (res != "false")
            {
                //删除数据
                res = DBHelper.GetScalar("exec DelArticleImage '" + url + "'").ToString();
            }
            return Content(res);
        }

        /// <summary>
        /// 上传文件单页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult UpLoad()
        {
            return View();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult UploadFile()
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            string resValue = string.Empty;
            string msgValue = string.Empty;
            //文件非空判断
            if (files.Count == 0)
            {
                return Json(new { res = "false", msg = "您未选择文件！" });
            }
            ////格式验证
            string fileName = Path.GetFileName(files[0].FileName);
            string type = fileName.Substring(fileName.LastIndexOf('.'));
            string[] allType = new string[] { "gif", "jpg", "jpeg", "png", "bmp", "doc", "docx", "xls", "xlsx", "ppt", "pptx", "pdf", "zip", "rar", "7z", "mp3", "mp4", "avi", "txt", "wav", "wma", "swf" };
            for (int i = 0; i < allType.Length; i++)
            {
                if (type == "." + allType[i])
                {
                    resValue = "true";
                    break;
                }
            }
            if (resValue != "true")
            {
                return Json(new { res = "false", msg = "上传的文件格式有误！" });
            }
            //目录验证
            string filePath = string.Empty;
            try
            {
                string dirPath = "~/UpLoad/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/";
                if (!Directory.Exists(Server.MapPath(dirPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(dirPath));
                }
                //保存文件
                filePath = dirPath + fileName;
                if (System.IO.File.Exists(Server.MapPath(filePath)))
                {
                    return Json(new { res = "false", msg = "该文件已存在，请勿重复上传！" });
                }
                files[0].SaveAs(Server.MapPath(filePath));
            }
            catch
            {
                return Json(new { res = "false", msg = "文件处理失败！" });
            }
            //更新数据库
            filePath = filePath.Remove(0, 1);
            string sql = "exec AddFile '" + fileName + "','" + filePath + "','" + type.Remove(0, 1).ToUpper() + "'," + User.Identity.Name.ToString() + ",0";
            if (DBHelper.GetScalar(sql).ToString() == "true")
            {
                resValue = "true";
                msgValue = filePath;
            }
            else
            {
                resValue = "false";
                msgValue = "数据库操作失败！";
            }
            return Json(new { res = resValue, msg = msgValue });
        }

        /// <summary>
        /// 文件列表页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult List()
        {
            string key = string.Empty;
            int nav = 0;
            try
            {
                key = RouteData.Values["id"].ToString();
            }
            catch
            {
                key = string.Empty;
            }
            try
            {
                nav = int.Parse(Request.QueryString["nav"].ToString());
            }
            catch
            {
                nav = 0;
            }
            DataTable data = null;
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    data = DBHelper.GetDT("exec GetMediaList");
                }
                else
                {
                    data = DBHelper.GetDT("exec GetMediaList '" + key.Substring(0, 1) + "','" + key.Substring(1) + "'");
                }
            }
            catch
            {
                return Content("非法操作！");
            }
            DataTable types = DBHelper.GetDT("exec GetMediaType");
            ViewData["Type"] = types;
            PageData pd = new PageData(data, nav, 15);
            ViewData["PageInfo"] = pd.PageInfo;
            ViewData["Data"] = pd.Result;
            ViewData["Key"] = key;
            return View();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult DelFile()
        {
            string ids = string.Empty;
            try
            {
                ids = RouteData.Values["id"].ToString();
            }
            catch
            {
                return Content("false");
            }
            ids = ids.Substring(1);
            string []id=ids.Split(',');
            try
            {
                for (int i = 0; i < id.Length; i++)
                {
                    //获取URL
                    string url = DBHelper.GetScalar("exec GetUrlById " + id[i] + "").ToString();
                    //删除文件
                    string filepath = Server.MapPath("~"+url);
                    if (System.IO.File.Exists(filepath))
                    {
                        System.IO.File.Delete(filepath);
                    }
                    //删除数据库数据
                    DBHelper.GetLine("exec DelMediaFile "+id[i]+"");
                }
            }
            catch
            {
                return Content("false");
            }
            return Content("true");
        }

        /// <summary>
        /// 文章上传图片
        /// </summary>
        /// <param name="upImg"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult UpImg(HttpPostedFileBase upImg)
        {
            string error = "";
            bool res = true;
            string fileName = Path.GetFileName(upImg.FileName);
            string pic = "~/UpLoad/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/";
            try
            {
                //后缀验证
                Regex reg = new Regex("^\\w+(.gif|.jpg|.jpeg|.png|.bmp)$", RegexOptions.IgnoreCase);
                if (reg.IsMatch(fileName))
                {
                    string newName = DateTime.Now.ToString("yyyyMMddHHmmss") + fileName.Substring(fileName.LastIndexOf('.'));
                    if (!Directory.Exists(Server.MapPath(pic)))
                    {
                        Directory.CreateDirectory(Server.MapPath(pic));
                    }
                    upImg.SaveAs(Server.MapPath(pic) + fileName);
                    System.IO.File.Move(Server.MapPath(pic) + fileName, Server.MapPath(pic) + newName);  //重命名
                    pic += newName;
                    pic = pic.Remove(0, 1);
                    //附件库更新
                    string aid = RouteData.Values["id"].ToString();
                    string sql = "exec AddFile '" + newName + "','" + pic + "','"+fileName.Substring(fileName.LastIndexOf('.')+1).ToUpper()+"'," + User.Identity.Name + "," + aid;
                    if (DBHelper.GetScalar(sql).ToString() == "false")
                    {
                        res = false;
                        error = "附件库更新失败,操作已被取消！";
                        System.IO.File.Delete(Server.MapPath(pic));
                    }
                }
                else
                {
                    res = false;
                    error = "上传的文件的格式有误！";
                }
            }
            catch (Exception ex)
            {
                res = false;
                error = ex.Message;
            }
            return Json(new
            {
                success = res,
                msg = error,
                file_path = pic
            });
        }
    }
}
