<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="CreateOrder.aspx.cs" Inherits="CreateOrder" %>

<%@ PreviousPageType VirtualPath="~/ShopList.aspx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="jquery-ui/jquery-ui.min.css">

    <script type="text/javascript" src="<%=ResolveUrl("~/jquery-ui/external/jquery/jquery.js") %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/jquery-ui/jquery-ui.js") %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/bootstrap/js/bootstrap.min.js") %>"></script>
    <script type="text/javascript">
        $(function () {
            $("#<%=Datepicker.ClientID%>").datepicker({
                showOn: "button",
                buttonImage: "<%=ResolveUrl("~/jquery-ui/images/calendar.gif")%>",
                buttonImageOnly: true,
                buttonText: "選擇日期",
                dateFormat: "yy/mm/dd"
            });
            $("#<%=isOpenOtherDepartment.ClientID%>").selectmenu();
        });
    </script>
    <script type="text/javascript">
        function check() {
            var remark = document.getElementById("<%=Remark.ClientID%>");
            var date = document.getElementById("<%=Datepicker.ClientID%>");
            if (remark.value.length > 50) {
                if (IsIE8orIE7())
                    swal = alert('提醒事項內容超過50個字，請縮減提醒字句!');
                else
                    swal({ title: "提醒事項內容超過50個字，請縮減提醒字句!", type: "error" });
                return false;
            }
            if (date.value == '') {
                if (IsIE8orIE7())
                    swal = alert('請選擇收單日期!');
                else
                    swal({ title: "請選擇收單日期!", type: "error" });
                return false;
            }
            var hour, minute;
            hour = document.getElementById("<%=Hour.ClientID%>");
            minute = document.getElementById("<%=Minute.ClientID%>");
            var shopname = document.getElementById("<%=ShopName.ClientID%>").innerText;

            return confirm("確認新增該筆訂單?\n店家名稱：" + shopname + "\n" + date.value + " " + hour.options[hour.selectedIndex].value + "時" + minute.options[minute.selectedIndex].value + "分收單\n");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div style="margin: 0 auto;">
        <table class="table table-bordered">
            <tr style="width: 900px; height: 50px;">
                <td style="width: 150px;">
                    <center>
                        <asp:Label ID="ShopName" runat="server" Text=""></asp:Label>
                    </center>
                </td>
                <td style="width: 350px;">
                    <center>
                        收單時間：<asp:TextBox ID="Datepicker" runat="server" Width="91px" ReadOnly="true"></asp:TextBox>
                        &nbsp;<asp:DropDownList ID="Hour" runat="server"></asp:DropDownList>：<asp:DropDownList ID="Minute" runat="server"></asp:DropDownList></center>
                </td>
                <td style="width: 300px;">
                    <center>
                        是否開放其他部門訂購：<asp:DropDownList ID="isOpenOtherDepartment" runat="server" Height="16px" Style="margin-left: 0px" Width="89px">
                            <asp:ListItem Value="0">否</asp:ListItem>
                            <asp:ListItem Value="1" Selected="True">是</asp:ListItem>
                        </asp:DropDownList>
                    </center>
                </td>
                <td style="width: 100px;">
                    <center>
                        <asp:Button ID="AddOder" CssClass="btn btn-primary" runat="server" Text="新增訂單" OnClick="AddOder_Click" OnClientClick="return check();" />
                    </center>
                </td>
            </tr>
            <tr>
                <td colspan="4">訂單提醒事項：<asp:TextBox ID="Remark" runat="server" Height="64px" Width="758px" Style="margin-bottom: 0px" MaxLength="100" TextMode="MultiLine" ToolTip="字數上限100字"></asp:TextBox></td>
            </tr>
        </table>
    </div>
</asp:Content>

