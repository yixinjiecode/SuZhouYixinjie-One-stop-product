using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using YXJ.Common;
using YXJ.DAL;
using YXJ.Entity;
using YXJ.Entity.Model;
using Newtonsoft.Json;
using System.Collections;
using DBUtility.Data;
using NPOI.SS.Formula.Functions;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Net.Security;
using System.Reflection;
using System.Threading;

public partial class service_api : System.Web.UI.Page
{
    private Users _CurrentUser;
    private Users CurrentUser
    {
        get
        {
            if (_CurrentUser != null) return _CurrentUser;
            _CurrentUser = LoginController.User;
            return _CurrentUser;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        int userid = 0;
        string authHeader = this.Request.Headers["Authorization"];//Header中的token
        if (authHeader != null)
        {
            string token = authHeader.Substring(0).Trim();
            //验证token是否正确是否过期
            userid = Jwt.Check(token);
        }
        else
        {
            ResultModel rm = new ResultModel();
            rm.state = ResultStatu.失败;
            rm.error = "没有获取到token值";
            Response.Write(rm.ToJsonString());
            Response.End();
            return;
        }

        if (userid <= 0)
        {
            ResultModel rm = new ResultModel();
            rm.state = ResultStatu.失败;
            rm.error = "请登录后操作:" + authHeader;
            Response.Write(rm.ToJsonString());
            Response.End();
            return;
        }
        Users user = new Users();
        user.Id = userid;
        System.Web.HttpContext con = System.Web.HttpContext.Current;
        con.Session["CurrentUser"] = user;

        string Command = GetRequest("Command");
        HttpContext content = HttpContext.Current;
        YXJ.Common.Log.WriteLog("请求地址:" + content.Request.ServerVariables["url"]);
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
            //case "userlogin": UserLogin(); break;//登录验证
            case "getfunctionmenulist": GetFunctionMenuList(); break;//获取子菜单下功能的接口
            case "getlogout": GetLogout(); break;//退出登录
            case "getuserinfo": GetUserInfo(); break;//获取登录人信息
            case "getbranch": GetBranch(); break;//获取分公司
            case "getsalesman": GetSalesman(); break;//获取业务员
            case "getmenulist": GetMenuList(); break;//获取当前用户的所有菜单
            case "getsubmenulist": GetSubmenuList(); break;//获取当前用户菜单下的所有子菜单
            case "getallmenu": GetAllMenu(); break;//获取所有的权限
            case "getuserlist": GetUserList(); break;//获取用户列表
            case "adduser": AddUser(); break;//新增用户
            case "editoruser": EditorUser(); break;//编辑用户
            case "deleteuser": DeleteUser(); break;//删除用户
            case "oneusermenu": OneUserMenu(); break;//获取单个用户的权限
            case "changepwd": ChangePwd(); break;//修改密码
            case "getcustomerlist": GetCustomerList(); break;//获取客户列表
            case "getcustomerinfo": GetCustomerInfo(); break;//获取单个客户的信息
            case "addcustomer": AddCustomer(); break;//新增客户
            case "editorcustomer": EditorCustomer(); break;//编辑客户
            case "deletecustomer": DeleteCustomer(); break;//删除客户
            case "customerloglist": CustomerLogList(); break;//客户操作日志
            case "addorganization": AddOrganization(); break;//新增组织
            case "getorganizationlist": GetOrganizationList(); break;//组织树状图
            case "getorganization": GetOrganization(); break;//组织列表
            case "editororganization": EditorOrganization(); break;//编辑组织
            case "deleteorganization": DeleteOrganization(); break;//删除组织
            case "getorganizationinfo": GetOrganizationInfo(); break;//获取单个组织的信息
            case "getforwarderlist": GetForwarderList(); break;//转包运商列表
            case "addforwarder": AddForwarder(); break;//新增转包运商
            case "editorforwarder": EditorForwarder(); break;//编辑转包运商
            case "deleteforwarder": DeleteForwarder(); break;//删除转包运商 
            case "getforwarderinfo": GetForwarderInfo(); break;//获取单个转包运商的信息
            case "getorderformlist": GetOrderFormList(); break;//获取订单列表
            case "addorderform": AddOrderForm(); break;//新增订单
            case "editororderform": EditorOrderForm(); break;//编辑订单
            case "deleteorderform": DeleteOrderForm(); break;//删除订单
            case "orderformloglist": OrderFormLogList(); break;//订单操作日志
            case "orderamountloglist": OrderAmountLogList(); break;//账单操作日志
            case "allcustomer": AllCustomer(); break;//订单中选择客户的信息
            case "allforwarder": AllForwarder(); break;//订单中选择转包运商
            case "getpack": GetPack(); break;//订单中选择包装
            case "gettransport": GetTransport(); break;//订单中选择运输方式
            case "getcargo": GetCargo(); break;//订单中选择货单状态
            // case "getforwarder": GetForwarder(); break;//订单中选择合作公司
            case "getorderamountlist": GetOrderAmount(); break;//获取单个订单账单
            case "addorderamount": AddOrderAmount(); break;//新增订单账单
            case "editororderamount": EditorOrderAmount(); break;//编辑订单账单
            case "deleteorderamount": DeleteOrderAmount(); break;//删除订单账单
            case "auditorderform": AuditOrderForm(); break;//获取审核订单列表
            case "auditorderformdetail": AuditOrderFormDetail(); break;//获取每一个审核订单的详细信息
            case "editorauditorderform": EditorAuditOrderForm(); break;//订单审核是否同意
            case "auditorderamount": AuditOrderAmount(); break;//获取审核账单列表
            case "auditorderamountdetail": AuditOrderAmountDetail(); break;//获取每一个审核账单的详细信息
            case "editorauditorderamount": EditorAuditOrderAmount(); break;//账单审核是否同意
            case "send": Send(); break;//快递100物流查询
            case "sendruiqing": SendRuiQing(); break;//瑞庆快递物流查询
            case "addsendruiqing": AddSendRuiQing(); break;//修改瑞庆快递物流状态
            case "searchprovince": SearchProvince(); break;//查询省份
            case "searchcity": SearchCity(); break;//查询城市
            case "orderformbalanceno": OrderFormBalanceNo(); break;//订单未结算
            case "orderformbalancetype": OrderFormBalanceType(); break;//订单结算状态修改
            case "orderformbalanceon": OrderFormBalanceOn(); break;//订单已结算
            case "orderformbalancelog": OrderFormBalanceLog(); break;//订单结算状态操作日志
            case "getnewslist": GetNewsList(); break;//获取新闻列表
            case "addnews": AddNews(); break;//新增新闻
            case "etitornews": EtitorNews(); break;// 编辑新闻
            case "deletenews": DeleteNews(); break;//删除新闻
            case "getpaymenttype": GetPaymentType(); break;//账单中选择支付方式
            case "uploadimg": UploadImg(); break;//新闻上传图片
            case "dictionarieskeylist": DictionariesKeyList(); break;//所有字典的父级
            case "dictionarieslist": DictionariesList(); break;//字典列表
            case "adddictionaries": AddDictionaries(); break;//新增字典
            case "editordictionaries": EditorDictionaries(); break;//编辑字典
            case "auditcustomer": AuditCustomer(); break;//客户审核列表
            case "auditcustomerdetail": AuditCustomerDetail(); break;//获取每一个审核客户的详细信息
            case "editorauditcustomer": EditorAuditCustomer(); break;//客户审核是否同意

            default:
                ResultModel rm = new ResultModel();
                rm.state = ResultStatu.失败;
                rm.error = "请求的命令不存在。";
                Response.Write(rm.ToJsonString());
                Response.End();
                break;
        }
    }

    public class Itemmenu
    {
        public int userid { get; set; }//用户的id
        public string index { get; set; }//父级的index
    }
    /// <summary>
    /// 获取子菜单下功能的接口
    /// </summary>
    private void GetFunctionMenuList()
    {
        string index = Main.GetstrQueryString("index", false);
        int userid = Main.GetintQueryString("userid", false);
        string result = null;
        //根据父级的index获取父级的id
        string sql = string.Format("select Id from Menu where [index]='{0}'", index);
        DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            result = "未查父级的信息！";
            Response.Write(result);
            return;
        }
        //获取菜单下的功能
        UserMenu usermenu = new UserMenu();
        List<YXJ.Entity.UserMenu.ItemMenu> FunctionMenu = usermenu.GetFunctionMenu(dt.Rows[0]["Id"].ToString().ToInt(), userid);
        if (null == FunctionMenu || FunctionMenu.Count == 0)
        {
            result = "您没有菜单下的功能权限！";
        }
        else
        {
            var arrcount = FunctionMenu.ToList();
            result = JsonHelper.Serializer<ItemBranch>(arrcount);

        }
        Response.Write(result);
        Response.End();
    }

    public class Itemlogin
    {
        public string session { get; set; }//session值
    }
    #region 注销
    /// <summary>
    /// 退出登录
    /// </summary>
    private void GetLogout()
    {
        Session.Clear();
        ResultModel rm = new ResultModel();
        rm.state = ResultStatu.成功;
        Response.Write(rm.ToJsonString());
        Response.End();
    }
    #endregion

    public class ItemInfo
    {
        public int UserId { get; set; }//用户的id
        public string UserLoginName { get; set; }//登录名
        public string UserName { get; set; }//姓名
        public string DICValue { get; set; }//分公司名
        public string Email { get; set; }//邮箱
        public string LastLoginTime { get; set; }//最后登录时间
        public string InsertTime { get; set; }//创建时间
    }
    /// <summary>
    /// 获取登录人信息
    /// </summary>
    private void GetUserInfo()
    {
        string UserLoginName = Main.GetstrQueryString("UserName", false);
        ResultModel rm = new ResultModel();
        //查询登录人的信息
        string sql = string.Format("select Users.Id,UserLoginName,UserName,Email,LastLoginTime,Users.InsertTime,DICValue from Users left outer join DictionariesValue on Users.ComNo = DictionariesValue.Id where UserLoginName ='{0}'", UserLoginName);
        DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "未查询到信息！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemInfo> list = new List<ItemInfo>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemInfo iteminfo = new ItemInfo();
                iteminfo.UserId = dr["Id"].ToString().ToInt();
                iteminfo.UserLoginName = dr["UserLoginName"].ToString();
                iteminfo.UserName = dr["UserName"].ToString();
                iteminfo.DICValue = dr["DICValue"].ToString();
                iteminfo.Email = dr["Email"].ToString();
                iteminfo.LastLoginTime = dr["LastLoginTime"].ToString();
                iteminfo.InsertTime = dr["InsertTime"].ToString();
                list.Add(iteminfo);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemInfo>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }

    public class ItemBranch
    {
        public int Id { get; set; }//分公司id
        public string BranchName { get; set; }//分公司名称
    }
    /// <summary>
    /// 获取分公司
    /// </summary>
    private void GetBranch()
    {
        ResultModel rm = new ResultModel();
        //查询分公司
        string sql = string.Format("select Id,DICValue from DictionariesValue where KeyId=1 and Enable=1 and IsDelete=0 ");
        DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "暂无分公司！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemBranch> list = new List<ItemBranch>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemBranch itembranch = new ItemBranch();
                itembranch.Id = dr["Id"].ToString().ToInt();
                itembranch.BranchName = dr["DICValue"].ToString();
                list.Add(itembranch);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemBranch>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }

    public class ItemSalesman
    {
        public int Id { get; set; }//业务员id
        public string SalesmanName { get; set; }//业务员姓名
    }
    /// <summary>
    /// 获取业务员
    /// </summary>
    private void GetSalesman()
    {
        ResultModel rm = new ResultModel();
        //查询所有的业务员
        string sql = string.Format("select Id,UserName from Users where OrganizationId=3");
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "没有业务员！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemSalesman> list = new List<ItemSalesman>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemSalesman itemsalesman = new ItemSalesman();
                itemsalesman.Id = dr["Id"].ToString().ToInt();
                itemsalesman.SalesmanName = dr["UserName"].ToString();
                list.Add(itemsalesman);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemSalesman>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }

    /// <summary>
    /// 获取当前用户的所有菜单
    /// </summary>
    private void GetMenuList()
    {
        ResultModel rm = new ResultModel();
        Users user = new Users();
        List<YXJ.Entity.Menu> MyMenu = user.GetMyMenu();
        if (null == MyMenu)
        {
            rm.error = "该用户没有菜单权限！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            rm.result = MyMenu.ToString();
            Response.Write(rm.ToJsonString());
            Response.End();
        }
    }
    /// <summary>
    /// 获取当前用户菜单下的所有子菜单
    /// </summary>
    private void GetSubmenuList()
    {
        ResultModel rm = new ResultModel();
        int MenuId = GetIntRequest("MenuId");//父级菜单的OrderId
        Users user = new Users();
        List<YXJ.Entity.Menu> MySubmenu = user.GetMySubmenu(MenuId);

        if (null == MySubmenu)
        {
            rm.error = "该用户没有菜单权限！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            rm.result = MySubmenu.ToString();
            Response.Write(rm.ToJsonString());
            Response.End();
        }
    }

    public class ItemAllMenu
    {
        public int Id { get; set; }//权限id
        public string MenuName { get; set; }//权限名称
        public int Pid { get; set; }//权限父级id
        public int Type { get; set; }//0菜单 1功能
        public List<ItemAllMenu> children { get; set; }//孩子
    }
    /// <summary>
    /// 获取所有的权限
    /// </summary>
    private void GetAllMenu()
    {
        ResultModel rm = new ResultModel();
        //查询主菜单
        string sql = string.Format("select Id,MenuName,Pid from Menu where Pid=0 and Type=0 and IsDelete=0 and Id!=1");
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {

            rm.error = "暂无权限表！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemAllMenu> list1 = new List<ItemAllMenu>();
            foreach (DataRow dr in dt.Rows)
            {
                //查询子菜单
                string sql2 = string.Format("select Id,MenuName,Pid from Menu where Pid={0} and Type=0 and IsDelete=0", dr["Id"]);
                System.Data.DataTable dt2 = DMain.Get(sql2);
                List<ItemAllMenu> list2 = new List<ItemAllMenu>();
                foreach (DataRow dr2 in dt2.Rows)
                {
                    //查询功能
                    string sql3 = string.Format("select Id,MenuName,Pid from Menu where Pid={0} and Type=1 and IsDelete=0", dr2["Id"]);
                    System.Data.DataTable dt3 = DMain.Get(sql3);
                    List<ItemAllMenu> list3 = new List<ItemAllMenu>();
                    foreach (DataRow dr3 in dt3.Rows)
                    {
                        ItemAllMenu itemallmenu3 = new ItemAllMenu();
                        itemallmenu3.Id = dr3["Id"].ToString().ToInt();
                        itemallmenu3.MenuName = dr3["MenuName"].ToString();
                        itemallmenu3.Pid = dr3["Pid"].ToString().ToInt();
                        itemallmenu3.children = new List<ItemAllMenu>();
                        list3.Add(itemallmenu3);
                    }
                    ItemAllMenu itemallmenu2 = new ItemAllMenu();
                    itemallmenu2.Id = dr2["Id"].ToString().ToInt();
                    itemallmenu2.MenuName = dr2["MenuName"].ToString();
                    itemallmenu2.Pid = dr2["Pid"].ToString().ToInt();
                    itemallmenu2.children = list3;
                    list2.Add(itemallmenu2);
                }
                ItemAllMenu itemallmenu1 = new ItemAllMenu();
                itemallmenu1.Id = dr["Id"].ToString().ToInt();
                itemallmenu1.MenuName = dr["MenuName"].ToString();
                itemallmenu1.Pid = dr["Pid"].ToString().ToInt();
                itemallmenu1.children = list2;
                list1.Add(itemallmenu1);
            }
            var arrcountnew = list1.ToList();
            string dataresule = JsonHelper.Serializer<ItemAllMenu>(arrcountnew);
            Response.Write(dataresule);
            Response.End();
        }
    }

    public class ItemUser
    {
        public int Id { get; set; }//用户id
        public string UserLoginName { get; set; }//登录名
        public string UserName { get; set; }//姓名
        public string ComNo { get; set; }//分公司编号
        public string ComName { get; set; }//分公司编号
        public string Email { get; set; }//邮箱
        public int OrganizationId { get; set; }//组织id
        public string OrganizationName { get; set; }//组织名称
        public int Enabled { get; set; }//是否激活 0 否 1 是
        public int RecordCount { get; set; }//总条数
        public string InsertTime { get; set; }//创建时间
        public int InsertUserId { get; set; }//创建人id
    }
    /// <summary>
    /// 获取用户列表
    /// </summary>
    private void GetUserList()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<Item> list = (List<Item>)JsonHelper.DeSerialze<List<Item>>(data);
        int tpagesize = 10;//每页显示数量
        int tpageindex = 1;//当前页数
        int RecordCount = 0;
        foreach (Item dr in list)
        {
            tpagesize = dr.tpagesize;
            tpageindex = dr.tpageindex;
        }

        ResultModel rm = new ResultModel();
        DataTable dt = new Users().GetUserList(tpageindex, tpagesize, ref RecordCount);
        if (null == dt)
        {
            rm.error = "暂无用户！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemUser> list1 = new List<ItemUser>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemUser itemuser = new ItemUser();
                itemuser.Id = dr["Id"].ToString().ToInt();
                itemuser.UserLoginName = dr["UserLoginName"].ToString();
                itemuser.UserName = dr["UserName"].ToString();
                itemuser.ComNo = dr["ComNo"].ToString();
                itemuser.Email = dr["Email"].ToString();
                itemuser.OrganizationId = dr["OrganizationId"].ToString().ToInt();
                itemuser.Enabled = dr["Enabled"].ToString().ToInt();
                itemuser.ComName = dr["ComName"].ToString();
                itemuser.OrganizationName = dr["OrganizationName"].ToString();
                itemuser.InsertTime = dr["InsertTime"].ToString();
                itemuser.InsertUserId = dr["InsertUserId"].ToString().ToInt();
                list1.Add(itemuser);
            }
            Dictionary<string, List<ItemUser>> JsonDJ = new Dictionary<string, List<ItemUser>>();
            List<ItemUser> listnew = new List<ItemUser>();
            ItemUser iof = new ItemUser();
            iof.RecordCount = RecordCount;
            listnew.Add(iof);
            JsonDJ.Add("user", list1);
            JsonDJ.Add("recordcount", listnew);
            string JsonDT = JsonHelper.newTonSerialize(JsonDJ);

            Response.Write(JsonDT);
            Response.End();
        }
    }
    public class ItemUserMenu
    {
        public int UserId { get; set; }//用户的id
    }
    public class ItemOneUserMenu
    {
        public int MenuId { get; set; }//权限id
    }
    /// <summary>
    /// 获取一个用户的权限
    /// </summary>
    private void OneUserMenu()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemUserMenu> list = (List<ItemUserMenu>)JsonHelper.DeSerialze<List<ItemUserMenu>>(data);
        ResultModel rm = new ResultModel();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemUserMenu dr in list)
                {
                    string sql = string.Format("select MenuId from UserMenu LEFT JOIN Menu ON UserMenu.MenuId = Menu.Id where UserId = {0} and Menu.Type != 0", dr.UserId);
                    System.Data.DataTable dt = DMain.Get(sql);
                    if (null == dt)
                    {
                        rm.error = "该客户没有任何权限！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    else
                    {
                        List<ItemOneUserMenu> listnew = new List<ItemOneUserMenu>();
                        foreach (DataRow dr1 in dt.Rows)
                        {
                            ItemOneUserMenu oneusermenu = new ItemOneUserMenu();
                            oneusermenu.MenuId = dr1["MenuId"].ToString().ToInt();
                            listnew.Add(oneusermenu);
                        }
                        var arrcount = listnew.ToList();
                        string datanew = JsonHelper.Serializer<ItemOneUserMenu>(arrcount);
                        Response.Write(datanew);
                        Response.End();
                    }

                }
            }
        }
    }

    public class ItemAddUser
    {
        public string UserLoginName { get; set; }//登录名
        public string UserName { get; set; }//用户名
        public string UserPwd { get; set; }//密码
        public string ComNo { get; set; }//分公司编号
        public string Email { get; set; }//邮件
        public string OrganizationId { get; set; }//组织的id
        public int Enabled { get; set; }//是否激活 0 否 1 是
        public string MenuId { get; set; }//所有功能的id“，”分隔
        public string PId { get; set; }//所有功能父级的id“，”分隔
    }
    /// <summary>
    /// 新增用户
    /// </summary>
    private void AddUser()
    {
        ResultModel rm = new ResultModel();
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemAddUser> list = (List<ItemAddUser>)JsonHelper.DeSerialze<List<ItemAddUser>>(data);
        int id = 0;
        string menuid = "";
        bool r = false;
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemAddUser dr in list)
                {
                    //查询用户是否存在
                    string sql = string.Format("select count(1) from Users where UserloginName='{0}' and IsDelete=0", dr.UserLoginName);
                    System.Data.DataTable dt = DMain.Get(sql);
                    int count = dt.Rows[0][0].ToString().ToInt();
                    if (count >= 1)
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "用户已经存在！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    //密码加密
                    string NewPwd = YXJ.Common.RSACrypto.EncryptText(dr.UserPwd, YXJ.Entity.Users._password_public_Key);
                    Users user = new Users()
                    {
                        UserLoginName = dr.UserLoginName,
                        UserName = dr.UserName,
                        PassWord = NewPwd,
                        ComNo = dr.ComNo,
                        Email = dr.Email,
                        OrganizationId = dr.OrganizationId.ToInt(),
                        InsertTime = DateTime.Now,
                        InsertUserId = CurrentUser.Id,
                        Enabled = dr.Enabled,
                        UpdateTime = Const.DateTimeDefaultValue,
                    };
                    id = user.Save();
                    if (id > 0)
                    {
                        string pid = dr.PId;//子集菜单id
                        List<int> menupid = (from p in pid.Split(',') select p.ToInt()).ToList();
                        string listnew = "";                        
                        foreach (int mmid in menupid)
                        {
                            if (mmid == 0)
                            {
                                //不做处理
                            }
                            else
                            {
                                string sqlnew = string.Format("select Pid from Menu where Id={0} ", mmid);
                                System.Data.DataTable dtnew = DMain.Get(sqlnew);
                                int meid = dtnew.Rows[0][0].ToString().ToInt();
                                listnew = meid + "," + listnew;//父级菜单id
                            }
                            
                        }

                        //获取所有菜单的id
                        menuid = listnew + dr.PId + "," +  dr.MenuId;
                        List<int> Ids = (from p in menuid.Split(',') select p.ToInt()).ToList();
                        HashSet<int> hs = new HashSet<int>(Ids); //此时已经去掉重复的数据保存在hashset中
                        System.Text.StringBuilder sql1 = new System.Text.StringBuilder();
                        foreach (int usermenuid in hs)
                        {
                            if (usermenuid == 0)
                            {
                                //不做处理
                            }
                            else
                            {
                                UserMenu menu = new UserMenu()
                                {
                                    UserId = id,
                                    MenuId = usermenuid,
                                    InsertTime = DateTime.Now,
                                    IsDelete = 0,
                                    UpdateTime = Const.DateTimeDefaultValue,
                                };
                                sql1.Append(menu.BuildInsertSql());
                                sql1.Append("\r\n");
                            }
                        }
                        r = DMain.Run(sql1.ToString());
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "新用户创建失败！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                }
            }
        }
        if (r)
        {
            rm.state = ResultStatu.成功;
        }
        else
        {
            rm.state = ResultStatu.失败;
            rm.error = "新用户创建失败！";
        }
        Response.Write(rm.ToJsonString());
        Response.End();
    }
    public class ItemEditorUser
    {
        public int UserId { get; set; }//用户id
        public string UserLoginName { get; set; }//登录名
        public string UserName { get; set; }//姓名
        public string UserPwd { get; set; }//密码
        public string ComNo { get; set; }//分公司编号
        public string Email { get; set; }//邮箱
        public string OrganizationId { get; set; }//组织id
        public int Enabled { get; set; }//是否激活 是否激活 0 否 1 是
        public string InsertTime { get; set; }//创建时间
        public int InsertUserId { get; set; }//创建人id

        public string MenuId { get; set; }//所有功能的id“，”分隔
        public string PId { get; set; }//所有功能父级的id“，”分隔
    }
    /// <summary>
    /// 编辑用户
    /// </summary>
    private void EditorUser()
    {
        ResultModel rm = new ResultModel();
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemEditorUser> list = (List<ItemEditorUser>)JsonHelper.DeSerialze<List<ItemEditorUser>>(data);
        int id = 0;
        string newpwd = "";
        string menuid = "";
        bool r = false;
        Users users = new Users();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemEditorUser dr in list)
                {
                    if (!users.Load(dr.UserId))
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "用户不存在!";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    //查询用户是否存在
                    string sql = string.Format("select PassWord from Users where Id={0}", dr.UserId);
                    System.Data.DataTable dt = DMain.Get(sql);
                    string oldpwd = dt.Rows[0]["PassWord"].ToString();
                    if (string.IsNullOrWhiteSpace(dr.UserPwd))
                    {
                        newpwd = oldpwd;
                    }
                    else
                    {
                        //密码加密
                        newpwd = YXJ.Common.RSACrypto.EncryptText(dr.UserPwd, YXJ.Entity.Users._password_public_Key);
                    }
                    users.Id = dr.UserId;
                    users.UserLoginName = dr.UserLoginName;
                    users.UserName = dr.UserName;
                    users.PassWord = newpwd;
                    users.ComNo = dr.ComNo;
                    users.Email = dr.Email;
                    users.OrganizationId = dr.OrganizationId.ToInt();
                    users.Enabled = dr.Enabled;
                    users.InsertTime = Convert.ToDateTime(dr.InsertTime);
                    users.InsertUserId = dr.InsertUserId;
                    users.UpdateTime = DateTime.Now;

                    id = users.Save();
                    if (id == dr.UserId)
                    {
                        System.Text.StringBuilder sql1 = new System.Text.StringBuilder();

                        string sqldelete = string.Format("delete from UserMenu where UserId={0}", dr.UserId);
                        sql1.Append(sqldelete);
                        sql1.Append("\r\n");

                        string pid = dr.PId;//子集菜单id
                        List<int> menupid = (from p in pid.Split(',') select p.ToInt()).ToList();
                        string listnew = "";
                        foreach (int mmid in menupid)
                        {
                            if (mmid == 0)
                            {
                                //不做处理
                            }
                            else
                            {
                                string sqlnew = string.Format("select Pid from Menu where Id={0} ", mmid);
                                System.Data.DataTable dtnew = DMain.Get(sqlnew);
                                int meid = dtnew.Rows[0][0].ToString().ToInt();
                                listnew = meid + "," + listnew;//父级菜单id
                            }
                        }

                        //获取所有菜单的id
                        menuid = listnew + dr.PId + "," + dr.MenuId;
                        List<int> Ids = (from p in menuid.Split(',') select p.ToInt()).ToList();
                        HashSet<int> hs = new HashSet<int>(Ids); //此时已经去掉重复的数据保存在hashset中
                        
                        foreach (int usermenuid in hs)
                        {
                            if (usermenuid == 0)
                            {
                                //不做处理
                            }
                            else
                            {
                                UserMenu menu = new UserMenu()
                                {
                                    UserId = id,
                                    MenuId = usermenuid,
                                    InsertTime = DateTime.Now,
                                    IsDelete = 0,
                                    UpdateTime = Const.DateTimeDefaultValue,
                                };
                                sql1.Append(menu.BuildInsertSql());
                                sql1.Append("\r\n");
                            }
                        }
                        r = DMain.Run(sql1.ToString());
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "用户修改失败！";
                    }
                }
            }
        }
        if (r)
        {
            rm.state = ResultStatu.成功;
        }
        else
        {
            rm.state = ResultStatu.失败;
            rm.error = "用户修改失败！";
            Response.Write(rm.ToJsonString());
            return;
        }
        Response.Write(rm.ToJsonString());
        Response.End();
    }

    public class ItemDeleteUser
    {
        public int Id { get; set; }//用户的id
    }
    /// <summary>
    /// 删除用户
    /// </summary>
    private void DeleteUser()
    {
        ResultModel rm = new ResultModel()
        {
            state = ResultStatu.失败
        };
        Users users = new Users();
        int id = 0;
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemDeleteUser> list = (List<ItemDeleteUser>)JsonHelper.DeSerialze<List<ItemDeleteUser>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemDeleteUser dr in list)
                {
                    id = dr.Id;
                    if (!users.Load(id))
                    {
                        rm.error = "用户不存在";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    if (users.UpdateDelete(id))
                    {
                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "删除用户失败！";
                    }
                }
            }
        }
        Response.Write(rm.ToJsonString());
        Response.End();
    }

    public class ItemPwd
    {
        public int UserId { get; set; }//用户的id
        public string UserPwd1 { get; set; }//原始密码
        public string UserPwd2 { get; set; }//新密码
        public string UserPwd3 { get; set; }//确认新密码
    }
    /// <summary>
    /// 修改密码
    /// </summary>
    private void ChangePwd()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemPwd> list = (List<ItemPwd>)JsonHelper.DeSerialze<List<ItemPwd>>(data);
        ResultModel rm = new ResultModel();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemPwd dr in list)
                {
                    if (dr.UserPwd2 == string.Empty)
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "请输入新密码！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    if (dr.UserPwd2 != dr.UserPwd3)
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "两次密码输入不一致！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    YXJ.Entity.Users u = new Users();
                    string userloginname = u.GetUserLoginName(LoginController.User.Id);
                    if (userloginname == "")
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "用户不正确！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    if (!YXJ.Entity.Users.Exists(userloginname, dr.UserPwd1))
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "“原始密码”输入不正确！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    else
                    {
                        if (!YXJ.Entity.Users.ChangePwd(dr.UserId, dr.UserPwd2))
                        {
                            rm.state = ResultStatu.失败;
                            rm.error = "修改密码失败，请重新尝试！";
                            Response.Write(rm.ToJsonString());
                            return;
                        }
                        rm.state = ResultStatu.成功;
                        Response.Write(rm.ToJsonString());
                        Response.End();
                    }
                }
            }
        }
    }

    public class ItemCustomer
    {
        public string CompanyName { get; set; }//客户公司全称
        public string CompanyAbbreviation { get; set; }//客户公司简称
        public string PrimaryContact { get; set; }//客户联系人姓名
        public string TaxPoint { get; set; }//纳税点
        public string RegistrationTime { get; set; }//客户注册时间
        public string BusinessAddress { get; set; }//公司经营地址
        public string SalesmanName { get; set; }//业务员的姓名
        public int Recordcount { get; set; }//总数量
    }
    /// <summary>
    /// 获取客户列表
    /// </summary>
    private void GetCustomerList()
    {
        ResultModel rm = new ResultModel();
        int tpagesize = GetIntRequest("pagesize");//每页显示数量
        int tpageindex = GetIntRequest("pageindex");//当前页数
        int RecordCount = 0;
        DataTable dt = new Customer().GetCustomerList(tpageindex, tpagesize, ref RecordCount);

        if (null == dt)
        {
            rm.error = "暂无客户！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemAddCustomer> list = new List<ItemAddCustomer>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemAddCustomer itemcustomer = new ItemAddCustomer();
                itemcustomer.Id = dr["Id"].ToString();
                itemcustomer.CompanyName = dr["CompanyName"].ToString();
                itemcustomer.CompanyAbbreviation = dr["CompanyAbbreviation"].ToString();
                itemcustomer.TaxpayerNo = dr["TaxpayerNo"].ToString();
                itemcustomer.RegisteredAddress = dr["RegisteredAddress"].ToString();
                itemcustomer.BusinessAddress = dr["BusinessAddress"].ToString();
                itemcustomer.Bank = dr["Bank"].ToString();
                itemcustomer.BankAccountNo = dr["BankAccountNo"].ToString();
                itemcustomer.InvoiceType = dr["InvoiceType"].ToString();
                itemcustomer.TaxPoint = dr["TaxPoint"].ToString();
                itemcustomer.PrimaryContact = dr["PrimaryContact"].ToString();
                itemcustomer.PrimaryTel = dr["PrimaryTel"].ToString();
                itemcustomer.PrimarySex = dr["PrimarySex"].ToString().ToInt();
                itemcustomer.CustomerFax = dr["CustomerFax"].ToString();
                itemcustomer.CustomerPostalCode = dr["CustomerPostalCode"].ToString();
                itemcustomer.CustomerEmail = dr["CustomerEmail"].ToString();
                itemcustomer.SecondaryContact = dr["SecondaryContact"].ToString();
                itemcustomer.SecondaryTel = dr["SecondaryTel"].ToString();
                itemcustomer.RegistrationTime = dr["RegistrationTime"].ToString();
                itemcustomer.MonthlyStatementTime = dr["MonthlyStatementTime"].ToString();
                itemcustomer.BranchId = dr["BranchId"].ToString();
                itemcustomer.SalesmanId = dr["SalesmanId"].ToString();
                itemcustomer.SalesmanName = dr["UserName"].ToString();
                itemcustomer.VersionId = dr["VersionId"].ToString();
                itemcustomer.Status = dr["Status"].ToString().ToInt();
                itemcustomer.AuditorRemarks = dr["AuditorRemarks"].ToString();
                itemcustomer.CustomerNumber = dr["CustomerNumber"].ToString();
                list.Add(itemcustomer);
            }
            rm.state = ResultStatu.成功;

            Dictionary<string, List<ItemAddCustomer>> JsonDJ = new Dictionary<string, List<ItemAddCustomer>>();
            List<ItemAddCustomer> listnew = new List<ItemAddCustomer>();
            ItemAddCustomer itc = new ItemAddCustomer();
            itc.Recordcount = RecordCount;
            listnew.Add(itc);

            JsonDJ.Add("customer", list);
            JsonDJ.Add("recordcount", listnew);
            string JsonDT = JsonHelper.newTonSerialize(JsonDJ);

            Response.Write(JsonDT);
            Response.End();
        }
    }
    /// <summary>
    /// 获取单个客户的信息
    /// </summary>
    private void GetCustomerInfo()
    {
        ResultModel rm = new ResultModel();
        int CustomerId = GetIntRequest("id");//客户的id
        string sql = string.Format("select * from Customer where Id={0} and IsDelete=0", CustomerId);
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "未查询到该客户信息！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemAddCustomer> list = new List<ItemAddCustomer>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemAddCustomer Itemcustomerinfo = new ItemAddCustomer();
                Itemcustomerinfo.Id = dr["Id"].ToString();
                Itemcustomerinfo.CompanyName = dr["CompanyName"].ToString();
                Itemcustomerinfo.CompanyAbbreviation = dr["CompanyAbbreviation"].ToString();
                Itemcustomerinfo.TaxpayerNo = dr["TaxpayerNo"].ToString();
                Itemcustomerinfo.RegisteredAddress = dr["RegisteredAddress"].ToString();
                Itemcustomerinfo.BusinessAddress = dr["BusinessAddress"].ToString();
                Itemcustomerinfo.Bank = dr["Bank"].ToString();
                Itemcustomerinfo.BankAccountNo = dr["BankAccountNo"].ToString();
                Itemcustomerinfo.InvoiceType = dr["InvoiceType"].ToString();
                Itemcustomerinfo.TaxPoint = dr["TaxPoint"].ToString();
                Itemcustomerinfo.PrimaryContact = dr["PrimaryContact"].ToString();
                Itemcustomerinfo.PrimaryTel = dr["PrimaryTel"].ToString();
                Itemcustomerinfo.PrimarySex = dr["PrimarySex"].ToString().ToInt();
                Itemcustomerinfo.CustomerFax = dr["CustomerFax"].ToString();
                Itemcustomerinfo.CustomerPostalCode = dr["CustomerPostalCode"].ToString();
                Itemcustomerinfo.CustomerEmail = dr["CustomerEmail"].ToString();
                Itemcustomerinfo.SecondaryContact = dr["SecondaryContact"].ToString();
                Itemcustomerinfo.SecondaryTel = dr["SecondaryTel"].ToString();
                Itemcustomerinfo.RegistrationTime = dr["RegistrationTime"].ToString();
                Itemcustomerinfo.MonthlyStatementTime = dr["MonthlyStatementTime"].ToString();
                Itemcustomerinfo.SalesmanId = dr["SalesmanId"].ToString();
                Itemcustomerinfo.BranchId = dr["BranchId"].ToString();
                Itemcustomerinfo.CustomerNumber = dr["CustomerNumber"].ToString();
                list.Add(Itemcustomerinfo);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemAddCustomer>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }
    public class ItemAddCustomer
    {
        public string Id { get; set; }//客户的Id
        public string CompanyName { get; set; }//客户公司全称
        public string CompanyAbbreviation { get; set; }//客户公司简称
        public string TaxpayerNo { get; set; }//纳税人识别号
        public string RegisteredAddress { get; set; }//公司注册地址
        public string BusinessAddress { get; set; }//公司经营地址
        public string Bank { get; set; }//公司开户银行
        public string BankAccountNo { get; set; }//开户行账号
        public string InvoiceType { get; set; }//开票类型
        public string TaxPoint { get; set; }//纳税点
        public string PrimaryContact { get; set; }//主要联系人
        public string PrimaryTel { get; set; }//主要联系人电话
        public int PrimarySex { get; set; }//主要联系人性别
        public string CustomerFax { get; set; }//客户传真
        public string CustomerPostalCode { get; set; }//客户邮政编码
        public string CustomerEmail { get; set; }//客户电子邮件
        public string SecondaryContact { get; set; }//次要联系人
        public string SecondaryTel { get; set; }//次要联系人电话
        public string RegistrationTime { get; set; }//客户注册时间
        public string MonthlyStatementTime { get; set; }//月结时间
        public string SalesmanId { get; set; }//业务员的id
        public string BranchId { get; set; }//分公司编号

        public string SalesmanName { get; set; }//业务员的姓名
        public int Recordcount { get; set; }//总数量

        public string VersionId { get; set; }//修改前一版本的id
        public int Status { get; set; }//审核状态 0待审核 1通过 2不通过 3不需要审核
        public string AuditorRemarks { get; set; }//审批备注
        public string CustomerNumber { get; set; }//客户编号
    }

    /// <summary>
    /// 新增客户
    /// </summary>
    private void AddCustomer()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }

        ResultModel rm = new ResultModel();
        List<ItemAddCustomer> list = (List<ItemAddCustomer>)JsonHelper.DeSerialze<List<ItemAddCustomer>>(data);
        int id = 0;
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemAddCustomer dr in list)
                {
                    //查询用户是否存在
                    string sql = string.Format("select count(1) from Customer where CompanyName='{0}'", dr.CompanyName);
                    System.Data.DataTable dt = DMain.Get(sql);
                    int count = dt.Rows[0][0].ToString().ToInt();
                    if (count >= 1)
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "客户已经存在了！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    //取到当前时间的年、月、日、分、秒的值，并使用字符串格式把它们组合成一个字符串
                    string number = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    Customer cus = new Customer()
                    {
                        CompanyName = dr.CompanyName,
                        CompanyAbbreviation = dr.CompanyAbbreviation,
                        TaxpayerNo = dr.TaxpayerNo,
                        RegisteredAddress = dr.RegisteredAddress,
                        BusinessAddress = dr.BusinessAddress,
                        Bank = dr.Bank,
                        BankAccountNo = dr.BankAccountNo,
                        InvoiceType = dr.InvoiceType,
                        TaxPoint = dr.TaxPoint,
                        PrimaryContact = dr.PrimaryContact,
                        PrimaryTel = dr.PrimaryTel,
                        PrimarySex = dr.PrimarySex,
                        CustomerFax = dr.CustomerFax,
                        CustomerPostalCode = dr.CustomerPostalCode,
                        CustomerEmail = dr.CustomerEmail,
                        SecondaryContact = dr.SecondaryContact,
                        SecondaryTel = dr.SecondaryTel,
                        RegistrationTime = DateTime.Now,
                        MonthlyStatementTime = Convert.ToDateTime(dr.MonthlyStatementTime),
                        //SalesmanId = LoginController.User.Id,//默认当前登录人
                        SalesmanId = dr.SalesmanId.ToInt(),
                        BranchId = dr.BranchId.ToInt(),
                        IsDelete = 0,
                        InsertTime = DateTime.Now,
                        InsertUserId = LoginController.User.Id,
                        UpdateTime = Const.DateTimeDefaultValue,//1900
                        UpdateUserId = 0,
                        Version = 1,
                        VersionId = 0,
                        Status = 3,//新增不需要审核
                        Show = 0,
                        AuditorRemarks = "",
                        CustomerNumber = number,
                    };
                    id = cus.Save();

                    CustomerLog cuslog = new CustomerLog();
                    if (id > 0)
                    {
                        cuslog.CustomerId = id;
                        cuslog.CustomerNumber = number;
                        cuslog.Type = 0;
                        cuslog.Status = 3;
                        cuslog.InsertTime = DateTime.Now;
                        cuslog.InsertUserId = LoginController.User.Id;
                        cuslog.OperationStatus = "成功";
                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        cuslog.CustomerId = id;
                        cuslog.CustomerNumber = number;
                        cuslog.Type = 0;
                        cuslog.Status = 3;
                        cuslog.InsertTime = DateTime.Now;
                        cuslog.InsertUserId = LoginController.User.Id;
                        cuslog.OperationStatus = "失败";
                        rm.state = ResultStatu.失败;
                        rm.error = "新客户创建失败！";
                    }
                    cuslog.Save();
                }
            }
        }
        Response.Write(rm.ToJsonString());
        Response.End();
    }
    /// <summary>
    /// 编辑客户
    /// </summary>
    private void EditorCustomer()
    {
        string data = "";
        ResultModel rm = new ResultModel();
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemAddCustomer> list = (List<ItemAddCustomer>)JsonHelper.DeSerialze<List<ItemAddCustomer>>(data);
        //查询当前操作人的权限，是直接修改权限，还是需要什么的权限
        int AuditType = 0;//是否需要审核 0不要审核 1要审核
        string sql = string.Format("select AuditType from UserMenu where UserId={0} and IsDelete=0 and MenuId=5", LoginController.User.Id);
        System.Data.DataTable dt1 = DMain.Get(sql);
        if (null == dt1 || dt1.Rows.Count <= 0)
        {
            rm.error = "您没有编辑订单的权限！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            AuditType = dt1.Rows[0]["AuditType"].ToString().ToInt();
        }
        int cid = 0;
        bool flag = false;
        CustomerLog cuslog = new CustomerLog();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemAddCustomer dr in list)
                {
                    string sql1 = string.Format("select count(1) from Customer where CustomerNumber='{0}'", dr.CustomerNumber);
                    System.Data.DataTable dtnew = DMain.Get(sql1);
                    int count = dtnew.Rows[0][0].ToString().ToInt();

                    Customer cus = new Customer()
                    {
                        Id = dr.Id.ToInt(),
                    };
                    if (cus.Id < 1)
                    {
                        rm.error = "客户不存在!";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    if (AuditType == 1)//是否需要审核 0不要审核 1要审核
                    {
                        cus.Id = 0;
                        cus.CustomerNumber = dr.CustomerNumber;
                        cus.CompanyName = dr.CompanyName;
                        cus.CompanyAbbreviation = dr.CompanyAbbreviation;
                        cus.TaxpayerNo = dr.TaxpayerNo;
                        cus.RegisteredAddress = dr.RegisteredAddress;
                        cus.BusinessAddress = dr.BusinessAddress;
                        cus.Bank = dr.Bank;
                        cus.BankAccountNo = dr.BankAccountNo;
                        cus.InvoiceType = dr.InvoiceType;
                        cus.TaxPoint = dr.TaxPoint;
                        cus.PrimaryContact = dr.PrimaryContact;
                        cus.PrimaryTel = dr.PrimaryTel;
                        cus.PrimarySex = dr.PrimarySex;
                        cus.CustomerFax = dr.CustomerFax;
                        cus.CustomerPostalCode = dr.CustomerPostalCode;
                        cus.CustomerEmail = dr.CustomerEmail;
                        cus.SecondaryContact = dr.SecondaryContact;
                        cus.SecondaryTel = dr.SecondaryTel;
                        cus.RegistrationTime = Convert.ToDateTime(dr.RegistrationTime);
                        cus.MonthlyStatementTime = Convert.ToDateTime(dr.MonthlyStatementTime);
                        cus.SalesmanId = dr.SalesmanId.ToInt();
                        cus.BranchId = dr.BranchId.ToInt();
                        cus.InsertTime = DateTime.Now;
                        cus.InsertUserId = LoginController.User.Id;//修改人为当前登录人,修改人就是创建人
                        cus.UpdateTime = Const.DateTimeDefaultValue;//1900
                        cus.UpdateUserId = 0;
                        cus.Version = count + 1;//版本加一
                        cus.VersionId = dr.Id.ToInt();
                        cus.Status = 0;//修改要审核
                        cus.Show = 0;
                        cus.AuditorRemarks = "";
                        cus.Id = cus.Save();
                        cid = cus.Id;
                        
                        if (cid > 0)
                        {
                            string sql2 = string.Format("update Customer set Show=1 where id={0}", dr.Id.ToInt());
                            flag = DMain.RunUpdate(sql2);
                        }
                        //存日志
                        if (flag)
                        {
                            cuslog.CustomerId = dr.Id.ToInt();
                            cuslog.CustomerNumber = dr.CustomerNumber;
                            cuslog.Type = 1;
                            cuslog.Status = 0;
                            cuslog.InsertTime = DateTime.Now;
                            cuslog.InsertUserId = LoginController.User.Id;
                            cuslog.OperationStatus = "成功";
                            rm.state = ResultStatu.成功;
                            
                        }
                        else
                        {
                            cuslog.CustomerId = dr.Id.ToInt();
                            cuslog.CustomerNumber = dr.CustomerNumber;
                            cuslog.Type = 1;
                            cuslog.Status = 0;
                            cuslog.InsertTime = DateTime.Now;
                            cuslog.InsertUserId = LoginController.User.Id;
                            cuslog.OperationStatus = "失败";
                            rm.state = ResultStatu.失败;
                            rm.error = "客户修改失败！";
                        }
                        cuslog.Save();
                        Response.Write(rm.ToJsonString());
                        Response.End();
                    }
                    else if (AuditType == 0)//是否需要审核 0不要审核 1要审核
                    {
                        cus.Id = 0;
                        cus.CustomerNumber = dr.CustomerNumber;
                        cus.CompanyName = dr.CompanyName;
                        cus.CompanyAbbreviation = dr.CompanyAbbreviation;
                        cus.TaxpayerNo = dr.TaxpayerNo;
                        cus.RegisteredAddress = dr.RegisteredAddress;
                        cus.BusinessAddress = dr.BusinessAddress;
                        cus.Bank = dr.Bank;
                        cus.BankAccountNo = dr.BankAccountNo;
                        cus.InvoiceType = dr.InvoiceType;
                        cus.TaxPoint = dr.TaxPoint;
                        cus.PrimaryContact = dr.PrimaryContact;
                        cus.PrimaryTel = dr.PrimaryTel;
                        cus.PrimarySex = dr.PrimarySex;
                        cus.CustomerFax = dr.CustomerFax;
                        cus.CustomerPostalCode = dr.CustomerPostalCode;
                        cus.CustomerEmail = dr.CustomerEmail;
                        cus.SecondaryContact = dr.SecondaryContact;
                        cus.SecondaryTel = dr.SecondaryTel;
                        cus.RegistrationTime = Convert.ToDateTime(dr.RegistrationTime);
                        cus.MonthlyStatementTime = Convert.ToDateTime(dr.MonthlyStatementTime);
                        cus.SalesmanId = dr.SalesmanId.ToInt();
                        cus.BranchId = dr.BranchId.ToInt();
                        cus.InsertTime = DateTime.Now;
                        cus.InsertUserId = LoginController.User.Id;//修改人为当前登录人,修改人就是创建人
                        cus.UpdateTime = Const.DateTimeDefaultValue;//1900
                        cus.UpdateUserId = 0;
                        cus.Version = count + 1;//版本加一
                        cus.VersionId = dr.Id.ToInt();
                        cus.Status = 3;//修改不要审核
                        cus.Show = 0;
                        cus.AuditorRemarks = "";
                        cus.Id = cus.Save();
                        cid = cus.Id;

                        if (cid > 0)
                        {
                            string sql2 = string.Format("update Customer set Show=1 where id={0}", dr.Id.ToInt());
                            flag = DMain.RunUpdate(sql2);
                        }
                        //存日志
                        if (flag)
                        {
                            cuslog.CustomerId = dr.Id.ToInt();
                            cuslog.CustomerNumber = dr.CustomerNumber;
                            cuslog.Type = 1;
                            cuslog.Status = 3;
                            cuslog.InsertTime = DateTime.Now;
                            cuslog.InsertUserId = LoginController.User.Id;
                            cuslog.OperationStatus = "成功";
                            rm.state = ResultStatu.成功;

                        }
                        else
                        {
                            cuslog.CustomerId = dr.Id.ToInt();
                            cuslog.CustomerNumber = dr.CustomerNumber;
                            cuslog.Type = 1;
                            cuslog.Status = 3;
                            cuslog.InsertTime = DateTime.Now;
                            cuslog.InsertUserId = LoginController.User.Id;
                            cuslog.OperationStatus = "失败";
                            rm.state = ResultStatu.失败;
                            rm.error = "客户修改失败！";
                        }
                        cuslog.Save();
                        Response.Write(rm.ToJsonString());
                        Response.End();
                    }
                    
                }
            }
        }
    }

    public class ItemDeleteCustomer
    {
        public string id { get; set; }//客户的Id列表
        public string CustomerNumber { get; set; }//客户编号
    }
    /// <summary>
    /// 删除客户
    /// </summary>
    private void DeleteCustomer()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemDeleteCustomer> list = (List<ItemDeleteCustomer>)JsonHelper.DeSerialze<List<ItemDeleteCustomer>>(data);
        ResultModel rm = new ResultModel()
        {
            state = ResultStatu.失败
        };
        int idlog = 0;
        foreach (ItemDeleteCustomer dr in list)
        {
            Customer cus = new Customer();
            if (!cus.Load(dr.id.ToInt()))
            {
                rm.error = "客户不存在";
                Response.Write(rm.ToJsonString());
                return;
            }
            string result = cus.UpdateDeleteCus(dr.CustomerNumber);
            CustomerLog cuslog = new CustomerLog();
            if (result != "0")
            {
                cuslog.CustomerId = dr.id.ToInt();
                cuslog.CustomerNumber = dr.CustomerNumber;
                cuslog.Type = 2;
                cuslog.Status = 4;
                cuslog.InsertTime = DateTime.Now;
                cuslog.InsertUserId = LoginController.User.Id;
                cuslog.OperationStatus = "成功";
                idlog = cuslog.Save();
                if (idlog <= 0)
                {
                    rm.state = ResultStatu.失败;
                    rm.error = "客户删除成功，操作日志存储失败！";
                    Response.Write(rm.ToJsonString());
                    return;
                }
            }
            else
            {
                cuslog.CustomerId = dr.id.ToInt();
                cuslog.CustomerNumber = dr.CustomerNumber;
                cuslog.Type = 2;
                cuslog.Status = 4;
                cuslog.InsertTime = DateTime.Now;
                cuslog.InsertUserId = LoginController.User.Id;
                cuslog.OperationStatus = "失败";
                idlog = cuslog.Save();
                if (idlog <= 0)
                {
                    rm.state = ResultStatu.失败;
                    rm.error = "客户删除操作失败！";
                    Response.Write(rm.ToJsonString());
                    return;
                }

                rm.state = ResultStatu.失败;
                rm.error = "客户删除失败！";
                Response.Write(rm.ToJsonString());
                return;
            }
        }
        rm.state = ResultStatu.成功;
        Response.Write(rm.ToJsonString());
        Response.End();
    }

    public class ItemCustomerLog
    {
        public int id { get; set; }//客户的id
        public string CustomerNumber { get; set; }//客户编号
    }
    public class ItemCustomerLoglist
    {
        public string CompanyAbbreviation { get; set; }//客户的简称
        public string InsertTime { get; set; }//操作时间
        public string Type_Show { get; set; }//操作状态
        public string UserName { get; set; }//操作人
    }
    /// <summary>
    /// 客户操作日志
    /// </summary>
    private void CustomerLogList()
    {
        ResultModel rm = new ResultModel();
        int id = 0;//客户的id
        string data = "";
        string CustomerNumber = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemCustomerLog> list = (List<ItemCustomerLog>)JsonHelper.DeSerialze<List<ItemCustomerLog>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemCustomerLog dr in list)
                {
                    id = dr.id;
                    CustomerNumber = dr.CustomerNumber;
                }
            }
        }
        if (id < 0)
        {
            rm.error = "请选择客户！";
            Response.Write(rm.ToJsonString());
            return;
        }
        CustomerLog cuslog = new CustomerLog();
        DataTable dt = cuslog.GetCustomerLogList(CustomerNumber);
        if (null == dt)
        {
            rm.error = "该客户暂无操作记录！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemCustomerLoglist> listnew = new List<ItemCustomerLoglist>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemCustomerLoglist customerlog = new ItemCustomerLoglist();
                customerlog.CompanyAbbreviation = dr["CompanyAbbreviation"].ToString();
                customerlog.InsertTime = dr["InsertTime"].ToString();
                customerlog.Type_Show = dr["Type_Show"].ToString();
                customerlog.UserName = dr["UserName"].ToString();
                listnew.Add(customerlog);
            }
            var arrcount = listnew.ToList();
            string datanew = JsonHelper.Serializer<ItemCustomerLoglist>(arrcount);
            Response.Write(datanew);
            Response.End();
        }
    }

    public class ItemOrganization
    {
        public string OrganizationName { get; set; }
    }
    /// <summary>
    /// 新增组织
    /// </summary>
    private void AddOrganization()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        ResultModel rm = new ResultModel();
        List<ItemOrganization> list = (List<ItemOrganization>)JsonHelper.DeSerialze<List<ItemOrganization>>(data);
        int id = 0;
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemOrganization dr in list)
                {
                    Organization org = new Organization();
                    string sql = string.Format("select count(1) from Organization where OrganizationName='{0}'", dr.OrganizationName);
                    System.Data.DataTable dt = DMain.Get(sql);
                    int count = dt.Rows[0][0].ToString().ToInt();
                    if (count >= 1)
                    {
                        rm.error = "组织已经存在了！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    org.OrganizationName = dr.OrganizationName;
                    org.InsertTime = DateTime.Now;
                    org.InsertUserId = LoginController.User.Id;
                    org.UpdateTime = Const.DateTimeDefaultValue;
                    org.IsDelete = 0;
                    id = org.Save();
                    if (id > 0)
                    {
                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "新组织创建失败！";
                    }
                    Response.Write(rm.ToJsonString());
                    Response.End();
                }
            }
        }
    }

    public class ItemOrganizationUserName
    {
        public string UserLoginName { get; set; }
    }

    public class ItemOrganizationList
    {
        public int id { get; set; }//组织id
        public string name { get; set; }//组织名称
        public string children { get; set; }//数据
    }
    /// <summary>
    /// 组织树状图
    /// </summary>
    private void GetOrganizationList()
    {
        ResultModel rm = new ResultModel();
        string sql = string.Format("select Id ,OrganizationName from Organization where IsDelete=0");
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {

            rm.error = "暂无组织！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            //Dictionary<string, List<ItemOrganizationList>> JsonD = new Dictionary<string, List<ItemOrganizationList>>();
            List<ItemOrganizationList> list1 = new List<ItemOrganizationList>();
            foreach (DataRow dr in dt.Rows)
            {
                string sql2 = string.Format("select Id,UserLoginName from Users where OrganizationId={0}", dr["Id"]);
                System.Data.DataTable dt2 = DMain.Get(sql2);
                List<ItemOrganizationList> list2 = new List<ItemOrganizationList>();
                foreach (DataRow dr2 in dt2.Rows)
                {
                    ItemOrganizationList itemorganization2 = new ItemOrganizationList();
                    itemorganization2.id = dr2["Id"].ToString().ToInt();
                    itemorganization2.name = dr2["UserLoginName"].ToString();
                    itemorganization2.children = "";
                    list2.Add(itemorganization2);
                }
                //JsonD.Add(dr["OrganizationName"].ToString(), list1);
                var arrcount = list2.ToList();
                string datanew = JsonHelper.Serializer<ItemOrganizationList>(arrcount);
                ItemOrganizationList itemorganization1 = new ItemOrganizationList();
                itemorganization1.id = dr["Id"].ToString().ToInt();
                itemorganization1.name = dr["OrganizationName"].ToString();
                itemorganization1.children = datanew;
                list1.Add(itemorganization1);
            }
            //string JsonDT = JsonHelper.Serializer(JsonD);   
            var arrcountnew = list1.ToList();
            string dataresule = JsonHelper.Serializer<ItemOrganizationList>(arrcountnew);
            Response.Write(dataresule);
            Response.End();
        }
    }

    public class OrganizationList
    {
        public int id { get; set; }//组织id
        public string organizationname { get; set; }//组织名称
        public string inserttime { get; set; }//组织创建时间
    }
    /// <summary>
    /// 组织列表
    /// </summary>
    private void GetOrganization()
    {
        ResultModel rm = new ResultModel();

        Organization org = new Organization();
        string sql = string.Format("select Id,OrganizationName,InsertTime from Organization where IsDelete=0");
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.state = ResultStatu.失败;
            rm.error = "暂无组织！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<OrganizationList> list = new List<OrganizationList>();
            foreach (DataRow dr in dt.Rows)
            {
                OrganizationList organizationlist = new OrganizationList();
                organizationlist.id = dr["Id"].ToString().ToInt();
                organizationlist.organizationname = dr["OrganizationName"].ToString();
                organizationlist.inserttime = dr["InsertTime"].ToString();
                list.Add(organizationlist);

            }
            var arrcount = list.ToList();
            string datanew = JsonHelper.newTonSerialize(arrcount);
            Response.Write(datanew);
            Response.End();
        }
    }
    public class ItemEditorOrganization
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
    }
    /// <summary>
    /// 编辑组织
    /// </summary>
    private void EditorOrganization()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        ResultModel rm = new ResultModel();
        List<ItemEditorOrganization> list = (List<ItemEditorOrganization>)JsonHelper.DeSerialze<List<ItemEditorOrganization>>(data);
        int id = 0;
        Organization org = new Organization();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemEditorOrganization dr in list)
                {
                    id = dr.Id;
                    if (!org.Load(id))
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "组织不存在!";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    org.Id = dr.Id;
                    org.OrganizationName = dr.OrganizationName;
                    org.UpdateTime = DateTime.Now;
                    org.Id = org.Save();
                    id = org.Id;

                    if (id == dr.Id)
                    {
                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "组织修改失败！";
                    }
                }
            }
        }
        Response.Write(rm.ToJsonString());
        Response.End();
    }

    public class ItemDeleteOrganization
    {
        public int Id { get; set; }//组织的id
    }
    /// <summary>
    /// 删除组织
    /// </summary>
    private void DeleteOrganization()
    {
        ResultModel rm = new ResultModel()
        {
            state = ResultStatu.失败
        };
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemDeleteOrganization> list = (List<ItemDeleteOrganization>)JsonHelper.DeSerialze<List<ItemDeleteOrganization>>(data);
        Organization org = new Organization();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemDeleteOrganization dr in list)
                {
                    if (!org.Load(dr.Id))
                    {
                        rm.error = "该组织不存在";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    if (org.UpdateDelete(dr.Id))
                    {
                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "组织删除失败！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                }
            }
        }
        Response.Write(rm.ToJsonString());
        Response.End();
    }
    /// <summary>
    /// 获取单个组织的信息
    /// </summary>
    private void GetOrganizationInfo()
    {
        ResultModel rm = new ResultModel();
        int OrganizationId = GetIntRequest("id");//组织的id
        string sql = string.Format("select * from Organization where id={0} and IsDelete=0", OrganizationId);
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "未查询到该组织的信息！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemEditorOrganization> list = new List<ItemEditorOrganization>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemEditorOrganization Itemeditororganization = new ItemEditorOrganization();
                Itemeditororganization.Id = dr["Id"].ToString().ToInt();
                Itemeditororganization.OrganizationName = dr["OrganizationName"].ToString();
                list.Add(Itemeditororganization);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemEditorOrganization>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }

    public class ItemForwarderList
    {
        public int PageIndex { get; set; }//页码
        public int PageSize { get; set; }//每页的数量
        public string ForwarderName { get; set; }//转包运商名称
        public string Enabled { get; set; }//是否启用 0不启用 1启用
    }
    /// <summary>
    /// 转包运商列表
    /// </summary>
    private void GetForwarderList()
    {
        ResultModel rm = new ResultModel();
        string data = "";
        int PageIndex = 1;//页码
        int PageSize = 10;//每页的数量
        int RecordCount = 0;//返回的总数
        string ForwarderName = "";
        int Enabled = -1;
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemForwarderList> list1 = (List<ItemForwarderList>)JsonHelper.DeSerialze<List<ItemForwarderList>>(data);
        Forwarder forw = new Forwarder();
        if (list1 != null)
        {
            if (list1.Count > 0)
            {
                foreach (ItemForwarderList dr1 in list1)
                {
                    PageIndex = dr1.PageIndex;
                    PageSize = dr1.PageSize;
                    ForwarderName = dr1.ForwarderName;
                    Enabled = dr1.Enabled.ToInt();

                    System.Data.DataTable dt = forw.GetOrderFormList(PageIndex, PageSize, ref RecordCount, ForwarderName, Enabled);
                    if (null == dt)
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "暂无转包运商！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    else
                    {
                        List<ItemEditorForwarder> list = new List<ItemEditorForwarder>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            ItemEditorForwarder itemforwarder = new ItemEditorForwarder();
                            itemforwarder.Id = dr["Id"].ToString().ToInt();
                            itemforwarder.ForwarderName = dr["ForwarderName"].ToString();
                            itemforwarder.ForwarderNo = dr["ForwarderNo"].ToString();
                            itemforwarder.Remark = dr["Remark"].ToString();
                            itemforwarder.Enabled = dr["Enabled"].ToString().ToInt();
                            itemforwarder.InsertTime = dr["InsertTime"].ToString();
                            itemforwarder.InsertUserId = dr["InsertUserId"].ToString().ToInt();
                            list.Add(itemforwarder);

                        }
                        //var arrcount = list.ToList();
                        Dictionary<string, List<ItemEditorForwarder>> JsonDJ = new Dictionary<string, List<ItemEditorForwarder>>();
                        List<ItemEditorForwarder> listnew = new List<ItemEditorForwarder>();
                        ItemEditorForwarder iof = new ItemEditorForwarder();
                        iof.RecordCount = RecordCount;
                        listnew.Add(iof);

                        JsonDJ.Add("forwarder", list);
                        JsonDJ.Add("recordcount", listnew);
                        string JsonDT = JsonHelper.newTonSerialize(JsonDJ);
                        Response.Write(JsonDT);
                        Response.End();
                    }
                }
            }
        }
    }

    public class ItemForwarder
    {
        public string ForwarderName { get; set; }//转包运商名称
        public string ForwarderNo { get; set; }//转包运商编号
        public string Remark { get; set; }
        public int Enabled { get; set; }
    }
    /// <summary>
    /// 新增转包运商
    /// </summary>
    private void AddForwarder()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }

        ResultModel rm = new ResultModel();
        List<ItemForwarder> list = (List<ItemForwarder>)JsonHelper.DeSerialze<List<ItemForwarder>>(data);
        int id = 0;
        OrderAmountLog orderamountlog = new OrderAmountLog();
        Forwarder forw = new Forwarder();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemForwarder dr in list)
                {
                    //查询转包运商是否存在
                    string sql = string.Format("select count(1) from Forwarder where ForwarderNo='{0}' and IsDelete=0", dr.ForwarderNo);
                    System.Data.DataTable dt = DMain.Get(sql);
                    int count = dt.Rows[0][0].ToString().ToInt();
                    if (count >= 1)
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "转包运商已经存在了！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    forw.ForwarderName = dr.ForwarderName;
                    forw.ForwarderNo = dr.ForwarderNo;
                    forw.Remark = dr.Remark;
                    forw.Enabled = dr.Enabled;
                    forw.InsertTime = DateTime.Now;
                    forw.UpdateTime = Const.DateTimeDefaultValue;
                    forw.InsertUserId = LoginController.User.Id;
                    forw.IsDelete = 0;
                    id = forw.Save();

                    if (id > 0)
                    {
                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "新转包运商创建失败！";
                    }
                    Response.Write(rm.ToJsonString());
                    Response.End();
                }
            }
        }
    }
    public class ItemEditorForwarder
    {
        public int Id { get; set; }
        public int RecordCount { get; set; }
        public string ForwarderName { get; set; }//转包运商名称
        public string ForwarderNo { get; set; }//转包运商编号
        public string Remark { get; set; }
        public int Enabled { get; set; }
        public string InsertTime { get; set; }//创建时间不变
        public int InsertUserId { get; set; }//创建人id
    }
    /// <summary>
    /// 编辑转包运商
    /// </summary>
    /// <param name="entity"></param>
    private void EditorForwarder()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        ResultModel rm = new ResultModel();
        List<ItemEditorForwarder> list = (List<ItemEditorForwarder>)JsonHelper.DeSerialze<List<ItemEditorForwarder>>(data);
        Forwarder forw = new Forwarder();
        int id = 0;
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemEditorForwarder dr in list)
                {
                    forw.Id = dr.Id;
                    if (!forw.Load(forw.Id))
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "转包运商不存在!";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    else
                    {
                        forw.ForwarderName = dr.ForwarderName;
                        forw.ForwarderNo = dr.ForwarderNo;
                        forw.Remark = dr.Remark;
                        forw.Enabled = dr.Enabled;
                        forw.InsertTime = Convert.ToDateTime(dr.InsertTime);
                        forw.UpdateTime = DateTime.Now;
                        forw.InsertUserId = dr.InsertUserId;
                    }
                    forw.Id = forw.Save();
                    id = forw.Id;
                }
            }
        }
        if (id > 0)
        {
            rm.state = ResultStatu.成功;
        }
        else
        {

            rm.state = ResultStatu.失败;
            rm.error = "转包运商修改失败！";
        }
        Response.Write(rm.ToJsonString());
        Response.End();
    }
    public class ItemForw
    {
        public int Id { get; set; }
    }
    /// <summary>
    /// 删除转包运商
    /// </summary>
    private void DeleteForwarder()
    {
        ResultModel rm = new ResultModel()
        {
            state = ResultStatu.失败
        };
        Forwarder forw = new Forwarder();
        int id = 0;
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemForw> list = (List<ItemForw>)JsonHelper.DeSerialze<List<ItemForw>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemForw dr in list)
                {
                    id = dr.Id;
                    if (!forw.Load(id))
                    {
                        rm.error = "该转包运商不存在";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    if (forw.UpdateDelete(id))
                    {
                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "转包运商删除失败！";
                    }
                }
            }
        }
        Response.Write(rm.ToJsonString());
        Response.End();
    }
    /// <summary>
    /// 获取单个转包运商的信息
    /// </summary>
    private void GetForwarderInfo()
    {
        ResultModel rm = new ResultModel();
        int ForwarderId = GetIntRequest("id");//转包运商的id
        string sql = string.Format("select * from Forwarder where id={0} and IsDelete=0", ForwarderId);
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "未查询到该转包运商的信息！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemForwarder> list = new List<ItemForwarder>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemForwarder itemforwarder = new ItemForwarder();
                //itemforwarder.Id = dr["Id"].ToString().ToInt();
                itemforwarder.ForwarderName = dr["ForwarderName"].ToString();
                list.Add(itemforwarder);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemForwarder>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }

    public class ItemOrderForm
    {
        public int Id { get; set; }//订单id
        public int CustomerId { get; set; }//客户id
        public string CompanyAbbreviation { get; set; }//客户简称
        //public int InsertUserId { get; set; }//订单创建人
        public string ExpressNumber { get; set; }//公司单号
        public int ForwarderId { get; set; }//转包运商id
        public string CooperationNo { get; set; }//合作单号
        public string ShipperName { get; set; }//发件人姓名
        public string ShipperTel { get; set; }//发件人电话
        public string ShipperDF { get; set; }//发件人地址国内外
        public string ShipperAddress { get; set; }//发件人详细地址
        public string RecipientCompany { get; set; }//收件人公司
        public string RecipientTel { get; set; }//收件人电话
        public string RecipientName { get; set; }//收件人姓名
        public string RecipientDF { get; set; }//收件人地址国内外
        public string RecipientAddress { get; set; }//收件人详细地址
        public string Packages { get; set; }//货物件数
        public string Weight { get; set; }//货物重量
        public string Volume { get; set; }//货物体积
        public string PackType { get; set; }//包装类型
        public string PackName { get; set; }//包装类型名称
        public string Measurementrules { get; set; }//计重比
        public string VolumeWeight { get; set; }//体积重量
        public string ChargeableWeight { get; set; }//计费重量
        public string TransportType { get; set; }//运输类型
        public string TransportName { get; set; }//运输类型
        public string Value { get; set; }//货值
        public string InsureType { get; set; }//是否投保
        public string ReceiptType { get; set; }//回单类型 0否 1是
        public string Payer { get; set; }//支付方
        public string Remark { get; set; }//说明
        public string Url { get; set; }//图片路径
        public DateTime CreationTime { get; set; }//订单生成时间
        public DateTime UpdateTime { get; set; }//状态更新时间
        public int IsDelete { get; set; }//是否删除 0不删除 1删除
        public string BalanceType { get; set; }//订单结算状态 0未结算 1已结算
        public int Version { get; set; }//历史版本
        public int Status { get; set; }//审核状态 0待审核 1通过 2不通过 3不需要审核
        public int Show { get; set; }//是否展示  0展示  1不展示
        public string CargoName { get; set; }//货单物流状态
        public int ShipperProvinceId { get; set; }//发件人省份id
        public int ShipperCityId { get; set; }//发件人城市id
        public int RecipientProvinceId { get; set; }//收件人省份id
        public int RecipientCityId { get; set; }//收件人城市id 
        public string ShipperCityName { get; set; }//发件人城市名称
        public string RecipientCityName { get; set; }//收件人城市名称

        public string ForwarderName { get; set; }//合作公司名称
        public string ForwarderNo { get; set; }//合作公司编码
        public string PayFreight { get; set; }//应付运费 
        public string PayOther { get; set; }//应付其他费用  
        public string PayCost { get; set; }//应付总费用  
        public int RecordCount { get; set; }//返回的总条数
        public string SalesmanName { get; set; }//业务员的姓名
        public string CargoType { get; set; }//货单物流状态id
    }

    public class ItemFormlist
    {
        public int pageindex { get; set; }//当前页数
        public int pagesize { get; set; }//每页显示数量
        public string cusid { get; set; }//客户id
        public string reccity { get; set; }//目的地
        public string expnumber { get; set; }//订单编号
        public string CargoType { get; set; }//物流状态类型
        public string forwid { get; set; }//合作公司
        public string starttime { get; set; }//开始时间
        public string endtime { get; set; }//结束时间
    }

    /// <summary>
    /// 获取订单列表
    /// </summary>
    private void GetOrderFormList()
    {
        ResultModel rm = new ResultModel();
        int tpagesize = 10;//每页显示数量
        int tpageindex = 1;//当前页数
        int RecordCount = 0;
        int cusid = 0;//客户id
        string reccity = null;//目的地
        int reccityid = 0;//目的地id
        string expnumber = null;//订单编号
        int CargoType = 0;//物流状态类型
        int forwid = 0;//合作公司
        string starttime = null;//开始时间
        string endtime = null;//结束时间
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemFormlist> list = (List<ItemFormlist>)JsonHelper.DeSerialze<List<ItemFormlist>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemFormlist dr in list)
                {
                    tpagesize = dr.pagesize;
                    tpageindex = dr.pageindex;
                    cusid = dr.cusid.ToInt();
                    reccity = dr.reccity;
                    expnumber = dr.expnumber;
                    CargoType = dr.CargoType.ToInt();
                    forwid = dr.forwid.ToInt();
                    starttime = dr.starttime;
                    endtime = dr.endtime;
                }
                if (!string.IsNullOrWhiteSpace(reccity))
                {
                    string sql1 = string.Format("select id from districts where ext_name='{0}'", reccity);
                    DataTable dt1 = DMain.Get(sql1);
                    if (null == dt1)
                    {
                        rm.error = "未查询到目的地信息！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    else
                    {
                        reccityid = dt1.Rows[0][0].ToString().ToInt();
                    }
                }
            }
        }
        DataTable dt = new OrderForm().GetOrderFormList(tpageindex, tpagesize, ref RecordCount, cusid, reccityid, expnumber, CargoType, forwid, starttime, endtime, -1);
        if (null == dt)
        {
            rm.error = "暂无订单！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemOrderForm> list1 = new List<ItemOrderForm>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemOrderForm itemorderform = new ItemOrderForm();
                itemorderform.Id = dr["Id"].ToString().ToInt();
                itemorderform.CustomerId = dr["CustomerId"].ToString().ToInt();
                itemorderform.CompanyAbbreviation = dr["CompanyAbbreviation"].ToString();
                //itemorderform.InsertUserId = LoginController.User.Id;
                itemorderform.ExpressNumber = dr["ExpressNumber"].ToString();
                itemorderform.ForwarderId = dr["ForwarderId"].ToString().ToInt();
                itemorderform.ForwarderName = dr["ForwarderName"].ToString();
                itemorderform.ForwarderNo = dr["ForwarderNo"].ToString();
                itemorderform.CooperationNo = dr["CooperationNo"].ToString();
                itemorderform.ShipperName = dr["ShipperName"].ToString();
                itemorderform.ShipperTel = dr["ShipperTel"].ToString();
                itemorderform.ShipperDF = dr["ShipperDF"].ToString();
                itemorderform.ShipperAddress = dr["ShipperAddress"].ToString();
                itemorderform.RecipientCompany = dr["RecipientCompany"].ToString();
                itemorderform.RecipientTel = dr["RecipientTel"].ToString();
                itemorderform.RecipientName = dr["RecipientName"].ToString();
                itemorderform.RecipientDF = dr["RecipientDF"].ToString();
                //itemorderform.RecipientCity = dr["RecipientCity"].ToString();
                itemorderform.RecipientAddress = dr["RecipientAddress"].ToString();
                itemorderform.Packages = dr["Packages"].ToString();
                itemorderform.Weight = dr["Weight"].ToString();
                itemorderform.Volume = dr["Volume"].ToString();
                itemorderform.PackType = dr["PackType"].ToString();
                itemorderform.PackName = dr["PackName"].ToString();
                itemorderform.Measurementrules = dr["Measurementrules"].ToString();
                itemorderform.VolumeWeight = dr["VolumeWeight"].ToString();
                itemorderform.ChargeableWeight = dr["ChargeableWeight"].ToString();
                itemorderform.TransportType = dr["TransportType"].ToString();
                itemorderform.Value = dr["Value"].ToString();
                itemorderform.InsureType = dr["InsureType"].ToString();
                itemorderform.ReceiptType = dr["ReceiptType"].ToString();
                itemorderform.Payer = dr["Payer"].ToString();
                itemorderform.Remark = dr["Remark"].ToString();
                //itemorderform.Url = dr["Url"].ToString();
                itemorderform.CreationTime = Convert.ToDateTime(dr["CreationTime"]);
                itemorderform.BalanceType = dr["BalanceType"].ToString();
                itemorderform.CargoName = dr["CargoName"].ToString();//货单物流状态名称
                itemorderform.CargoType = dr["CargoType"].ToString();//货单物流状态的id
                itemorderform.PayFreight = dr["PayFreight"].ToString();//应付运费 
                itemorderform.PayOther = dr["PayOther"].ToString();//应付其他费用  
                itemorderform.PayCost = dr["PayCost"].ToString();//应付总费用    
                itemorderform.ShipperProvinceId = dr["ShipperProvinceId"].ToString().ToInt();//发件人省份id
                itemorderform.ShipperCityId = dr["ShipperCityId"].ToString().ToInt();//发件人城市id
                itemorderform.RecipientProvinceId = dr["RecipientProvinceId"].ToString().ToInt();//收件人省份id
                itemorderform.RecipientCityId = dr["RecipientCityId"].ToString().ToInt();//收件人城市id
                itemorderform.ShipperCityName = dr["ShipperCity"].ToString();//发件人城市名称
                itemorderform.RecipientCityName = dr["RecipientCity"].ToString();//收件人城市名称
                itemorderform.SalesmanName = dr["UserName"].ToString();//业务员的姓名
                itemorderform.Status = dr["Status"].ToString().ToInt();//审核状态 0待审核 1通过 2不通过 3不需要审核
                list1.Add(itemorderform);
            }
            //var arrcount = list1.ToList();
            Dictionary<string, List<ItemOrderForm>> JsonDJ = new Dictionary<string, List<ItemOrderForm>>();
            List<ItemOrderForm> listnew = new List<ItemOrderForm>();
            ItemOrderForm iof = new ItemOrderForm();
            iof.RecordCount = RecordCount;
            listnew.Add(iof);

            JsonDJ.Add("orderform", list1);
            JsonDJ.Add("recordcount", listnew);
            string JsonDT = JsonHelper.newTonSerialize(JsonDJ);
            Response.Write(JsonDT);
            Response.End();
        }
    }

    public class ItemAddOrderForm
    {
        public string CustomerId { get; set; }//客户id
        public string ExpressNumber { get; set; }//公司单号
        public string ForwarderId { get; set; }//转包运商id
        public string CooperationNo { get; set; }//合作单号
        public string ShipperName { get; set; }//发件人姓名
        public string ShipperTel { get; set; }//发件人电话
        public string ShipperDF { get; set; }//发件人地址国内外
        public string ShipperAddress { get; set; }//发件人详细地址
        public string RecipientCompany { get; set; }//收件人公司
        public string RecipientTel { get; set; }//收件人电话
        public string RecipientName { get; set; }//收件人姓名
        public string RecipientDF { get; set; }//收件人地址国内外
        public string RecipientAddress { get; set; }//收件人详细地址
        public string Packages { get; set; }//货物件数
        public string Weight { get; set; }//货物重量
        public string Volume { get; set; }//货物体积
        public string PackType { get; set; }//包装类型
        public string Measurementrules { get; set; }//计重比
        public string VolumeWeight { get; set; }//体积重量
        public string ChargeableWeight { get; set; }//计费重量
        public string TransportType { get; set; }//运输类型
        public string Value { get; set; }//货值
        public string InsureType { get; set; }//是否投保
        public string ReceiptType { get; set; }//回单类型
        public string Payer { get; set; }//支付方
        public string Remark { get; set; }//说明
        public string VolumeJsonStr { get; set; }//记录体积的计算数据
        public string ShipperProvinceId { get; set; }//发件人省份id
        public string ShipperCityId { get; set; }//发件人城市id
        public string RecipientProvinceId { get; set; }//收件人省份id
        public string RecipientCityId { get; set; }//收件人城市id      
    }
    /// <summary>
    /// 新增订单
    /// </summary>
    private void AddOrderForm()
    {
        ResultModel rm = new ResultModel();
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemAddOrderForm> list = (List<ItemAddOrderForm>)JsonHelper.DeSerialze<List<ItemAddOrderForm>>(data);
        int id = 0;
        string ExpressNumber_s = "";
        OrderFormLog ordl = new OrderFormLog();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemAddOrderForm dr in list)
                {
                    //查询订单是否存在
                    string sql = string.Format("select count(1) from OrderForm where ExpressNumber='{0}' and Show=0 and IsDelete=0", dr.ExpressNumber);
                    System.Data.DataTable dt = DMain.Get(sql);
                    int count = dt.Rows[0][0].ToString().ToInt();
                    if (count >= 1)
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "订单已经存在了！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    OrderForm ord = new OrderForm()
                    {
                        CustomerId = dr.CustomerId.ToInt(),
                        InsertUserId = LoginController.User.Id,//新增人为当前登录人
                        ExpressNumber = dr.ExpressNumber,
                        ForwarderId = dr.ForwarderId.ToInt(),
                        CooperationNo = dr.CooperationNo,
                        ShipperName = dr.ShipperName,
                        ShipperTel = dr.ShipperTel,
                        ShipperDF = dr.ShipperDF,
                        ShipperProvinceId = dr.ShipperProvinceId.ToInt(),
                        ShipperCityId = dr.ShipperCityId.ToInt(),
                        ShipperAddress = dr.ShipperAddress,
                        RecipientCompany = dr.RecipientCompany,
                        RecipientTel = dr.RecipientTel,
                        RecipientName = dr.RecipientName,
                        RecipientDF = dr.RecipientDF,
                        RecipientProvinceId = dr.RecipientProvinceId.ToInt(),
                        RecipientCityId = dr.RecipientCityId.ToInt(),
                        RecipientAddress = dr.RecipientAddress,
                        Packages = dr.Packages,
                        Weight = dr.Weight,
                        Volume = dr.Volume,
                        // VolumeJsonStr = dr.VolumeJsonStr,//计算体积的字符串存储
                        PackType = dr.PackType,
                        Measurementrules = dr.Measurementrules,
                        VolumeWeight = dr.VolumeWeight,
                        ChargeableWeight = dr.ChargeableWeight,
                        TransportType = dr.TransportType,
                        Value = dr.Value,
                        InsureType = dr.InsureType,
                        ReceiptType = dr.ReceiptType,
                        Payer = dr.Payer,
                        Remark = dr.Remark,
                        //Url = dr.Url,
                        InsertTime = DateTime.Now,
                        UpdateTime = Const.DateTimeDefaultValue,
                        IsDelete = 0,
                        Version = 1,
                        Status = 3,
                        BalanceType = "0",
                        Show = 0,
                        CargoType = 10,//揽件
                        CreationTime = DateTime.Now,
                        VersionId = 0,
                    };
                    id = ord.Save();
                    ExpressNumber_s = dr.ExpressNumber;

                    //存日志
                    if (id > 0)
                    {
                        ordl.OrderFormId = id;
                        ordl.ExpressNumber = ExpressNumber_s;
                        ordl.Status = 3;//3不需要审核
                        ordl.Type = 0;//新增
                        ordl.InsertTime = DateTime.Now;
                        ordl.InsertUserId = LoginController.User.Id;
                        ordl.AuditTime = Const.DateTimeDefaultValue;
                        ordl.OperationStatus = "成功";
                        ordl.CustomerId = dr.CustomerId.ToInt();

                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        ordl.OrderFormId = id;
                        ordl.ExpressNumber = ExpressNumber_s;
                        ordl.Status = 3;//3不需要审核
                        ordl.Type = 0;//新增
                        ordl.InsertTime = DateTime.Now;
                        ordl.InsertUserId = LoginController.User.Id;
                        ordl.AuditTime = Const.DateTimeDefaultValue;
                        ordl.OperationStatus = "失败";
                        ordl.CustomerId = dr.CustomerId.ToInt();

                        rm.state = ResultStatu.失败;
                        rm.error = "新订单创建失败！";
                    }
                }
            }
        }
        ordl.Save();
        Response.Write(rm.ToJsonString());
        Response.End();
    }

    public class ItemEditorOrderForm
    {
        public int Id { get; set; }//订单的id
        public int CustomerId { get; set; }//客户id
        public string ExpressNumber { get; set; }//公司单号
        public int ForwarderId { get; set; }//转包运商id
        public string CooperationNo { get; set; }//合作单号
        public string ShipperName { get; set; }//发件人姓名
        public string ShipperTel { get; set; }//发件人电话
        public string ShipperDF { get; set; }//发件人地址国内外
        public string ShipperAddress { get; set; }//发件人详细地址
        public string RecipientCompany { get; set; }//收件人公司
        public string RecipientTel { get; set; }//收件人电话
        public string RecipientName { get; set; }//收件人姓名
        public string RecipientDF { get; set; }//收件人地址国内外
        public string RecipientAddress { get; set; }//收件人详细地址
        public string Packages { get; set; }//货物件数
        public string Weight { get; set; }//货物重量
        public string Volume { get; set; }//货物体积
        public string PackType { get; set; }//包装类型
        public string Measurementrules { get; set; }//计重比
        public string VolumeWeight { get; set; }//体积重量
        public string ChargeableWeight { get; set; }//计费重量
        public string TransportType { get; set; }//运输类型
        public string Value { get; set; }//货值
        public string InsureType { get; set; }//是否投保
        public string ReceiptType { get; set; }//回单类型
        public string Payer { get; set; }//支付方
        public string Remark { get; set; }//说明
        public string VolumeJsonStr { get; set; }//记录体积的计算数据
        //public int InsertUserId { get; set; }//原创建人的id
        public DateTime CreationTime { get; set; }//订单生成时间
        public string CargoType { get; set; }//货单物流状态id

        public int ShipperProvinceId { get; set; }//发件人省份id
        public int ShipperCityId { get; set; }//发件人城市id
        public int RecipientProvinceId { get; set; }//收件人省份id
        public int RecipientCityId { get; set; }//收件人城市id
        public string BalanceType { get; set; }//结算状态
    }
    /// <summary>
    /// 编辑订单
    /// </summary>
    private void EditorOrderForm()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        ResultModel rm = new ResultModel();
        List<ItemEditorOrderForm> list = (List<ItemEditorOrderForm>)JsonHelper.DeSerialze<List<ItemEditorOrderForm>>(data);

        //查询当前操作人的权限，是直接修改权限，还是需要什么的权限
        int AuditType = 0;//是否需要审核 0不要审核 1要审核
        string sql = string.Format("select AuditType from UserMenu where UserId={0} and IsDelete=0 and MenuId=10", LoginController.User.Id);
        System.Data.DataTable dt1 = DMain.Get(sql);
        if (null == dt1 || dt1.Rows.Count <= 0)
        {
            rm.error = "您没有编辑订单的权限！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            AuditType = dt1.Rows[0]["AuditType"].ToString().ToInt();
        }

        OrderForm ord1 = new OrderForm();
        int id = 0;
        string ExpressNumber_s = "";
        OrderFormLog ordl = new OrderFormLog();
        bool flag = false;
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemEditorOrderForm dr in list)
                {
                    string sql1 = string.Format("select count(1) from OrderForm where ExpressNumber='{0}'", dr.ExpressNumber);
                    System.Data.DataTable dtnew = DMain.Get(sql1);
                    int count = dtnew.Rows[0][0].ToString().ToInt();

                    ExpressNumber_s = dr.ExpressNumber;
                    DataTable dt = ord1.GetOrderForm(dr.Id, dr.ExpressNumber.ToString());
                    if (null == dt || 0 == dt.Rows.Count)
                    {
                        rm.error = "订单不存在!";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    else
                    {
                        if (dt != null && AuditType == 1)//此处应该是，如果操作人修改需要审核
                        {
                            OrderForm ord = new OrderForm()
                            {
                                CustomerId = dr.CustomerId,
                                InsertUserId = LoginController.User.Id,//修改人为当前登录人,修改人就是创建人
                                ExpressNumber = dr.ExpressNumber,
                                ForwarderId = dr.ForwarderId,
                                CooperationNo = dr.CooperationNo,
                                ShipperName = dr.ShipperName,
                                ShipperTel = dr.ShipperTel,
                                ShipperDF = dr.ShipperDF,
                                ShipperProvinceId = dr.ShipperProvinceId,
                                ShipperCityId = dr.ShipperCityId,
                                ShipperAddress = dr.ShipperAddress,
                                RecipientCompany = dr.RecipientCompany,
                                RecipientTel = dr.RecipientTel,
                                RecipientName = dr.RecipientName,
                                RecipientDF = dr.RecipientDF,
                                RecipientProvinceId = dr.RecipientProvinceId,
                                RecipientCityId = dr.RecipientCityId,
                                RecipientAddress = dr.RecipientAddress,
                                Packages = dr.Packages,
                                Weight = dr.Weight,
                                Volume = dr.Volume,
                                PackType = dr.PackType,
                                Measurementrules = dr.Measurementrules,
                                VolumeWeight = dr.VolumeWeight,
                                ChargeableWeight = dr.ChargeableWeight,
                                TransportType = dr.TransportType,
                                Value = dr.Value,
                                InsureType = dr.InsureType,
                                ReceiptType = dr.ReceiptType,
                                Payer = dr.Payer,
                                Remark = dr.Remark,
                                // Url = dr.Url,
                                InsertTime = DateTime.Now,
                                UpdateTime = Const.DateTimeDefaultValue,
                                IsDelete = 0,
                                Version = count + 1,//版本加一
                                Status = 0,//待审核
                                BalanceType = dr.BalanceType,
                                Show = 0,//0展示
                                CreationTime = dr.CreationTime,
                                CargoType = dr.CargoType.ToInt(),//货单的物流状态
                                VersionId = dr.Id,
                            };
                            id = ord.Save();
                            if (id > 0)
                            {
                                string sql2 = string.Format("update OrderForm set Show=1 where id={0}", dr.Id);
                                flag = DMain.RunUpdate(sql2);
                            }
                            //存日志
                            if (id > 0 && flag == true)
                            {
                                ordl.OrderFormId = id;
                                ordl.ExpressNumber = ExpressNumber_s;
                                ordl.Status = 0;//0待审核
                                ordl.Type = 1;//修改
                                ordl.InsertTime = DateTime.Now;
                                ordl.InsertUserId = LoginController.User.Id;
                                ordl.AuditTime = Const.DateTimeDefaultValue;
                                ordl.OperationStatus = "成功";
                                ordl.CustomerId = dr.CustomerId;
                                rm.state = ResultStatu.成功;
                            }
                            else
                            {
                                ordl.OrderFormId = id;
                                ordl.ExpressNumber = ExpressNumber_s;
                                ordl.Status = 0;//0待审核
                                ordl.Type = 1;//修改
                                ordl.InsertTime = DateTime.Now;
                                ordl.InsertUserId = LoginController.User.Id;
                                ordl.AuditTime = Const.DateTimeDefaultValue;
                                ordl.OperationStatus = "失败";
                                ordl.CustomerId = dr.CustomerId;
                                rm.state = ResultStatu.失败;
                                rm.error = "订单修改失败！";
                            }
                            ordl.Save();
                            Response.Write(rm.ToJsonString());
                            Response.End();
                        }
                        else if (dt != null && AuditType == 0)//此处应该是，如果操作人修改不需要审核
                        {
                            OrderForm ord = new OrderForm()
                            {
                                CustomerId = dr.CustomerId,
                                InsertUserId = LoginController.User.Id,//修改人为当前登录人,修改人和创建人都是当前登录人
                                ExpressNumber = dr.ExpressNumber,
                                ForwarderId = dr.ForwarderId,
                                CooperationNo = dr.CooperationNo,
                                ShipperName = dr.ShipperName,
                                ShipperTel = dr.ShipperTel,
                                ShipperDF = dr.ShipperDF,
                                ShipperProvinceId = dr.ShipperProvinceId,
                                ShipperCityId = dr.ShipperCityId,
                                ShipperAddress = dr.ShipperAddress,
                                RecipientCompany = dr.RecipientCompany,
                                RecipientTel = dr.RecipientTel,
                                RecipientName = dr.RecipientName,
                                RecipientDF = dr.RecipientDF,
                                RecipientProvinceId = dr.RecipientProvinceId,
                                RecipientCityId = dr.RecipientCityId,
                                RecipientAddress = dr.RecipientAddress,
                                Packages = dr.Packages,
                                Weight = dr.Weight,
                                Volume = dr.Volume,
                                PackType = dr.PackType,
                                Measurementrules = dr.Measurementrules,
                                VolumeWeight = dr.VolumeWeight,
                                ChargeableWeight = dr.ChargeableWeight,
                                TransportType = dr.TransportType,
                                Value = dr.Value,
                                InsureType = dr.InsureType,
                                ReceiptType = dr.ReceiptType,
                                Payer = dr.Payer,
                                Remark = dr.Remark,
                                // Url = dr.Url,
                                InsertTime = DateTime.Now,
                                UpdateTime = Const.DateTimeDefaultValue,
                                IsDelete = 0,
                                Version = count + 1,//版本加一
                                Status = 3,//不需要审核
                                BalanceType = dr.BalanceType,
                                Show = 0,//0展示
                                CargoType = dr.CargoType.ToInt(),
                                CreationTime = dr.CreationTime,
                                VersionId = dr.Id,
                            };
                            id = ord.Save();
                            if (id > 0)
                            {
                                string sql2 = string.Format("update OrderForm set Show=1 where id={0}", dr.Id);
                                flag = DMain.RunUpdate(sql2);
                            }
                            else
                            {
                                rm.state = ResultStatu.失败;
                                rm.error = "订单修改失败！";
                            }
                            if (id > 0 && flag == true)
                            {
                                ordl.OrderFormId = id;
                                ordl.ExpressNumber = ExpressNumber_s;
                                ordl.Status = 3;//不需要审核
                                ordl.Type = 1;//修改
                                ordl.InsertTime = DateTime.Now;
                                ordl.InsertUserId = LoginController.User.Id;
                                ordl.AuditTime = Const.DateTimeDefaultValue;
                                ordl.OperationStatus = "成功";
                                ordl.CustomerId = dr.CustomerId;
                                rm.state = ResultStatu.成功;
                            }
                            else
                            {
                                ordl.OrderFormId = id;
                                ordl.ExpressNumber = ExpressNumber_s;
                                ordl.Status = 3;//不需要审核
                                ordl.Type = 1;//修改
                                ordl.InsertTime = DateTime.Now;
                                ordl.InsertUserId = LoginController.User.Id;
                                ordl.AuditTime = Const.DateTimeDefaultValue;
                                ordl.OperationStatus = "失败";
                                ordl.CustomerId = dr.CustomerId;
                                rm.state = ResultStatu.失败;
                                rm.error = "订单修改失败！";
                            }
                            ordl.Save();
                            Response.Write(rm.ToJsonString());
                            Response.End();
                        }
                    }
                }
            }
        }

    }

    public class ItemDeleteOrderForm
    {
        public int id { get; set; }//id值
        public string ExpressNumber { get; set; }//订单编号
    }
    /// <summary>
    /// 删除订单  需要订单id和订单编号ExpressNumber
    /// </summary>
    public void DeleteOrderForm()
    {
        int idform = 0;
        int idamunt = 0;
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemDeleteOrderForm> list = (List<ItemDeleteOrderForm>)JsonHelper.DeSerialze<List<ItemDeleteOrderForm>>(data);
        ResultModel rm = new ResultModel()
        {
            state = ResultStatu.失败
        };
        OrderForm orderform = new OrderForm();
        OrderFormLog orderformlog = new OrderFormLog();
        OrderAmount orderamount = new OrderAmount();
        OrderAmountLog orderamountlog = new OrderAmountLog();
        foreach (ItemDeleteOrderForm dr in list)
        {
            if (!orderform.Load(dr.id))
            {
                rm.error = "订单不存在";
                Response.Write(rm.ToJsonString());
                return;
            }
            string result = orderform.UpdateDeleteEN(dr.ExpressNumber);
            string deleteid = "0";
            if (result != "0")
            {
                deleteid = orderamount.UpdateDeleteEN(dr.ExpressNumber);//删除账单

                orderformlog.OrderFormId = dr.id;
                orderformlog.ExpressNumber = dr.ExpressNumber;
                orderformlog.Status = 4;//4删除无状态
                orderformlog.Type = 2;//删除
                orderformlog.InsertTime = DateTime.Now;
                orderformlog.InsertUserId = LoginController.User.Id;
                orderformlog.AuditTime = Const.DateTimeDefaultValue;
                orderformlog.OperationStatus = "成功";
                orderformlog.Remark = result;

                idform = orderformlog.Save();
                if (idform <= 0)
                {
                    rm.state = ResultStatu.失败;
                    rm.error = "订单删除成功，操作日志存储失败！";
                    Response.Write(rm.ToJsonString());
                    return;
                }
            }
            else
            {
                orderformlog.OrderFormId = dr.id;
                orderformlog.ExpressNumber = dr.ExpressNumber;
                orderformlog.Status = 4;//4删除无状态
                orderformlog.Type = 2;//删除
                orderformlog.InsertTime = DateTime.Now;
                orderformlog.InsertUserId = LoginController.User.Id;
                orderformlog.AuditTime = Const.DateTimeDefaultValue;
                orderformlog.OperationStatus = "失败";
                orderformlog.Remark = "批量删除失败";

                idform = orderformlog.Save();
                if (idform <= 0)
                {
                    rm.state = ResultStatu.失败;
                    rm.error = "订单删除操作失败！";
                    Response.Write(rm.ToJsonString());
                    return;
                }

                rm.state = ResultStatu.失败;
                rm.error = "订单删除失败！";
                Response.Write(rm.ToJsonString());
                return;
            }
            if (deleteid != "0")
            {
                orderamountlog.ExpressNumber = dr.ExpressNumber;
                orderamountlog.OrderAmountId = 0;
                orderamountlog.Status = 4;//4删除无状态
                orderamountlog.Type = 2;//删除
                orderamountlog.InsertTime = DateTime.Now;
                orderamountlog.InsertUserId = LoginController.User.Id;
                orderamountlog.AuditTime = Const.DateTimeDefaultValue;
                orderamountlog.AuditUserId = 0;
                orderamountlog.OperationStatus = "成功";
                orderamountlog.Remark = deleteid;

                idamunt = orderamountlog.Save();
                if (idform <= 0)
                {
                    rm.state = ResultStatu.失败;
                    rm.error = "账单删除成功，操作日志存储失败！";
                    Response.Write(rm.ToJsonString());
                    return;
                }
            }
            else
            {
                orderamountlog.ExpressNumber = dr.ExpressNumber;
                orderamountlog.OrderAmountId = 0;
                orderamountlog.Status = 4;//4删除无状态
                orderamountlog.Type = 2;//删除
                orderamountlog.InsertTime = DateTime.Now;
                orderamountlog.InsertUserId = LoginController.User.Id;
                orderamountlog.AuditTime = Const.DateTimeDefaultValue;
                orderamountlog.AuditUserId = 0;
                orderamountlog.OperationStatus = "失败";
                orderamountlog.Remark = "账单删除失败";

                idamunt = orderamountlog.Save();
                if (idform <= 0)
                {
                    rm.state = ResultStatu.失败;
                    rm.error = "账单删除失败！";
                    Response.Write(rm.ToJsonString());
                    return;
                }
            }
        }
        rm.state = ResultStatu.成功;
        Response.Write(rm.ToJsonString());
        Response.End();
    }

    public class ItemOrderFormLog
    {
        public string ExpressNumber { get; set; }//订单的编号
    }
    public class ItemOrderFormLogList
    {
        public string CompanyAbbreviation { get; set; }//公司简称
        public string UserName { get; set; }//操作人姓名
        public string Type_Show { get; set; }//操作状态
        public string InsertTime { get; set; }//操作时间
    }
    /// <summary>
    /// 订单操作日志
    /// </summary>
    public void OrderFormLogList()
    {
        ResultModel rm = new ResultModel();
        string ExpressNumber = "";//订单的编号
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemOrderFormLog> list = (List<ItemOrderFormLog>)JsonHelper.DeSerialze<List<ItemOrderFormLog>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemOrderFormLog dr in list)
                {
                    ExpressNumber = dr.ExpressNumber;
                }
            }
        }
        OrderFormLog ordforlog = new OrderFormLog();
        DataTable dt = ordforlog.GetOrderFormLogList(ExpressNumber);
        if (null == dt)
        {
            rm.error = "该订单暂无操作记录！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemOrderFormLogList> listnew = new List<ItemOrderFormLogList>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemOrderFormLogList orderlog = new ItemOrderFormLogList();
                orderlog.CompanyAbbreviation = dr["CompanyAbbreviation"].ToString();
                orderlog.UserName = dr["UserName"].ToString();
                orderlog.Type_Show = dr["Type_Show"].ToString();
                orderlog.InsertTime = dr["InsertTime"].ToString();
                listnew.Add(orderlog);
            }
            var arrcount = listnew.ToList();
            string datanew = JsonHelper.Serializer<ItemOrderFormLogList>(arrcount);
            Response.Write(datanew);
            Response.End();
        }
    }

    public class ItemOrdeAmountLogList
    {
        public string ExpressNumber { get; set; }//订单的编号
        public string UserName { get; set; }//操作人姓名
        public string Type_Show { get; set; }//操作状态
        public string InsertTime { get; set; }//操作时间
    }
    /// <summary>
    /// 账单操作日志
    /// </summary>
    public void OrderAmountLogList()
    {
        ResultModel rm = new ResultModel();
        string ExpressNumber = "";//订单的编号
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemOrderFormLog> list = (List<ItemOrderFormLog>)JsonHelper.DeSerialze<List<ItemOrderFormLog>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemOrderFormLog dr in list)
                {
                    ExpressNumber = dr.ExpressNumber;
                }
            }
        }
        OrderAmountLog orderamountlog = new OrderAmountLog();
        DataTable dt = orderamountlog.GetOrderAmountLogList(ExpressNumber);
        if (null == dt)
        {
            rm.error = "无操作记录！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemOrdeAmountLogList> listnew = new List<ItemOrdeAmountLogList>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemOrdeAmountLogList orderlog = new ItemOrdeAmountLogList();
                orderlog.ExpressNumber = dr["ExpressNumber"].ToString();
                orderlog.UserName = dr["UserName"].ToString();
                orderlog.Type_Show = dr["Type_Show"].ToString();
                orderlog.InsertTime = dr["InsertTime"].ToString();
                listnew.Add(orderlog);
            }
            var arrcount = listnew.ToList();
            string datanew = JsonHelper.Serializer<ItemOrdeAmountLogList>(arrcount);
            Response.Write(datanew);
            Response.End();
        }
    }

    public class ItemAllCustomer
    {
        public int Id { get; set; }
        public string CompanyAbbreviation { get; set; }//客户公司简称
        public string PrimaryContact { get; set; }//主要联系人
        public string PrimaryTel { get; set; }//主要联系人电话
        public string SalesmanName { get; set; }//业务员的姓名
        public string BusinessAddress { get; set; }//公司经营地址
    }
    /// <summary>
    /// 订单中选择客户的信息
    /// </summary>
    private void AllCustomer()
    {
        ResultModel rm = new ResultModel();

        string sql = string.Format("select Customer.Id,CompanyAbbreviation,PrimaryContact,PrimaryTel,BusinessAddress,Users.UserName from Customer left outer join Users on Customer.SalesmanId = Users.Id where Customer.IsDelete=0 and Show=0 and Status!=2");
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "暂无客户！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemAllCustomer> list = new List<ItemAllCustomer>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemAllCustomer itemallcustomer = new ItemAllCustomer();
                itemallcustomer.Id = dr["Id"].ToString().ToInt();
                itemallcustomer.CompanyAbbreviation = dr["CompanyAbbreviation"].ToString();
                itemallcustomer.PrimaryContact = dr["PrimaryContact"].ToString();
                itemallcustomer.PrimaryTel = dr["PrimaryTel"].ToString();
                itemallcustomer.BusinessAddress = dr["BusinessAddress"].ToString();
                itemallcustomer.SalesmanName = dr["UserName"].ToString();
                list.Add(itemallcustomer);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemAllCustomer>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }
    public class ItemAllForwarder
    {
        public int Id { get; set; }
        public string ForwarderName { get; set; }//转包运商的名称
    }
    /// <summary>
    /// 订单中选择转包运商
    /// </summary>
    private void AllForwarder()
    {
        ResultModel rm = new ResultModel();

        string sql = string.Format("select Id,ForwarderName from Forwarder where Enabled=1 and IsDelete=0");
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "暂无合作公司！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemAllForwarder> list = new List<ItemAllForwarder>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemAllForwarder itemallforwarder = new ItemAllForwarder();
                itemallforwarder.Id = dr["Id"].ToString().ToInt();
                itemallforwarder.ForwarderName = dr["ForwarderName"].ToString();
                list.Add(itemallforwarder);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemAllForwarder>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }

    public class ItemPack
    {
        public int Id { get; set; }//包装的id
        public string PackName { get; set; }//包装类型的名称
    }
    /// <summary>
    /// 订单中选择包装
    /// </summary>
    private void GetPack()
    {
        ResultModel rm = new ResultModel();
        //查询包装类型
        string sql = string.Format("select Id,DICValue from DictionariesValue where KeyId=3 and Enable=1 and IsDelete=0");
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "暂无包装类型！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemPack> list = new List<ItemPack>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemPack itempack = new ItemPack();
                itempack.Id = dr["Id"].ToString().ToInt();
                itempack.PackName = dr["DICValue"].ToString();
                list.Add(itempack);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemPack>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }
    public class ItemTransport
    {
        public int Id { get; set; }//运输方式的id
        public string TransportName { get; set; }//运输方式的名称
    }
    /// <summary>
    /// 订单中选择运输方式
    /// </summary>
    private void GetTransport()
    {
        ResultModel rm = new ResultModel();
        //查询运输方式
        string sql = string.Format("select Id,DICValue from DictionariesValue where KeyId=4 and Enable=1 and IsDelete=0");
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "暂无运输方式！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemTransport> list = new List<ItemTransport>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemTransport itemtransport = new ItemTransport();
                itemtransport.Id = dr["Id"].ToString().ToInt();
                itemtransport.TransportName = dr["DICValue"].ToString();
                list.Add(itemtransport);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemTransport>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }
    public class ItemCargo
    {
        public int Id { get; set; }//货单状态的id
        public string CargoName { get; set; }//货单状态的名称
    }
    /// <summary>
    /// 订单中选择货单状态
    /// </summary>
    private void GetCargo()
    {
        ResultModel rm = new ResultModel();
        //查询货单状态
        string sql = string.Format("select Id,DICValue from DictionariesValue where KeyId=2 and Enable=1 and IsDelete=0");
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "暂无订单状态！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemCargo> list = new List<ItemCargo>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemCargo itemcargo = new ItemCargo();
                itemcargo.Id = dr["Id"].ToString().ToInt();
                itemcargo.CargoName = dr["DICValue"].ToString();
                list.Add(itemcargo);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemCargo>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }

    public class Item
    {
        public int tpageindex { get; set; }//当前页数
        public int tpagesize { get; set; }//每页显示数量
    }
    public class AuditOrderFormList
    {
        public int Id { get; set; }//订单id
        public string ExpressNumber { get; set; }//订单编号
        public string InsertTime { get; set; }//申请时间
        public string Version { get; set; }//版本号
        public string UserName { get; set; }//申请人姓名
        public string Status_Show { get; set; }//审批状态
        public string Auditor { get; set; }//审批人
        public string Editor { get; set; }//修改
        public string AuditorRemarks { get; set; }//审批备注
        public string AuditorTime { get; set; }//审批时间
        public int VersionId { get; set; }//修改订单的上一级订单id
        public int RecordCount { get; set; }//总条数
    }
    /// <summary>
    /// 获取审核订单列表
    /// </summary>
    private void AuditOrderForm()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<Item> list = (List<Item>)JsonHelper.DeSerialze<List<Item>>(data);
        int tpagesize = 10;//每页显示数量
        int tpageindex = 1;//当前页数
        int RecordCount = 0;
        foreach (Item dr in list)
        {
            tpagesize = dr.tpagesize;
            tpageindex = dr.tpageindex;
        }
        ResultModel rm = new ResultModel();
        DataTable dt = new OrderForm().GetAuditOrderFormList(tpageindex, tpagesize, ref RecordCount);
        if (null == dt)
        {
            rm.error = "暂无要审核订单！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<AuditOrderFormList> list1 = new List<AuditOrderFormList>();
            foreach (DataRow dr in dt.Rows)
            {
                AuditOrderFormList auditorderform = new AuditOrderFormList();
                auditorderform.Id = dr["Id"].ToString().ToInt();//订单id
                auditorderform.ExpressNumber = dr["ExpressNumber"].ToString();//订单编号
                auditorderform.InsertTime = dr["InsertTime"].ToString();//申请发起时间
                auditorderform.Version = dr["Version"].ToString();//历史版本数
                auditorderform.UserName = dr["UserName"].ToString();//申请人姓名
                auditorderform.Status_Show = dr["Status_Show"].ToString();//审核状态
                auditorderform.Auditor = dr["Auditor"].ToString();//审批人姓名
                auditorderform.Editor = dr["Editor"].ToString();//全部是修改
                auditorderform.AuditorRemarks = dr["AuditorRemarks"].ToString();//审批备注               
                auditorderform.AuditorTime = dr["AuditorTime"].ToString();//审批时间
                auditorderform.VersionId = dr["VersionId"].ToString().ToInt();//此条修改订单的上一级订单的id
                list1.Add(auditorderform);
            }
            //var arrcount = list1.ToList();
            Dictionary<string, List<AuditOrderFormList>> JsonDJ = new Dictionary<string, List<AuditOrderFormList>>();
            List<AuditOrderFormList> listnew = new List<AuditOrderFormList>();
            AuditOrderFormList iof = new AuditOrderFormList();
            iof.RecordCount = RecordCount;
            listnew.Add(iof);

            JsonDJ.Add("orderform", list1);
            JsonDJ.Add("recordcount", listnew);
            string JsonDT = JsonHelper.newTonSerialize(JsonDJ);
            Response.Write(JsonDT);
            Response.End();
        }

    }
    public class ItemAuditOrderFormDetail
    {
        public int Id { get; set; }//订单的id
        public int VersionId { get; set; }//上一版本的id
    }
    /// <summary>
    /// 获取每一个审核订单的详细信息
    /// </summary>
    private void AuditOrderFormDetail()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }

        ResultModel rm = new ResultModel();
        List<ItemAuditOrderFormDetail> list = (List<ItemAuditOrderFormDetail>)JsonHelper.DeSerialze<List<ItemAuditOrderFormDetail>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemAuditOrderFormDetail dr1 in list)
                {
                    //查询订单是否存在
                    string sql = string.Format("SELECT o.*,u.UserName,c.CompanyAbbreviation,f.ForwarderName,f.ForwarderNo,d1.ext_name as shipperCity,d2.ext_name as recipientCity,dict1.DICValue CargoName,dict2.DICValue PackName, oas.PayFreight, oas.PayOther, oas.PayCost from OrderForm o INNER JOIN Users u ON o.InsertUserId = u.Id INNER JOIN Customer c ON o.CustomerId = c.Id INNER JOIN Forwarder f ON o.ForwarderId = f.Id INNER JOIN districts d1 ON o.ShipperCityId = d1.id INNER JOIN districts d2 ON o.RecipientCityId = d2.id INNER JOIN DictionariesValue dict1 ON o.CargoType = dict1.Id INNER JOIN DictionariesValue dict2 ON o.PackType = dict2.Id LEFT JOIN(SELECT * FROM OrderAmount oa WHERE oa.Show = 0) oas ON o.ExpressNumber = oas.ExpressNumber WHERE o.IsDelete = 0 and o.id in ({0},{1})", dr1.VersionId, dr1.Id);
                    System.Data.DataTable dt = DMain.Get(sql);
                    if (null == dt || dt.Rows.Count <= 0)
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "审核订单不存在！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    else
                    {
                        List<ItemOrderForm> list1 = new List<ItemOrderForm>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            ItemOrderForm itemorderform = new ItemOrderForm();
                            itemorderform.Id = dr["Id"].ToString().ToInt();
                            itemorderform.CustomerId = dr["CustomerId"].ToString().ToInt();
                            itemorderform.CompanyAbbreviation = dr["CompanyAbbreviation"].ToString();
                            //itemorderform.InsertUserId = LoginController.User.Id;
                            itemorderform.ExpressNumber = dr["ExpressNumber"].ToString();
                            itemorderform.ForwarderId = dr["ForwarderId"].ToString().ToInt();
                            itemorderform.ForwarderName = dr["ForwarderName"].ToString();
                            itemorderform.ForwarderNo = dr["ForwarderNo"].ToString();
                            itemorderform.CooperationNo = dr["CooperationNo"].ToString();
                            itemorderform.ShipperName = dr["ShipperName"].ToString();
                            itemorderform.ShipperTel = dr["ShipperTel"].ToString();
                            itemorderform.ShipperDF = dr["ShipperDF"].ToString();
                            itemorderform.ShipperAddress = dr["ShipperAddress"].ToString();
                            itemorderform.RecipientCompany = dr["RecipientCompany"].ToString();
                            itemorderform.RecipientTel = dr["RecipientTel"].ToString();
                            itemorderform.RecipientName = dr["RecipientName"].ToString();
                            itemorderform.RecipientDF = dr["RecipientDF"].ToString();
                            //itemorderform.RecipientCity = dr["RecipientCity"].ToString();
                            itemorderform.RecipientAddress = dr["RecipientAddress"].ToString();
                            itemorderform.Packages = dr["Packages"].ToString();
                            itemorderform.Weight = dr["Weight"].ToString();
                            itemorderform.Volume = dr["Volume"].ToString();
                            itemorderform.PackType = dr["PackType"].ToString();
                            itemorderform.PackName = dr["PackName"].ToString();
                            itemorderform.Measurementrules = dr["Measurementrules"].ToString();
                            itemorderform.VolumeWeight = dr["VolumeWeight"].ToString();
                            itemorderform.ChargeableWeight = dr["ChargeableWeight"].ToString();
                            itemorderform.TransportType = dr["TransportType"].ToString();
                            itemorderform.Value = dr["Value"].ToString();
                            itemorderform.InsureType = dr["InsureType"].ToString();
                            itemorderform.ReceiptType = dr["ReceiptType"].ToString();
                            itemorderform.Payer = dr["Payer"].ToString();
                            itemorderform.Remark = dr["Remark"].ToString();
                            //itemorderform.Url = dr["Url"].ToString();
                            itemorderform.CreationTime = Convert.ToDateTime(dr["CreationTime"]);
                            itemorderform.BalanceType = dr["BalanceType"].ToString();
                            itemorderform.CargoName = dr["CargoName"].ToString();//货单物流状态名称
                            itemorderform.CargoType = dr["CargoType"].ToString();//货单物流状态的id
                            itemorderform.PayFreight = dr["PayFreight"].ToString();//应付运费 
                            itemorderform.PayOther = dr["PayOther"].ToString();//应付其他费用  
                            itemorderform.PayCost = dr["PayCost"].ToString();//应付总费用    
                            itemorderform.ShipperProvinceId = dr["ShipperProvinceId"].ToString().ToInt();//发件人省份id
                            itemorderform.ShipperCityId = dr["ShipperCityId"].ToString().ToInt();//发件人城市id
                            itemorderform.RecipientProvinceId = dr["RecipientProvinceId"].ToString().ToInt();//收件人省份id
                            itemorderform.RecipientCityId = dr["RecipientCityId"].ToString().ToInt();//收件人城市id
                            itemorderform.ShipperCityName = dr["ShipperCity"].ToString();//发件人城市名称
                            itemorderform.RecipientCityName = dr["RecipientCity"].ToString();//收件人城市名称
                            itemorderform.SalesmanName = dr["UserName"].ToString();//业务员的姓名
                            itemorderform.Status = dr["Status"].ToString().ToInt();//审核状态 0待审核 1通过 2不通过 3不需要审核
                            list1.Add(itemorderform);
                        }
                        var arrcount = list1.ToList();
                        string datat = JsonHelper.newTonSerialize(arrcount);
                        Response.Write(datat);
                        Response.End();
                    }
                }
            }
        }
    }

    public class ItemEditor
    {
        public int Id { get; set; }//订单的id
        public int VersionId { get; set; }//上一版本的id
        public int Status { get; set; }//1通过 2不通过
        public string AuditorRemarks { get; set; }//审核备注
    }
    /// <summary>
    /// 审核订单是否通过
    /// </summary>
    private void EditorAuditOrderForm()
    {
        ResultModel rm = new ResultModel();
        string data = "";
        int id = 0;
        int status = 5;
        bool flag = false;
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }

        List<ItemEditor> list = (List<ItemEditor>)JsonHelper.DeSerialze<List<ItemEditor>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemEditor dr1 in list)
                {
                    status = dr1.Status;
                    if (status == 1)
                    {
                        string sql = string.Format("update OrderForm set Status={1} ,UpdateTime='{2}',UpdateUserId={3},AuditorRemarks='{4}' where Id={0}", dr1.Id, dr1.Status, DateTime.Now, LoginController.User.Id, dr1.AuditorRemarks);
                        flag = DMain.RunUpdate(sql);

                        if (flag)
                        {
                            string sql2 = string.Format("update OrderFormLog set Status={1} where OrderFormId={0}", id, status);
                            flag = DMain.RunUpdate(sql2);
                        }
                    }
                    else if (status == 2)
                    {
                        string sql = string.Format("update OrderForm set Status={1} ,UpdateTime='{2}',UpdateUserId={3},AuditorRemarks='{4}',Show=1 where Id={0}", dr1.Id, dr1.Status, DateTime.Now, LoginController.User.Id, dr1.AuditorRemarks);
                        flag = DMain.RunUpdate(sql);
                        if (flag)
                        {
                            string sql2 = string.Format("update OrderForm set Show=0 where id={0}", dr1.VersionId);
                            flag = DMain.RunUpdate(sql2);
                            if (flag)
                            {
                                string sql3 = string.Format("update OrderFormLog set Status={1} where OrderFormId={0}", id, status);
                                flag = DMain.RunUpdate(sql3);
                            }
                        }
                    }
                }
                if (flag == true)
                {
                    rm.state = ResultStatu.成功;
                }
                else
                {
                    rm.state = ResultStatu.失败;
                    rm.error = "审核失败！";
                }
                Response.Write(rm.ToJsonString());
                Response.End();
            }
        }
    }

    public class ItemOrderAmount
    {
        public string expressnumber { get; set; }//订单编号
    }
    public class ItemOrderAmountDetail
    {
        public int Id { get; set; }//账单的id
        public string PayFreight { get; set; }//应付运费
        public string PayOther { get; set; }//应付其他费用
        public string PayCost { get; set; }//应付总费用
        public string PayRemark { get; set; }//应付备注
        public string ReceiveFreight { get; set; }//应收运费
        public string ReceiveOther { get; set; }//应收其他费用
        public string ReceiveCharges { get; set; }//应收总费用
        public string PaymentType { get; set; }//支付方式
        public string PaymentTypeName { get; set; }//支付方式名称
        public string ReceiveRemark { get; set; }//应收备注
        public string CreationTime { get; set; }//账单创建时间
        public int Status { get; set; }//审核状态 0待审核 1通过 2不通过 3不需要审核
    }
    /// <summary>
    /// 获取单个订单账单
    /// </summary>
    private void GetOrderAmount()
    {
        ResultModel rm = new ResultModel();
        string ExpressNumber = null;//订单编号
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemOrderAmount> list = (List<ItemOrderAmount>)JsonHelper.DeSerialze<List<ItemOrderAmount>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemOrderAmount dr in list)
                {
                    ExpressNumber = dr.expressnumber;
                }
            }
        }
        OrderAmount order = new OrderAmount();
        DataTable dt = order.GetAmountDetail(ExpressNumber);
        if (null == dt)
        {
            rm.state = ResultStatu.失败;
            rm.error = "未查询到该账单的信息！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemOrderAmountDetail> list1 = new List<ItemOrderAmountDetail>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemOrderAmountDetail orderamountdetail = new ItemOrderAmountDetail();
                orderamountdetail.Id = dr["Id"].ToString().ToInt();
                orderamountdetail.PayFreight = dr["PayFreight"].ToString();
                orderamountdetail.PayOther = dr["PayOther"].ToString();
                orderamountdetail.PayCost = dr["PayCost"].ToString();
                orderamountdetail.PayRemark = dr["PayRemark"].ToString();
                orderamountdetail.ReceiveFreight = dr["ReceiveFreight"].ToString();
                orderamountdetail.ReceiveOther = dr["ReceiveOther"].ToString();
                orderamountdetail.ReceiveCharges = dr["ReceiveCharges"].ToString();
                orderamountdetail.PaymentType = dr["PaymentType"].ToString();
                orderamountdetail.PaymentTypeName = dr["DICValue"].ToString();
                orderamountdetail.ReceiveRemark = dr["ReceiveRemark"].ToString();
                orderamountdetail.CreationTime = dr["CreationTime"].ToString();
                orderamountdetail.Status = dr["Status"].ToString().ToInt();
                list1.Add(orderamountdetail);
            }
            var arrcount = list1.ToList();
            string datanew = JsonHelper.Serializer<ItemOrderAmountDetail>(arrcount);
            Response.Write(datanew);
            Response.End();
        }
    }

    public class ItemAddOrderAmount
    {
        public string ExpressNumber { get; set; }//订单编号
        public string PayFreight { get; set; }//应付运费
        public string PayOther { get; set; }//应付其他费用
        public string PayCost { get; set; }//应付总费用
        public string PayRemark { get; set; }//应付备注
        public string ReceiveFreight { get; set; }//应收运费
        public string ReceiveOther { get; set; }//应收其他费用
        public string ReceiveCharges { get; set; }//应收总费用
        public string PaymentType { get; set; }//支付方式
        public string ReceiveRemark { get; set; }//应收备注
    }
    /// <summary>
    /// 新增订单账单
    /// </summary>
    private void AddOrderAmount()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }

        ResultModel rm = new ResultModel();
        List<ItemAddOrderAmount> list = (List<ItemAddOrderAmount>)JsonHelper.DeSerialze<List<ItemAddOrderAmount>>(data);
        int id = 0;
        OrderAmountLog orderamountlog = new OrderAmountLog();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemAddOrderAmount dr in list)
                {
                    //查询订单账单是否存在
                    string sql = string.Format("select count(1) from OrderAmount where ExpressNumber='{0}' and IsDelete=0 and Show=0 and Status in(0,1,3)", dr.ExpressNumber);
                    System.Data.DataTable dt = DMain.Get(sql);
                    int count = dt.Rows[0][0].ToString().ToInt();
                    if (count >= 1)
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "订单账单已经存在了！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    OrderAmount orderamount = new OrderAmount()
                    {
                        ExpressNumber = dr.ExpressNumber,//订单编号
                        PayFreight = dr.PayFreight,//应付运费
                        PayOther = dr.PayOther,//应付其他费用
                        PayCost = dr.PayCost,//应付总费用
                        PayRemark = dr.PayRemark,//应付备注
                        ReceiveFreight = dr.ReceiveFreight,//应收运费
                        ReceiveOther = dr.ReceiveOther,//应收其他费用
                        ReceiveCharges = dr.ReceiveCharges,//应收总费用
                        PaymentType = dr.PaymentType,//支付方式
                        ReceiveRemark = dr.ReceiveRemark,//应收备注
                        InsertUserId = LoginController.User.Id,
                        InsertTime = DateTime.Now,
                        UpdateTime = Const.DateTimeDefaultValue,//1900
                        UpdateUserId = 0,
                        Status = 3,//0待审核 1通过 2不通过 3不需要审核
                        IsDelete = 0,
                        Version = 1,
                        Show = 0,
                        CreationTime = DateTime.Now,
                        VersionId = 0,
                    };
                    id = orderamount.Save();

                    if (id > 0)
                    {
                        orderamountlog.ExpressNumber = dr.ExpressNumber;
                        orderamountlog.OrderAmountId = id;
                        orderamountlog.Status = 3;
                        orderamountlog.Type = 0;
                        orderamountlog.InsertTime = DateTime.Now;
                        orderamountlog.InsertUserId = LoginController.User.Id;
                        orderamountlog.AuditTime = Const.DateTimeDefaultValue;//1900
                        orderamountlog.AuditUserId = 0;
                        orderamountlog.OperationStatus = "成功";

                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        orderamountlog.ExpressNumber = dr.ExpressNumber;
                        orderamountlog.OrderAmountId = id;
                        orderamountlog.Status = 3;
                        orderamountlog.Type = 0;
                        orderamountlog.InsertTime = DateTime.Now;
                        orderamountlog.InsertUserId = LoginController.User.Id;
                        orderamountlog.AuditTime = Const.DateTimeDefaultValue;//1900
                        orderamountlog.AuditUserId = 0;
                        orderamountlog.OperationStatus = "失败";

                        rm.state = ResultStatu.失败;
                        rm.error = "订单账单创建失败！";
                    }
                }
            }
        }
        orderamountlog.Save();
        Response.Write(rm.ToJsonString());
        Response.End();
    }

    public class ItemEditorOrderAmount
    {
        public int Id { get; set; }//订单账单id
        public string ExpressNumber { get; set; }//订单编号
        public string PayFreight { get; set; }//应付运费
        public string PayOther { get; set; }//应付其他费用
        public string PayCost { get; set; }//应付总费用
        public string PayRemark { get; set; }//应付备注
        public string ReceiveFreight { get; set; }//应收运费
        public string ReceiveOther { get; set; }//应收其他费用
        public string ReceiveCharges { get; set; }//应收总费用
        public string PaymentType { get; set; }//支付方式
        public string ReceiveRemark { get; set; }//应收备注
        public string CreationTime { get; set; }//订单账单创建时间
    }
    /// <summary>
    /// 编辑订单账单
    /// </summary>
    private void EditorOrderAmount()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        ResultModel rm = new ResultModel();
        List<ItemEditorOrderAmount> list = (List<ItemEditorOrderAmount>)JsonHelper.DeSerialze<List<ItemEditorOrderAmount>>(data);

        //查询当前操作人的权限，是直接修改权限，还是需要什么的权限
        int AuditType = 0;//编辑权限，10编辑不用审核，11编辑需要审核
        string sql = string.Format("select AuditType from UserMenu where UserId={0} and IsDelete=0 and MenuId=10", LoginController.User.Id);
        System.Data.DataTable dt1 = DMain.Get(sql);
        if (null == dt1 || dt1.Rows.Count <= 0)
        {
            rm.error = "您没有编辑订单的权限！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            AuditType = dt1.Rows[0]["AuditType"].ToString().ToInt();
        }
        OrderAmount orda = new OrderAmount();
        int id = 0;
        string ExpressNumber_s = "";
        bool flag = false;
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemEditorOrderAmount dr in list)
                {
                    string sql1 = string.Format("select count(1) from OrderAmount where ExpressNumber='{0}'", dr.ExpressNumber);
                    System.Data.DataTable dtnew = DMain.Get(sql1);
                    int count = dtnew.Rows[0][0].ToString().ToInt();

                    ExpressNumber_s = dr.ExpressNumber;
                    DataTable dt = orda.GetOrderAmount(dr.Id, dr.ExpressNumber.ToString());
                    if (null == dt || 0 == dt.Rows.Count)
                    {
                        rm.error = "账单不存在!";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    else
                    {
                        if (dt != null && AuditType == 1)//此处应该是，如果操作人修改需要审核
                        {
                            orda.ExpressNumber = dr.ExpressNumber;//订单编号
                            orda.PayFreight = dr.PayFreight;//应付运费
                            orda.PayOther = dr.PayOther;//应付其他费用
                            orda.PayCost = dr.PayCost;//应付总费用
                            orda.PayRemark = dr.PayRemark;//应付备注
                            orda.ReceiveFreight = dr.ReceiveFreight;//应收运费
                            orda.ReceiveOther = dr.ReceiveOther;//应收其他费用
                            orda.ReceiveCharges = dr.ReceiveCharges;//应收总费用
                            orda.PaymentType = dr.PaymentType;//支付方式
                            orda.ReceiveRemark = dr.ReceiveRemark;//应收备注
                            orda.InsertUserId = LoginController.User.Id;
                            orda.InsertTime = DateTime.Now;
                            orda.UpdateTime = Const.DateTimeDefaultValue;//1900
                            orda.UpdateUserId = 0;
                            orda.Status = 0;//0待审核 1通过 2不通过 3不需要审核
                            orda.IsDelete = 0;
                            orda.Version = count + 1;//版本加一
                            orda.Show = 0;//0展示
                            orda.CreationTime = Convert.ToDateTime(dr.CreationTime);//账单创建时间不变
                            orda.VersionId = dr.Id;

                            id = orda.Save();
                            //将前一版本状态修改为不展示
                            if (id > 0)
                            {
                                string sql2 = string.Format("update OrderAmount set Show=1 where id={0}", dr.Id);
                                flag = DMain.RunUpdate(sql2);
                            }
                            OrderAmountLog ordalog = new OrderAmountLog();
                            if (id > 0 && flag == true)
                            {
                                ordalog.OrderAmountId = id;
                                ordalog.ExpressNumber = ExpressNumber_s;
                                ordalog.Status = 0;//0待审核
                                ordalog.Type = 1;//修改
                                ordalog.InsertTime = DateTime.Now;
                                ordalog.InsertUserId = LoginController.User.Id;
                                ordalog.AuditTime = Const.DateTimeDefaultValue;//审核时间
                                ordalog.AuditUserId = 0;
                                ordalog.OperationStatus = "成功";
                                rm.state = ResultStatu.成功;
                            }
                            else
                            {
                                ordalog.OrderAmountId = id;
                                ordalog.ExpressNumber = ExpressNumber_s;
                                ordalog.Status = 0;//0待审核
                                ordalog.Type = 1;//修改
                                ordalog.InsertTime = DateTime.Now;
                                ordalog.InsertUserId = LoginController.User.Id;
                                ordalog.AuditTime = Const.DateTimeDefaultValue;//审核时间
                                ordalog.AuditUserId = 0;
                                ordalog.OperationStatus = "失败";
                                rm.state = ResultStatu.失败;
                                rm.error = "订单修改失败！";
                            }
                            ordalog.Save();
                            Response.Write(rm.ToJsonString());
                            Response.End();
                        }
                        else if (dt != null && AuditType == 0)//此处应该是，如果操作人修改不需要审核
                        {
                            orda.ExpressNumber = dr.ExpressNumber;//订单编号
                            orda.PayFreight = dr.PayFreight;//应付运费
                            orda.PayOther = dr.PayOther;//应付其他费用
                            orda.PayCost = dr.PayCost;//应付总费用
                            orda.PayRemark = dr.PayRemark;//应付备注
                            orda.ReceiveFreight = dr.ReceiveFreight;//应收运费
                            orda.ReceiveOther = dr.ReceiveOther;//应收其他费用
                            orda.ReceiveCharges = dr.ReceiveCharges;//应收总费用
                            orda.PaymentType = dr.PaymentType;//支付方式
                            orda.ReceiveRemark = dr.ReceiveRemark;//应收备注
                            orda.InsertUserId = LoginController.User.Id;
                            orda.InsertTime = DateTime.Now;
                            orda.UpdateTime = Const.DateTimeDefaultValue;//1900
                            orda.UpdateUserId = 0;
                            orda.Status = 3;//0待审核 1通过 2不通过 3不需要审核
                            orda.IsDelete = 0;
                            orda.Version = count + 1;//版本加一
                            orda.Show = 0;//0展示
                            orda.CreationTime = Convert.ToDateTime(dr.CreationTime);//账单创建时间不变
                            orda.VersionId = dr.Id;

                            id = orda.Save();
                            //将前一版本状态修改为不展示
                            if (id > 0)
                            {
                                string sql2 = string.Format("update OrderAmount set Show=1 where id={0}", dr.Id);
                                flag = DMain.RunUpdate(sql2);
                            }
                            else
                            {
                                rm.state = ResultStatu.失败;
                                rm.error = "订单修改失败！";
                            }
                            OrderAmountLog ordalog = new OrderAmountLog();
                            if (id > 0 && flag == true)
                            {
                                ordalog.OrderAmountId = id;
                                ordalog.ExpressNumber = ExpressNumber_s;
                                ordalog.Status = 3;//3不需要审核
                                ordalog.Type = 1;//修改
                                ordalog.InsertTime = DateTime.Now;
                                ordalog.InsertUserId = LoginController.User.Id;
                                ordalog.AuditTime = Const.DateTimeDefaultValue;//审核时间
                                ordalog.AuditUserId = 0;
                                ordalog.OperationStatus = "成功";
                                rm.state = ResultStatu.成功;
                            }
                            else
                            {
                                ordalog.OrderAmountId = id;
                                ordalog.ExpressNumber = ExpressNumber_s;
                                ordalog.Status = 3;//3不需要审核
                                ordalog.Type = 1;//修改
                                ordalog.InsertTime = DateTime.Now;
                                ordalog.InsertUserId = LoginController.User.Id;
                                ordalog.AuditTime = Const.DateTimeDefaultValue;//审核时间
                                ordalog.AuditUserId = 0;
                                ordalog.OperationStatus = "失败";
                                rm.state = ResultStatu.失败;
                                rm.error = "订单修改失败！";
                            }
                            ordalog.Save();
                            Response.Write(rm.ToJsonString());
                            Response.End();
                        }
                    }
                }
            }
        }
    }

    public class ItemDeleteOrderAmount
    {
        public int id { get; set; }//id值
        public string ExpressNumber { get; set; }//订单编号
    }
    /// <summary>
    /// 删除订单账单
    /// </summary>
    private void DeleteOrderAmount()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemDeleteOrderAmount> list = (List<ItemDeleteOrderAmount>)JsonHelper.DeSerialze<List<ItemDeleteOrderAmount>>(data);
        ResultModel rm = new ResultModel()
        {
            state = ResultStatu.失败
        };
        OrderAmount orda = new OrderAmount();
        OrderAmountLog ordalog = new OrderAmountLog();
        foreach (ItemDeleteOrderAmount dr in list)
        {
            if (!orda.Load(dr.id))
            {
                rm.error = "账单不存在";
                Response.Write(rm.ToJsonString());
                return;
            }
            string result = orda.UpdateDeleteEN(dr.ExpressNumber);
            if (result != "0")
            {

                ordalog.OrderAmountId = dr.id;
                ordalog.ExpressNumber = dr.ExpressNumber;
                ordalog.Status = 4;//4删除无状态
                ordalog.Type = 2;//修改
                ordalog.InsertTime = DateTime.Now;
                ordalog.InsertUserId = LoginController.User.Id;
                ordalog.AuditTime = Const.DateTimeDefaultValue;//审核时间
                ordalog.AuditUserId = 0;
                ordalog.OperationStatus = "成功";
                ordalog.Remark = result;

                rm.state = ResultStatu.成功;
            }
            else
            {
                ordalog.OrderAmountId = dr.id;
                ordalog.ExpressNumber = dr.ExpressNumber;
                ordalog.Status = 4;//4删除无状态
                ordalog.Type = 2;//修改
                ordalog.InsertTime = DateTime.Now;
                ordalog.InsertUserId = LoginController.User.Id;
                ordalog.AuditTime = Const.DateTimeDefaultValue;//审核时间
                ordalog.AuditUserId = 0;
                ordalog.OperationStatus = "失败";
                ordalog.Remark = "批量删除失败";

                rm.state = ResultStatu.失败;
                rm.error = "订单删除失败！";

                Response.Write(rm.ToJsonString());
                return;
            }
        }
        ordalog.Save();
        rm.state = ResultStatu.成功;
        Response.Write(rm.ToJsonString());
        Response.End();
    }

    public class AuditOrderAmountList
    {
        public int Id { get; set; }//订单id
        public string ExpressNumber { get; set; }//订单编号
        public string InsertTime { get; set; }//申请时间
        public string Version { get; set; }//版本号
        public string InsertUserName { get; set; }//申请人姓名
        public string Status_Show { get; set; }//审批状态
        public string Auditor { get; set; }//审批人
        public string Editor { get; set; }//修改
        public string AuditorRemarks { get; set; }//审批备注
        public string AuditorTime { get; set; }//审批时间
        public int VersionId { get; set; }//修改订单的上一级订单id
        public int RecordCount { get; set; }//总条数
    }
    /// <summary>
    /// 获取审核账单列表
    /// </summary>
    private void AuditOrderAmount()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<Item> list = (List<Item>)JsonHelper.DeSerialze<List<Item>>(data);
        int tpagesize = 10;//每页显示数量
        int tpageindex = 1;//当前页数
        int RecordCount = 0;
        foreach (Item dr in list)
        {
            tpagesize = dr.tpagesize;
            tpageindex = dr.tpageindex;
        }
        ResultModel rm = new ResultModel();
        DataTable dt = new OrderAmount().GetAuditOrderAmountList(tpageindex, tpagesize, ref RecordCount);
        if (null == dt)
        {
            rm.error = "暂无要审核账单！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<AuditOrderFormList> list1 = new List<AuditOrderFormList>();
            foreach (DataRow dr in dt.Rows)
            {
                AuditOrderFormList auditorderform = new AuditOrderFormList();
                auditorderform.Id = dr["Id"].ToString().ToInt();//订单id
                auditorderform.ExpressNumber = dr["ExpressNumber"].ToString();//订单编号
                auditorderform.InsertTime = dr["InsertTime"].ToString();//申请发起时间
                auditorderform.Version = dr["Version"].ToString();//历史版本数
                auditorderform.UserName = dr["InsertUserName"].ToString();//申请人姓名
                auditorderform.Status_Show = dr["Status_Show"].ToString();//审核状态
                auditorderform.Auditor = dr["UpdateUserName"].ToString();//审批人姓名
                auditorderform.Editor = dr["Editor"].ToString();//全部是修改
                auditorderform.AuditorRemarks = dr["AuditorRemarks"].ToString();//审批备注               
                auditorderform.AuditorTime = dr["AuditorTime"].ToString();//审批时间
                auditorderform.VersionId = dr["VersionId"].ToString().ToInt();//此条修改订单的上一级订单的id
                list1.Add(auditorderform);
            }
            //var arrcount = list1.ToList();
            Dictionary<string, List<AuditOrderFormList>> JsonDJ = new Dictionary<string, List<AuditOrderFormList>>();
            List<AuditOrderFormList> listnew = new List<AuditOrderFormList>();
            AuditOrderFormList iof = new AuditOrderFormList();
            iof.RecordCount = RecordCount;
            listnew.Add(iof);

            JsonDJ.Add("orderform", list1);
            JsonDJ.Add("recordcount", listnew);
            string JsonDT = JsonHelper.newTonSerialize(JsonDJ);
            Response.Write(JsonDT);
            Response.End();
        }

    }

    public class ItemAuditOrderAmountDetail
    {
        public int Id { get; set; }//账单的id
        public int VersionId { get; set; }//上一版本的id
    }
    /// <summary>
    /// 获取每一个审核账单的详细信息
    /// </summary>
    private void AuditOrderAmountDetail()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }

        ResultModel rm = new ResultModel();
        List<ItemAuditOrderAmountDetail> list = (List<ItemAuditOrderAmountDetail>)JsonHelper.DeSerialze<List<ItemAuditOrderAmountDetail>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemAuditOrderAmountDetail dr1 in list)
                {
                    //查询订单是否存在
                    string sql = string.Format("select OrderAmount.*,DictionariesValue.DICValue PaymentName from OrderAmount left outer join DictionariesValue on OrderAmount.PaymentType = DictionariesValue.Id where OrderAmount.IsDelete = 0 and OrderAmount.id in ({0}, {1})", dr1.VersionId, dr1.Id);
                    System.Data.DataTable dt = DMain.Get(sql);
                    if (null == dt || dt.Rows.Count <= 0)
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "审核账单不存在！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    else
                    {
                        List<ItemOrderAmountDetail> list1 = new List<ItemOrderAmountDetail>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            ItemOrderAmountDetail orderamountdetail = new ItemOrderAmountDetail();
                            orderamountdetail.Id = dr["Id"].ToString().ToInt();
                            orderamountdetail.PayFreight = dr["PayFreight"].ToString();
                            orderamountdetail.PayOther = dr["PayOther"].ToString();
                            orderamountdetail.PayCost = dr["PayCost"].ToString();
                            orderamountdetail.PayRemark = dr["PayRemark"].ToString();
                            orderamountdetail.ReceiveFreight = dr["ReceiveFreight"].ToString();
                            orderamountdetail.ReceiveOther = dr["ReceiveOther"].ToString();
                            orderamountdetail.ReceiveCharges = dr["ReceiveCharges"].ToString();
                            orderamountdetail.PaymentType = dr["PaymentType"].ToString();
                            orderamountdetail.PaymentTypeName = dr["PaymentName"].ToString();
                            orderamountdetail.ReceiveRemark = dr["ReceiveRemark"].ToString();
                            orderamountdetail.CreationTime = dr["CreationTime"].ToString();
                            list1.Add(orderamountdetail);
                        }
                        var arrcount = list1.ToList();
                        string datat = JsonHelper.newTonSerialize(arrcount);
                        Response.Write(datat);
                        Response.End();
                    }
                }
            }
        }
    }

    public class ItemAmount
    {
        public int Id { get; set; }//订单的id
        public int VersionId { get; set; }//上一版本的id
        public int Status { get; set; }//1通过 2不通过
        public string AuditorRemarks { get; set; }//审核备注
    }
    /// <summary>
    /// 账单审核是否同意
    /// </summary>
    private void EditorAuditOrderAmount()
    {
        ResultModel rm = new ResultModel();
        string data = "";
        int id = 0;
        int status = 5;
        bool flag = false;
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemAmount> list = (List<ItemAmount>)JsonHelper.DeSerialze<List<ItemAmount>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemAmount dr1 in list)
                {
                    status = dr1.Status;
                    if (status == 1)
                    {
                        string sql = string.Format("update OrderAmount set Status={1} ,UpdateTime='{2}',UpdateUserId={3},AuditorRemarks='{4}' where Id={0}", dr1.Id, dr1.Status, DateTime.Now, LoginController.User.Id, dr1.AuditorRemarks);
                        flag = DMain.RunUpdate(sql);

                        if (flag)
                        {
                            string sql2 = string.Format("update OrderAmountLog set Status={1} where OrderAmountId={0}", id, status);
                            flag = DMain.RunUpdate(sql2);
                        }
                    }
                    else if (status == 2)
                    {
                        string sql = string.Format("update OrderAmount set Status={1} ,UpdateTime='{2}',UpdateUserId={3},AuditorRemarks='{4}',Show=1 where Id={0}", dr1.Id, dr1.Status, DateTime.Now, LoginController.User.Id, dr1.AuditorRemarks);
                        flag = DMain.RunUpdate(sql);
                        if (flag)
                        {
                            string sql2 = string.Format("update OrderAmount set Show=0 where id={0}", dr1.VersionId);
                            flag = DMain.RunUpdate(sql2);
                            if (flag)
                            {
                                string sql3 = string.Format("update OrderAmountLog set Status={1} where OrderAmountId={0}", id, status);
                                flag = DMain.RunUpdate(sql3);
                            }
                        }
                    }
                }
                if (flag == true)
                {
                    rm.state = ResultStatu.成功;
                }
                else
                {
                    rm.state = ResultStatu.失败;
                    rm.error = "审核失败！";
                }
                Response.Write(rm.ToJsonString());
                Response.End();
            }
        }
    }


    public class ItemSend
    {
        public string com { get; set; }//查询的快递公司的编码，一律用小写字母
        public string str1 { get; set; }//查询的快递单号， 单号的最大长度是32个字符
    }
    /// <summary>
    /// 快递100请求接口
    /// </summary>
    private void Send()
    {
        ResultModel rm = new ResultModel();
        string com = "";
        string str1 = "";
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemSend> list = (List<ItemSend>)JsonHelper.DeSerialze<List<ItemSend>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemSend dr in list)
                {
                    com = dr.com;
                    str1 = dr.str1;
                }
                string task = Send100.Instance().Get(com, str1);
                Response.Write(task);
                Response.End();
            }
        }

    }

    public class ItemRuiQing
    {
        public string cooperationno { get; set; }//查询的快递单号， 单号的最大长度是32个字符
    }
    public class ItemLogistics
    {
        public string time { get; set; }//创建时间用户填写的时间
        public string context { get; set; }//物流信息
    }
    /// <summary>
    /// 瑞庆快递物流查询
    /// </summary>
    private void SendRuiQing()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemRuiQing> list = (List<ItemRuiQing>)JsonHelper.DeSerialze<List<ItemRuiQing>>(data);
        string cooperationno = null;
        foreach (ItemRuiQing dr in list)
        {
            cooperationno = dr.cooperationno;
        }
        ResultModel rm = new ResultModel();
        LogisticsInformation logist = new LogisticsInformation();
        DataTable dt = logist.GetLogistics(cooperationno);
        if (null == dt)
        {
            rm.state = ResultStatu.失败;
            rm.error = "暂无物流信息！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemLogistics> listnew = new List<ItemLogistics>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemLogistics itemlogistics = new ItemLogistics();
                itemlogistics.time = dr["CreatorTime"].ToString();
                itemlogistics.context = dr["logistics"].ToString();
                listnew.Add(itemlogistics);
            }
            var arrcount = listnew.ToList();
            string datanew = JsonHelper.Serializer<ItemLogistics>(arrcount);
            Response.Write(datanew);
            Response.End();
        }
    }

    public class ItemAddLogistics
    {
        public int Id { get; set; }//订单id
        public string ExpressNumber { get; set; }//订单编号
        public string CooperationNo { get; set; }//合作公司单号也是查询物流信息的单号
        public int CargoType { get; set; }//货单状态对应字典表  揽件等
        public string Context { get; set; }//物流信息详情
        public string CreatorTime { get; set; }//创建时间用户填写的时间
    }
    /// <summary>
    /// 新增瑞庆物流信息
    /// </summary>
    private void AddSendRuiQing()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }

        ResultModel rm = new ResultModel();
        List<ItemAddLogistics> list = (List<ItemAddLogistics>)JsonHelper.DeSerialze<List<ItemAddLogistics>>(data);
        LogisticsInformation logist = new LogisticsInformation();
        int id = 0;
        bool flag = false;
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemAddLogistics dr in list)
                {
                    if (string.IsNullOrWhiteSpace(dr.CreatorTime))
                    {
                        dr.CreatorTime = Const.DateTimeDefaultValue.ToString();
                    }
                    logist.ExpressNumber = dr.ExpressNumber;
                    logist.CooperationNo = dr.CooperationNo;
                    logist.CargoType = dr.CargoType.ToString().ToInt();
                    logist.Context = dr.Context;
                    logist.InsertTime = DateTime.Now;
                    logist.InsertUserId = LoginController.User.Id;
                    logist.CreatorTime = Convert.ToDateTime(dr.CreatorTime);

                    id = logist.Save();
                    if (id > 0)
                    {
                        flag = OrderForm.UpdateCargoType(dr.Id, dr.ExpressNumber, dr.CargoType);
                        if (flag)
                        {
                            rm.state = ResultStatu.成功;
                        }
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "物流信息创建失败！";
                    }
                }
            }
        }
        Response.Write(rm.ToJsonString());
        Response.End();
    }


    public class ItemAddress
    {
        public int id { get; set; }//省份或者城市的id
        public string ext_name { get; set; }//省份或者城市的名称
    }
    /// <summary>
    /// 查询省份
    /// </summary>
    private void SearchProvince()
    {
        ResultModel rm = new ResultModel();
        districts dis = new districts();
        DataTable dt = dis.GetAllProvince();
        if (null == dt)
        {
            rm.state = ResultStatu.失败;
            rm.error = "暂无省份！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemAddress> listnew = new List<ItemAddress>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemAddress itemaddress = new ItemAddress();
                itemaddress.id = dr["id"].ToString().ToInt();
                itemaddress.ext_name = dr["ext_name"].ToString();
                listnew.Add(itemaddress);
            }
            var arrcount = listnew.ToList();
            string datanew = JsonHelper.Serializer<ItemAddress>(arrcount);
            Response.Write(datanew);
            Response.End();
        }
    }
    public class ItemId
    {
        public int id { get; set; }//省份的id
    }
    /// <summary>
    /// 查询城市
    /// </summary>
    private void SearchCity()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemId> list = (List<ItemId>)JsonHelper.DeSerialze<List<ItemId>>(data);
        int pid = 0;
        foreach (ItemId dr in list)
        {
            pid = dr.id;
        }
        ResultModel rm = new ResultModel();
        districts dis = new districts();
        DataTable dt = dis.GetAllCity(pid);
        if (null == dt)
        {
            rm.state = ResultStatu.失败;
            rm.error = "暂无城市！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemAddress> listnew = new List<ItemAddress>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemAddress itemaddress = new ItemAddress();
                itemaddress.id = dr["id"].ToString().ToInt();
                itemaddress.ext_name = dr["ext_name"].ToString();
                listnew.Add(itemaddress);
            }
            var arrcount = listnew.ToList();
            string datanew = JsonHelper.Serializer<ItemAddress>(arrcount);
            Response.Write(datanew);
            Response.End();
        }
    }
    /// <summary>
    /// 订单未结算
    /// </summary>
    private void OrderFormBalanceNo()
    {
        ResultModel rm = new ResultModel();
        int tpagesize = 10;//每页显示数量
        int tpageindex = 1;//当前页数
        int RecordCount = 0;
        int cusid = 0;//客户id
        int BalanceType = -1;//结算状态 0未结算 1已结算
        string expnumber = null;//订单编号
        int CargoType = 0;//物流状态类型
        int forwid = 0;//合作公司
        string starttime = null;//开始时间
        string endtime = null;//结束时间
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemFormlist> list = (List<ItemFormlist>)JsonHelper.DeSerialze<List<ItemFormlist>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemFormlist dr in list)
                {
                    tpagesize = dr.pagesize;
                    tpageindex = dr.pageindex;
                    cusid = dr.cusid.ToInt();
                    expnumber = dr.expnumber;
                    CargoType = dr.CargoType.ToInt();
                    forwid = dr.forwid.ToInt();
                    starttime = dr.starttime;
                    endtime = dr.endtime;
                }
            }
        }
        DataTable dt = new OrderForm().GetOrderFormList(tpageindex, tpagesize, ref RecordCount, cusid, 0, expnumber, CargoType, forwid, starttime, endtime, 0);
        if (null == dt)
        {
            rm.error = "暂无订单！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemOrderForm> list1 = new List<ItemOrderForm>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemOrderForm itemorderform = new ItemOrderForm();
                itemorderform.Id = dr["Id"].ToString().ToInt();
                itemorderform.CustomerId = dr["CustomerId"].ToString().ToInt();
                itemorderform.CompanyAbbreviation = dr["CompanyAbbreviation"].ToString();
                //itemorderform.InsertUserId = LoginController.User.Id;
                itemorderform.ExpressNumber = dr["ExpressNumber"].ToString();
                itemorderform.ForwarderId = dr["ForwarderId"].ToString().ToInt();
                itemorderform.ForwarderName = dr["ForwarderName"].ToString();
                itemorderform.ForwarderNo = dr["ForwarderNo"].ToString();
                itemorderform.CooperationNo = dr["CooperationNo"].ToString();
                itemorderform.ShipperName = dr["ShipperName"].ToString();
                itemorderform.ShipperTel = dr["ShipperTel"].ToString();
                itemorderform.ShipperDF = dr["ShipperDF"].ToString();
                itemorderform.ShipperAddress = dr["ShipperAddress"].ToString();
                itemorderform.RecipientCompany = dr["RecipientCompany"].ToString();
                itemorderform.RecipientTel = dr["RecipientTel"].ToString();
                itemorderform.RecipientName = dr["RecipientName"].ToString();
                itemorderform.RecipientDF = dr["RecipientDF"].ToString();
                //itemorderform.RecipientCity = dr["RecipientCity"].ToString();
                itemorderform.RecipientAddress = dr["RecipientAddress"].ToString();
                itemorderform.Packages = dr["Packages"].ToString();
                itemorderform.Weight = dr["Weight"].ToString();
                itemorderform.Volume = dr["Volume"].ToString();
                itemorderform.PackType = dr["PackType"].ToString();
                itemorderform.PackName = dr["PackName"].ToString();
                itemorderform.Measurementrules = dr["Measurementrules"].ToString();
                itemorderform.VolumeWeight = dr["VolumeWeight"].ToString();
                itemorderform.ChargeableWeight = dr["ChargeableWeight"].ToString();
                itemorderform.TransportType = dr["TransportType"].ToString();
                itemorderform.TransportName = dr["TransportName"].ToString();
                itemorderform.Value = dr["Value"].ToString();
                itemorderform.InsureType = dr["InsureType"].ToString();
                itemorderform.ReceiptType = dr["ReceiptType"].ToString();
                itemorderform.Payer = dr["Payer"].ToString();
                itemorderform.Remark = dr["Remark"].ToString();
                //itemorderform.Url = dr["Url"].ToString();
                itemorderform.CreationTime = Convert.ToDateTime(dr["CreationTime"]);
                itemorderform.BalanceType = dr["BalanceType"].ToString();
                itemorderform.CargoName = dr["CargoName"].ToString();//货单物流状态名称
                itemorderform.CargoType = dr["CargoType"].ToString();//货单物流状态的id
                itemorderform.PayFreight = dr["PayFreight"].ToString();//应付运费 
                itemorderform.PayOther = dr["PayOther"].ToString();//应付其他费用  
                itemorderform.PayCost = dr["PayCost"].ToString();//应付总费用    
                itemorderform.ShipperProvinceId = dr["ShipperProvinceId"].ToString().ToInt();//发件人省份id
                itemorderform.ShipperCityId = dr["ShipperCityId"].ToString().ToInt();//发件人城市id
                itemorderform.RecipientProvinceId = dr["RecipientProvinceId"].ToString().ToInt();//收件人省份id
                itemorderform.RecipientCityId = dr["RecipientCityId"].ToString().ToInt();//收件人城市id
                itemorderform.ShipperCityName = dr["ShipperCity"].ToString();//发件人城市名称
                itemorderform.RecipientCityName = dr["RecipientCity"].ToString();//收件人城市名称
                itemorderform.SalesmanName = dr["UserName"].ToString();//业务员的姓名
                itemorderform.Status = dr["Status"].ToString().ToInt();//审核状态 0待审核 1通过 2不通过 3不需要审核
                list1.Add(itemorderform);
            }
            //var arrcount = list1.ToList();
            Dictionary<string, List<ItemOrderForm>> JsonDJ = new Dictionary<string, List<ItemOrderForm>>();
            List<ItemOrderForm> listnew = new List<ItemOrderForm>();
            ItemOrderForm iof = new ItemOrderForm();
            iof.RecordCount = RecordCount;
            listnew.Add(iof);

            JsonDJ.Add("orderform", list1);
            JsonDJ.Add("recordcount", listnew);
            string JsonDT = JsonHelper.newTonSerialize(JsonDJ);
            Response.Write(JsonDT);
            Response.End();
        }
    }

    public class ItemBalanceType
    {
        public int Id { get; set; }//id值
        public string ExpressNumber { get; set; }//订单编号
        public int BalanceType { get; set; }//结算状态 0未结算 1已结算
    }
    /// <summary>
    /// 订单结算状态修改
    /// </summary>
    private void OrderFormBalanceType()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemBalanceType> list = (List<ItemBalanceType>)JsonHelper.DeSerialze<List<ItemBalanceType>>(data);
        ResultModel rm = new ResultModel()
        {
            state = ResultStatu.失败
        };
        OrderForm orderform = new OrderForm();
        OrderFormLog orderformlog = new OrderFormLog();
        bool flag = false;
        foreach (ItemBalanceType dr in list)
        {
            if (!orderform.Load(dr.Id))
            {
                rm.state = ResultStatu.失败;
                rm.error = "订单不存在";
                Response.Write(rm.ToJsonString());
                return;
            }
            string sql = string.Format("update OrderForm set BalanceType={1} where Id={0}", dr.Id, dr.BalanceType);
            flag = DMain.RunUpdate(sql);
            if (flag)
            {
                orderformlog.OrderFormId = dr.Id;
                orderformlog.ExpressNumber = dr.ExpressNumber;
                orderformlog.Status = 4;//4删除、结算无状态
                orderformlog.Type = 3;//结算状态修改
                orderformlog.InsertTime = DateTime.Now;
                orderformlog.InsertUserId = LoginController.User.Id;
                orderformlog.AuditTime = Const.DateTimeDefaultValue;
                orderformlog.Remark = "结算状态修改成功";
                orderformlog.OperationStatus = "成功";
            }
            else
            {
                orderformlog.OrderFormId = dr.Id;
                orderformlog.ExpressNumber = dr.ExpressNumber;
                orderformlog.Status = 4;//4删除、结算无状态
                orderformlog.Type = 3;//结算状态修改
                orderformlog.InsertTime = DateTime.Now;
                orderformlog.InsertUserId = LoginController.User.Id;
                orderformlog.AuditTime = Const.DateTimeDefaultValue;
                orderformlog.OperationStatus = "失败";
                orderformlog.Remark = "结算状态修改失败";
                rm.state = ResultStatu.失败;
                rm.error = "结算状态修改失败！";

                Response.Write(rm.ToJsonString());
                return;
            }
        }
        orderformlog.Save();
        rm.state = ResultStatu.成功;
        Response.Write(rm.ToJsonString());
        Response.End();
    }
    /// <summary>
    /// 订单已结算
    /// </summary>
    private void OrderFormBalanceOn()
    {
        ResultModel rm = new ResultModel();
        int tpagesize = 10;//每页显示数量
        int tpageindex = 1;//当前页数
        int RecordCount = 0;
        int cusid = 0;//客户id
        int BalanceType = -1;//结算状态 0未结算 1已结算
        string expnumber = null;//订单编号
        int CargoType = 0;//物流状态类型
        int forwid = 0;//合作公司
        string starttime = null;//开始时间
        string endtime = null;//结束时间
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemFormlist> list = (List<ItemFormlist>)JsonHelper.DeSerialze<List<ItemFormlist>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemFormlist dr in list)
                {
                    tpagesize = dr.pagesize;
                    tpageindex = dr.pageindex;
                    cusid = dr.cusid.ToInt();
                    expnumber = dr.expnumber;
                    CargoType = dr.CargoType.ToInt();
                    forwid = dr.forwid.ToInt();
                    starttime = dr.starttime;
                    endtime = dr.endtime;
                }
            }
        }
        DataTable dt = new OrderForm().GetOrderFormList(tpageindex, tpagesize, ref RecordCount, cusid, 0, expnumber, CargoType, forwid, starttime, endtime, 1);
        if (null == dt)
        {
            rm.error = "暂无订单！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemOrderForm> list1 = new List<ItemOrderForm>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemOrderForm itemorderform = new ItemOrderForm();
                itemorderform.Id = dr["Id"].ToString().ToInt();
                itemorderform.CustomerId = dr["CustomerId"].ToString().ToInt();
                itemorderform.CompanyAbbreviation = dr["CompanyAbbreviation"].ToString();
                //itemorderform.InsertUserId = LoginController.User.Id;
                itemorderform.ExpressNumber = dr["ExpressNumber"].ToString();
                itemorderform.ForwarderId = dr["ForwarderId"].ToString().ToInt();
                itemorderform.ForwarderName = dr["ForwarderName"].ToString();
                itemorderform.ForwarderNo = dr["ForwarderNo"].ToString();
                itemorderform.CooperationNo = dr["CooperationNo"].ToString();
                itemorderform.ShipperName = dr["ShipperName"].ToString();
                itemorderform.ShipperTel = dr["ShipperTel"].ToString();
                itemorderform.ShipperDF = dr["ShipperDF"].ToString();
                itemorderform.ShipperAddress = dr["ShipperAddress"].ToString();
                itemorderform.RecipientCompany = dr["RecipientCompany"].ToString();
                itemorderform.RecipientTel = dr["RecipientTel"].ToString();
                itemorderform.RecipientName = dr["RecipientName"].ToString();
                itemorderform.RecipientDF = dr["RecipientDF"].ToString();
                //itemorderform.RecipientCity = dr["RecipientCity"].ToString();
                itemorderform.RecipientAddress = dr["RecipientAddress"].ToString();
                itemorderform.Packages = dr["Packages"].ToString();
                itemorderform.Weight = dr["Weight"].ToString();
                itemorderform.Volume = dr["Volume"].ToString();
                itemorderform.PackType = dr["PackType"].ToString();
                itemorderform.PackName = dr["PackName"].ToString();
                itemorderform.Measurementrules = dr["Measurementrules"].ToString();
                itemorderform.VolumeWeight = dr["VolumeWeight"].ToString();
                itemorderform.ChargeableWeight = dr["ChargeableWeight"].ToString();
                itemorderform.TransportType = dr["TransportType"].ToString();
                itemorderform.TransportName = dr["TransportName"].ToString();
                itemorderform.Value = dr["Value"].ToString();
                itemorderform.InsureType = dr["InsureType"].ToString();
                itemorderform.ReceiptType = dr["ReceiptType"].ToString();
                itemorderform.Payer = dr["Payer"].ToString();
                itemorderform.Remark = dr["Remark"].ToString();
                //itemorderform.Url = dr["Url"].ToString();
                itemorderform.CreationTime = Convert.ToDateTime(dr["CreationTime"]);
                itemorderform.BalanceType = dr["BalanceType"].ToString();
                itemorderform.CargoName = dr["CargoName"].ToString();//货单物流状态名称
                itemorderform.CargoType = dr["CargoType"].ToString();//货单物流状态的id
                itemorderform.PayFreight = dr["PayFreight"].ToString();//应付运费 
                itemorderform.PayOther = dr["PayOther"].ToString();//应付其他费用  
                itemorderform.PayCost = dr["PayCost"].ToString();//应付总费用    
                itemorderform.ShipperProvinceId = dr["ShipperProvinceId"].ToString().ToInt();//发件人省份id
                itemorderform.ShipperCityId = dr["ShipperCityId"].ToString().ToInt();//发件人城市id
                itemorderform.RecipientProvinceId = dr["RecipientProvinceId"].ToString().ToInt();//收件人省份id
                itemorderform.RecipientCityId = dr["RecipientCityId"].ToString().ToInt();//收件人城市id
                itemorderform.ShipperCityName = dr["ShipperCity"].ToString();//发件人城市名称
                itemorderform.RecipientCityName = dr["RecipientCity"].ToString();//收件人城市名称
                itemorderform.SalesmanName = dr["UserName"].ToString();//业务员的姓名
                itemorderform.Status = dr["Status"].ToString().ToInt();//审核状态 0待审核 1通过 2不通过 3不需要审核
                list1.Add(itemorderform);
            }
            //var arrcount = list1.ToList();
            Dictionary<string, List<ItemOrderForm>> JsonDJ = new Dictionary<string, List<ItemOrderForm>>();
            List<ItemOrderForm> listnew = new List<ItemOrderForm>();
            ItemOrderForm iof = new ItemOrderForm();
            iof.RecordCount = RecordCount;
            listnew.Add(iof);

            JsonDJ.Add("orderform", list1);
            JsonDJ.Add("recordcount", listnew);
            string JsonDT = JsonHelper.newTonSerialize(JsonDJ);
            Response.Write(JsonDT);
            Response.End();
        }
    }
    public class OrderFormBalanceLogList
    {
        public string ExpressNumber { get; set; }//订单编号
        public string UserName { get; set; }//操作人姓名
        public string Type_Show { get; set; }//操作状态
        public string InsertTime { get; set; }//操作时间
    }
    /// <summary>
    /// 订单结算状态操作日志
    /// </summary>
    public void OrderFormBalanceLog()
    {
        ResultModel rm = new ResultModel();
        string ExpressNumber = "";//订单的编号
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemOrderFormLog> list = (List<ItemOrderFormLog>)JsonHelper.DeSerialze<List<ItemOrderFormLog>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemOrderFormLog dr in list)
                {
                    ExpressNumber = dr.ExpressNumber;
                }
            }
        }
        OrderFormLog ordforlog = new OrderFormLog();
        DataTable dt = ordforlog.GetOrderFormBalanceLog(ExpressNumber);
        if (null == dt)
        {
            rm.error = "订单暂无结算记录！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<OrderFormBalanceLogList> listnew = new List<OrderFormBalanceLogList>();
            foreach (DataRow dr in dt.Rows)
            {
                OrderFormBalanceLogList orderlog = new OrderFormBalanceLogList();
                orderlog.ExpressNumber = dr["ExpressNumber"].ToString();
                orderlog.UserName = dr["UserName"].ToString();
                orderlog.Type_Show = dr["Type_Show"].ToString();
                orderlog.InsertTime = dr["InsertTime"].ToString();
                listnew.Add(orderlog);
            }
            var arrcount = listnew.ToList();
            string datanew = JsonHelper.Serializer<OrderFormBalanceLogList>(arrcount);
            Response.Write(datanew);
            Response.End();
        }
    }

    public class ItemPayment
    {
        public int Id { get; set; }//支付方式的id
        public string PaymentName { get; set; }//支付方式的名称
    }
    /// <summary>
    /// 账单中选择支付方式
    /// </summary>
    private void GetPaymentType()
    {
        ResultModel rm = new ResultModel();
        //查询支付方式，月结、现金、支票
        string sql = string.Format("select Id,DICValue from DictionariesValue where KeyId=5 and Enable=1 and IsDelete=0");
        System.Data.DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "暂无支付方式！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemPayment> list = new List<ItemPayment>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemPayment itempayment = new ItemPayment();
                itempayment.Id = dr["Id"].ToString().ToInt();
                itempayment.PaymentName = dr["DICValue"].ToString();
                list.Add(itempayment);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemPayment>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }

    public class ItemNewList
    {
        public int Id { get; set; }//新闻的id
        public string Title { get; set; }//新闻标题
        public string Context { get; set; }//新闻内容
        public string ImgUrl { get; set; }//标题图片路径
        public int Sort { get; set; }//排序
        public string InsertTime { get; set; }//创建时间
        public int InsertUserId { get; set; }//创建人id
        public string UpdateTime { get; set; }//更新时间
        public int UpdateUserId { get; set; }//更新人id
        public int RecordCount { get; set; }//返回的总条数
    }
    /// <summary>
    /// 获取新闻列表
    /// </summary>
    private void GetNewsList()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<Item> list = (List<Item>)JsonHelper.DeSerialze<List<Item>>(data);
        int tpagesize = 10;//每页显示数量
        int tpageindex = 1;//当前页数
        int RecordCount = 0;
        foreach (Item dr in list)
        {
            tpagesize = dr.tpagesize;
            tpageindex = dr.tpageindex;
        }
        ResultModel rm = new ResultModel();
        DataTable dt = new News().GetNewsList(tpageindex, tpagesize, ref RecordCount);
        if (null == dt)
        {
            rm.error = "暂无新闻！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemNewList> list1 = new List<ItemNewList>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemNewList itemnewlist = new ItemNewList();
                itemnewlist.Id = dr["Id"].ToString().ToInt();//新闻id
                itemnewlist.Title = dr["Title"].ToString();//新闻标题
                itemnewlist.Context = dr["Context"].ToString();//新闻内容
                itemnewlist.ImgUrl = dr["ImgUrl"].ToString();//标题图片路径
                itemnewlist.Sort = dr["Sort"].ToString().ToInt();//排序
                itemnewlist.InsertTime = dr["InsertTime"].ToString();//创建时间
                itemnewlist.InsertUserId = dr["InsertUserId"].ToString().ToInt();//创建人id
                itemnewlist.UpdateTime = dr["UpdateTime"].ToString();//更新时间
                itemnewlist.UpdateUserId = dr["UpdateUserId"].ToString().ToInt();//更新人id
                list1.Add(itemnewlist);
            }
            Dictionary<string, List<ItemNewList>> JsonDJ = new Dictionary<string, List<ItemNewList>>();
            List<ItemNewList> listnew = new List<ItemNewList>();
            ItemNewList iof = new ItemNewList();
            iof.RecordCount = RecordCount;
            listnew.Add(iof);

            JsonDJ.Add("orderform", list1);
            JsonDJ.Add("recordcount", listnew);
            string JsonDT = JsonHelper.newTonSerialize(JsonDJ);
            Response.Write(JsonDT);
            Response.End();
        }
    }
    public class ItemAddNew
    {
        public string Title { get; set; }//新闻标题
        public string Context { get; set; }//新闻内容
        public string ImgUrl { get; set; }//标题图片路径
        public int Sort { get; set; }//排序
        public string InsertTime { get; set; }//创建时间
        public string InsertUserId { get; set; }//创建人id
        public string UpdateTime { get; set; }//更新时间
        public int UpdateUserId { get; set; }//更新人id
        public int RecordCount { get; set; }//返回的总条数
    }
    /// <summary>
    /// 新增新闻
    /// </summary>
    private void AddNews()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemAddNew> list = (List<ItemAddNew>)JsonHelper.DeSerialze<List<ItemAddNew>>(data);
        ResultModel rm = new ResultModel();
        int id = 0;
        News news = new News();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemAddNew dr in list)
                {
                    news.Title = dr.Title;
                    news.Context = dr.Context;
                    news.ImgUrl = dr.ImgUrl;
                    news.Sort = dr.Sort;
                    news.InsertTime = DateTime.Now;
                    news.InsertUserId = LoginController.User.Id;
                    news.UpdateTime = Const.DateTimeDefaultValue;//1900
                    news.UpdateUserId = 0;

                    id = news.Save();
                    if (id > 0)
                    {
                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "新闻添加失败！";
                    }
                }
                Response.Write(rm.ToJsonString());
                Response.End();
            }

        }
    }
    /// <summary>
    /// 编辑新闻
    /// </summary>
    private void EtitorNews()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemNewList> list = (List<ItemNewList>)JsonHelper.DeSerialze<List<ItemNewList>>(data);
        ResultModel rm = new ResultModel();
        int id = 0;
        News news = new News();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemNewList dr in list)
                {
                    news.Id = dr.Id;
                    news.Title = dr.Title;
                    news.Context = dr.Context;
                    news.ImgUrl = dr.ImgUrl;
                    news.Sort = dr.Sort;
                    news.InsertTime = Convert.ToDateTime(dr.InsertTime);
                    news.InsertUserId = dr.InsertUserId;
                    news.UpdateTime = DateTime.Now;
                    news.UpdateUserId = LoginController.User.Id;

                    id = news.Save();
                    if (id > 0 && id == dr.Id)
                    {
                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "新闻修改失败！";
                    }
                }
                Response.Write(rm.ToJsonString());
                Response.End();
            }

        }
    }
    public class ItemDeleteNew
    {
        public int Id { get; set; }//新闻的id
    }
    /// <summary>
    /// 删除新闻
    /// </summary>
    private void DeleteNews()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemDeleteNew> list = (List<ItemDeleteNew>)JsonHelper.DeSerialze<List<ItemDeleteNew>>(data);
        ResultModel rm = new ResultModel()
        {
            state = ResultStatu.失败
        };
        News news = new News();
        bool flag = false;
        foreach (ItemDeleteNew dr in list)
        {
            if (!news.Load(dr.Id))
            {
                rm.error = "新闻不存在";
                Response.Write(rm.ToJsonString());
                return;
            }
            flag = news.UpdateDelete(dr.Id);
            if (flag)
            {
                rm.state = ResultStatu.成功;
            }
            else
            {
                rm.state = ResultStatu.失败;
                rm.error = "新闻删除失败！";
                Response.Write(rm.ToJsonString());
                return;
            }
        }
        Response.Write(rm.ToJsonString());
        Response.End();
    }
    /// <summary>
    /// 新闻图片上传
    /// </summary>
    private void UploadImg()
    {
        string resultnew = null;
        string filenamenew = "";
        ResultModel rm = new ResultModel();
        HttpFileCollection img = this.Request.Files;//获取图片
        System.Text.StringBuilder strMsg = new System.Text.StringBuilder();
        strMsg.Append("上传的文件分别是：<hr color=red>");
        try
        {
            for (int iFile = 0; iFile < img.Count; iFile++)
            {
                ///取到当前时间的年、月、日、分、秒的值，并使用字符串格式把它们组合成一个字符串
                filenamenew = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                ///'检查文件扩展名字
                HttpPostedFile postedFile = img[iFile];
                string fileName, fileExtension;
                fileName = System.IO.Path.GetFileName(postedFile.FileName);
                if (fileName != "")
                {
                    fileExtension = System.IO.Path.GetExtension(fileName);
                    strMsg.Append("上传的文件类型：" + postedFile.ContentType.ToString() + "<br>");
                    strMsg.Append("客户端文件地址：" + postedFile.FileName + "<br>");
                    strMsg.Append("上传文件的文件名：" + filenamenew + "<br>");
                    strMsg.Append("上传文件的扩展名：" + fileExtension + "<br><hr>");
                    ///'可根据扩展名字的不同保存到不同的文件夹
                    ///注意：可能要修改你的文件夹的匿名写入权限。
                    filenamenew = filenamenew + fileExtension;
                    postedFile.SaveAs(System.Web.HttpContext.Current.Request.MapPath("Upload/Images/") + filenamenew);
                    //图片上传的路径
                    resultnew = ("Upload/Images/") + filenamenew;
                }
            }

            rm.state = ResultStatu.成功;
        }
        catch (System.Exception Ex)
        {
            resultnew = Ex.Message;
            rm.state = ResultStatu.失败;
        }
        rm.result = resultnew;
        Response.Write(rm.ToJsonString());
        Response.End();
    }

    public class ItemKeyList
    {
        public int Id { get; set; }//字典的id
        public string KeyName { get; set; }//父级的名称
    }
    /// <summary>
    /// 所有字典的父级
    /// </summary>
    private void DictionariesKeyList()
    {
        ResultModel rm = new ResultModel();
        //查询分公司
        string sql = string.Format("select * from DictionariesKey");
        DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "暂无分公司！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemKeyList> list = new List<ItemKeyList>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemKeyList itemkeylist = new ItemKeyList();
                itemkeylist.Id = dr["Id"].ToString().ToInt();
                itemkeylist.KeyName = dr["Key"].ToString();
                list.Add(itemkeylist);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemKeyList>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }

    public class ItemDictionaries
    {
        public int Id { get; set; }//字典的id
        public int KeyId { get; set; }//父级的id
        public string KeyName { get; set; }//父级的名称
        public string DICValue { get; set; }//字典的名称
        public string Enable { get; set; }//是否显示 0不显示 1显示
        public string InsertTime { get; set; }//创建时间
        public string InsertUserId { get; set; }//创建人id
    }
    /// <summary>
    /// 字典列表
    /// </summary>
    private void DictionariesList()
    {
        ResultModel rm = new ResultModel();
        string sql = string.Format("select v.*,k.[Key] KeyName from DictionariesValue v LEFT JOIN DictionariesKey k ON v.KeyId=k.Id where IsDelete=0");
        DataTable dt = DMain.Get(sql);
        if (null == dt)
        {
            rm.error = "暂无字典值！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemDictionaries> list = new List<ItemDictionaries>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemDictionaries itemdictionaries = new ItemDictionaries();
                itemdictionaries.Id = dr["Id"].ToString().ToInt();//字典的id
                itemdictionaries.KeyId = dr["KeyId"].ToString().ToInt();//父级的id
                itemdictionaries.KeyName = dr["KeyName"].ToString();//父级的名称
                itemdictionaries.DICValue = dr["DICValue"].ToString();//字典的名称
                itemdictionaries.Enable = dr["Enable"].ToString();//是否显示 0不显示 1显示
                itemdictionaries.InsertTime = dr["InsertTime"].ToString();//创建时间
                itemdictionaries.InsertUserId = dr["InsertUserId"].ToString();//创建人id
                list.Add(itemdictionaries);
            }
            var arrcount = list.ToList();
            string data = JsonHelper.Serializer<ItemAddCustomer>(arrcount);
            Response.Write(data);
            Response.End();
        }
    }

    public class ItemAddDictionaries
    {
        public int KeyId { get; set; }//父级的id
        public string DICValue { get; set; }//字典的名称
        public string Enable { get; set; }//是否显示 0不显示 1显示
    }
    /// <summary>
    /// 新增字典
    /// </summary>
    private void AddDictionaries()
    {
        ResultModel rm = new ResultModel();
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemAddDictionaries> list = (List<ItemAddDictionaries>)JsonHelper.DeSerialze<List<ItemAddDictionaries>>(data);
        int id = 0;
        DictionariesValue dvalue = new DictionariesValue();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemAddDictionaries dr in list)
                {
                    dvalue.KeyId = dr.KeyId;
                    dvalue.DICValue = dr.DICValue;
                    dvalue.Enable = dr.Enable.ToInt();
                    dvalue.IsDelete = 0;
                    dvalue.InsertTime = DateTime.Now;
                    dvalue.InsertUserId = LoginController.User.Id;
                    dvalue.UpdateTime = Const.DateTimeDefaultValue;//1900
                    dvalue.UpdateUserId = 0;
                    id = dvalue.Save();
                    if (id > 0)
                    {
                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "新闻添加失败！";
                    }
                    Response.Write(rm.ToJsonString());
                    Response.End();
                }
            }
        }
    }

    public class ItemEditorDictionaries
    {
        public int Id { get; set; }//字典的id
        public int KeyId { get; set; }//父级的id
        public string DICValue { get; set; }//字典的名称
        public string Enable { get; set; }//是否显示 0不显示 1显示
        public string InsertTime { get; set; }//创建时间
        public string InsertUserId { get; set; }//创建人id
    }
    /// <summary>
    /// 编辑字典
    /// </summary>
    private void EditorDictionaries()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemEditorDictionaries> list = (List<ItemEditorDictionaries>)JsonHelper.DeSerialze<List<ItemEditorDictionaries>>(data);
        ResultModel rm = new ResultModel();
        int id = 0;
        DictionariesValue dvalue = new DictionariesValue();
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemEditorDictionaries dr in list)
                {
                    dvalue.Id = dr.Id;
                    dvalue.KeyId = dr.KeyId;
                    dvalue.DICValue = dr.DICValue;
                    dvalue.Enable = dr.Enable.ToInt();
                    dvalue.IsDelete = 0;
                    dvalue.InsertTime = Convert.ToDateTime(dr.InsertTime);
                    dvalue.InsertUserId = dr.InsertUserId.ToInt();
                    dvalue.UpdateTime = DateTime.Now;
                    dvalue.UpdateUserId = LoginController.User.Id;
                    id = dvalue.Save();
                    if (id > 0 && id == dr.Id)
                    {
                        rm.state = ResultStatu.成功;
                    }
                    else
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "修改失败！";
                    }
                }
                Response.Write(rm.ToJsonString());
                Response.End();
            }

        }
    }

    public class ItemAuditCustomer
    {
        public int Id { get; set; }//客户的id
        public string CompanyName { get; set; }//客户名称
        public string InsertTime { get; set; }//申请时间
        public string Version { get; set; }//历史版本
        public string VersionId { get; set; }//修改前一版本的id
        public string UpdateTime { get; set; }//审核时间
        public string InsertUserName { get; set; }//申请人姓名
        public string UpdateUserName { get; set; }//审核人姓名
        public string AuditorRemarks { get; set; }//审核备注
        public string Status_Show { get; set; }//审批状态
        public string Editor { get; set; }//修改
        public int RecordCount { get; set; }//返回总数
    }
    /// <summary>
    /// 客户审核列表
    /// </summary>
    private void AuditCustomer()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<Item> list = (List<Item>)JsonHelper.DeSerialze<List<Item>>(data);
        int tpagesize = 10;//每页显示数量
        int tpageindex = 1;//当前页数
        int RecordCount = 0;
        foreach (Item dr in list)
        {
            tpagesize = dr.tpagesize;
            tpageindex = dr.tpageindex;
        }
        ResultModel rm = new ResultModel();
        DataTable dt = new Customer().GetAuditCustomer(tpageindex, tpagesize, ref RecordCount);
        if (null == dt)
        {
            rm.error = "暂无要审核客户！";
            Response.Write(rm.ToJsonString());
            return;
        }
        else
        {
            List<ItemAuditCustomer> list1 = new List<ItemAuditCustomer>();
            foreach (DataRow dr in dt.Rows)
            {
                ItemAuditCustomer auditcustomer = new ItemAuditCustomer();
                auditcustomer.Id = dr["Id"].ToString().ToInt();//订单id
                auditcustomer.CompanyName = dr["CompanyName"].ToString();
                auditcustomer.InsertTime = dr["InsertTime"].ToString();
                auditcustomer.Version = dr["Version"].ToString();
                auditcustomer.VersionId = dr["VersionId"].ToString();
                auditcustomer.UpdateTime = dr["AuditorTime"].ToString();
                auditcustomer.InsertUserName = dr["InsertUserName"].ToString();
                auditcustomer.UpdateUserName = dr["UpdateUserName"].ToString();
                auditcustomer.AuditorRemarks = dr["AuditorRemarks"].ToString();
                auditcustomer.Status_Show = dr["Status_Show"].ToString();
                auditcustomer.Editor = dr["Editor"].ToString();
                list1.Add(auditcustomer);
            }
            Dictionary<string, List<ItemAuditCustomer>> JsonDJ = new Dictionary<string, List<ItemAuditCustomer>>();
            List<ItemAuditCustomer> listnew = new List<ItemAuditCustomer>();
            ItemAuditCustomer iof = new ItemAuditCustomer();
            iof.RecordCount = RecordCount;
            listnew.Add(iof);

            JsonDJ.Add("customer", list1);
            JsonDJ.Add("recordcount", listnew);
            string JsonDT = JsonHelper.newTonSerialize(JsonDJ);
            Response.Write(JsonDT);
            Response.End();
        }

    }
    public class ItemAuditCustomerDetail
    {
        public int Id { get; set; }//客户的id
        public string VersionId { get; set; }//修改前一版本的id        
    }
    /// <summary>
    /// 获取每一个审核客户的详细信息
    /// </summary>
    private void AuditCustomerDetail()
    {
        string data = "";
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }

        ResultModel rm = new ResultModel();
        List<ItemAuditCustomerDetail> list = (List<ItemAuditCustomerDetail>)JsonHelper.DeSerialze<List<ItemAuditCustomerDetail>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemAuditCustomerDetail dr1 in list)
                {
                    //查询订单是否存在
                    string sql = string.Format("select c.*,u.UserName from Customer c INNER JOIN Users u on c.SalesmanId = u.Id where c.IsDelete = 0 and c.id in ({0},{1})", dr1.VersionId, dr1.Id);
                    System.Data.DataTable dt = DMain.Get(sql);
                    if (null == dt || dt.Rows.Count <= 0)
                    {
                        rm.state = ResultStatu.失败;
                        rm.error = "审核客户不存在！";
                        Response.Write(rm.ToJsonString());
                        return;
                    }
                    else
                    {
                        List<ItemAddCustomer> list1 = new List<ItemAddCustomer>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            ItemAddCustomer itemcustomer = new ItemAddCustomer();
                            itemcustomer.Id = dr["Id"].ToString();
                            itemcustomer.CompanyName = dr["CompanyName"].ToString();
                            itemcustomer.CompanyAbbreviation = dr["CompanyAbbreviation"].ToString();
                            itemcustomer.TaxpayerNo = dr["TaxpayerNo"].ToString();
                            itemcustomer.RegisteredAddress = dr["RegisteredAddress"].ToString();
                            itemcustomer.BusinessAddress = dr["BusinessAddress"].ToString();
                            itemcustomer.Bank = dr["Bank"].ToString();
                            itemcustomer.BankAccountNo = dr["BankAccountNo"].ToString();
                            itemcustomer.InvoiceType = dr["InvoiceType"].ToString();
                            itemcustomer.TaxPoint = dr["TaxPoint"].ToString();
                            itemcustomer.PrimaryContact = dr["PrimaryContact"].ToString();
                            itemcustomer.PrimaryTel = dr["PrimaryTel"].ToString();
                            itemcustomer.PrimarySex = dr["PrimarySex"].ToString().ToInt();
                            itemcustomer.CustomerFax = dr["CustomerFax"].ToString();
                            itemcustomer.CustomerPostalCode = dr["CustomerPostalCode"].ToString();
                            itemcustomer.CustomerEmail = dr["CustomerEmail"].ToString();
                            itemcustomer.SecondaryContact = dr["SecondaryContact"].ToString();
                            itemcustomer.SecondaryTel = dr["SecondaryTel"].ToString();
                            itemcustomer.RegistrationTime = dr["RegistrationTime"].ToString();
                            itemcustomer.MonthlyStatementTime = dr["MonthlyStatementTime"].ToString();
                            itemcustomer.BranchId = dr["BranchId"].ToString();
                            itemcustomer.SalesmanId = dr["SalesmanId"].ToString();
                            itemcustomer.SalesmanName = dr["UserName"].ToString();
                            itemcustomer.VersionId = dr["VersionId"].ToString();
                            itemcustomer.Status = dr["Status"].ToString().ToInt();
                            itemcustomer.AuditorRemarks = dr["AuditorRemarks"].ToString();
                            itemcustomer.CustomerNumber = dr["CustomerNumber"].ToString();
                            list1.Add(itemcustomer);
                        }
                        var arrcount = list1.ToList();
                        string datat = JsonHelper.newTonSerialize(arrcount);
                        Response.Write(datat);
                        Response.End();
                    }
                }
            }
        }
    }

    public class ItemEditorAuditCustomer
    {
        public int Id { get; set; }//客户的id
        public int VersionId { get; set; }//上一版本的id
        public int Status { get; set; }//1通过 2不通过
        public string AuditorRemarks { get; set; }//审核备注
    }
    /// <summary>
    /// 客户审核是否同意
    /// </summary>
    private void EditorAuditCustomer()
    {
        ResultModel rm = new ResultModel();
        string data = "";
        int id = 0;
        int status = 5;
        bool flag = false;
        using (StreamReader sr = new StreamReader(Request.InputStream))
        {
            data = sr.ReadLine();
        }
        List<ItemEditorAuditCustomer> list = (List<ItemEditorAuditCustomer>)JsonHelper.DeSerialze<List<ItemEditorAuditCustomer>>(data);
        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (ItemEditorAuditCustomer dr1 in list)
                {
                    status = dr1.Status;
                    if (status == 1)
                    {
                        string sql = string.Format("update Customer set Status={1} ,UpdateTime='{2}',UpdateUserId={3},AuditorRemarks='{4}' where Id={0}", dr1.Id, dr1.Status, DateTime.Now, LoginController.User.Id, dr1.AuditorRemarks);
                        flag = DMain.RunUpdate(sql);

                        if (flag)
                        {
                            string sql2 = string.Format("update CustomerLog set Status={1} where CustomerId={0}", id, status);
                            flag = DMain.RunUpdate(sql2);
                        }
                    }
                    else if (status == 2)
                    {
                        string sql = string.Format("update Customer set Status={1} ,UpdateTime='{2}',UpdateUserId={3},AuditorRemarks='{4}',Show=1 where Id={0}", dr1.Id, dr1.Status, DateTime.Now, LoginController.User.Id, dr1.AuditorRemarks);
                        flag = DMain.RunUpdate(sql);
                        if (flag)
                        {
                            string sql2 = string.Format("update Customer set Show=0 where id={0}", dr1.VersionId);
                            flag = DMain.RunUpdate(sql2);
                            if (flag)
                            {
                                string sql3 = string.Format("update CustomerLog set Status={1} where CustomerId={0}", id, status);
                                flag = DMain.RunUpdate(sql3);
                            }
                        }
                    }
                }
                if (flag == true)
                {
                    rm.state = ResultStatu.成功;
                }
                else
                {
                    rm.state = ResultStatu.失败;
                    rm.error = "审核失败！";
                }
                Response.Write(rm.ToJsonString());
                Response.End();
            }
        }
    }

    private void GetById(EntityBase entity)
    {
        int id = GetIntRequest("Id");
        ResultModel rm = new ResultModel();
        if (entity.Load(id))
        {
            rm.state = ResultStatu.成功;
            rm.result = entity.ToJsonString();
        }
        else
        {
            rm.state = ResultStatu.失败;
            rm.error = "该编号数据不存在或已删除。";
        }
        Response.Write(rm.ToJsonString());
        Response.End();
    }
    private void DeleteById(EntityBase entity)
    {
        int id = GetIntRequest("Id");
        ResultModel rm = new ResultModel();
        if (entity.Delete(id))
        {
            rm.state = ResultStatu.成功;
            rm.result = "";
        }
        else
        {
            rm.state = ResultStatu.失败;
            rm.error = "该编号数据不存在或已删除。";
        }
        Response.Write(rm.ToJsonString());
        Response.End();
    }

    private string GetRequest(string req)
    {
        return Main.GetstrQueryString(req, false);
    }
    private float GetFloatRequest(string req)
    {
        float n = 0;
        string str = Request[req];
        float.TryParse(str, out n);
        return n;
    }
    private int GetIntRequest(string req)
    {
        int n = 0;
        string str = Request[req];
        if (string.IsNullOrWhiteSpace(str)) return 0;
        int.TryParse(str, out n);
        return n;
    }

    private static string _dowebget(string posturl)
    {
        Stream instream = null;
        StreamReader sr = null;
        HttpWebResponse response = null;
        HttpWebRequest request = null;
        Encoding encoding = Encoding.GetEncoding("GBK");
        // 准备请求...
        try
        {
            // 设置参数
            request = WebRequest.Create(posturl) as HttpWebRequest;
            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            //发送请求并获取相应回应数据
            response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            instream = response.GetResponseStream();
            sr = new StreamReader(instream, encoding);
            //返回结果网页（html）代码
            string content = sr.ReadToEnd();
            sr.Close();
            if (YXJ.Common.Config.GetConfig("DebugMode") == "true")
            {
                YXJ.Common.Log.WriteLog("WechatGet结果：" + content);
            }
            return content;
        }
        catch (Exception ex)
        {
            string err = ex.Message;
            return string.Empty;
        }
    }

    /// <summary>
    /// 格式化json,替换值里的双引号为单引号
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private string JsonFormat(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        s = Regex.Replace(s, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
        s = s.Replace("\r\n", "");
        s = s.Replace("\"", "\'");
        //处理ckedtor上传图片加域名
        s = s.Replace("src='/upload/images/", "src='" + YXJ.Common.Config.GetConfig("domain") + "upload/images/");
        return s;
    }
    private string Xml2Json(string xml)
    {
        //处理返回状态
        try
        {
            int index = xml.IndexOf("<");
            xml = xml.Substring(0, index) + "<Result>" + xml.Substring(index) + "</Result>";
            xml = xml.Replace("&", "");
        }
        catch (Exception) { }
        return xml;
    }

    public static T GetRequestPrams(HttpContext context)
    {
        Stream sream = context.Request.InputStream;
        StreamReader sr = new StreamReader(sream);
        string search = sr.ReadToEnd();
        sr.Close();
        var jSetting = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        return JsonConvert.DeserializeObject<T>(search, jSetting);
    }
}