using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class ADLogin1 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            //string[] cADName = wp.Identity.Name.Split(new Char[] { '\\' });
            string cADName = User.Identity.Name.Split(new Char[] { '\\' })[1].Trim().ToUpper();

            //如果帳號不是這三個開頭的就轉去註冊頁面
            if (!cADName.StartsWith("TA") && !cADName.StartsWith("itap") && !cADName.StartsWith("TOO"))
            {
                Response.Redirect("Login.aspx", true);
            }

            
            string userID = null;
            /*#region 檢查使用者是否在白名單之中
             * using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT userID FROM WhiteList Where userID=@userID", conn))
                {
                    cmd.Parameters.Add("@userID", SqlDbType.VarChar, 10);
                    cmd.Parameters[0].Value = cADName;
                    userID = (string)cmd.ExecuteScalar();
                }
                conn.Close();
            }
            if (userID == null)
            {
                Response.End();
            }
            #endregion*/

            #region 檢查該使用者存不存在，存在就進入首頁，不存在就註冊
            using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT UserID FROM UserData WHERE UserID=@UserID", conn))
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.VarChar, 10);
                    cmd.Parameters[0].Value = cADName;
                    userID = cmd.ExecuteScalar() as string;
                    cmd.Cancel();
                }
                conn.Close();
            }
            if (userID != null)
            {
                Session.Add("userID", cADName);
                Response.Redirect("Index.aspx", true);
            }
            else
            {
                ViewState.Add("userID", cADName);
                NickName.Attributes["placeholder"] = "可以輸入任何名稱，中英文都可以，只要別人認得就行";
                NickName.Attributes["required"] = "";
                NickName.Attributes["autofocus"] = "";

                Department.Items.Add(new ListItem("請選擇您的部門"));

                using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT DepartmentName,DepartmentID FROM Department", conn))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                                Department.Items.Add(new ListItem(dr.GetString(0), dr.GetByte(1).ToString()));
                            dr.Close();
                        }
                        cmd.Cancel();
                    }
                    conn.Close();
                }
            }
            #endregion
        }
    }

    private void Alert(string ErrorMessage)
    {
        string blockjs = @"alert('" + ErrorMessage + "');";
        if (ScriptManager.GetCurrent(this.Page) == null)
            Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "buttonStartup", blockjs, true);
        else
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "buttonStartupBySM", blockjs, true);
    }

    protected void Register_Click(object sender, EventArgs e)
    {
        if (NickName.Text.Trim().Length > 10)
        {
            Alert("暱稱不得大於10個字!");
            return;
        }
        if (Department.SelectedIndex == 0)
        {
            Alert("請選擇您的部門!");
            return;
        }

        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("INSERT INTO UserData (UserID,UserName,DepartmentID) VALUES (@UserID,@UserName,@DepartmentID)", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.VarChar, 10));
                cmd.Parameters[0].Value = ViewState["userID"];
                cmd.Parameters.Add(new SqlParameter("@UserName", SqlDbType.NVarChar, 10));
                cmd.Parameters[1].Value = NickName.Text.Trim();
                cmd.Parameters.Add("@DepartmentID", SqlDbType.TinyInt);
                cmd.Parameters[2].Value = Department.SelectedValue;
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }

        Session.Add("userID", ViewState["userID"].ToString());
        Response.Redirect("Index.aspx", true);
    }
    protected override void Render(HtmlTextWriter writer)
    {
        ClientScript.RegisterForEventValidation(Register.UniqueID);
        base.Render(writer);
    }
}