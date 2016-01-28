using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CreateOrder : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["userID"] == null)
        {
            Response.Redirect("ADLogin.aspx", true);
        }
        if (!IsPostBack)
        {
            if (PreviousPage != null && !IsCrossPagePostBack)//如果是Server.Transfer()轉來的網頁IsCrossPagePostBack就會是false
                ViewState["ShopID"] = PreviousPage.ShopID;
            else
            {
                Response.Redirect("~/Index.aspx");
                Response.End();
            }

            Remark.Attributes["placeholder"] = "在這裡輸入你要對訂購人說的話";

            for (int i = 0 ; i < 13 ; i++)
            {
                Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0')));
                Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0')));
            }
            for (int i = 13 ; i < 24 ; i++)
            {
                Hour.Items.Add(new ListItem(i.ToString()));
                Minute.Items.Add(new ListItem(i.ToString()));
            }
            for (int i = 25 ; i < 60 ; i++)
            {
                Minute.Items.Add(new ListItem(i.ToString()));
            }

            Hour.SelectedValue = DateTime.Now.Hour.ToString().PadLeft(2, '0');
            Minute.SelectedValue = DateTime.Now.Minute.ToString().PadLeft(2, '0');

            //查店名
            using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"SELECT ShopName FROM ShopHead where ShopID=@ShopID", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@ShopID", SqlDbType.UniqueIdentifier));
                    cmd.Parameters[0].Value = PreviousPage.ShopID;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dr.Read();
                        ShopName.Text = HttpUtility.HtmlEncode(dr.GetString(0));
                    }
                    cmd.Cancel();
                }
            }
        }
    }

    protected void AddOder_Click(object sender, EventArgs e)
    {
        DateTime Deadline;
        Datepicker.Text = Request.Form[Datepicker.UniqueID];
        if (!DateTime.TryParse(Datepicker.Text, out Deadline))
        {
            AlertMessage("請輸入正確日期格式");
            return;
        }


        Deadline = Deadline.Date + new TimeSpan(int.Parse(Hour.SelectedValue), int.Parse(Minute.SelectedValue), 0);
        if (DateTime.Now.CompareTo(Deadline) >= 0)
        {
            AlertMessage("收單時間不得在目前時間之前");
            return;
        }

        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            string insertOrder = @"insert into OrderHead (OrderID,Creator,Deadline,ShopID,IsOpenOtherDepartment,Remark,CreateDateTime,IsCancel)
                                values (@OrderID,@Creator,@Deadline,@ShopID,@IsOpenOtherDepartment,@Remark,@CreateDateTime,@IsCancel)";
            conn.Open();
            SqlCommand cmd = null;
            try
            {
                using (cmd = new SqlCommand(insertOrder, conn, conn.BeginTransaction("Order")))
                {
                    cmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.UniqueIdentifier));
                    cmd.Parameters[0].Value = Guid.NewGuid();
                    cmd.Parameters.Add(new SqlParameter("@Creator", SqlDbType.VarChar, 10));
                    cmd.Parameters[1].Value = Session["userID"];
                    cmd.Parameters.Add("@Deadline", SqlDbType.DateTime);
                    cmd.Parameters[2].Value = Deadline;
                    cmd.Parameters.Add("@ShopID", SqlDbType.UniqueIdentifier);
                    cmd.Parameters[3].Value = ViewState["ShopID"];
                    cmd.Parameters.Add("@IsOpenOtherDepartment", SqlDbType.Bit);
                    cmd.Parameters[4].Value = isOpenOtherDepartment.SelectedValue.Equals("1");
                    cmd.Parameters.Add("@Remark", SqlDbType.NVarChar, 50);
                    cmd.Parameters[5].Value = Remark.Text.Trim();
                    cmd.Parameters.Add("@CreateDateTime", SqlDbType.DateTime);
                    cmd.Parameters[6].Value = DateTime.Now;
                    cmd.Parameters.Add("@IsCancel", SqlDbType.Bit);
                    cmd.Parameters[7].Value = false;
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
            }
            catch (SqlException SQLEX)
            {
                cmd.Transaction.Rollback();
                AlertMessage(SQLEX.Message);
                ErrorLog(SQLEX);
            }
            catch (Exception EX)
            {
                cmd.Transaction.Rollback();
                AlertMessage(EX.Message);
                ErrorLog(EX);
            }

            string blockjs = null;
            if (Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7"))
                Response.Write("<script>alert('新增訂單成功!');location.href='Index.aspx';</script>");
            else
                blockjs = @"swal({title: ""新增訂單成功!"",type:""success""},function (){location.href='Index.aspx';})";
            ShowAlert(blockjs);
        }
    }
}