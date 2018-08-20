using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NineCode.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        //验证码结果
        private static string codeResult;

        //登录
        [HttpGet]
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
            {
                return Redirect("~/Admin/Index");
            }
            return View();
        }


        //注销
        [Authorize]
        public ActionResult OutLogin()
        {
            FormsAuthentication.SignOut();
            return Redirect("~");
        }


        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="u">登录数据</param>
        /// <returns>前台验证</returns>
        [HttpPost]
        public ActionResult LoginState(Models.Users u)
        {
            //验证码
            string code = RouteData.Values["id"].ToString();
            if (code.ToUpper() != codeResult)
            {
                return Content("Code");
            }
            if (!ModelState.IsValid)
            {
                return Content("false");
            }
            //md5密码加密 上线后开启
            u.UPass = FormsAuthentication.HashPasswordForStoringInConfigFile(u.UPass,"md5");
            //通过验证
            string res = DBHelper.GetScalar("exec CheckLogin '" + u.UName + "','" + u.UPass + "'").ToString().Trim();
            if (res == "State")
            {
                return Content(res);
            }
            else if (res == "false")
            {
                return Content(res);
            }
            else {
                //生成认证Session
                FormsAuthentication.SetAuthCookie(res, true);
                return Content(res);
            }
            
        }


        //找回密码
        [HttpGet]
        public ActionResult LostPass()
        {
            return View();
        }

        /// <summary>
        /// 发送授权码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendMail(Models.Users u)
        {
            string mail = DBHelper.GetScalar("exec GetMailByName '"+u.UName+"'").ToString();
            if (mail == "false")
            {
                return Json(new { res="false",msg="您的帐号未绑定安全邮箱！"},JsonRequestBehavior.DenyGet);
            }
            MailHelper m = new MailHelper();
            codeResult = MailHelper.RandomCode(12);
            m.To = mail;
            m.Title = "重置密码";
            m.Body = "<h3>您此次重置密码操作的授权码是【"+codeResult+"】</h3>";
            if (m.Send())
            {
                codeResult = codeResult + "-" + u.UName;
                mail = mail.Substring(0, 1) + "***" + mail.Substring(mail.LastIndexOf('@')-3);
                return Json(new { res = "true", msg = "授权码已发送至："+mail }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(new { res = "false", msg = "邮件发送失败！" }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetPass(Models.Users u)
        {
            string code = codeResult.Split('-')[0].ToString();
            string name = codeResult.Split('-')[1].ToString();
            if (u.UName != name || u.UState != code)
            {
                return Json(new { res = "false", msg = "授权码验证失败！" }, JsonRequestBehavior.DenyGet);
            }
            u.UPass = FormsAuthentication.HashPasswordForStoringInConfigFile(u.UPass,"md5");
            string sql = "update Users set UPass='" + u.UPass + "' where UName='" + u.UName + "'";
            if (DBHelper.GetLine(sql) == 1)
            {
                return Json(new { res = "true", msg = "恭喜您，密码重置成功！" }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(new { res = "false", msg = "抱歉，密码重置失败！" }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 验证码实现
        /// </summary>
        [HttpGet]
        public ActionResult VCode()
        {
            Code gif = new Code();//初始化验证码生成类  
            string valid = "";//定义随机数  
            MemoryStream ms = gif.Create(out valid);//获取包括验证码图片的内存流  
            codeResult = valid.ToUpper();//验证码存储在Session中，供验证。  
            return File(ms.ToArray(), "image/jpeg");
        }

    }

    /// <summary>
    /// 验证码生成类
    /// </summary>
    public class Code
    {
        /// <summary>  
        /// 该方法用于生成指定位数的随机数  
        /// </summary>  
        /// <param name="VcodeNum">参数是随机数的位数</param>  
        /// <returns>返回一个随机数字符串</returns>  
        private string RndNum(int VcodeNum)
        {
            //验证码可以显示的字符集合  
            string Vchar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p" +
                ",q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q" +
                ",R,S,T,U,V,W,X,Y,Z";
            // string Vchar = "1,2,3,4,5,6,7,8,9";//======这里使用纯数字
            string[] VcArray = Vchar.Split(new Char[] { ',' });//拆分成数组  
            string VNum = "";//产生的随机数  
            int temp = -1;//记录上次随机数值，尽量避避免生产几个一样的随机数  

            Random rand = new Random();
            //采用一个简单的算法以保证生成随机数的不同  
            for (int i = 1; i < VcodeNum + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));//初始化随机类  
                }
                //int t = rand.Next(61);//获取随机数  
                int t = rand.Next(62);//=====与VcArray定义的长度匹配
                if (temp != -1 && temp == t)
                {
                    return RndNum(VcodeNum);//如果获取的随机数重复，则递归调用  
                }
                temp = t;//把本次产生的随机数记录起来  
                VNum += VcArray[t];//随机数的位数加一  
            }
            return VNum;
        }

        /// <summary>  
        /// 该方法是将生成的随机数写入图像文件  
        /// </summary>  
        /// <param name="VNum">VNum是一个随机数</param>  
        public MemoryStream Create(out string VNum)
        {
            VNum = RndNum(4);
            Bitmap Img = null;
            Graphics g = null;
            MemoryStream ms = null;
            System.Random random = new Random();
            //验证码颜色集合  
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            //验证码字体集合  
            string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };


            //定义图像的大小，生成图像的实例  
            Img = new Bitmap((int)VNum.Length * 18, 32);

            g = Graphics.FromImage(Img);//从Img对象生成新的Graphics对象    

            g.Clear(Color.White);//背景设为白色  

            //在随机位置画背景点  
            for (int i = 0; i < 100; i++)
            {
                int x = random.Next(Img.Width);
                int y = random.Next(Img.Height);
                g.DrawRectangle(new Pen(Color.LightGray, 0), x, y, 1, 1);
            }
            //验证码绘制在g中  
            for (int i = 0; i < VNum.Length; i++)
            {
                int cindex = random.Next(7);//随机颜色索引值  
                int findex = random.Next(5);//随机字体索引值  
                Font f = new System.Drawing.Font(fonts[findex], 20, System.Drawing.FontStyle.Bold);//字体======================字体大小  
                Brush b = new System.Drawing.SolidBrush(c[cindex]);//颜色  
                int ii = 4;
                if ((i + 1) % 2 == 0)//控制验证码不在同一高度  
                {
                    ii = 2;
                }
                g.DrawString(VNum.Substring(i, 1), f, b, 3 + (i * 12), ii);//绘制一个验证字符  
            }
            ms = new MemoryStream();//生成内存流对象  
            Img.Save(ms, ImageFormat.Jpeg);//将此图像以Png图像文件的格式保存到流中  

            //回收资源  
            g.Dispose();
            Img.Dispose();
            return ms;
        }
    }

}
