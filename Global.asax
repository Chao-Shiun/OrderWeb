<%@ Application Language="C#" %>
<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // 應用程式啟動時執行的程式碼

    }

    void Application_End(object sender, EventArgs e)
    {
        //  應用程式關閉時執行的程式碼

    }

    void Application_Error(object sender, EventArgs e)
    {
        // 發生未處理錯誤時執行的程式碼

        string ClientIP = (Request.ServerVariables["HTTP_VIA"] == null) ? Request.ServerVariables["REMOTE_ADDR"].ToString() : Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
        Exception ex = Server.GetLastError();
        ex = ex.GetBaseException();
        HttpBrowserCapabilities browser = Request.Browser;

        System.IO.StreamWriter sw = null;

        string filePath = string.Format(@"{0}ErrorLog\err_log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", Server.MapPath("~/"));

        using (sw = System.IO.File.Exists(filePath) ? new System.IO.StreamWriter(filePath, true) : System.IO.File.CreateText(filePath))
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
        Response.Write(ex.Message + "請通知系統管理員");
        Server.ClearError();
    }

    void Session_Start(object sender, EventArgs e)
    {
        // 啟動新工作階段時執行的程式碼

    }

    void Session_End(object sender, EventArgs e)
    {
        // 工作階段結束時執行的程式碼。 
        // 注意: 只有在 Web.config 檔將 sessionstate 模式設定為 InProc 時，
        // 才會引發 Session_End 事件。如果將工作階段模式設定為 StateServer 
        // 或 SQLServer，就不會引發這個事件。

    }
       
</script>
