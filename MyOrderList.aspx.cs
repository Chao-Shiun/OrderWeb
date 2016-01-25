using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MyOrderList : BasePage
{
    Dictionary<string, string> QueryString = new Dictionary<string, string>();

    public Guid OrderID
    {
        get;
        set;
    }

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

            if (!string.IsNullOrWhiteSpace(Request["SDate"]) && !string.IsNullOrWhiteSpace(Request["EDate"]))
            {
                QueryString.Add("StartDate", Request["SDate"]);
                QueryString.Add("EndDate", Request["EDate"]);
                StartDate.Text = Request["SDate"];
                EndDate.Text = Request["EDate"];
            }
            int PageNumber;
            if (string.IsNullOrWhiteSpace(Request["Page"]) || !int.TryParse(Request["Page"], out PageNumber))
                PageNumber = 1;
            ucPagination.CPage = PageNumber;
            Query(PageNumber);
        }
    }

    protected void Query_Click(object sender, EventArgs e)
    {
        ucPagination.CPage = 1;
        if (!string.IsNullOrWhiteSpace(ShopName.Text.Trim()))
            QueryString.Add("ShopName", ShopName.Text.Trim());
        if (Category.SelectedValue != "不拘")
            QueryString.Add("Category", Category.SelectedItem.Text);
        if (!string.IsNullOrWhiteSpace(Request.Form[StartDate.UniqueID]) && !string.IsNullOrWhiteSpace(Request.Form[EndDate.UniqueID]))
        {
            DateTime sDate = DateTime.Parse(Request.Form[StartDate.UniqueID]);
            DateTime eDate = DateTime.Parse(Request.Form[EndDate.UniqueID]);

            QueryString.Add("StartDate", sDate.ToString());
            QueryString.Add("EndDate", eDate.ToString());
        }
        Query(1);
    }
    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        OrderID = Guid.Parse(e.CommandName);
        Server.Transfer("OrderManagement.aspx");
    }

    /// <summary>
    /// 查詢網頁內容
    /// </summary>
    /// <param name="Pagenumber">第幾頁</param>
    /// <param name="QueryString">網址的參數</param>
    private void Query(int Pagenumber)
    {
        string sqlstr = @"SELECT GroupOrder.OrderID,SH.ShopName,
                                        OH.Deadline,MC.CategoryName,GroupOrder.OrderSum FROM (
	                                        SELECT a.OrderID,COUNT(b.OrderDetailID) AS OrderSum
	                                        FROM OrderHead a
	                                        INNER JOIN OrderDetails b
	                                        on a.OrderID=b.OrderID and a.Creator=@Creator
	                                        GROUP BY a.OrderID
                                        ) GroupOrder
                                        INNER JOIN OrderHead OH
                                        ON GroupOrder.OrderID=OH.OrderID
                                        INNER JOIN ShopHead SH
                                        ON OH.ShopID=SH.ShopID
                                        INNER JOIN MenuCategory MC
                                        ON SH.CategoryID=MC.CategoryID 
                                        where 1=1";

        string Countsqlstr = @"SELECT COUNT(a.OrderID) 
                            FROM OrderHead a
                            INNER JOIN ShopHead b
                            ON a.ShopID=b.ShopID and a.Creator=@Creator
                            INNER JOIN MenuCategory c
                            ON b.CategoryID=c.CategoryID
                            WHERE 1=1";

        List<SqlParameter> Para = new List<SqlParameter>();
        List<SqlParameter> CountPara = new List<SqlParameter>();

        Para.Add(new SqlParameter("@Creator", SqlDbType.VarChar, 10));
        Para[0].Value = Session["userID"];

        CountPara.Add(new SqlParameter("@Creator", SqlDbType.VarChar, 10));
        CountPara[0].Value = Session["userID"];

        #region 設定查詢條件
        if (QueryString.Count == 0)//如果沒有任何查詢條件
        {
            SetEndPage(Countsqlstr, CountPara.ToArray(), Pagenumber);
        }
        else
        {
            int i = 1;
            StringBuilder QuerySB = new StringBuilder(sqlstr);
            StringBuilder CountSB = new StringBuilder(Countsqlstr);
            if (QueryString.ContainsKey("ShopName"))
            {
                QuerySB.Append(" and SH.ShopName like @ShopName");
                CountSB.Append(" and b.ShopName like @ShopName");
                Para.Add(new SqlParameter("@ShopName", SqlDbType.NVarChar, 20));
                Para[i].Value = QueryString["ShopName"] + "%";
                CountPara.Add(new SqlParameter("@ShopName", SqlDbType.NVarChar, 20));
                CountPara[i++].Value = QueryString["ShopName"] + "%";
            }

            if (QueryString.ContainsKey("Category"))
            {
                QuerySB.Append(" and MC.CategoryName=@CategoryName");
                CountSB.Append(" and c.CategoryName=@CategoryName");
                Para.Add(new SqlParameter("@CategoryName", SqlDbType.NVarChar, 5));
                Para[i].Value = QueryString["Category"];
                CountPara.Add(new SqlParameter("@CategoryName", SqlDbType.NVarChar, 5));
                CountPara[i++].Value = QueryString["Category"];
            }
            if (QueryString.ContainsKey("StartDate") && QueryString.ContainsKey("EndDate"))
            {
                DateTime sdate = DateTime.Parse(QueryString["StartDate"]);
                DateTime edate = DateTime.Parse(QueryString["EndDate"]);

                QuerySB.Append(" and OH.Deadline between @StartDate and @EndDate");
                CountSB.Append(" and a.Deadline between @StartDate and @EndDate");

                Para.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
                Para.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));
                CountPara.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
                CountPara.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));


                if (sdate.CompareTo(edate) > 0)
                {
                    Para[i].Value = edate;
                    CountPara[i++].Value = edate;
                    Para[i].Value = sdate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    CountPara[i].Value = sdate.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                else
                {
                    Para[i].Value = sdate;
                    CountPara[i++].Value = sdate;
                    Para[i].Value = edate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    CountPara[i].Value = edate.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }
            sqlstr = QuerySB.ToString();
            Countsqlstr = CountSB.ToString();
            SetEndPage(Countsqlstr, CountPara.ToArray(), Pagenumber);
        }
        #endregion

        Control ctl = Page.FindControl(ucPagination.UniqueID);
        Type ctlType = ctl.GetType();
        MethodInfo ctlMethod = ctlType.GetMethod("Initialization");
        ctlMethod.Invoke(ctl, null);

        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
            {
                conn.Open();
                cmd.Parameters.AddRange(Para.ToArray());

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        Repeater1.DataSource = dr;
                        Repeater1.DataBind();
                    }
                    else
                    {
                        Repeater1.DataSource = null;
                        Repeater1.DataBind();
                        AlertMessage("查無開單紀錄!", "warning");
                        ucPagination.EPage = 1;
                    }
                    dr.Close();
                }
            }
            conn.Close();
        }
    }
    /// <summary>
    /// 設定該查詢有多少分頁
    /// </summary>
    /// <param name="sqlstr">查詢字串</param>
    /// <param name="para">條件參數</param>
    /// <param name="Pagenumber">目前頁數</param>
    private void SetEndPage(string sqlstr, SqlParameter[] para, int pagenumber)
    {
        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
            {
                cmd.Parameters.AddRange(para);

                int count = (int)cmd.ExecuteScalar();
                count = (count % 10 == 0) ? count / 10 : (count / 10) + 1;
                ucPagination.EPage = count;
                if (count <= pagenumber)
                    ucPagination.CPage = count;
            }
            conn.Close();
        }
    }
}