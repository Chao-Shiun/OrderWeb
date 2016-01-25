<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="MyOrderList.aspx.cs" Inherits="MyOrderList" %>
<%@ Register Src="~/uc/pagination.ascx" TagPrefix="uc" TagName="Pagination" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="jquery-ui/jquery-ui.min.css">
    <link rel="Stylesheet" href="CSS/pagination/pagination.css" />
    <style type="text/css">
        .table > tbody > tr > td, .table > tbody > tr > th, .table > tfoot > tr > td, .table > tfoot > tr > th, .table > thead > tr > td, .table > thead > tr > th
        {
            vertical-align: middle;
        }
        #<%=StartDate.ClientID%>,#<%=EndDate.ClientID%>,#<%=ShopName.ClientID%>,#<%=Category.ClientID%>
        {
            display:inline;
        }
    </style>
    <script type="text/javascript" src="<%=ResolveUrl("~/jquery-ui/external/jquery/jquery.js") %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/jquery-ui/jquery-ui.js") %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/bootstrap/js/bootstrap.min.js") %>"></script>
    <script type="text/javascript">
        $(function () {
            $("#<%=StartDate.ClientID%>,#<%=EndDate.ClientID%>").datepicker({
                showOn: "button",
                buttonImage: "<%=ResolveUrl("~/jquery-ui/images/calendar.gif")%>",
                buttonImageOnly: true,
                buttonText: "選擇日期",
                dateFormat: "yy/mm/dd"
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="QueryPanel" style="margin-bottom: 20px;">
        <table style="margin: 0 auto;" cellpadding="0" cellspacing="0">
            <tr>
                <td style="padding: 10px 50px">
                    <center>
                        店家名稱
                        <asp:TextBox ID="ShopName" runat="server" CssClass="form-control" Width="200px"></asp:TextBox>
                    </center>
                </td>
                <td style="padding: 10px 50px">
                    <center>
                        訂單類型
                        <asp:DropDownList ID="Category" runat="server" CssClass="form-control" Width="200px">
                        </asp:DropDownList>
                    </center>
                </td>
                <td rowspan="2">
                    <asp:Button ID="btnQuery" runat="server" Text="查詢" CssClass="btn btn-primary" OnClick="Query_Click" />
                </td>
            </tr>
            <tr>
                <td style="padding: 10px 50;">
                    <center>
                        起始日
                        <asp:TextBox ID="StartDate" runat="server" CssClass="form-control" ReadOnly="True"
                            Width="184px"></asp:TextBox>
                    </center>
                </td>
                <td style="padding: 10px 50;">
                    <center>
                        結束日
                        <asp:TextBox ID="EndDate" runat="server" CssClass="form-control" ReadOnly="True"
                            Width="184px"></asp:TextBox>
                    </center>
                </td>
            </tr>
        </table>
    </div>
    <div>
        <table class="table table-hover">
            <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand">
                <HeaderTemplate>
                    <tr>
                        <th>
                        </th>
                        <th>
                            <center>
                                店名
                            </center>
                        </th>
                        <th>
                            <center>
                                收單時間
                            </center>
                        </th>
                        <th>
                            <center>
                                餐點類型
                            </center>
                        </th>
                        <th>
                            <center>
                                訂購單數
                            </center>
                        </th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <center>
                                <asp:Button ID="OrderManager" runat="server" CssClass="btn btn-default" Text="訂單管理"
                                    CommandName='<%#Eval("OrderID") %>' />
                            </center>
                        </td>
                        <td>
                            <center>
                                <%#Eval("ShopName")%>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%# ((DateTime)Eval("Deadline")).ToString("yyyy/MM/dd HH:mm") %>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%#Eval("CategoryName")%>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%#Eval("OrderSum")%>
                            </center>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="info">
                        <td>
                            <center>
                                <asp:Button ID="OrderManager" runat="server" CssClass="btn btn-default" Text="訂單管理"
                                    CommandName='<%#Eval("OrderID") %>' /></center>
                        </td>
                        <td>
                            <center>
                                <%#Eval("ShopName")%>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%# ((DateTime)Eval("Deadline")).ToString("yyyy/MM/dd HH:mm") %>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%#Eval("CategoryName")%>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%#Eval("OrderSum")%>
                            </center>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </table>
        <uc:Pagination runat="server" ID="ucPagination" />
    </div>
</asp:Content>
