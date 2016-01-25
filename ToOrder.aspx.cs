using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;

public partial class ToOrder : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Remark.Attributes["placeholder"] = "餐點備註(餐點大中小、冷熱、甜度、你想跟開單人說的話、etc. 請Key在這，字數上限20)";
            AddItem.Attributes["placeholder"] = "請輸入餐點名稱";
            AddPrice.Attributes["placeholder"] = "請輸入品項金額";

            if (PreviousPage != null && !IsCrossPagePostBack)
            {
                #region 判斷送出時是否已經超過收單時間，順便設定頁面
                using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
                {
                    conn.Open();
                    DateTime? Deadline = null;
                    using (SqlCommand cmd = new SqlCommand("SELECT Deadline,Remark FROM OrderHead WHERE OrderID=@OrderID and IsCancel=@IsCancel and Deadline > CURRENT_TIMESTAMP", conn))
                    {
                        cmd.Parameters.Add("@OrderID", SqlDbType.UniqueIdentifier);
                        cmd.Parameters[0].Value = PreviousPage.OrderID;
                        cmd.Parameters.Add("@IsCancel", SqlDbType.Bit);
                        cmd.Parameters[1].Value = false;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                Deadline = dr.GetDateTime(0);
                                OrderRemark.Text = HttpUtility.HtmlEncode(dr.GetString(1));
                                OrderRemark.Font.Size = FontUnit.Large;
                            }
                            dr.Close();
                        }
                    }
                    conn.Close();
                    if (Deadline == null)
                    {
                        string blockjs=null;
                        if (Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7"))
                        {
                            Response.Write("<script>alert('此訂單已經超過收單時間，請直接找開單人訂購!');location.href='Index.aspx';</script>");
                            return;
                        }
                        else
                            blockjs = @"swal({title: ""此訂單已經超過收單時間，請直接找開單人訂購!"",""warning""},function (){location.href='Index.aspx';})";
                        ShowAlert(blockjs);
                    }
                }
                #endregion

                ViewState["ShopID"] = PreviousPage.ShopID;
                ViewState["OrderID"] = PreviousPage.OrderID;
                ListItem SelectItem = new ListItem("請選擇品項", "請選擇品項");
                FoodItems.Items.Add(SelectItem);

                #region 建立該店家品項選單
                using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT ItemName,ItemID FROM ShopDetails a WHERE a.ShopID=@ShopID and a.IsDisable=0", conn))
                    {
                        cmd.Parameters.Add("ShopID", SqlDbType.UniqueIdentifier);
                        cmd.Parameters[0].Value = PreviousPage.ShopID;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                                FoodItems.Items.Add(new ListItem(HttpUtility.HtmlEncode(dr.GetString(0)), dr.GetGuid(1).ToString()));
                            dr.Close();
                        }
                        cmd.Cancel();
                    }
                    conn.Close();
                }
                #endregion

                ListItem additem = new ListItem("新增品項", "新增品項");
                FoodItems.Items.Add(additem);

                for (int i = 1 ; i <= 50 ; i++)
                    Quantity.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
            else
            {
                Response.Redirect("Index.aspx", true);
            }
        }
        //每次載入網頁都一定要重建統計圖
        CreatePic();
    }

    /// <summary>
    /// 建立統計圖
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
                    dr.Close();
                }
            }
            conn.Close();
        }
    }
    /// <summary>
    /// 判斷送出時是否已經超過收單時間
    /// </summary>
    private void CheckOrderDealLine()
    {
        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            conn.Open();
            DateTime Deadline;
            using (SqlCommand cmd = new SqlCommand("SELECT Deadline FROM OrderHead WHERE OrderID=@OrderID AND IsCancel=@IsCancel and Deadline > CURRENT_TIMESTAMP", conn))
            {
                cmd.Parameters.Add("@OrderID", SqlDbType.UniqueIdentifier);
                cmd.Parameters[0].Value = ViewState["OrderID"];
                cmd.Parameters.Add("@IsCancel", SqlDbType.Bit);
                cmd.Parameters[1].Value = false;
                Deadline = (DateTime)cmd.ExecuteScalar();
            }
            conn.Close();

            string blockjs;

            if (Deadline == null)
            {
                if (Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7"))
                {
                    Response.Write("<script>alert('查無該筆訂單紀錄，可能已取消!');location.href='Index.aspx';</script>");
                    return;
                }
                else
                    blockjs = @"swal({title: ""查無該筆訂單紀錄，可能已取消!"",""info""},function (){location.href='Index.aspx';})";
                ShowAlert(blockjs);
                return;
            }

            if (DateTime.Now.CompareTo(Deadline) >= 0)
            {
                if (Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7"))
                {
                    Response.Write("<script>alert('此訂單已經超過收單時間，請直接找開單人訂購!');location.href='Index.aspx';</script>");
                    return;
                }
                else
                {
                    blockjs = @"swal({title: ""此訂單已經超過收單時間，請直接找開單人訂購!"",""warning""},function (){location.href='Index.aspx';})";
                    ShowAlert(blockjs);
                }
            }
        }
    }

    protected void AddOrder_Click(object sender, EventArgs e)
    {
        //這邊要判斷是否超過訂單截止時間
        //如果使用者選擇品項就直接新增
        //如果不是就要先新增品項再新增訂單明細
        //需要檢查是否已經有該品項，必須鎖定資料庫確保沒有其他人同時讀取寫入

        if (FoodItems.SelectedIndex == 0)
        {
            AlertMessage("請選擇品項或新增品項!");
            return;
        }

        if ((FoodItems.Items.Count - 1 == FoodItems.SelectedIndex) && string.IsNullOrWhiteSpace(AddItem.Text))
        {
            AlertMessage("如果要新增品項，請記得輸入品名!");
            return;
        }

        if ((FoodItems.Items.Count - 1 == FoodItems.SelectedIndex) && string.IsNullOrWhiteSpace(AddPrice.Text))
        {
            AlertMessage("如果要新增品項，請記得輸入金額!");
            return;
        }

        CheckOrderDealLine();

        using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
        {
            string insertsqlstr = @"insert into OrderDetails (OrderID,ItemID,OrderDetailID,Creator,Quantity,IsCancel,IsCheck,CreateDateTime,Remark) values 
                              (@OrderID,@ItemID,@OrderDetailID,@Creator,@Quantity,@IsCancel,@IsCheck,@CreateDateTime,@Remark)";

            Guid? itemID = null;
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(@"SELECT a.ItemID FROM ShopDetails a where ShopID=@ShopID and ItemName=@ItemName", conn, conn.BeginTransaction(IsolationLevel.ReadCommitted)))
            {
                try
                {

                    if (FoodItems.SelectedIndex == FoodItems.Items.Count - 1)
                    {
                        cmd.Parameters.Add("@ShopID", SqlDbType.UniqueIdentifier);
                        cmd.Parameters[0].Value = ViewState["ShopID"];
                        cmd.Parameters.Add("@ItemName", SqlDbType.NVarChar, 20);
                        cmd.Parameters[1].Value = AddItem.Text.Trim();

                        itemID = GetGuidFromDb(cmd.ExecuteScalar());
                        if (itemID == null)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "insert into ShopDetails(ShopID,ItemID,ItemName,Price,IsDisable) values (@ShopID,@ItemID,@ItemName,@Price,@IsDisable)";
                            cmd.Parameters.Add("@ShopID", SqlDbType.UniqueIdentifier);
                            cmd.Parameters[0].Value = ViewState["ShopID"];
                            cmd.Parameters.Add("@ItemID", SqlDbType.UniqueIdentifier);
                            cmd.Parameters[1].Value = itemID = Guid.NewGuid();
                            cmd.Parameters.Add("@ItemName", SqlDbType.NVarChar, 20);
                            cmd.Parameters[2].Value = AddItem.Text.Trim();
                            cmd.Parameters.Add("@Price", SqlDbType.SmallMoney);
                            cmd.Parameters[3].Value = decimal.Parse(AddPrice.Text.Trim());
                            cmd.Parameters.Add("@IsDisable", SqlDbType.Bit);
                            cmd.Parameters[4].Value = false;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    cmd.Parameters.Clear();
                    cmd.CommandText = insertsqlstr;

                    cmd.Parameters.Add("@OrderID", SqlDbType.UniqueIdentifier);
                    cmd.Parameters[0].Value = ViewState["OrderID"];
                    cmd.Parameters.Add("@ItemID", SqlDbType.UniqueIdentifier);
                    cmd.Parameters[1].Value = itemID != null ? itemID : Guid.Parse(FoodItems.SelectedValue);
                    cmd.Parameters.Add("@OrderDetailID", SqlDbType.UniqueIdentifier);
                    cmd.Parameters[2].Value = Guid.NewGuid();
                    cmd.Parameters.Add("@Creator", SqlDbType.VarChar, 5);
                    cmd.Parameters[3].Value = Session["userID"];
                    cmd.Parameters.Add("@Quantity", SqlDbType.SmallInt);
                    cmd.Parameters[4].Value = Quantity.SelectedValue;
                    cmd.Parameters.Add("@IsCancel", SqlDbType.Bit);
                    cmd.Parameters[5].Value = false;
                    cmd.Parameters.Add("@IsCheck", SqlDbType.Bit);
                    cmd.Parameters[6].Value = false;
                    cmd.Parameters.Add("@CreateDateTime", SqlDbType.DateTime);
                    cmd.Parameters[7].Value = DateTime.Now;
                    cmd.Parameters.Add("@Remark", SqlDbType.NVarChar, 20);
                    cmd.Parameters[8].Value = Server.HtmlEncode(Remark.Text.Trim());
                    cmd.ExecuteNonQuery();

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
                finally
                {
                    conn.Close();
                }
            }
        }
        if (Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7"))
        {
            Response.Write("<script>alert('訂購完成!');location.href='Index.aspx';</script>");
            return;
        }
        else
        {
            string blockjs = @"swal({title:""訂購完成!"",text: ""三秒後回到首頁"",type:""success"",timer: 3000,showConfirmButton: false},
                                function(){
                                    location.href='Index.aspx';
                                })";
            ShowAlert(blockjs);
        }
    }

    /// <summary>
    /// Guid預設不得爲NULL，所以必須要有判斷NULL的方法
    /// </summary>
    /// <param name="dbValue"></param>
    /// <returns></returns>
    static Guid? GetGuidFromDb(object dbValue)
    {
        if (dbValue == null || DBNull.Value.Equals(dbValue))
        {
            return null;
        }
        else
        {
            return (Guid)dbValue;
        }
    }

    protected void FoodItems_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddl = sender as DropDownList;

        if (ddl.SelectedIndex == ddl.Items.Count - 1)
        {
            AddItem.Visible = true;
            AddPrice.Visible = true;
            Total.Text = string.Empty;
            Price.Text = string.Empty;
        }
        else if (ddl.SelectedIndex == 0)
        {
            Price.Text = string.Empty;
            Total.Text = string.Empty;
            AddItem.Visible = false;
            AddPrice.Visible = false;
        }
        else
        {
            AddItem.Visible = false;
            AddPrice.Visible = false;

            using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT a.Price FROM ShopDetails a WHERE a.ShopID=@ShopID and a.ItemID=@ItemID and a.IsDisable=0", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@ShopID", SqlDbType.UniqueIdentifier));
                    cmd.Parameters[0].Value = ViewState["ShopID"];
                    cmd.Parameters.Add(new SqlParameter("@ItemID", SqlDbType.UniqueIdentifier));
                    cmd.Parameters[1].Value = Guid.Parse(FoodItems.SelectedValue);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            Price.Text = string.Format("{0:C1}", dr.GetDecimal(0));
                            Total.Text = string.Format("{0:C1}", dr.GetDecimal(0) * int.Parse(Quantity.SelectedValue));
                        }
                        dr.Close();
                    }
                }
                conn.Close();
            }
        }
    }
    protected void Quantity_SelectedIndexChanged(object sender, EventArgs e)
    {
        int i;
        if (FoodItems.SelectedIndex == 0)
            return;
        if (AddPrice.Visible == true && !int.TryParse(AddPrice.Text, out i))
            return;

        if (FoodItems.SelectedItem.Text.Equals("新增品項"))
        {
            int x;
            if (int.TryParse(AddPrice.Text, out x))
            {
                Total.Text = string.Format("{0:C0}", int.Parse(Quantity.SelectedValue) * decimal.Parse(string.Format("{0:D}", AddPrice.Text.Trim())));
            }
        }
        else
        {
            DropDownList ddl = sender as DropDownList;
            Total.Text = string.Format("{0:C0}", int.Parse(Quantity.SelectedValue) * decimal.Parse(string.Format("{0:D}", Price.Text.Replace("NT$", string.Empty))));
        }
    }
    protected void AddPrice_TextChanged(object sender, EventArgs e)
    {
        TextBox tb = sender as TextBox;
        int intPrice;
        if (!int.TryParse(tb.Text, out intPrice))
        {
            AlertMessage("單價請輸入數字!");
            tb.Text = string.Empty;
            tb.Focus();
            return;
        }
        Total.Text = string.Format("{0:C0}", int.Parse(Quantity.SelectedValue) * intPrice);
    }
}