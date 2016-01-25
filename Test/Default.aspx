<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Test_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script type="text/javascript">
        /*function LoadData(str) {
        debugger;
        var xmlhttp;
        if (str == "") {
        document.getElementById("txtHint").innerHTML = "";
        return;
        }
        if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
        }
        else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
        xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
        document.getElementById("Result").innerHTML = xmlhttp.responseText;
        console.log(xmlhttp.responseText);
        }
        }
        xmlhttp.open("POST", "Default2.aspx?CustomerID=" + str, true);
        xmlhttp.send();
        }*/
    </script>
    <script type="text/javascript">
        function appendText() {
            var str = document.getElementById("DropDownList1").value;
            var txt1 = "<p>" + str + ".</p>";              // Create text with HTML
            var txt2 = $("<p></p>").text(str + ".");  // Create text with jQuery
            var txt3 = document.createElement("p");
            txt3.innerHTML = str + ".";               // Create text with DOM
            $("body").append(txt1, txt2, txt3);     // Append new elements
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:DropDownList ID="DropDownList1" runat="server">
        </asp:DropDownList>
        <input id="Button1" type="button" value="button" onclick="appendText();" /></div>
    <div id="Result">
    </div>
    </form>
</body>
</html>
