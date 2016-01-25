using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

/// <summary>
/// 全部網頁的共用方法所在地
/// </summary>
public class BasePage : Page
{
    public BasePage()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 判斷瀏覽器選擇相對應script訊息(預設error)
    /// </summary>
    /// <param name="Message">要輸出的訊息</param>
    protected void AlertMessage(string Message)
    {
        AlertMessage(Message, "error");
    }
    protected void AlertMessage(string Message, string Type)
    {
        string blockjs;
        HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;

        if (browser.Type.Equals("IE8") && !browser.Type.Equals("IE7"))
            blockjs = @"alert('" + Message + "');";
        else
            blockjs = @"swal({ title: """ + Message + @""", type: """ + Type + @""" })";
        ShowAlert(blockjs);
    }

    /// <summary>
    /// 顯示script訊息
    /// </summary>
    /// <param name="blockjs">script訊息</param>
    protected void ShowAlert(string blockjs)
    {
        if (ScriptManager.GetCurrent(this.Page) == null)
            Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "buttonStartup", blockjs, true);
        else
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "buttonStartupBySM", blockjs, true);
    }

    /// <summary>
    /// 紀錄錯誤訊息
    /// </summary>
    /// <param name="ex">錯誤的例外</param>
    protected void ErrorLog(Exception ex)
    {
        string ClientIP = (Request.ServerVariables["HTTP_VIA"] == null) ? Request.ServerVariables["REMOTE_ADDR"].ToString() : Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
        ex = ex.GetBaseException();
        HttpBrowserCapabilities browser = Request.Browser;

        StreamWriter sw = null;

        string filePath = string.Format(@"{0}ErrorLog\err_log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", Server.MapPath("~/"));

        using (sw = File.Exists(filePath) ? new StreamWriter(filePath, true) : System.IO.File.CreateText(filePath))
        {
            sw.WriteLine("----------Strat----------");
            sw.WriteLine("----------[目前時間：" + DateTime.Now.ToString() + "]----------");
            sw.WriteLine("事件發生網頁網址:" + Request.Url);
            sw.WriteLine("事件發生路徑:" + Request.Path);
            sw.WriteLine("例外狀況訊息:" + ex.Message);
            //事件發生網頁網址

            sw.WriteLine("例外堆疊:\n" + ex.StackTrace);

            sw.WriteLine("造成錯誤的程式名稱:" + ex.Source);

            //使用者名稱
            HttpSessionState session = HttpContext.Current.Session;

            if (session != null && session["UserID"] != null)
                sw.WriteLine("使用者名稱:" + session["UserID"].ToString());
            else
                sw.WriteLine("Session已遺失!");

            sw.WriteLine("識別系統別:" + Request.UserHostAddress);

            sw.WriteLine("IP:" + ClientIP);

            sw.WriteLine("使用瀏覽器:" + browser.Type);

            sw.WriteLine("瀏覽器版本:" + browser.Version);

            sw.WriteLine("是否支援Cookie:" + (browser.Cookies ? "是" : "否"));

            sw.WriteLine("伺服器名稱:" + Request.ServerVariables["SERVER_NAME"]);

            sw.WriteLine("----------END----------\n");

            sw.Close();
        }
        Server.ClearError();
    }
}