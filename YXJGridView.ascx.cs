using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using YXJ.DAL;
using YXJ.Common;
//using YXJ.WebControls;
using YXJ.Entity;
using System.Reflection;
using System.Globalization;
using System.Text.RegularExpressions;
using C1.C1Excel;
public partial class YXJGridView : System.Web.UI.UserControl
{
    #region PageSize
    public int PageSize
    {
        get { return Pager1.PageSize; }
        set { Pager1.PageSize = value; }
    }
    #endregion

    #region Page_Load
    protected void Page_Load(object sender, EventArgs e)
    {
        Pager1.Pager += new Ctrls_Pager_Background.PageHandler(Pager1_Pager);
    }
    #endregion

    #region DataField
    /// <summary>
    /// GridView中要显示的列
    /// </summary>
    public List<YXJGridViewBounder> DataField
    {
        set
        {
            if (gv1.Columns.Count == 1)
            {
                foreach (YXJGridViewBounder clb in value)
                {
                    gv1.Columns.Add(clb);
                }
                //ViewState["DataField"] = value;
            }
        }
        //get
        //{
        //    return ViewState["DataField"] as List<YXJGridViewBounder>;
        //}
    }



    #endregion

    #region Sort
    public string Sort
    {
        get { return ViewState["Sort_" + this.ID] as string; }
        set { ViewState["Sort_" + this.ID] = value; }
    }
    #endregion

    #region SkinID
    public new string SkinID
    {
        get { return ViewState["SkinId_" + this.ID] as string; }
        set { ViewState["SkinId_" + this.ID] = value; this.gv1.CssClass = value; }
    }
    #endregion

    #region GetSelectedId
    /// <summary>
    /// 获取用户选择的行的ID列表
    /// </summary>
    /// <returns></returns>
    public List<int> GetSelectedId()
    {
        List<int> Ids = new List<int>();
        foreach (GridViewRow gvr in gv1.Rows)
        {
            if (gvr.RowType == DataControlRowType.DataRow)
            {
                CheckBox cb = (CheckBox)gvr.FindControl("YXJGridView_CheckBox1");
                if (cb != null && cb.Checked)
                {
                    Ids.Add(gv1.DataKeys[gvr.RowIndex].Value.ToString().ToInt());
                }
            }
        }
        return Ids;
    }
    #endregion

    #region 绑定数据
    /// <summary>
    /// 绑定数据
    /// </summary>
    public void BindData()
    {
        gv1.DataKeyNames = new string[] { "Id" };
        this.Pager1.CurrentPageIndex = Pager1.CurrentPageIndex;
        DataTable dt = GetDataSource(true);
        gv1.DataSource = dt;
        gv1.DataBind();
        if (dt == null || dt.Rows.Count == 0)
        {
            Pager1.Visible = false;
        }
        else
        {
            Pager1.Visible = true;
        }
    }
    public void BindData(DataTable DataSource)
    {
        gv1.DataKeyNames = new string[] { "Id" };
        gv1.DataSource = DataSource;
        gv1.DataBind();
        Pager1.Visible = false;
    }
    /// <summary>
    /// 绑定，并返回数据
    /// </summary>
    /// <param name="DataSource"></param>
    public void BindData(out DataTable DataSource)
    {
        DataSource = GetDataSource(true);
        gv1.DataKeyNames = new string[] { "Id" };
        gv1.DataSource = DataSource;
        gv1.DataBind();

        if (DataSource == null || DataSource.Rows.Count == 0)
        {
            Pager1.Visible = false;
        }
        else
        {
            Pager1.Visible = true;
        }
    }
    #endregion

    #region Filter
    /// <summary>
    /// 过滤器，用于“搜索”二维数组，第一维：搜索的字段名（对应于数据库），第二维：值
    /// </summary>
    public List<EntityBase.Filter> Filter
    {
        get
        {
            if (null == ViewState["Filter" + this.ID]) return new List<EntityBase.Filter>();
            return ViewState["Filter" + this.ID] as List<EntityBase.Filter>;
        }
        set
        {
            if (null == value) return;
            ViewState["Filter" + this.ID] = value;
        }
    }
    #endregion

    #region BusinessClassName
    /// <summary>
    /// 业务处理类名
    /// </summary>
    public string BusinessClassName
    {
        get { return ViewState["BusinessClassName_" + this.ID] as string; }
        set { ViewState["BusinessClassName_" + this.ID] = value; }
    }
    #endregion

    #region CustomerParam
    public object CustomerParam
    {
        get { return ViewState["CustomerParam_" + this.ID] as string; }
        set { ViewState["CustomerParam_" + this.ID] = value; }
    }
    #endregion

    #region RecordCount
    public int RecordCount
    {
        get
        {
            if (ViewState["RecordCount"] != null)
            {
                return int.Parse(ViewState["RecordCount"].ToString());
            }
            return 0;
        }
        private set
        {
            ViewState["RecordCount"] = value;
        }
    }
    #endregion

    #region 获取数据源
    /// <summary>
    /// 获取数据源
    /// </summary>
    /// <param name="AllowPager">是否允许数据源分页</param>
    /// <returns></returns>
    public DataTable GetDataSource(bool AllowPager)
    {
        int iRecordCount = 0;
        EntityBase eb = (EntityBase)Assembly.Load("Entity").CreateInstance("YXJ.Entity." + BusinessClassName);
        DataTable dt = null;
        if (AllowPager)
        {
            dt = eb.GetList(Filter, Sort, Pager1.CurrentPageIndex, Pager1.PageSize, ref iRecordCount, CustomerParam);
        }
        else
        {
            dt = eb.GetList(Filter, Sort, 0, 0, ref iRecordCount, CustomerParam);
        }
        this.RecordCount = iRecordCount;
        eb.ProcessDataTableAdapt2UIList(dt,CustomerParam);
        Pager1.RecordCount = RecordCount;
        return dt;
    }
    #endregion

    #region 导出Excel
    /// <summary>
    /// 根据DataTable生成Excel
    /// </summary>
    /// <param name="ExcelPath">Excel要保存的路径（文件夹）</param>
    /// <param name="ExcelName">Excel要保存的文件名</param>
    /// <returns></returns>
    public bool DataSource2Excel(List<YXJGridViewBounder> ExcelField, string ExcelPath, string ExcelName, string CustomerParam)
    {
        DataTable dt = GetDataSource(false);
        if (null == dt)
        {
            return false;
        }

        C1XLBook book = new C1XLBook();
        XLSheet sheet = book.Sheets[0];

        #region styleColumnName
        XLStyle styleColumnName = new XLStyle(book);
        styleColumnName.ForeColor = System.Drawing.Color.Black;
        #endregion

        #region styleContent
        XLStyle styleContent = new XLStyle(book);
        styleContent.ForeColor = System.Drawing.Color.Black;
        #endregion

        for (int i = 0; i < ExcelField.Count; i++)
        {
            sheet[0, i].SetValue(ExcelField[i].HeaderText, styleColumnName);
        }

        DateTime _dateTime;
        float _float;
        string temp = null;
        object tempo = null;
        string DataFormatString = null;
        for (int m = 0; m < dt.Rows.Count; m++)
        {
            for (int i = 0; i < ExcelField.Count; i++)
            {
                //IFormatProvider format=new DummyProvider(ExcelField[i]);                    
                //temp=Convert.ToString(dt.Rows[m][ExcelField[i].DataField],format);
                //sheet[m+1,i].SetValue(temp,styleContent);
                tempo = dt.Rows[m][ExcelField[i].DataField];
                DataFormatString = ExcelField[i].DataFormatString;
                if (DataFormatString.StartsWith("{0:") && DataFormatString.EndsWith("}"))
                {
                    DataFormatString = DataFormatString.Substring(3, DataFormatString.Length - 4);
                }
                switch (tempo.GetType().FullName)
                {
                    case "System.String":
                        if (!ExcelField[i].HtmlEncode)
                        {
                            temp = Main.RemoveHTMLTags(tempo.ToString());
                            sheet[m + 1, i].SetValue(temp, styleContent);
                        }
                        else
                        {
                            sheet[m + 1, i].SetValue(tempo, styleContent);
                        }
                        break;
                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                        string sTempo = tempo.ToString();
                        string pattern = "([\\w]{1,3});;([\\w]{1,3})";
                        if (Regex.IsMatch(DataFormatString, pattern))
                        {
                            Regex r = new Regex(pattern);
                            Match _m = r.Match(DataFormatString);
                            if (_m.Success)
                            {
                                if (sTempo == "0")
                                {
                                    sheet[m + 1, i].SetValue(_m.Groups[2].Value, styleContent);
                                }
                                else if (sTempo == "1")
                                {
                                    sheet[m + 1, i].SetValue(_m.Groups[1].Value, styleContent);
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }
                            }
                            else
                            {
                                sheet[m + 1, i].SetValue(tempo, styleContent);
                            }
                        }
                        else
                        {
                            sheet[m + 1, i].SetValue(tempo, styleContent);
                        }                        
                        break;
                    case "System.Single":
                    case "System.Double":
                    case "System.Decimal":
                        if (!float.TryParse(tempo.ToString(), out _float) || _float == float.MinValue)
                        {
                            sheet[m + 1, i].SetValue(string.Empty, styleContent);
                        }
                        else if (string.IsNullOrEmpty(DataFormatString))
                        {
                            sheet[m + 1, i].SetValue(_float, styleContent);
                        }
                        else
                        {

                            sheet[m + 1, i].SetValue(_float.ToString(DataFormatString), styleContent);
                        }
                        break;
                    case "System.DateTime":
                        if (tempo is DBNull || tempo.ToString().Trim() == string.Empty)
                        {
                            sheet[m + 1, i].SetValue(string.Empty, styleContent);
                        }
                        else
                        {
                            if (!DateTime.TryParse(tempo.ToString(), out _dateTime))
                            {
                                sheet[m + 1, i].SetValue(string.Empty, styleContent);
                            }
                            else
                            {
                                if (_dateTime == Const.DateTimeDefaultValue)
                                {
                                    sheet[m + 1, i].SetValue(string.Empty, styleContent);
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(DataFormatString))
                                    {
                                        sheet[m + 1, i].SetValue(_dateTime, styleContent);
                                    }
                                    else
                                    {
                                        sheet[m + 1, i].SetValue(_dateTime.ToString(DataFormatString), styleContent);
                                    }
                                }
                            }
                        }
                        break;
                    case "System.DBNull":
                        sheet[m + 1, i].SetValue(string.Empty, styleContent);
                        break;
                    default:
                        Log.WriteLog("在执行YXJGridView导出为Excel时出错！列类型名称：" + tempo.GetType().FullName);
                        throw new NotImplementedException();
                }

            }
        }

        try
        {
            if(!System.IO.Directory.Exists(ExcelPath))
            {
                System.IO.Directory.CreateDirectory(ExcelPath);
            }
            book.Save(ExcelPath + ExcelName);
            return true;
        }
        catch (Exception ex2)
        {
            Log.WriteLog(ex2, "保存Excel文件时出错");
            return false;
        }
    }
    #endregion

    #region 分页事件
    protected void Pager1_Pager()
    {
        BindData();
    }
    #endregion

    #region GridView列绑定类
    /// <summary>
    /// GridView列绑定类
    /// </summary>
    [Serializable]
    public class YXJGridViewBounder : BoundField
    {
        /// <summary>
        /// 绑定字段
        /// </summary>
        public new string DataField
        {
            get { return base.DataField; }
            set { base.DataField = value; }
        }
        /// <summary>
        /// 列标题
        /// </summary>
        public new string HeaderText
        {
            get { return base.HeaderText; }
            set { base.HeaderText = value; }
        }
        public new string DataFormatString
        {
            get { return base.DataFormatString; }
            set { base.DataFormatString = value; }
        }
        public new bool HtmlEncode
        {
            get { return base.HtmlEncode; }
            set { base.HtmlEncode = value; }
        }
        /// <summary>
        /// 排序表达式
        /// </summary>
        public new string SortExpression
        {
            get { return base.SortExpression; }
            set { base.SortExpression = value; }
        }
    }
    #endregion

    #region 排序
    protected void gv1_Sorting(object sender, GridViewSortEventArgs e)
    {

        string s = e.SortExpression;
        if (null == Session["SortDirection"] || Session["SortDirection"].ToString() == "ASC")
        {
            s += " asc";
            Session["SortDirection"] = "DESC";
        }
        else
        {
            s += " desc";
            Session["SortDirection"] = "ASC";
        }
        this.Sort = s;
        BindData();
    }
    #endregion
        
    public event YXJGVRowDataBoundHandler RowDataBound;
    public delegate void YXJGVRowDataBoundHandler(GridView gv, GridViewRowEventArgs e);
    protected void gv1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex >= 0)
        {
            int ColumnsCount = e.Row.Cells.Count;
            for (int i = 0; i < ColumnsCount; i++)
            {
                var m = gv1.Columns[i] as YXJGridViewBounder;
                if (m == null) continue;
                
                #region 处理DateTime类型
                    string DataFieldName = ((YXJGridViewBounder)(gv1.Columns[i])).DataField;
                    DataRowView drRow = e.Row.DataItem as DataRowView;
                    object valueo = drRow[DataFieldName];
                    switch (valueo.GetType().ToString())
                    {
                        case "System.DateTime":
                            string value = e.Row.Cells[i].Text.Trim();
                            if (value == string.Empty)
                            {
                                break;
                            }
                            DateTime dtValue;
                            if (!DateTime.TryParse(value, out dtValue))
                            {
                                break;
                            }
                            else
                            {
                                if (dtValue == Const.DateTimeDefaultValue)
                                {
                                    e.Row.Cells[i].Text = "&nbsp;";
                                    break;
                                }
                            }
                            break;
                    }
                #endregion
                    
            }
        }
        if (null != RowDataBound)
        {
            RowDataBound(gv1, e);
        }
    }

    public Ctrls_Pager_Background Pager
    {
        get
        {
            return this.Pager1;

        }
    }
}