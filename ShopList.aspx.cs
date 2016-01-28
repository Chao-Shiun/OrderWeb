using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class ShopList : BasePage
{
    public Guid ShopID
    {
        set;
        get;
    }
    Dictionary<string, string> QueryString = new Dictionary<string, string>();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["userID"] == null)
        {
            Response.Redirect("ADLogin.aspx", true);
        }
        if (!IsPostBack)
        {
            #region 設定餐點類型
            Category.Items.Add(new ListItem("不拘", "不拘"));
            Category.Items.Add(new ListItem("中餐", "0"));
            Category.Items.Add(new ListItem("點心", "1"));
            Category.Items.Add(new ListItem("飲料", "2"));
            #endregion

            #region 設定素食選項
            IsVegetarianism.Items.Add(new ListItem("不拘", "不拘"));
            IsVegetarianism.Items.Add(new ListItem("否", "false"));
            IsVegetarianism.Items.Add(new ListItem("是", "true"));
            #endregion

            #region 查詢菜單內容
            if (!string.IsNullOrWhiteSpace(Request["ShopName"]))
            {
                QueryString.Add("ShopName", Request["ShopName"]);
                ShopName.Text = Request["ShopName"];
            }
            if (!string.IsNullOrWhiteSpace(Request["Category"]))
            {
                QueryString.Add("Category", Request["Category"]);
                Category.Items.FindByText(Request["Category"]).Selected = true;
            }


            if (!string.IsNullOrWhiteSpace(Request["IsVegetarianism"]) && (Request["IsVegetarianism"].Equals("是") || Request["IsVegetarianism"].Equals("否")))
            {
                QueryString.Add("IsVegetarianism", Request["IsVegetarianism"]);
                IsVegetarianism.Items.FindByText(Request["IsVegetarianism"]).Selected = true;
            }

            int PageNumber;
            if (string.IsNullOrWhiteSpace(Request["Page"]) || !int.TryParse(Request["Page"], out PageNumber) || PageNumber <= 0)
                PageNumber = 1;
            ucPagination.CPage = PageNumber;
            Query(PageNumber, QueryString);
            #endregion
        }
    }

    /// <summary>
    /// 設定該查詢有多少分頁
    /// </summary>
    /// <param name="sqlstr">查詢字串</param>
    /// <param name="para">條件參數</param>
    /// <param name="pagenumber">目前頁數</param>
    private void SetEndPage(string sqlstr, SqlParameter[] para, int pagenumber)
    {
        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
            {
                if (para != null)
                    cmd.Parameters.AddRange(para);

                int count = (int)cmd.ExecuteScalar();
                count = (count % 10 == 0) ? count / 10 : (count / 10) + 1;
                ucPagination.EPage = count;
                if (count <= pagenumber)
                    ucPagination.CPage = count;
            }
        }
    }

    /// <summary>
    /// 查詢網頁內容
    /// </summary>
    /// <param name="pagenumber">第幾頁</param>
    /// <param name="QueryString">網址的參數</param>
    private void Query(int pagenumber, Dictionary<string, string> QueryString)
    {
        string sqlstr = null;
        List<SqlParameter> para = null;
        List<SqlParameter> CountPara = null;

        if (QueryString.ContainsKey("ShopName") || QueryString.ContainsKey("Category") || QueryString.ContainsKey("IsVegetarianism"))
        {
            int i = 0;
            para = new List<SqlParameter>();
            CountPara = new List<SqlParameter>();

            sqlstr = string.Format(@"SELECT a.ShopID,a.ShopName,b.CategoryName
                                    FROM ShopHead a
                                    INNER JOIN MenuCategory b
                                    on a.CategoryID=b.CategoryID 
                                    WHERE a.IsCancel=0");

            //這是用來計算頁數的sql
            string CountSqlStr = string.Format(@"SELECT COUNT(a.ShopID)
                                                FROM ShopHead a
                                                INNER JOIN MenuCategory b
                                                on a.CategoryID=b.CategoryID 
                                                WHERE a.IsCancel=0");

            if (QueryString.ContainsKey("ShopName"))
            {
                sqlstr += " and a.ShopName like @ShopName ";
                para.Add(new SqlParameter("@ShopName", SqlDbType.NVarChar, 20));
                para[i].Value = QueryString["ShopName"] + "%";

                CountSqlStr += " and a.ShopName like @ShopName ";
                CountPara.Add(new SqlParameter("@ShopName", SqlDbType.NVarChar, 20));
                CountPara[i++].Value = QueryString["ShopName"] + "%";
            }
            if (QueryString.ContainsKey("Category"))
            {
                sqlstr += " and b.CategoryName=@CategoryName ";
                para.Add(new SqlParameter("@CategoryName", SqlDbType.NVarChar, 5));
                para[i].Value = QueryString["Category"];

                CountSqlStr += " and b.CategoryName=@CategoryName ";
                CountPara.Add(new SqlParameter("@CategoryName", SqlDbType.NVarChar, 5));
                CountPara[i++].Value = QueryString["Category"];
            }

            if (QueryString.ContainsKey("IsVegetarianism"))
            {
                sqlstr += " and a.IsVegetarianismShop=@IsVegetarianismShop ";
                para.Add(new SqlParameter("@IsVegetarianismShop", SqlDbType.Bit));
                para[i].Value = QueryString["IsVegetarianism"].Equals("是");

                CountSqlStr += " and a.IsVegetarianismShop=@IsVegetarianismShop ";
                CountPara.Add(new SqlParameter("@IsVegetarianismShop", SqlDbType.Bit));
                CountPara[i++].Value = QueryString["IsVegetarianism"].Equals("是");
            }

            sqlstr += string.Format(@"ORDER BY a.createdatetime desc
                                    OFFSET {0} ROWS
                                    FETCH NEXT 10 ROWS ONLY", 10 * (pagenumber - 1));

            ucPagination.CPage = pagenumber;
            ucPagination.CList = QueryString;

            SetEndPage(CountSqlStr, CountPara.ToArray() as SqlParameter[], pagenumber);
        }
        else
        {
            sqlstr = string.Format(@"SELECT a.ShopID,a.ShopName,b.CategoryName
                                    FROM ShopHead a
                                    INNER JOIN MenuCategory b
                                    on a.CategoryID=b.CategoryID 
                                    WHERE a.IsCancel=0
                                    ORDER BY a.createdatetime desc
                                    OFFSET {0} ROWS
                                    FETCH NEXT 10 ROWS ONLY", 10 * (pagenumber - 1));

            SetEndPage("SELECT COUNT(x.ShopID) FROM ShopHead x where x.IsCancel=0", null, pagenumber);
        }

        Control ctl = Page.FindControl(ucPagination.UniqueID);
        Type ctlType = ctl.GetType();
        MethodInfo ctlMethod = ctlType.GetMethod("Initialization");
        //取得已定義方法的參數
        //ParameterInfo[] p = ctlMethod.GetParameters();

        //輸入參數宣告
        //Object[] parameters = new Object[p.Length];
        //string strPassInfo = "Hello my usercontrol!";
        //parameters[0] = strPassInfo;
        ctlMethod.Invoke(ctl, null);

        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
            {
                if (QueryString.ContainsKey("ShopName") || QueryString.ContainsKey("Category") || QueryString.ContainsKey("IsVegetarianism"))
                    cmd.Parameters.AddRange(para.ToArray() as SqlParameter[]);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    Repeater1.DataSource = dr;
                    Repeater1.DataBind();
                    if (!dr.HasRows)
                    {
                        AlertMessage("查無店家資料!");
                        ucPagination.Visible = false;
                        ucPagination.EPage = 1;
                    }
                    else
                    {
                        ucPagination.Visible = true;
                    }
                }
            }
        }
    }

    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            HiddenField HiddenShopID = e.Item.FindControl("ShopID") as HiddenField;

            using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
            {
                conn.Open();
                HtmlTableCell td = e.Item.FindControl("ImgLink") as HtmlTableCell;
                using (SqlCommand cmd = new SqlCommand("select ShopID,FileName from MenuImg where ShopID=@ShopID", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@ShopID", SqlDbType.UniqueIdentifier));
                    cmd.Parameters[0].Value = Guid.Parse(HiddenShopID.Value);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        LiteralControl startCenter = new LiteralControl();
                        startCenter.Text = "<center>";
                        td.Controls.Add(startCenter);
                        while (dr.Read())
                        {
                            LiteralControl lit = new LiteralControl();
                            //上線要改的地方
                            lit.Text = @"<a href=""http://" + Request.Url.Authority + "/Menu/" + dr[1].ToString() + @""" target=""_blank"">" + dr[1].ToString() + "</a>";
                            //lit.Text = @"<a href=""" + Request.Url.Scheme + "://" + Request.Url.Authority + "/" + Request.Url.Segments[1] + "Menu/" + HttpUtility.UrlEncode(dr.GetString(1)) + @""" target=""_blank"">" + dr.GetString(1) + "</a>";
                            td.Controls.Add(lit);
                            LiteralControl br = new LiteralControl();
                            br.Text = "<br/>";
                            td.Controls.Add(br);
                        }
                        LiteralControl endCenter = new LiteralControl();
                        endCenter.Text = "</center>";
                        td.Controls.Add(endCenter);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 設定使用者按下按鈕的相關資訊，並傳到下一頁
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        HiddenField HiddenShopID = e.Item.FindControl("ShopID") as HiddenField;
        ShopID = Guid.Parse(HiddenShopID.Value);
        Server.Transfer("~/CreateOrder.aspx", false);
    }


    protected void QueryCondition_Click(object sender, EventArgs e)
    {
        ucPagination.CPage = 1;
        if (!string.IsNullOrWhiteSpace(ShopName.Text.Trim()))
            QueryString.Add("ShopName", ShopName.Text.Trim());
        if (Category.SelectedValue != "不拘")
            QueryString.Add("Category", Category.SelectedItem.Text);
        if (IsVegetarianism.SelectedValue != "不拘")
            QueryString.Add("IsVegetarianism", IsVegetarianism.SelectedItem.Text);
        Query(1, QueryString);
    }

    /// <summary>
    /// 判斷是目前哪個button觸發postback
    /// </summary>
    /// <returns>回傳</returns>
    /// http://stackoverflow.com/questions/3175513/on-postback-how-can-i-check-which-control-cause-postback-in-page-init-event
    private string getPostBackControlName()
    {

        Control control = null;

        //first we will check the "__EVENTTARGET" because if post back made by       the controls

        //which used "_doPostBack" function also available in Request.Form collection.

        string ctrlname = Page.Request.Params["__EVENTTARGET"];

        if (ctrlname != null && ctrlname != String.Empty)
        {
            control = Page.FindControl(ctrlname);
        }

        // if __EVENTTARGET is null, the control is a button type and we need to

        // iterate over the form collection to find it

        else
        {
            string ctrlStr = String.Empty;
            Control c = null;
            foreach (string ctl in Page.Request.Form)
            {
                //handle ImageButton they having an additional "quasi-property" in their Id which identifies

                //mouse x and y coordinates

                if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
                {
                    ctrlStr = ctl.Substring(0, ctl.Length - 2);
                    c = Page.FindControl(ctrlStr);
                }
                else
                    c = Page.FindControl(ctl);

                if (c is System.Web.UI.WebControls.Button || c is System.Web.UI.WebControls.ImageButton)
                {
                    control = c;
                    break;
                }
            }
        }
        return control.ID;
    }
}