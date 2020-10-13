using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YXJ.Common;
using YXJ.DAL;
using YXJ.Entity;
using YXJ.Entity.Model;

public partial class service_login : System.Web.UI.Page
{
    private string GetRequest(string req)
    {
        return Main.GetstrQueryString(req, false);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string Command = GetRequest("Command");
        if (string.IsNullOrWhiteSpace(Command))
        {
            ResultModel rm = new ResultModel();
            rm.state = ResultStatu.失败;
            rm.error = "请求的命令为空。";
            Response.Write(rm.ToJsonString());
            Response.End();
            return;
        }
        switch (Command.ToLower())
        {
            case "userlogin": UserLogin(); break;//登录验证


            default:
                ResultModel rm = new ResultModel();
                rm.state = ResultStatu.失败;
                rm.error = "请求的命令不存在。";
                Response.Write(rm.ToJsonString());
                Response.End();
                break;
        }
    }

    public class Itemlogin
    {
        public string session { get; set; }//session值
    }
    
    #region 登录、注销
    /// <summary>
    /// 用户登录
    /// </summary>
    private void UserLogin()
    {
        string UserName = Main.GetstrQueryString("UserName", false);
        string Pwd = Main.GetstrQueryString("Pwd", false);

        List<Itemlogin> list = new List<Itemlogin>();
        #region 将登录信息记入数据库
        System.Web.HttpContext con = System.Web.HttpContext.Current;
        LoginLog login = new LoginLog()
        {
            UserLoginName = UserName,
            LoginTime = DateTime.Now,
            Ip = System.Web.HttpContext.Current.Request.UserHostAddress,
            UserAgent = con.Request.UserAgent,
            InsertTime = DateTime.Now,
        };
        if (login.Ip == "::1") login.Ip = "127.0.0.1";
        #endregion
        Users user = new Users();
        if (!user.Load(UserName, Pwd))
        {//登录失败
            login.LoginStatus = 0;
            login.Save();

            ResultModel rm = new ResultModel();
            rm.state = ResultStatu.失败;
            rm.error = "用户名或密码错误";
            Response.Write(rm.ToJsonString());
            Response.End();
        }
        else
        {//登录成功
            con.Session["CurrentUser"] = user;
            con.Session["UserLoginName"] = user.UserLoginName;
            login.LoginStatus = 1;
            login.Save();
            string sql = string.Format("update Users set LastLoginTime='{1}' where id={0}", user.Id,DateTime.Now);
            bool flag = DMain.RunUpdate(sql);

            UserMenu usermenu = new UserMenu();
            List<YXJ.Entity.UserMenu.Item> MyMenu = usermenu.GetMyMenu(user.Id);
            ResultModel rm = new ResultModel();
            if (null == MyMenu)
            {
                rm.error = "该用户没有菜单权限！";
            }
            else
            {
                rm.state = ResultStatu.成功;
            }

            Dictionary<string, object> JsonD = new Dictionary<string, object>();
            JsonD.Add("userid", user.Id);
            string Token = Jwt.Create(JsonD);

            Dictionary<string, List<YXJ.Entity.UserMenu.Item>> JsonDJ = new Dictionary<string, List<YXJ.Entity.UserMenu.Item>>();
            List<YXJ.Entity.UserMenu.Item> listnew = new List<YXJ.Entity.UserMenu.Item>();
            YXJ.Entity.UserMenu.Item it = new YXJ.Entity.UserMenu.Item();
            it.name = Token;
            listnew.Add(it);

            JsonDJ.Add("token", listnew);
            JsonDJ.Add("menu", MyMenu);
            string JsonDT = JsonHelper.Serializer(JsonDJ);

            Response.Write(JsonDT);
            Response.End();
        }
    }
    #endregion
}