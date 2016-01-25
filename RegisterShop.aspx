<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="RegisterShop.aspx.cs" Inherits="RegisterShop" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="jquery-ui/jquery-ui.min.css" />
    <style type="text/css">
        tr {
            width: 900px;
        }
    </style>
    
    <script type="text/javascript" src="<%=ResolveUrl("~/jquery-ui/jquery-ui.js") %>"></script>
    <script type="text/javascript">
        $(function () {
            $("#<%= MenuType.ClientID%>").selectmenu();
            $("#<%= IsVegetarianismShop.ClientID%>").selectmenu();
            $("#<%= FileUpload1.ClientID%>,#<%= FileUpload2.ClientID%>").change(function () {
                showFileSize(this);
            });
        });
    </script>
    <script type="text/javascript">
        function CheckForm() {
            var shopname = document.getElementById("<%=ShopName.ClientID%>");
            debugger;
            if (typeof String.prototype.trim !== 'function') {
                String.prototype.trim = function () {
                    return this.replace(/^\s+|\s+$/g, '');
                }
            }
            if (shopname.value.length == 0) {
                if (IsIE8orIE7())
                    swal = alert('請輸入店家名稱!');
                else
                    swal({ title: "請輸入店家名稱!", type: "error" });
                return false;
            }

            if (shopname.value.length > 20) {
                if (IsIE8orIE7())
                    swal = alert('店家名稱不得超過20字!');
                else
                    swal({ title: "店家名稱不得超過20字!", type: "error" });
                return false;
            }
            var file1 = document.getElementById("<%=FileUpload1.ClientID%>").files[0];
            var file2 = document.getElementById("<%=FileUpload2.ClientID%>").files[0];

            if (file1 === undefined && file2 === undefined) {

                var nopic = confirm('確定該店家不新增任何菜單圖示?');
                if (!nopic)
                    return false;
            } else if (file1 != undefined && file2 != undefined && (file1.name === file2.name)) {
                if (IsIE8orIE7())
                    swal = alert('上傳菜單名稱不得相同!');
                else
                    swal({ title: "上傳菜單名稱不得相同!", type: "error" });
                return false;
            }
            var VegetarianismShop = document.getElementById('<%=IsVegetarianismShop.ClientID%>');
            VegetarianismShop = VegetarianismShop.options[VegetarianismShop.selectedIndex].text === '否' ? '不是' : '是';
            var MenuCategory = document.getElementById('<%=MenuType.ClientID%>');

            return confirm('確認新增店家：' + shopname.value + '\n' + VegetarianismShop + '素食店家\n菜單類型為' + MenuCategory.options[MenuCategory.selectedIndex].text);
        }

        function showFileSize(fu) {
            var input = fu, file = input.files[0];

            /*if (!window.FileReader) {
            bodyAppend("p", "The file API isn't supported on this browser yet.");
            return;
            }
            //input = document.getElementById(fuName);

            if (!input) {
            bodyAppend("p", "Um, couldn't find the fileinput element.");
            }
            else if (!input.files) {
            bodyAppend("p", "This browser doesn't seem to support the `files` property of file inputs.");
            }
            else if (!input.files[0]) {
            bodyAppend("p", "Please select a file before clicking 'Load'");
            }
            else {
            file = input.files[0];
            //bodyAppend("p", "File " + file.name + " is " + file.size + " bytes in size");

                
            /*else
            alert("目前檔案大小為" + Number(file.size / 1024 / 1024).toFixed(2) + "MB");
            alert("File " + file.name + " is " + file.size / 1024 + " kbytes in size");
            }*/

            if (file.name.length > 20) {
                if (IsIE8orIE7())
                    swal = alert('檔案名稱不得超過20個字元(包含副檔名)!');
                else
                    swal({ title: "檔案名稱不得超過20個字元(包含副檔名)!", type: "error" });
                input.value = "";
            }
            var extension = file.name.split(".").pop().toLowerCase();
            if (extension != "jpg" && extension != "jpeg" && extension != "gif" && extension != "png" && extension != "bmp" && extension != "pdf" && extension != "tif") {
                if (IsIE8orIE7())
                    swal = alert('上傳檔案限定jpge、jpg、gif、png、bmp、pdf、tif其中一種!');
                else
                    swal({ title: "上傳檔案限定jpge、jpg、gif、png、bmp、pdf、tif其中一種!", type: "error" });
                input.value = "";
                return;
            }

            if (file.size > 5242880) {
                if (IsIE8orIE7())
                    swal = alert("檔案大小超過5MB，目前為" + Number(file.size / 1024 / 1024).toFixed(2) + "MB，請壓縮檔案再上傳");
                else
                    swal("檔案大小超過5MB，目前為" + Number(file.size / 1024 / 1024).toFixed(2) + "MB，請壓縮檔案再上傳");
                input.value = "";
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table class="table table-bordered">
        <tr>
            <td>
                <center>
                    店家名稱<asp:TextBox ID="ShopName" runat="server" CssClass="form-control"></asp:TextBox>
                </center>
            </td>
            <td>
                <center>
                    是否為素食店家？<asp:DropDownList ID="IsVegetarianismShop" runat="server" Width="100px">
                        <asp:ListItem Value="false">否</asp:ListItem>
                        <asp:ListItem Value="true">是</asp:ListItem>
                    </asp:DropDownList>
                </center>
            </td>
            <td>
                <center>
                    菜單類型：<asp:DropDownList ID="MenuType" runat="server" Width="100px">
                        <asp:ListItem Value="0">午餐</asp:ListItem>
                        <asp:ListItem Value="1">點心</asp:ListItem>
                        <asp:ListItem Value="2">飲料</asp:ListItem>
                    </asp:DropDownList>
                </center>
            </td>
        </tr>
        <tr>
            <td>上傳菜單1：<asp:FileUpload ID="FileUpload1" runat="server" />
            </td>
            <td>上傳菜單2：<asp:FileUpload ID="FileUpload2" runat="server" />
            </td>
            <td>
                <center>
                    <asp:Button ID="submit" runat="server" Text="確認新增" CssClass="btn btn-primary" OnClick="submit_Click"
                        OnClientClick="return CheckForm();" />
                </center>
            </td>
        </tr>
    </table>
</asp:Content>
