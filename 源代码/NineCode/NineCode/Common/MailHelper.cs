using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Data;
using System.Text;

namespace NineCode
{
    public class MailHelper
    {
        //********************************
        //**      邮件设置实体          **
        //********************************
        /// <summary>
        /// SMTP主机地址
        /// </summary>
        private string Server { get; set; }
        /// <summary>
        /// SMTP主机端口
        /// </summary>
        private int Port { get; set; }
        /// <summary>
        /// SMTP登录帐号
        /// </summary>
        private string User { get; set; }
        /// <summary>
        /// SMTP登录密码
        /// </summary>
        private string Pass { get; set; }
        /// <summary>
        /// 发件人地址
        /// </summary>
        private string From { get; set; }
        /// <summary>
        /// 收件人地址
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 邮件正文
        /// </summary>
        public string Body { get; set; }


        /// <summary>
        /// 加载SMTP服务器设置
        /// </summary>
        private void LoadData()
        {
            DataTable data = DBHelper.GetDT("select * from SiteConfig");
            this.Server = data.Rows[0]["CfgServer"].ToString();
            this.Port = int.Parse(data.Rows[0]["CfgPort"].ToString());
            this.User = data.Rows[0]["CfgUser"].ToString();
            this.Pass = data.Rows[0]["CfgPass"].ToString();
            this.From = data.Rows[0]["CfgFrom"].ToString();
        }

        /// <summary>
        /// 发送邮件方法
        /// </summary>
        /// <returns>是否发送成功</returns>
        public bool Send()
        {
            //结果变量
            bool result = true;

            //加载邮件设置
            LoadData();

            try
            {
                SmtpClient client = new SmtpClient(this.Server, this.Port);   //邮箱SMTP服务器地址

                client.UseDefaultCredentials = false;//是否和请求一起发送

                client.DeliveryMethod = SmtpDeliveryMethod.Network; //通过网络发送到SMTP服务器

                client.Credentials = new NetworkCredential(this.User, this.Pass); //SMTP服务账户、密码

                MailMessage mail = new MailMessage(new MailAddress(this.From), new MailAddress(this.To)); //发件人、收件人的邮箱地址

                mail.Subject = this.Title;      //邮件标题

                mail.SubjectEncoding = Encoding.UTF8;   //邮件标题编码

                mail.Body = this.Body;         //邮件正文

                mail.BodyEncoding = Encoding.UTF8;      //邮件正文编码格式

                mail.IsBodyHtml = true;    //邮件正文是否设置为HTML格式          

                mail.Priority = MailPriority.High;   //优先级

                client.Send(mail);
            }
            catch
            {
                result = false;
            }
            return result;
        }


        /// <summary>
        /// 生成指定长度的随机字符串
        /// </summary>
        /// <param name="length">随机字符串的长度</param>
        /// <returns></returns>
        public static string RandomCode(int length)
        {
            string code = string.Empty;
            char[] master ={'0','1','2','3','4','5','6','7','8','9',
                            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};
            Random index = new Random();
            for (int i = 0; i < length; i++)
            {
                code += master[index.Next(0, master.Length)];
            }
            return code;
        }
    }
}