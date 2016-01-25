using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Index : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["userID"] == null)
        {
            Response.Redirect("ADLogin.aspx");
        }
        if (!IsPostBack)
        {
            using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
            {
                conn.Open();

                string sqlstr = @"SELECT top 5 b.Category,a.Title,a.Context FROM WebAnnouncement a
                                INNER JOIN WebAnnouncementCategory b
                                on a.AnnouncementCategory=b.AnnouncementCategory
                                ORDER BY a.CreateDate DESC";

                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        Repeater1.DataSource = dr;
                        Repeater1.DataBind();
                        dr.Close();
                    }
                    cmd.Cancel();

                    cmd.CommandText = @"SELECT a.OrderID,a.OrderDetailID,b.ItemName,a.Quantity,(a.Quantity*b.Price) as Total,a.CreateDateTime,
                                    CASE 
	                                    WHEN c.IsCancel=1 THEN '開單者取消訂單'
	                                    WHEN a.IsCancel=1 THEN '您取消該筆訂單'
	                                    WHEN a.IsCheck=0 THEN '未付款'
	                                    WHEN a.IsCheck=1 THEN '已付款'
	                                    END as OrderStatus
                                    FROM OrderDetails a
                                    INNER JOIN ShopDetails b
                                    on a.ItemID=b.ItemID
                                    INNER JOIN OrderHead c
                                    on a.OrderID=c.OrderID
                                    WHERE a.Creator=@Creator AND a.CreateDateTime BETWEEN @SevenDaysAgo AND CURRENT_TIMESTAMP
                                    ORDER BY a.CreateDateTime DESC";
                    cmd.Parameters.Add("@Creator", SqlDbType.VarChar, 10);
                    cmd.Parameters[0].Value = Session["userID"];
                    cmd.Parameters.Add("@SevenDaysAgo", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.Now.AddDays(-7);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            Repeater2.DataSource = dr;
                            Repeater2.DataBind();
                            OrderMessage.Text = "最近七日的訂單";
                            OrderMessage.CssClass = "label label-info";
                            OrderMessage.Font.Size = FontUnit.Large;
                        }
                        else
                        {
                            OrderMessage.Text = "查無七日內訂單";
                            OrderMessage.CssClass = "label label-warning";
                            OrderMessage.Font.Size = FontUnit.Large;
                        }
                        dr.Close();
                    }
                }
                conn.Close();
            }
        }
    }
    protected void Repeater2_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            HtmlButton btnModifly = e.Item.FindControl("Modifly") as HtmlButton;
            LinkButton btnCancelOrder = e.Item.FindControl("CancelOrder") as LinkButton;

            Label lbOS = e.Item.FindControl("OrderStatus") as Label;
            lbOS.Font.Size = FontUnit.Medium;
            switch (lbOS.Text)
            {
                case "開單者取消訂單":
                case "您取消該筆訂單":
                    lbOS.CssClass = "label label-info";
                    btnModifly.Attributes["disabled"] = "disabled";
                    break;
                case "未付款"://未付款有分收單前跟收單後，收單前的未付款才能編輯
                    lbOS.CssClass = "label label-danger";
                    using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT a.Deadline FROM OrderHead a WHERE a.OrderID=@OrderID", conn))
                        {
                            cmd.Parameters.Add("@OrderID", SqlDbType.UniqueIdentifier);
                            cmd.Parameters[0].Value = Guid.Parse(btnCancelOrder.CommandName);

                            if (DateTime.Now.CompareTo((DateTime)cmd.ExecuteScalar()) >= 0)
                                btnModifly.Attributes["disabled"] = "disabled";
                        }
                        conn.Close();
                    }
                    break;
                case "已付款":
                    lbOS.CssClass = "label label-success";
                    btnModifly.Attributes["disabled"] = "disabled";
                    break;
            }
        }
    }
    protected void CancelOrder_Click(object sender, EventArgs e)
    {
        LinkButton btn = sender as LinkButton;
        string blockjs = null;
        string sqlstr = @"SELECT a.IsCancel,a.Deadline FROM OrderHead a
                          INNER JOIN OrderDetails b
                          on a.OrderID=b.OrderID
                          WHERE a.OrderID=@OrderID AND b.OrderDetailID=@OrderDetailID";
        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.UniqueIdentifier));
                cmd.Parameters[0].Value = Guid.Parse(btn.CommandName);
                cmd.Parameters.Add(new SqlParameter("@OrderDetailID", SqlDbType.UniqueIdentifier));
                cmd.Parameters[1].Value = Guid.Parse(btn.CommandArgument);


                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    dr.Read();
                    
                    if (dr.GetBoolean(0))
                    {
                        dr.Close();
                        conn.Close();
                        if (Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7"))
                            Response.Write("<script>alert('開單者已取消訂單!');location.href='Index.aspx';</script>");
                        else
                            blockjs = @"swal({title: ""開單者已取消訂單!"",""info""},function (){location.href='Index.aspx';})";
                        ShowAlert(blockjs);
                        return;
                    }
                    if (DateTime.Now.CompareTo(dr.GetDateTime(1)) >= 0)
                    {
                        dr.Close();
                        conn.Close();
                        if (Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7"))
                        Response.Write("<script>alert('已超過收單時間，無法取消訂單，請直接聯繫開單人!');location.href='Index.aspx';</script>");
                        else
                            blockjs = @"swal({title: ""已超過收單時間，無法取消訂單，請直接聯繫開單人!"",""info""},function (){location.href='Index.aspx';})";
                        ShowAlert(blockjs);
                        return;
                    }
                    dr.Close();
                }
                cmd.Parameters.Clear();

                cmd.CommandText = @"UPDATE OrderDetails SET IsCancel=1 WHERE OrderDetailID=@OrderDetailID";
                cmd.Parameters.Add("@OrderDetailID", SqlDbType.UniqueIdentifier);
                cmd.Parameters[0].Value = Guid.Parse(btn.CommandArgument);

                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
        if (Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7"))
            Response.Write("<script>alert('訂單已成功取消!');location.href='Index.aspx';</script>");
        else
            blockjs = @"swal({title: ""訂單已成功取消!"",type:""success""},function (){location.href='Index.aspx';})";
        ShowAlert(blockjs);
    }
}