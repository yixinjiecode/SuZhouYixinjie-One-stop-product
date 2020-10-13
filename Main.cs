using System;
using System.Collections.Generic;
using System.Text;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace YXJ.Common
{
    /// <summary>
    /// 一些常用方法、函数
    /// </summary>
    ///
    public class Main
    {
        public Main()
        {
        }
        #region 配置信息
        private static string _connectionString = null;
        /// <summary>
        /// 获得数据库连接字符串
        /// </summary>
        /// <param name="DatabaseUrl">数据库的名称 (*.mdb)</param>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            if (null == _connectionString)
            {
                _connectionString = Config.GetConfig("ConnectionString");
                if (!_connectionString.Contains("{0}"))
                {
                    return _connectionString;
                }
                else
                {
                    _connectionString= string.Format(_connectionString, Config.GetConfigDirectoryPath() + "DB.mdb");
                    return _connectionString;
                }
            }
            return _connectionString;
        }
        private static string _connectionString_Sqlserver = null;
        public static string GetConnection_SqlServer()
        {
            if (null == _connectionString_Sqlserver)
            {
                _connectionString_Sqlserver = Config.GetConfig("ConnectionString_SqlServer");
            }
            return _connectionString_Sqlserver;
        }
        #endregion

        #region 添加样式表文件、脚本文件等
        public static void AddCssStyle(string CssAddress)
        {

            Page page = (Page)HttpContext.Current.Handler;
            System.Web.UI.HtmlControls.HtmlLink cssLink = new System.Web.UI.HtmlControls.HtmlLink();
            cssLink.Href = CssAddress;
            cssLink.Attributes.Add("rel", "stylesheet");
            cssLink.Attributes.Add("type", "text/css");
            try
            {
                page.Header.Controls.Add(cssLink);
            }
            catch (Exception ex)
            {
                Log.WriteLog("无法将" + CssAddress + "添加到页面头中。e:" + ex.Message);
            }
        }

        public static void AddJavaScript(string JavascriptAddress)
        {
            Page page = (Page)HttpContext.Current.Handler;
            System.Web.UI.HtmlControls.
            HtmlGenericControl ScriptSetHomePage = new System.Web.UI.HtmlControls.HtmlGenericControl("script");
            ScriptSetHomePage.Attributes.Add("language", "javascript");
            ScriptSetHomePage.Attributes.Add("type", "text/javascript");
            ScriptSetHomePage.Attributes.Add("src", JavascriptAddress);
            page.Header.Controls.Add(ScriptSetHomePage);
        }
        #endregion

        #region Alert Function
        /// <summary>
        /// 在浏览器上显示Alert对话框，并且自动转向别一个页面
        /// </summary>
        /// <param name="ErrorMsg">Alert对话框的内容</param>
        /// <param name="RediretctUrl">转向的地址</param>
        public static void AlertAndRedirect(string ErrorMsg, string RediretctUrl)
        {
            HttpContext content = HttpContext.Current;
            string str = "";
            str += "<script language=\"javascript\">";
            str += "alert(\"" + ErrorMsg + "\");";
            str += "window.location.href=\"" + RediretctUrl + "\"";
            str += "</script>";
            content.Response.Write(str);
            content.Response.End();

        }
        public static void AlertAndGoBack(string AlertMsg)
        {
            string s = "<script language=\"Javascript\" defer>";
            s = s + "alert(\"" + AlertMsg + "\");" + "window.close();" + "</script>";
            HttpContext.Current.Response.Write(s);
            HttpContext.Current.Response.End();
        }
        public static void RegisterAlertScript(string AlertMsg)
        {
            Page handler = (Page)HttpContext.Current.Handler;
            ClientScriptManager manager = handler.ClientScript;
            Type type = handler.GetType();
            string str = "StartUpScript";
            if (!manager.IsStartupScriptRegistered(type, str))
            {
                string str2 = "alert(\"" + AlertMsg + "\");";
                manager.RegisterStartupScript(type, str, str2, true);
            }
        }
        public static void RegisterClientScript(string script)
        {
            Page handler = (Page)HttpContext.Current.Handler;
            ClientScriptManager manager = handler.ClientScript;
            Type type = handler.GetType();
            string str = "_Script";
            if (!manager.IsStartupScriptRegistered(type, str))
            {
                manager.RegisterStartupScript(type, str, script, true);
            }
        }
        /// <summary>
        /// 在浏览器上显示Alert对话框，并且关闭
        /// </summary>
        /// <param name="ErrorMsg">Alert对话框的内容</param>
        public static void AlertAndClose(string ErrorMsg)
        {
            HttpContext con = HttpContext.Current;
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("<script language=\"javascript\">");
            str.Append("alert(\"" + ErrorMsg + "\");");
            str.Append("window.close();");
            str.Append("</script>");
            con.Response.Write(str.ToString());
            con.Response.End();
        }
        #endregion

        #region 获得QueryString
        /// <summary>
        /// 返回指定键值的查询字符串[String 类型]
        /// </summary>
        /// <param name="QueryString">要查询的查询字符串的键名</param>
        /// <param name="ReportError">是否报错，如果为true，则直接向用户界面输出错误，
        /// 如果为false，遇到错误返回""</param>
        /// <returns></returns>
        public static string GetstrQueryString(string QueryString, bool ReportError)
        {
            HttpContext con = HttpContext.Current;
            if (null == con.Request[QueryString])
            {
                if (ReportError)
                {//报错
                    con.Response.Redirect("/Error.htm?m=参数不合法");
                }
                return string.Empty;
            }
            else
            {
                string res = con.Request[QueryString];
                bool falg = res.Contains("\'");

                if (falg == true)
                {
                    return string.Empty;
                }
                else
                {
                    return con.Request[QueryString];
                }
            }
        }
        /// <summary>
        /// 返回指定键值的查询字符串[int 类型]
        /// </summary>
        /// <param name="QueryString">要查询的查询字符串的键名</param>
        /// <param name="ReportError">是否报错，如果为true，则直接向用户界面输出错误，
        /// 如果为false，遇到错误返回-1</param>
        /// <returns></returns>
        public static int GetintQueryString(string QueryString, bool ReportError)
        {
            HttpContext con = HttpContext.Current;
            if (null == con.Request.QueryString[QueryString])
            {//为空
                if (ReportError)
                {//报错
                    con.Response.Redirect("/Error.htm?m=参数不合法");
                }
                return -1;
            }
            if (!IsNum(con.Request.QueryString[QueryString].ToString()))
            {//不是数字
                if (ReportError)
                {//报错
                    //con.Response.Redirect(getWebAddress() + "ErrorPage.aspx?m=参数不合法");
                    //报错
                }
                return -1;
            }
            return int.Parse(con.Request.QueryString[QueryString].ToString());
        }
        #endregion

        #region 其它常用函数

        /// <summary>
        /// 字符串转换成Int，失败返回0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int GetInt(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            int i = 0;
            int.TryParse(s.Trim(), out i);
            return i;
        }

        /// <summary>
        /// 字符串转换成decimal，失败返回0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal GetDecimal(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            decimal i = 0;
            decimal.TryParse(s.Trim(), out i);
            return i;
        }

        /// <summary>
        /// 字符串转换成DatetTime，失败返回默认日期
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return Const.DateTimeDefaultValue;
            DateTime date = Const.DateTimeDefaultValue;
            DateTime.TryParse(s.Trim(), out date);
            if (date.Year < Const.DateTimeDefaultValue.Year)
            {
                date = Const.DateTimeDefaultValue;
            }
            return date;
        }

        /// <summary>
        /// 比较两个日期相差
        /// </summary>
        /// <param name="DateTime1"></param>
        /// <param name="DateTime2"></param>
        /// <returns>mm天HH时MM分ss秒</returns>
        public static string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string str = null;
            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new
            TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            str = string.Format("{0}天{1}时{2}分{3}秒", ts.Days,ts.Hours,ts.Minutes, ts.Seconds);
            return str;
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="length">截取长度</param>
        /// <returns>字符串截取后的结果</returns>
        public static string CutString(string str, int length)
        {
            if (string.IsNullOrWhiteSpace(str)) return "";
            if (str.Length < length) return str;
            return str.Substring(0, length) + "...";
        }

        /// <summary>
        /// 判断一个字符串是否全由数字组成
        /// </summary>
        /// <param name="str">要判断的字符串</param>
        /// <returns>true:是数字，false:不是数字或为空</returns>
        public static bool IsNum(string str)
        {
            if (str.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] < '0' || str[i] > '9')
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 判断一个字符串是不是合法的Email格式
        /// </summary>
        /// <param name="Email">要判断的字符</param>
        /// <returns>true:是Email,false:不是Email</returns>
        public static bool IsEmail(string Email)
        {
            return Regex.IsMatch(Email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }

        /// <summary>
        /// 判断一个字符串是不是合法的手机号码格式
        /// </summary>
        /// <param name="Mobile">要判断的字符</param>
        /// <returns>true:是Mobile,false:不是Mobile</returns>
        public static bool IsMobile(string Mobile)
        {
            return Regex.IsMatch(Mobile, @"1\d{10}");
        }

        /// <summary>
        /// 判断一个字符串是不是完整的Http URL地址
        /// </summary>
        /// <param name="HttpUrl"></param>
        /// <returns></returns>
        public static bool IsHttpUrl(string HttpUrl)
        {
            return Regex.IsMatch(HttpUrl, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
        }

        /// <summary>
        /// 判断一个字符串是不是IP
        /// </summary>
        /// <param name="strIP"></param>
        /// <returns></returns>
        public static bool IsIP(string strIP)
        {
            return Regex.IsMatch(strIP, @"^((\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.){3}(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
        }

        #endregion

        #region 操作系统、浏览器
        /// <summary>
        /// 获得操作系统版本
        /// </summary>
        /// <param name="HTTP_USER_AGENT">Request.ServerVariables["HTTP_USER_AGENT"]
        /// </param>
        /// <returns></returns>
        public static string GetOS(string HTTP_USER_AGENT)
        {
            string SystemVer = "";
            string agent = HTTP_USER_AGENT;
            if (null == agent) agent = "";
            if (agent.Contains("NT 10.0"))
            {
                SystemVer = "Windows 10";
            }
            else if (agent.Contains("NT 6.2"))
            {
                SystemVer = "Windows 8/Server 2012";
            }
            else if (agent.Contains("NT 6.1"))
            {
                SystemVer = "Windows 7/Server 2008 R2";
            }
            else if (agent.Contains("NT 6.0"))
            {
                SystemVer = "Windows vista/Server 2008";
            }
            else if (agent.Contains("NT 5.2") )
            {
                SystemVer = "Windows 2003";
            }
            else if (agent.Contains("NT 5.1")  )
            {
                SystemVer = "Windows XP";
            }
            else if (agent.Contains("NT 5") )
            {
                SystemVer = "Windows 2000";
            }
            else if (agent.Contains("NT 4")   )
            {
                SystemVer = "Windows NT4";
            }
            else if (agent.Contains("4.9")  )
            {
                SystemVer = "Windows ME";
            }
            else if (agent.Contains("98")  )
            {
                SystemVer = "Windows 98";
            }
            else if (agent.Contains("95")  )
            {
                SystemVer = "Windows 95";
            }
            else if (agent.Contains("Mac"))
            {
                SystemVer = "Mac";
            }
            else if (agent.Contains("Unix"))
            {
                SystemVer = "Unix";
            }
            else if (agent.Contains("Linux"))
            {
                SystemVer = "Linux";
            }
            else
            {
                SystemVer = agent;
            }
            return SystemVer;
        }
        /// <summary>
        /// 获得用户浏览器类型
        /// </summary>
        ///<param name="HTTP_USER_AGENT">Request.ServerVariables["HTTP_USER_AGENT"]
        /// </param>
        /// <returns></returns>
        public static string GetBrowser(string HTTP_USER_AGENT)
        {
            string BrowseMsg = HTTP_USER_AGENT;
            string ie = "";
            if (null == BrowseMsg) BrowseMsg = "";
            if (BrowseMsg.IndexOf("MSIE") > 0)
            {
                if (BrowseMsg.Contains("MSIE 13.0"))
                {
                    ie = "IE 13.0";
                }
                else if (BrowseMsg.Contains("MSIE 12.0"))
                {
                    ie = "IE 12.0";
                }
                else if (BrowseMsg.Contains("MSIE 11.0"))
                {
                    ie = "IE 11.0";
                }
                else if (BrowseMsg.Contains("MSIE 10.0"))
                {
                    ie = "IE 10.0";
                }
                else if (BrowseMsg.Contains("MSIE 9.0"))
                {
                    ie = "IE 9.0";
                }
                else if (BrowseMsg.Contains("MSIE 8.0"))
                {
                    ie = "IE 8.0";
                }
                else if (BrowseMsg.Contains("MSIE 7.0")  )
                {
                    ie = "IE 7.0";
                }
                else if (BrowseMsg.Contains("MSIE 6.0")  )
                {
                    ie = "IE 6.0";
                }
                else if (BrowseMsg.Contains("MSIE 5.5")  )
                {
                    ie = "IE 5.5";
                }
                else if (BrowseMsg.Contains("MSIE 5.0")  )
                {
                    ie = "IE 5.0";
                }

                else if (BrowseMsg.Contains("MSIE 4.0"))
                {
                    ie = "IE 4.0";
                }
            }
            else if(BrowseMsg.Contains("rv:"))
            {
                if (BrowseMsg.Contains("rv:13.0"))
                {
                    ie = "IE 13.0";
                }
                else if (BrowseMsg.Contains("rv:12.0"))
                {
                    ie = "IE 12.0";
                }
                else if (BrowseMsg.Contains("rv:11.0"))
                {
                    ie = "IE 11.0";
                }
            }
            else if(BrowseMsg.Contains("Chrome"))
            {
                ie = "Chrome";
            }
            else if (BrowseMsg.Contains("Firefox"))
            {
                if (BrowseMsg.Contains("Firefox/3.0"))
                {
                    ie = "Firefox 3.0";
                }
                else if (BrowseMsg.Contains("Firefox/2.0"))
                {
                    ie = "Firefox 2.0";
                }
                else if (BrowseMsg.Contains("Firefox/1.0"))
                {
                    ie = "Firefox 1.0";
                }
                else if (BrowseMsg.Contains("Firefox/4.0"))
                {
                    ie = "Firefox 4.0";
                }
                else if (BrowseMsg.Contains("Firefox/5.0"))
                {
                    ie = "Firefox 5.0";
                }
            }
            else if (BrowseMsg.Contains("Netscape/9.0"))//Netscape　浏览器信息
            {
                ie = "Netscape 9.0";
            }
            else if (BrowseMsg.Contains("Netscape/8.0"))
            {
                ie = "Netscape 8.0";
            }
            else if (BrowseMsg.Contains("Netscape/7.0"))
            {
                ie = "Netscape 7.0";
            }
            else if (BrowseMsg.Contains("Netscape/6.0"))
            {
                ie = "Netscape 6.0";
            }
            else if (BrowseMsg.Contains("Netscape/5.0"))
            {
                ie = "Netscape 5.0";
            }
            else
            {
            }
            return ie;
        }
        #endregion

        #region 报错
        public static void ReportError(string ErrorMsg)
        {
            HttpContext.Current.Response.Redirect("~/Error.htm?"+HttpUtility.UrlEncode(ErrorMsg));
            HttpContext.Current.Response.End();
        }
        public static void ReportLoginError(string ErrorMsg)
        {
            HttpContext.Current.Response.Redirect("~/Login.aspx?ErrorMsg="+HttpUtility.UrlEncode(ErrorMsg));
            HttpContext.Current.Response.End();
        }
        #endregion

        #region 下载文件
        public static bool DownLoadPicFromWeb(string URL, string LocalFile)
        {
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(URL, LocalFile);
                return true;
            }
            catch (WebException exp)
            {
                Log.WriteLog(string.Format("下载文件{0}时出错，技术细节：",URL,exp.Message));
                return false;
            }
            catch (Exception ee)
            {
                Log.WriteLog(string.Format("下载文件{0}时出错，技术细节：", URL, ee.Message));
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 获取一个数组从维 Lower到维Upper之间的数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="Lower"></param>
        /// <param name="Upper"></param>
        /// <returns></returns>
        public static  object[] GetArrayDimensionLower2Upper(object[] obj, int Lower, int Upper)
        {
            if (null == obj) return obj;
            if (Lower < 0 || Upper < 0 || Lower > Upper || Upper>obj.Length)
            {
                return null;
            }
            object[] tmp=new object[Upper-Lower+1];
            for (int i = Lower; i <= Upper; i++)
            {
                tmp[i - Lower] = obj[i];
            }
            return tmp;
        }

        #region 将一段html代码中的标记去掉
        public static string RemoveHTMLTags(string Htmlstring)
        {
            if (null == Htmlstring) return null;
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpUtility.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }
        #endregion

        /// <summary>
        /// 读取一个文本文件的所有文本
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string GetTextFromFile(string FilePath)
        {
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(FilePath,Encoding.GetEncoding("gb2312"));
                string s=sr.ReadToEnd();
                sr.Close();
                return s;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetPath(string path)
        {
            if (path.StartsWith("~"))
            {
                path = HttpRuntime.AppDomainAppPath + path.Remove(0, 2);
            }
            else
            {
                path = HttpRuntime.AppDomainAppPath + path.Remove(0, 1);
            }
            return path;
        }
        public static string GetAttPath(string module,string attname)
        {
            if(attname.ToLower().Contains("http://"))
            {
                return attname;
            }
            else
            {
                return Config.GetConfig("domain") + "Upload/" + module + "/" + attname;
            }
        }
        public static bool DeleteAtt(string module,string attname)
        {
            if (attname == null || attname == string.Empty || !attname.Contains(";")) return false;
            if (attname.ToLower().Contains("http://"))
            {
                return false;
            }
            else
            {
                string absolutePath = HttpRuntime.AppDomainAppPath +"Upload\\" + module+"\\" + attname.Split(';')[1];
                try
                {
                    System.IO.File.Delete(absolutePath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }


        #region 上传图片，返回路径
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="fu">FileUpload服务器控件ID</param>
        /// <param name="PicPath">文件名</param>
        public static string UploadPic(FileUpload fu)
        {
            string PicPath = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(fu.FileName);
             return UploadPic(fu, PicPath);
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="fu">FileUpload服务器控件ID</param>
        /// <param name="PicPath">文件名</param>
        public static string UploadPic(FileUpload fu, string PicPath)
        {
            string PicFullPath = Config.GetConfig("PicturePath") + PicPath;
            if (fu.PostedFile.ContentLength == 0)
            {
                return null;
            }

            try
            {
                fu.SaveAs(Main.GetPath(PicFullPath));
                return "/Upload/Images/" + PicPath;
            }
            catch (Exception ex)
            {
                Log.WriteLog("保存失败。e:" + ex.Message);
                try
                {
                    System.IO.File.Delete(PicFullPath);
                }
                catch
                { }
            }
            return null;
        }
        #endregion


        #region 上传文件，返回路径
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fu">FileUpload服务器控件ID</param>
        /// <param name="PicPath">文件名</param>
        public static string UploadFile(FileUpload fu)
        {
            string PicPath = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(fu.FileName);
            return UploadFile(fu, PicPath);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fu">FileUpload服务器控件ID</param>
        /// <param name="PicPath">文件名</param>
        public static string UploadFile(FileUpload fu, string PicPath)
        {
            string PicFullPath = Config.GetConfig("FilePath") + PicPath;
            if (fu.PostedFile.ContentLength == 0)
            {
                return null;
            }

            try
            {
                fu.SaveAs(Main.GetPath(PicFullPath));
                return "/Upload/Files/" + PicPath;
            }
            catch (Exception ex)
            {
                Log.WriteLog("保存失败。e:" + ex.Message);
                try
                {
                    System.IO.File.Delete(PicFullPath);
                }
                catch
                { }
            }
            return null;
        }
        #endregion
    }
}