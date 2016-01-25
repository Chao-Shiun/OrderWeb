<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title></title>
    <link rel="stylesheet" href="bootstrap/css/bootstrap.min.css" />
    <link rel="stylesheet" href="bootstrap/css/bootstrap-theme.min.css" />
    <link rel="stylesheet" href="SweetAlert/sweetalert.css" />
    <style type="text/css">
        input[type=text], input[type=password] {
            margin: 10px 0px;
        }
    </style>
    <script type="text/javascript" src="<%=ResolveUrl("~/jquery-ui/external/jquery/jquery.js") %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/bootstrap/js/bootstrap.min.js") %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/SweetAlert/sweetalert.min.js") %>"></script>
</head>
<body>
    <form id="form1" runat="server" class="form-signin">
        <div class="container" style="width: 800px;">
            <h2 class="form-signin-heading">因為系統無法取得您正確帳號，麻煩請在這裡註冊登入。建議請使用Chrome或FireFox瀏覽，才能保證看到完整的網頁樣式<asp:Label ID="Label3"
                runat="server" Text=""></asp:Label></h2>
        </div>
        <div class="container" style="width: 400px;">
            <label for="InputUserID" class="sr-only">
                UserID</label>
            請輸入你的電腦帳號
        <asp:TextBox ID="InputUserID" runat="server" CssClass="form-control"></asp:TextBox>
            <label for="Nicknamelab" class="sr-only">
                網站中的稱呼</label>
            <asp:Label ID="Label1" runat="server" Text="請輸入在網頁中要顯示的名稱" Visible="false"></asp:Label>
            <asp:TextBox ID="Nickname" runat="server" Visible="false" CssClass="form-control"></asp:TextBox>
            <label for="inputPassword" class="sr-only">
                Password</label>
            <asp:Label ID="Label2" runat="server" Text="請輸入登入密碼"></asp:Label>
            <asp:TextBox ID="InputUserPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
            <asp:DropDownList ID="Department" runat="server" Visible="false" CssClass="form-control">
            </asp:DropDownList>
            <div class="checkbox">
                <label>
                    <asp:CheckBox ID="RememberMe" runat="server" />
                    Remember me
                </label>
            </div>
            <asp:Button ID="login" runat="server" Text="登入" CssClass="btn btn-lg btn-primary btn-block"
                OnClick="login_Click" />
            <asp:Button ID="AddUser" runat="server" Text="新增帳號" CssClass="btn btn-lg btn-primary btn-block"
                OnClick="AddUser_Click" UseSubmitBehavior="False" />
            <asp:Button ID="Register" runat="server" Text="註冊帳號" CssClass="btn btn-lg btn-primary btn-block"
                OnClick="Register_Click" Visible="false" />
        </div>
    </form>
</body>
</html>
