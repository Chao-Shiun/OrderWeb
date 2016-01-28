using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class OrderList : BasePage
{
    public Guid ShopID
    {
        get;
        set;
    }
    public Guid OrderID
    {
        get;
        set;
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

            #region 設定部門別
            DepartMent.Items.Add(new ListItem("不拘", "不拘"));
            using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Department", conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        //while (dr.Read())
                        //    DepartMent.Items.Add(new ListItem(dr.GetString(1), dr.GetString(1)));
                        DepartMent.DataTextField = "DepartmentName";
                        DepartMent.DataValueField = "DepartmentName";
                        DepartMent.DataSource = dr;
                        DepartMent.DataBind();
                    }
                    cmd.Cancel();
                }
            }
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
                Category.Items.FindByValue(Request["Category"]).Selected = true;
            }

            if (!string.IsNullOrWhiteSpace(Request["IsVegetarianism"]) && (Request["IsVegetarianism"].Equals("是") || Request["IsVegetarianism"].Equals("否")))
            {
                QueryString.Add("IsVegetarianism", Request["IsVegetarianism"]);
                IsVegetarianism.Items.FindByText(Request["IsVegetarianism"]).Selected = true;
            }

            if (!string.IsNullOrWhiteSpace(Request["Department"]))
            {
                QueryString.Add("Department", Request["Department"]);
                DepartMent.Items.FindByText(Request["Department"]).Selected = true;
            }

            if (!string.IsNullOrWhiteSpace(Request["Creator"]))
            {
                QueryString.Add("Creator", Request["Creator"]);
                Creator.Text = Request["Creator"];
            }

            int PageNumber;
            if (string.IsNullOrWhiteSpace(Request["Page"]) || !int.TryParse(Request["Page"], out PageNumber))
                PageNumber = 1;
            ucPagination.CPage = PageNumber;
            Query(PageNumber);
            #endregion
        }
    }
    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        Button SelectBtn = e.CommandSource as Button;
        HiddenField HiddenOrderID = e.Item.FindControl("OrderID") as HiddenField;
        /*if (SelectBtn.Text.Equals("點餐"))
        {*/
        HiddenField HiddenShopID = e.Item.FindControl("ShopID") as HiddenField;
        ShopID = Guid.Parse(HiddenShopID.Value);
        OrderID = Guid.Parse(HiddenOrderID.Value);
        Server.Transfer("ToOrder.aspx", false);
        //Response.Redirect("ToOrder.aspx");
        /*}
        else
        {
            OrderID = Guid.Parse(HiddenOrderID.Value);
            Server.Transfer("OrderManagement.aspx");
        }*/
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
                    #region 設定菜單連結
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
                            //上線要改
                            lit.Text = @"<a href=""http://" + Request.Url.Authority + "/Menu/" + dr[1].ToString() + @""" target=""_blank"">" + dr[1].ToString() + "</a>";
                            //lit.Text = @"<a href=""" + Request.Url.Scheme + "://" + Request.Url.Authority + "/" + Request.Url.Segments[1] + "Menu/" + HttpUtility.UrlEncode(dr[1].ToString()) + @""" target=""_blank"">" + dr[1].ToString() + "</a>";
                            td.Controls.Add(lit);
                            LiteralControl br = new LiteralControl();
                            br.Text = "</br>";
                            td.Controls.Add(br);
                        }
                        LiteralControl endCenter = new LiteralControl();
                        endCenter.Text = "</center>";
                        td.Controls.Add(endCenter);
                        cmd.Cancel();
                    }
                    #endregion
                    /*#region 設定訂單管理按鈕開單者才看得到
                    HiddenField hidOrderID = e.Item.FindControl("OrderID") as HiddenField;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "SELECT a.Creator FROM OrderHead a WHERE a.OrderID=@OrderID AND a.Creator=@Creator";
                    cmd.Parameters.Add("@OrderID", SqlDbType.UniqueIdentifier);
                    cmd.Parameters[0].Value = Guid.Parse(hidOrderID.Value);
                    cmd.Parameters.Add("@Creator", SqlDbType.VarChar, 10);
                    cmd.Parameters[1].Value = Session["userID"];

                    if (cmd.ExecuteScalar() == null)
                    {
                        Button btnOrderManagement = e.Item.FindControl("OrderManagement") as Button;
                        btnOrderManagement.Visible = false;
                    }
                    #endregion*/
                }
            }
        }
    }

    /// <summary>
    /// 查詢網頁內容
    /// </summary>
    /// <param name="pagenumber">第幾頁</param>
    /// <param name="QueryString">網址的參數</param>
    private void Query(int pagenumber)
    {
        List<SqlParameter> para = null;
        List<SqlParameter> CountPara = null;
        string sqlstr = null;
        if (QueryString.Count != 0)
        {
            StringBuilder sqlstrPart1 = new StringBuilder(), sqlstrPart2 = new StringBuilder();
            int i = 0;
            para = new List<SqlParameter>();
            CountPara = new List<SqlParameter>();

            #region 這是查詢本體
            sqlstrPart1.Append(string.Format(@"SELECT * FROM(
                                        select a.OrderID,a.ShopID,b.ShopName,
                                        (d.DepartmentName+'-'+c.UserName) as DepartmentUser,a.Deadline,e.CategoryName
                                        FROM OrderHead a
                                        INNER JOIN ShopHead b 
                                        on a.ShopID=b.ShopID
                                        INNER JOIN UserData c
                                        on a.Creator=c.UserID
                                        INNER join Department d
                                        on c.DepartmentID=d.DepartmentID
                                        INNER JOIN MenuCategory e
                                        on b.CategoryID=e.CategoryID
                                        WHERE a.Deadline > CURRENT_TIMESTAMP and a.IsCancel=0
                                        AND a.IsOpenOtherDepartment = 1"));

            sqlstrPart2.Append(string.Format(@"select a.OrderID,a.ShopID,b.ShopName,
                                        (d.DepartmentName+'-'+c.UserName) as DepartmentUser,a.Deadline,e.CategoryName 
                                        FROM OrderHead a
                                        INNER JOIN ShopHead b 
                                        on a.ShopID=b.ShopID
                                        INNER JOIN UserData c
                                        on a.Creator=c.UserID
                                        INNER join Department d
                                        on c.DepartmentID=d.DepartmentID
                                        INNER JOIN MenuCategory e
                                        on b.CategoryID=e.CategoryID
                                        WHERE a.Deadline > CURRENT_TIMESTAMP and a.IsCancel=0
                                        AND a.IsOpenOtherDepartment = 0 
                                        and d.DepartmentID=(
									        SELECT Dep.DepartmentID 
									        FROM UserData UD
									        INNER JOIN Department Dep
									        on UD.DepartmentID=Dep.DepartmentID
									        WHERE UD.UserID=@userID
									    )"));

            para.Add(new SqlParameter("@userID", SqlDbType.VarChar, 10));
            para[i].Value = Session["userID"];
            #endregion

            #region 這是用來計算頁數的sql
            StringBuilder CountSqlStrPart1 = new StringBuilder();
            StringBuilder CountSqlStrPart2 = new StringBuilder();

            CountSqlStrPart1.Append(string.Format(@"SELECT COUNT(EffectiveOrder.OrderID) FROM(
                                        select a.OrderID,a.ShopID,b.ShopName,
                                        (d.DepartmentName+'-'+c.UserName) as DepartmentUser,a.Deadline,e.CategoryName
                                        FROM OrderHead a
                                        INNER JOIN ShopHead b 
                                        on a.ShopID=b.ShopID
                                        INNER JOIN UserData c
                                        on a.Creator=c.UserID
                                        INNER join Department d
                                        on c.DepartmentID=d.DepartmentID
                                        INNER JOIN MenuCategory e
                                        on b.CategoryID=e.CategoryID
                                        WHERE a.Deadline > CURRENT_TIMESTAMP and a.IsCancel=0
                                        AND a.IsOpenOtherDepartment = 1"));
            CountSqlStrPart2.Append(string.Format(@"select a.OrderID,a.ShopID,b.ShopName,
                                        (d.DepartmentName+'-'+c.UserName) as DepartmentUser,a.Deadline,e.CategoryName 
                                        FROM OrderHead a
                                        INNER JOIN ShopHead b 
                                        on a.ShopID=b.ShopID
                                        INNER JOIN UserData c
                                        on a.Creator=c.UserID
                                        INNER join Department d
                                        on c.DepartmentID=d.DepartmentID
                                        INNER JOIN MenuCategory e
                                        on b.CategoryID=e.CategoryID
                                        WHERE a.Deadline > CURRENT_TIMESTAMP and a.IsCancel=0
                                        AND a.IsOpenOtherDepartment = 0 
                                        and d.DepartmentID=(
									        SELECT Dep.DepartmentID 
									        FROM UserData UD
									        INNER JOIN Department Dep
									        on UD.DepartmentID=Dep.DepartmentID
									        WHERE UD.UserID=@userID
									    )"));
            #endregion

            CountPara.Add(new SqlParameter("@userID", SqlDbType.VarChar, 10));
            CountPara[i++].Value = Session["userID"];

            if (QueryString.ContainsKey("ShopName"))
            {
                sqlstrPart1.Append(" and b.ShopName like @ShopName ");
                sqlstrPart2.Append(" and b.ShopName like @ShopName ");
                para.Add(new SqlParameter("@ShopName", SqlDbType.NVarChar, 20));
                para[i].Value = QueryString["ShopName"] + "%";

                CountSqlStrPart1.Append(" and b.ShopName like @ShopName ");
                CountSqlStrPart2.Append(" and b.ShopName like @ShopName ");
                CountPara.Add(new SqlParameter("@ShopName", SqlDbType.NVarChar, 20));
                CountPara[i++].Value = QueryString["ShopName"] + "%";
            }
            if (QueryString.ContainsKey("Category"))
            {
                sqlstrPart1.Append(" and e.CategoryID=@CategoryID ");
                sqlstrPart2.Append(" and e.CategoryID=@CategoryID ");
                para.Add(new SqlParameter("@CategoryID", SqlDbType.TinyInt));
                para[i].Value = QueryString["Category"];

                CountSqlStrPart1.Append(" and e.CategoryID=@CategoryID ");
                CountSqlStrPart2.Append(" and e.CategoryID=@CategoryID ");
                CountPara.Add(new SqlParameter("@CategoryID", SqlDbType.TinyInt));
                CountPara[i++].Value = QueryString["Category"];
            }

            if (QueryString.ContainsKey("IsVegetarianism"))
            {
                sqlstrPart1.Append(" and b.IsVegetarianismShop=@IsVegetarianismShop ");
                sqlstrPart2.Append(" and b.IsVegetarianismShop=@IsVegetarianismShop ");
                para.Add(new SqlParameter("@IsVegetarianismShop", SqlDbType.Bit));
                para[i].Value = QueryString["IsVegetarianism"].Equals("是");

                CountSqlStrPart1.Append(" and b.IsVegetarianismShop=@IsVegetarianismShop ");
                CountSqlStrPart2.Append(" and b.IsVegetarianismShop=@IsVegetarianismShop ");
                CountPara.Add(new SqlParameter("@IsVegetarianismShop", SqlDbType.Bit));
                CountPara[i++].Value = QueryString["IsVegetarianism"].Equals("是");
            }

            if (QueryString.ContainsKey("Department"))
            {
                sqlstrPart1.Append(" and d.DepartmentName=@DepartmentName ");
                sqlstrPart2.Append(" and d.DepartmentName=@DepartmentName ");
                para.Add(new SqlParameter("@DepartmentName", SqlDbType.NVarChar, 20));
                para[i].Value = QueryString["Department"];

                CountSqlStrPart1.Append(" and d.DepartmentName=@DepartmentName ");
                CountSqlStrPart2.Append(" and d.DepartmentName=@DepartmentName ");
                CountPara.Add(new SqlParameter("@DepartmentName", SqlDbType.NVarChar, 20));
                CountPara[i++].Value = QueryString["Department"];
            }

            if (QueryString.ContainsKey("Creator"))
            {
                sqlstrPart1.Append(" and c.UserName like @UserName ");
                sqlstrPart2.Append(" and c.UserName like @UserName ");
                para.Add(new SqlParameter("@UserName", SqlDbType.NVarChar, 20));
                para[i].Value = QueryString["Creator"] + "%";

                CountSqlStrPart1.Append(" and c.UserName like @UserName ");
                CountSqlStrPart2.Append(" and c.UserName like @UserName ");
                CountPara.Add(new SqlParameter("@UserName", SqlDbType.NVarChar, 20));
                CountPara[i++].Value = QueryString["Creator"] + "%";
            }

            sqlstrPart1.Append(" UNION ")
                       .Append(sqlstrPart2)
                       .Append(string.Format(@") EffectiveOrder
                                        ORDER BY EffectiveOrder.Deadline ASC
                                        OFFSET {0} ROWS
                                        FETCH NEXT 10 ROWS ONLY", 10 * (pagenumber - 1)));

            CountSqlStrPart1.Append(" UNION ").Append(CountSqlStrPart2).Append(" ) EffectiveOrder");

            sqlstr = sqlstrPart1.ToString();

            ucPagination.CPage = pagenumber;
            ucPagination.CList = QueryString;

            SetEndPage(CountSqlStrPart1.ToString(), CountPara.ToArray() as SqlParameter[], pagenumber);
        }
        else
        {
            sqlstr = string.Format(@"
                                    SELECT * FROM(
                                    select a.OrderID,a.ShopID,b.ShopName,
                                    (d.DepartmentName+'-'+c.UserName) as DepartmentUser,a.Deadline,e.CategoryName
                                    FROM OrderHead a
                                    INNER JOIN ShopHead b 
                                    on a.ShopID=b.ShopID
                                    INNER JOIN UserData c
                                    on a.Creator=c.UserID
                                    INNER join Department d
                                    on c.DepartmentID=d.DepartmentID
                                    INNER JOIN MenuCategory e
                                    on b.CategoryID=e.CategoryID
                                    WHERE a.Deadline > CURRENT_TIMESTAMP and a.IsCancel=0
                                    AND a.IsOpenOtherDepartment = 1
  
                                    UNION
  
                                    select a.OrderID,a.ShopID,b.ShopName,
                                    (d.DepartmentName+'-'+c.UserName) as DepartmentUser,a.Deadline,e.CategoryName 
                                    FROM OrderHead a
                                    INNER JOIN ShopHead b 
                                    on a.ShopID=b.ShopID
                                    INNER JOIN UserData c
                                    on a.Creator=c.UserID
                                    INNER join Department d
                                    on c.DepartmentID=d.DepartmentID
                                    INNER JOIN MenuCategory e
                                    on b.CategoryID=e.CategoryID
                                    WHERE a.Deadline > CURRENT_TIMESTAMP and a.IsCancel=0
                                    AND a.IsOpenOtherDepartment = 0 
                                    and d.DepartmentID=(
									    SELECT Dep.DepartmentID 
									    FROM UserData UD
									    INNER JOIN Department Dep
									    on UD.DepartmentID=Dep.DepartmentID
									    WHERE UD.UserID=@userID
									)
                                ) EffectiveOrder
                                ORDER BY EffectiveOrder.Deadline ASC
                                OFFSET {0} ROWS
                                FETCH NEXT 10 ROWS ONLY", 10 * (pagenumber - 1));
            para = new List<SqlParameter>();
            para.Add(new SqlParameter("@userID", SqlDbType.VarChar, 10));
            para[0].Value = Session["userID"];

            CountPara = new List<SqlParameter>();
            CountPara.Add(new SqlParameter("@userID", SqlDbType.VarChar, 10));
            CountPara[0].Value = Session["userID"];

            SetEndPage(@"SELECT COUNT(EffectiveOrder.OrderID) FROM(
                        select a.OrderID,a.ShopID,b.ShopName,
                        (d.DepartmentName+'-'+c.UserName) as DepartmentUser,a.Deadline,e.CategoryName
                        FROM OrderHead a
                        INNER JOIN ShopHead b 
                        on a.ShopID=b.ShopID
                        INNER JOIN UserData c
                        on a.Creator=c.UserID
                        INNER join Department d
                        on c.DepartmentID=d.DepartmentID
                        INNER JOIN MenuCategory e
                        on b.CategoryID=e.CategoryID
                        WHERE a.Deadline > CURRENT_TIMESTAMP and a.IsCancel=0
                        AND a.IsOpenOtherDepartment = 1
  
                        UNION
  
                        select a.OrderID,a.ShopID,b.ShopName,
                        (d.DepartmentName+'-'+c.UserName) as DepartmentUser,a.Deadline,e.CategoryName 
                        FROM OrderHead a
                        INNER JOIN ShopHead b 
                        on a.ShopID=b.ShopID
                        INNER JOIN UserData c
                        on a.Creator=c.UserID
                        INNER join Department d
                        on c.DepartmentID=d.DepartmentID
                        INNER JOIN MenuCategory e
                        on b.CategoryID=e.CategoryID
                        WHERE a.Deadline > CURRENT_TIMESTAMP and a.IsCancel=0
                        AND a.IsOpenOtherDepartment = 0 
                        and d.DepartmentID=(
							SELECT Dep.DepartmentID 
							FROM UserData UD
							INNER JOIN Department Dep
							on UD.DepartmentID=Dep.DepartmentID
							WHERE UD.UserID=@UserID
						)
                    ) EffectiveOrder", CountPara.ToArray(), pagenumber);
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
                //if (QueryString.ContainsKey("ShopName") || QueryString.ContainsKey("Category") || QueryString.ContainsKey("IsVegetarianism") || QueryString.ContainsKey("Department"))
                //現在不管有沒有查詢條件都需要Parameters
                cmd.Parameters.AddRange(para.ToArray());

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    Repeater1.DataSource = dr;
                    Repeater1.DataBind();
                    if (!dr.HasRows)
                    {
                        AlertMessage("查無有效訂單!", "warning");
                        ucPagination.EPage = 1;
                    }
                }
            }
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
    protected void QueryCondition_Click(object sender, EventArgs e)
    {
        ucPagination.CPage = 1;
        if (!string.IsNullOrWhiteSpace(ShopName.Text.Trim()))
            QueryString.Add("ShopName", ShopName.Text.Trim());
        if (Category.SelectedValue != "不拘")
            QueryString.Add("Category", Category.SelectedItem.Value);
        if (IsVegetarianism.SelectedValue != "不拘")
            QueryString.Add("IsVegetarianism", IsVegetarianism.SelectedItem.Text);
        if (DepartMent.SelectedValue != "不拘")
            QueryString.Add("Department", DepartMent.SelectedItem.Text);
        if (!string.IsNullOrWhiteSpace(Creator.Text.Trim()))
            QueryString.Add("Creator", Creator.Text.Trim());
        Query(1);
    }
}