<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default3.aspx.cs" Inherits="Test_Default3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <% 
        if (!(Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7")))
        {
            Response.Write(@"<link rel=""Stylesheet"" href=""../SweetAlert/sweetalert.css"" />");
            Response.Write(@"<script type=""text/javascript"" src=""../SweetAlert/sweetalert.min.js""></script>");
        }
    %>
    <script type="text/javascript">
        /*function aaa() {
            swal({ title: "Are you sure?",
                text: "You will not be able to recover this imaginary file!",
                type: "warning", showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Yes, delete it!",
                closeOnConfirm: false,
                closeOnCancel: false
            }, function (isConfirm) {
                if (isConfirm) {
                    return true;
                }
                else {
                    return false;
                }
            });
        }
        function bbb() {
            debugger;
            swal(
            {
                title: "An input!",
                text: "Write something interesting:",
                type: "input",
                showCancelButton: true,
                closeOnConfirm: true
            },
            function (response) {
                debugger;
                if (response == true)
                    console.log('You clicked OK!');
                else
                    console.log('You clicked cancel!');
            });
        }*/
        function ccc() {
            if (IsIE8orIE7())
                swal = alert;
            swal(IsIE8orIE7()?'這是IE7或是IE8!':'這不是IE7或是IE8!');
        }
        function IsIE8orIE7() {
            var BrowserVersion = navigator.userAgent;
            if (BrowserVersion.indexOf("MSIE 8.0") == -1 && BrowserVersion.indexOf("MSIE 7.0") == -1)
                return false;
            else
                return true;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input id="Button3" type="button" value="button" onclick="ccc();"/>
    </div>
    </form>
</body>
</html>
