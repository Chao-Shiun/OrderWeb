using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;

public partial class RegisterShop : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["userID"] == null)
        {
            Response.Redirect("ADLogin.aspx", true);
        }
        if (!IsPostBack)
        {
            ShopName.Attributes["placeholder"] = "在此輸入店家名稱，不可超過20個字";
        }
    }

    protected void submit_Click(object sender, EventArgs e)
    {
        String path = Server.MapPath("~/Menu/");

        if (ShopName.Text.Trim().Length == 0)
        {
            AlertMessage("請記得輸入店家名稱!");
            return;
        }

        if (FileUpload1.HasFile)
        {
            FileInfo file = new FileInfo(path + FileUpload1.FileName);
            if (file.Exists)
            {
                AlertMessage("第一個上傳菜單已經有相同名稱的檔名，麻煩請修改檔名再上傳");
                return;
            }
        }
        if (FileUpload2.HasFile)
        {
            FileInfo file = new FileInfo(path + FileUpload2.FileName);
            if (file.Exists)
            {
                AlertMessage("第二個上傳菜單已經有相同名稱的檔名，麻煩請修改檔名再上傳");
                return;
            }
        }

        Guid ShopID = Guid.NewGuid();
        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            string sqlstr = @"insert into ShopHead (ShopID,ShopName,CreateDateTime,IsCancel,CategoryID,IsVegetarianismShop) 
                            values (@ShopID,@ShopName,@CreateDateTime,@IsCancel,@CategoryID,@IsVegetarianismShop)";
            using (SqlCommand cmd = new SqlCommand(sqlstr, conn, conn.BeginTransaction("Order")))
            {
                try
                {
                    cmd.Parameters.Add(new SqlParameter("@ShopID", SqlDbType.UniqueIdentifier));
                    cmd.Parameters[0].Value = ShopID;
                    cmd.Parameters.Add(new SqlParameter("@ShopName", SqlDbType.NVarChar, 20));
                    cmd.Parameters[1].Value = ShopName.Text.Trim();
                    cmd.Parameters.Add(new SqlParameter("@CreateDateTime", SqlDbType.DateTime));
                    cmd.Parameters[2].Value = DateTime.Now;
                    cmd.Parameters.Add(new SqlParameter("@IsCancel", SqlDbType.Bit));
                    cmd.Parameters[3].Value = false;
                    cmd.Parameters.Add(new SqlParameter("@CategoryID", SqlDbType.TinyInt));
                    cmd.Parameters[4].Value = MenuType.SelectedValue;
                    cmd.Parameters.Add(new SqlParameter("@IsVegetarianismShop", SqlDbType.Bit));
                    cmd.Parameters[5].Value = Convert.ToBoolean(IsVegetarianismShop.SelectedValue);
                    cmd.ExecuteNonQuery();

                    if (FileUpload1.HasFile)
                    {
                        cmd.CommandText = "insert into MenuImg (ShopID,FileName) values (@ShopID,@FileName)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@ShopID", SqlDbType.UniqueIdentifier));
                        cmd.Parameters[0].Value = ShopID;
                        cmd.Parameters.Add(new SqlParameter("@FileName", SqlDbType.NVarChar, 20));
                        cmd.Parameters[1].Value = FileUpload1.FileName;
                        cmd.ExecuteNonQuery();
                        FileUpload1.SaveAs(Request.PhysicalApplicationPath + "\\Menu\\" + FileUpload1.FileName);
                    }
                    if (FileUpload2.HasFile)
                    {
                        cmd.CommandText = "insert into MenuImg (ShopID,FileName) values (@ShopID,@FileName)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@ShopID", SqlDbType.UniqueIdentifier));
                        cmd.Parameters[0].Value = ShopID;
                        cmd.Parameters.Add(new SqlParameter("@FileName", SqlDbType.NVarChar, 20));
                        cmd.Parameters[1].Value = FileUpload2.FileName;
                        cmd.ExecuteNonQuery();
                        FileUpload2.SaveAs(Request.PhysicalApplicationPath + "\\Menu\\" + FileUpload2.FileName);
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
                finally
                {
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
                }
                string blockjs = null;
                if (Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7"))
                {
                    Response.Write("<script>alert('新增店家「" + HttpUtility.HtmlEncode(ShopName.Text) + "」成功');location.href='ShopList.aspx';</script>");
                }
                else
                    blockjs = @"swal({title:""新增店家「" + HttpUtility.HtmlEncode(ShopName.Text.Trim()) + @"」成功"",text: ""三秒後進入店家列表"",type:""success"",timer: 3000,showConfirmButton: false},
                                function(){
                                    location.href='ShopList.aspx';
                                })";
                ShowAlert(blockjs);
            }
        }
    }
}