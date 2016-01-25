<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ADLogin.aspx.cs" Inherits="ADLogin1" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <style type="text/css">
        #sitebody
        {
            width: 800px;
            margin: 0 auto;
        }
        #header
        {
            height: 150px;
        }
        #Department
        {
            margin:20px 0px;
        }
        p
        {
            margin: 20px 20px;
            font-size: 120%;
        }
    </style>
    <link rel="stylesheet" href="bootstrap/css/bootstrap.min.css" />
    <link rel="stylesheet" href="bootstrap/css/bootstrap-theme.min.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="sitebody">
        <div id="header">
        </div>
        <div id="content" class="alert alert-info">
            <p>
                Hi,Everyone。每天我們許多人都必須要訂購中餐或是下午茶，但各位是否思考過現在的人工方式有哪些缺點?是否還有更方便的解決方式?有鑑於目前點餐的方式對於開單者跟訂購者都有不少的不便，所以我希望設計一套直覺、簡單的系統，可以解決這個說重要不重要，但每天上班又會遇到的事。</p>
            <p>
                本網站並非是正式系統，所以並不保證不會因為技術以外的問題而消失，但如果各位有任何問題歡迎跟我反應，或是你覺得新增什麼功能可以更方便，也可以跟我討論。
            </p>
            <p class="alert alert-danger">
                強烈建議如果你的電腦可以裝<a href="https://www.google.com.tw/chrome/browser/desktop/index.html">Chrome</a>、<a
                    href="https://www.mozilla.org/zh-TW/firefox/new/">FireFox</a>等瀏覽器，盡量避免使用不支援標準網頁格式的IE8，才能保證您能看到網頁完整的外觀。有鑑於有些同仁電腦可能無法安裝其他瀏覽器，設計上我還是盡量讓網頁在IE8能(比較)正常呈現，但如果用IE8的相容模式一定會讓網頁不正常，請各位注意!
            </p>
        </div>
        <div id="footer">
            <div style="margin: 0 auto;Width:450px;">
                你要在網站顯示的名稱<br />(可輸入中文、英文、NickName，別人認得你就行)
                <asp:TextBox ID="NickName" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:DropDownList ID="Department" runat="server" CssClass="form-control">
                </asp:DropDownList>
                <asp:Button ID="Register" runat="server" Text="註冊帳號" CssClass="btn btn-lg btn-primary btn-block"
                    OnClientClick="return confirm('確認名稱跟部門無誤?');" onclick="Register_Click" />
            </div>
        </div>
    </div>
    </form>
</body>
</html>
