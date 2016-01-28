using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Label3.Text = User.Identity.Name.Split(new Char[] { '\\' })[1].Trim().ToUpper();
            InputUserID.Attributes["placeholder"] = "請輸入你帳號登入電腦的帳號";
            InputUserID.Attributes["required"] = "";
            InputUserID.Attributes["autofocus"] = "";

            InputUserPassword.Attributes["placeholder"] = "請輸入密碼";
            InputUserPassword.Attributes["required"] = "";
            InputUserPassword.Attributes["autofocus"] = "";

            Nickname.Attributes["placeholder"] = "請輸入網站中你的稱呼，中文英文都行";
            Nickname.Attributes["required"] = "";
            Nickname.Attributes["autofocus"] = "";

            if (Request.Cookies["userData"] != null && !string.IsNullOrWhiteSpace(Request.Cookies["userData"].Values["userID"]))
            {
                InputUserID.Text = Request.Cookies["userData"].Values["userID"];
                RememberMe.Checked = true;
            }
        }
    }
    protected void Register_Click(object sender, EventArgs e)
    {
        if (InputUserID.Text.Trim().Length > 10)
        {
            AlertMessage(@"alert('帳號長度不得超過10個字');");
            return;
        }

        if (Nickname.Text.Trim().Length > 10)
        {
            AlertMessage(@"alert('網頁中暱稱不得超過10個字');");
            return;
        }

        if (string.IsNullOrWhiteSpace(InputUserPassword.Text) || InputUserPassword.Text.Length > 50)
        {
            AlertMessage(@"alert('密碼不得爲0或超過50的字');");
            return;
        }

        if (isChinese(InputUserID.Text.Trim()))
        {
            AlertMessage(@"alert('帳號不得使用中文!(有中文帳號???)');");
            return;
        }

        if (Department.SelectedIndex == 0)
        {
            AlertMessage(@"alert('請選擇您的部門!');");
            return;
        }

        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT UserID FROM UserData where UserID=@UserID", conn))
            {
                try
                {
                    cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.VarChar, 10));
                    cmd.Parameters[0].Value = InputUserID.Text.Trim();

                    if (cmd.ExecuteScalar() != null)
                    {
                        AlertMessage("該帳號已經存在，如有疑問請聯繫管理員!");
                        return;
                    }

                    cmd.Transaction = conn.BeginTransaction("Order");
                    cmd.CommandText = @"INSERT INTO UserData (UserID,UserName,DepartmentID) VALUES (@UserID,@UserName,@DepartmentID)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@UserID", SqlDbType.VarChar, 10);
                    cmd.Parameters[0].Value = InputUserID.Text.Trim().ToUpper();
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 10);
                    cmd.Parameters[1].Value = Nickname.Text.Trim();
                    cmd.Parameters.Add(new SqlParameter("@DepartmentID", SqlDbType.TinyInt));
                    cmd.Parameters[2].Value = Department.SelectedValue;

                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.CommandText = @"INSERT INTO LoginPassword (UserID,Password) VALUES (@UserID,@Password)";
                    cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.VarChar, 10));
                    cmd.Parameters[0].Value = InputUserID.Text.Trim().ToUpper();
                    cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 50));
                    cmd.Parameters[1].Value = InputUserPassword.Text;

                    cmd.ExecuteNonQuery();

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
        string blockjs = null;
        if (Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7"))
            Response.Write("<script>alert('新增帳號成功，請重新登入');location.href='Login.aspx';</script>");
        else
            blockjs = @"swal({title:""新增帳號成功，請重新登入!"",text: ""三秒後重新登入"",type:""success"",timer: 3000,showConfirmButton: false},
                                function(){
                                    location.href='Login.aspx';
                                })";
        ShowAlert(blockjs);
    }

    protected void AddUser_Click(object sender, EventArgs e)
    {
        Register.Visible = true;
        Department.Visible = true;
        login.Visible = false;
        Nickname.Visible = true;
        AddUser.Visible = false;
        Label1.Visible = true;

        Department.Items.Add(new ListItem("請選擇您的部門"));

        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT DepartmentName,DepartmentID from Department", conn))
            {
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        Department.Items.Add(new ListItem(dr.GetString(0), dr.GetByte(1).ToString()));
                }
                cmd.Cancel();
            }
        }
    }

    /// <summary>
    /// 判斷字串中是否有中文字
    /// </summary>
    /// <param name="strChinese"></param>
    /// <returns></returns>
    public static bool isChinese(string strChinese)
    {
        bool bresult = true;
        int dRange = 0;
        int dstringmax = Convert.ToInt32("9fff", 16);
        int dstringmin = Convert.ToInt32("4e00", 16);
        for (int i = 0 ; i < strChinese.Length ; i++)
        {
            dRange = Convert.ToInt32(Convert.ToChar(strChinese.Substring(i, 1)));
            if (dRange >= dstringmin && dRange < dstringmax)
            {
                bresult = true;
                break;
            }
            else
                bresult = false;
        }
        return bresult;
    }
    protected void login_Click(object sender, EventArgs e)
    {
        if (RememberMe.Checked && Request.Cookies["userData"] == null)
        {
            HttpCookie UserIDCookie = new HttpCookie("userData");
            UserIDCookie.Values.Add("userID", InputUserID.Text.Trim());
            UserIDCookie.Expires = DateTime.Now.AddDays(7);
            Response.Cookies.Add(UserIDCookie);
        }


        //string userID = null;
        /*#region 檢查使用者是否在白名單之中
        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT userID FROM WhiteList Where userID=@userID", conn))
            {
                cmd.Parameters.Add("@userID", SqlDbType.VarChar, 10);
                cmd.Parameters[0].Value = InputUserID.Text.Trim().ToUpper();
                userID = cmd.ExecuteScalar() as string;
            }
        }
        if (userID == null)
        {
            Response.End();
        }
        #endregion*/

        string ResultuserID;

        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            string sqlstr = string.Format(@"SELECT b.UserID FROM UserData a
                                        INNER JOIN LoginPassword b
                                        on a.UserID=b.UserID
                                        WHERE b.UserID=@UserID AND b.Password=@Password");
            using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.VarChar, 10);
                cmd.Parameters[0].Value = InputUserID.Text.Trim().ToUpper();
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 50);
                cmd.Parameters[1].Value = InputUserPassword.Text;

                ResultuserID = cmd.ExecuteScalar() as string;
            }
        }
        if (ResultuserID != null)
        {
            Session.Add("userID", ResultuserID);
            Response.Redirect("Index.aspx", true);
        }
        else
            AlertMessage("alert('帳號或密碼錯誤!');");
    }
}