using System;
using System.DirectoryServices;
using System.Web;
using YXJ.Common;
using YXJ.Entity;
/// <summary>
/// LoginController 的摘要说明
/// </summary>
public class LoginController
{
    public LoginController()
    {
    }

    #region 当前登录的用户
    /// <summary>
    /// 当前登录的用户
    /// </summary>
    public static Users User
    {
        get
        {
            if (null == System.Web.HttpContext.Current.Session["CurrentUser"])
            {
                return null;
            }
            else
            {
                return System.Web.HttpContext.Current.Session["CurrentUser"] as Users;
            }
        }
    }
    #endregion

    //public static bool Logined(string UserLoginName)
    //{
    //    bool r = SessionHelper.Get(UserLoginName);

    //}

    #region 当前用户是否已经登录
    /// <summary>
    /// 当前用户是否已经登录
    /// </summary>
    public static bool IsLogined
    {
        get
        {
            if (User != null)
            {
                return true;
            }
            else
            {
                string UserLoginName = null;
                string UserPwd = null;
                bool r = Cookie.GetLoginCookie(ref UserLoginName, ref UserPwd);
                if (r)
                {
                    bool r2 = Login(UserLoginName, UserPwd);
                    return r2;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    #endregion

    #region 登录
    public static bool Login(string UserLoginName, string UserPwd)
    {
        #region 将登录信息记入数据库
        System.Web.HttpContext con = System.Web.HttpContext.Current;
        DateTime now = DateTime.Now;
        LoginLog login = new LoginLog()
        {
            UserLoginName = UserLoginName,
            LoginTime = now,
            Ip = System.Web.HttpContext.Current.Request.UserHostAddress,
            UserAgent = con.Request.UserAgent,
            InsertTime = now,
        };
        if (login.Ip == "::1") login.Ip = "127.0.0.1";
        #endregion

        Users user = new Users();
        if (!user.Load(UserLoginName, UserPwd))
        {//登录失败
            Cookie.ClearLoginCookie();
            login.LoginStatus = 0;
            login.Save();
            return false;
        }
        else
        {//登录成功
            con.Session["CurrentUser"] = user;
            con.Session["UserLoginName"] = user.UserLoginName;
            login.LoginStatus = 1;
            login.Save();
            return true;
        }
    }

    public static bool Login(string UserLoginName, string UserPwd, bool Status)
    {
        if (Login(UserLoginName, UserPwd))
        {
            Cookie.SetLoginCookie(UserLoginName, UserPwd, Status);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static Users Userinfo
    {
        get
        {
            if (null == System.Web.HttpContext.Current.Session["CurrentUser"])
            {
                return null;
            }
            else
            {
                return System.Web.HttpContext.Current.Session["CurrentUser"] as Users;
            }
        }
    }
    #endregion
}