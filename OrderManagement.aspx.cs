using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class OrderManagement : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["userID"] == null)
        {
            Response.Redirect("ADLogin.aspx", true);
        }
        if (!IsPostBack)
        {
            if (PreviousPage != null && !IsCrossPagePostBack)
            {

                using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
                {

                    conn.Open();
                    string sqlstr = @"SELECT a.OrderDetailID,(e.DepartmentName+'-'+d.UserName) as DepartmentUser,
                                    b.ItemName,a.Quantity,(b.Price*a.Quantity) as Total,a.Remark,
                                    CASE 
	                                    WHEN c.IsCancel=1 THEN '開單者取消訂單'
	                                    WHEN a.IsCancel=1 THEN '訂購人取消訂單'
	                                    WHEN a.IsCheck=0 THEN '未付款'
	                                    WHEN a.IsCheck=1 THEN '已付款'
	                                    END as OrderStatus
                                    FROM OrderDetails a
                                    INNER JOIN ShopDetails b
                                    on a.ItemID=b.ItemID
                                    INNER JOIN OrderHead c
                                    on a.OrderID=c.OrderID
                                    INNER JOIN UserData d
                                    on a.Creator=d.UserID
                                    INNER JOIN Department e
                                    on d.DepartmentID=e.DepartmentID
                                    WHERE a.OrderID=@OrderID
                                    ORDER BY a.ItemID ASC ,a.CreateDateTime ASC";

                    using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                    {
                        #region 設定訂單明細
                        cmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.UniqueIdentifier));
                        cmd.Parameters[0].Value = PreviousPage.OrderID;
                        ViewState["OrderID"] = PreviousPage.OrderID;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                Repeater1.DataSource = dr;
                                Repeater1.DataBind();
                                LinkButton btnCancel = Repeater1.Controls[0].FindControl("Cancel") as LinkButton;
                                btnCancel.CommandName = PreviousPage.OrderID.ToString();
                            }
                            else
                            {
                                AlertMessage("目前尚未有人訂購!");
                            }
                        }
                        #endregion
                        cmd.Cancel();
                    }
                }
                CreatePic();
            }
            else
                Response.Redirect("~/Index.aspx");
        }
        CreatePic();
    }

    /// <summary>
    /// 建立統計
    /// </summary>
    private void CreatePic()
    {
        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            string sqlstr = string.Format(@"SELECT b.itemname as 品名,SUM(Quantity) as 數量 FROM OrderDetails a
                                                    INNER JOIN ShopDetails b
                                                    on a.ItemID=b.ItemID
                                                    WHERE OrderID=@OrderID AND a.IsCancel=0
                                                    GROUP BY b.itemname,a.ItemID");
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
            {
                cmd.Parameters.Add("@OrderID", SqlDbType.UniqueIdentifier);
                cmd.Parameters[0].Value = ViewState["OrderID"];

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    Chart1.DataSource = dr;
                    Chart1.DataBind();
                }
            }
        }
    }

    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            HtmlButton btnModifly = e.Item.FindControl("Modifly") as HtmlButton;
            LinkButton btnCancelOrder = e.Item.FindControl("CancelOrder") as LinkButton;

            Label lbOS = e.Item.FindControl("OrderStatus") as Label;
            lbOS.Font.Size = FontUnit.Medium;
            CheckBox cb = e.Item.FindControl("IsBill") as CheckBox;
            switch (lbOS.Text)
            {
                case "開單者取消訂單":
                case "訂購人取消訂單":
                    lbOS.CssClass = "label label-info";
                    cb.Enabled = false;
                    break;
                case "未付款":
                    lbOS.CssClass = "label label-danger";
                    break;
                case "已付款":
                    lbOS.CssClass = "label label-success";
                    cb.Enabled = false;
                    break;
            }
        }
        if (e.Item.ItemType == ListItemType.Footer)
        {
            Repeater Repeater = sender as Repeater;
            Label lbSum = e.Item.FindControl("Sum") as Label;
            Label lbTotal;
            decimal Sum = 0;
            foreach (RepeaterItem Item in Repeater.Items)
            {
                lbTotal = Item.Controls[0].FindControl("Total") as Label;
                Sum += decimal.Parse(lbTotal.Text);
            }
            lbSum.Text = string.Format("{0:N0}", Sum);
        }
    }

    /// <summary>
    /// 更新勾選的訂單為已付款
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void UpdateOrder_Click(object sender, EventArgs e)
    {
        bool CheckAnyCB = false;

        CheckBox cbBill;
        HiddenField hiddBill;

        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("UPDATE OrderDetails SET IsCheck=1 where OrderDetailID=@OrderDetailID", conn, conn.BeginTransaction("Order")))
            {
                try
                {
                    foreach (RepeaterItem Item in Repeater1.Items)
                    {
                        cmd.Parameters.Clear();
                        cbBill = Item.Controls[0].FindControl("IsBill") as CheckBox;
                        if (cbBill.Checked)
                        {
                            hiddBill = Item.Controls[0].FindControl("OrderDetailID") as HiddenField;
                            CheckAnyCB = true;

                            cmd.Parameters.Add(new SqlParameter("@OrderDetailID", SqlDbType.UniqueIdentifier));
                            cmd.Parameters[0].Value = Guid.Parse(hiddBill.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    cmd.Transaction.Commit();
                }
                catch (SqlException SE)
                {
                    cmd.Transaction.Rollback();
                    AlertMessage(SE.Message);
                    ErrorLog(SE);
                }
                catch (Exception EX)
                {
                    cmd.Transaction.Rollback();
                    AlertMessage(EX.Message);
                    ErrorLog(EX);
                }
            }
        }

        if (!CheckAnyCB)
            AlertMessage("請至少勾選一個訂單才能更新!", "warning");
        else
        {
            UpdatePanel();
            AlertMessage("更新成功!", "success");
        }
    }

    /// <summary>
    /// 勾選完後更新畫面
    /// </summary>
    private void UpdatePanel()
    {
        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            string sqlstr = @"SELECT a.OrderDetailID,(e.DepartmentName+'-'+d.UserName) as DepartmentUser,
                                    b.ItemName,a.Quantity,(b.Price*a.Quantity) as Total,a.Remark,
                                    CASE 
	                                    WHEN c.IsCancel=1 THEN '開單者取消訂單'
	                                    WHEN a.IsCancel=1 THEN '訂購人取消訂單'
	                                    WHEN a.IsCheck=0 THEN '未付款'
	                                    WHEN a.IsCheck=1 THEN '已付款'
	                                    END as OrderStatus
                                    FROM OrderDetails a
                                    INNER JOIN ShopDetails b
                                    on a.ItemID=b.ItemID
                                    INNER JOIN OrderHead c
                                    on a.OrderID=c.OrderID
                                    INNER JOIN UserData d
                                    on a.Creator=d.UserID
                                    INNER JOIN Department e
                                    on d.DepartmentID=e.DepartmentID
                                    WHERE a.OrderID=@OrderID
                                    ORDER BY a.ItemID ASC,a.CreateDateTime ASC";

            using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.UniqueIdentifier));
                cmd.Parameters[0].Value = ViewState["OrderID"];
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        Repeater1.DataSource = dr;
                        Repeater1.DataBind();
                        LinkButton btnCancel = Repeater1.Controls[0].FindControl("Cancel") as LinkButton;
                        btnCancel.CommandName = ViewState["OrderID"].ToString();
                    }
                }
                cmd.Cancel();
            }
        }
    }

    /// <summary>
    /// 取消該筆訂單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void CancelOrder_Click(object sender, EventArgs e)
    {
        LinkButton btnCancel = sender as LinkButton;
        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"UPDATE OrderHead SET IsCancel=1 WHERE OrderID=@OrderID", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.UniqueIdentifier));
                    cmd.Parameters[0].Value = Guid.Parse(btnCancel.CommandName);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException SE)
            {
                AlertMessage(SE.Message);
                ErrorLog(SE);
            }
            catch (Exception EX)
            {
                AlertMessage(EX.Message);
                ErrorLog(EX);
            }
            //Response.Write("<script>alert('已成功取消該訂單!');location.href='Index.aspx';</script>");
            string blockjs = null;
            if (Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7"))
                Response.Write("<script>alert('已成功取消該訂單!');location.href='Index.aspx';</script>");
            else
                blockjs = @"swal({title:""已成功取消該訂單!"",text: ""三秒後回到首頁"",type:""success"",timer: 3000,showConfirmButton: false},
                                function(){
                                    location.href='Index.aspx';
                                })";
            ShowAlert(blockjs);
        }
    }

    /// <summary>
    /// 全選訂單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SelectAll_Click(object sender, EventArgs e)
    {
        CheckBox cb;
        foreach (RepeaterItem Item in Repeater1.Items)
        {
            cb = Item.Controls[0].FindControl("IsBill") as CheckBox;

            if (cb.Enabled)
            {
                cb.Checked = true;
            }
        }
    }
}