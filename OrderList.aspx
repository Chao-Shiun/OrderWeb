<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="OrderList.aspx.cs" Inherits="OrderList" EnableEventValidation="false" %>

<%@ Register Src="~/UC/Pagination.ascx" TagPrefix="uc1" TagName="Pagination" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="Stylesheet" href="CSS/pagination/pagination.css" />
    <script type="text/javascript" src="<%=ResolveUrl("~/jquery-ui/external/jquery/jquery.js") %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/bootstrap/js/bootstrap.min.js") %>"></script>
    <style type="text/css">
        .button {
            width: 100px;
            height: 50px;
        }

        td {
            height: 50px;
        }
        .table>tbody>tr>td, .table>tbody>tr>th, .table>tfoot>tr>td, .table>tfoot>tr>th, .table>thead>tr>td, .table>thead>tr>th
        {
            vertical-align:middle;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="QueryPanel" style="margin-bottom:20px">
        <table style="margin: 0 auto;" cellpadding="0" cellspacing="0">
            <tr>
                <td style="padding: 10px 50px">
                    <center>店家名稱<asp:TextBox ID="ShopName" runat="server" CssClass="form-control"></asp:TextBox></center>
                </td>
                <td>
                    <div class="checkbox">
                        <center>
                            <label>
                                素食店家
                            </label>
                            <asp:DropDownList ID="IsVegetarianism" runat="server" CssClass="form-control"></asp:DropDownList>
                        </center>
                    </div>
                </td>
                <td style="padding: 10px 50px">
                    <center>
                        開單部門
                        <asp:DropDownList ID="DepartMent" runat="server" CssClass="form-control"></asp:DropDownList>
                    </center>
                </td>
            </tr>
            <tr>
                <td style="padding: 10px 50px">
                    <center>
                        訂單類型
                        <asp:DropDownList ID="Category" runat="server" CssClass="form-control"></asp:DropDownList>
                    </center>
                </td>
                <td>
                    <center>
                        開單人<asp:TextBox ID="Creator" runat="server" CssClass="form-control"></asp:TextBox>
                    </center>
                </td>
                <td style="padding: 10px 50px">
                    <center>
                        <asp:Button ID="QueryCondition" runat="server" Text="查詢" CssClass="btn btn-primary" OnClick="QueryCondition_Click" />
                    </center>
                </td>
            </tr>
        </table>
    </div>
    <div id="OrderList" style="margin: 0 auto;">
        <table class="table table-hover">
            <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand" OnItemDataBound="Repeater1_ItemDataBound">
                <HeaderTemplate>
                    <tr>
                        <th></th>
                        <th>
                            <center>店名</center>
                        </th>
                        <th>
                            <center>開單人</center>
                        </th>
                        <th>
                            <center>收單時間</center>
                        </th>
                        <th>
                            <center>菜單連結</center>
                        </th>
                        <th>
                            <center>餐點類型</center>
                        </th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <center>
                                <asp:Button ID="Meal" runat="server" Text="點餐" CssClass="btn btn-default" CausesValidation="False" UseSubmitBehavior="False" />
                            </center>
                            <asp:HiddenField ID="ShopID" runat="server" Value='<%# Eval("ShopID")%>' />
                            <asp:HiddenField ID="OrderID" runat="server" Value='<%# Eval("OrderID")%>' />
                        </td>
                        <td>
                            <center>
                                <%# HttpUtility.HtmlEncode(Eval("ShopName")) %>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%# Eval("DepartmentUser") %>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%# ((DateTime)Eval("Deadline")).ToString("yyyy/MM/dd HH:mm") %>
                            </center>
                        </td>
                        <td runat="server" id="ImgLink"></td>
                        <td>
                            <center>
                                <%# Eval("CategoryName") %>
                            </center>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="info">
                        <td>
                            <center>
                                <asp:Button ID="Meal" runat="server" Text="點餐" CssClass="btn btn-default" CausesValidation="False" UseSubmitBehavior="False" />
                            </center>
                            <asp:HiddenField ID="ShopID" runat="server" Value='<%# Eval("ShopID")%>' />
                            <asp:HiddenField ID="OrderID" runat="server" Value='<%# Eval("OrderID")%>' />
                        </td>
                        <td>
                            <center>
                                <%# HttpUtility.HtmlEncode(Eval("ShopName")) %>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%# Eval("DepartmentUser") %>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%# ((DateTime)Eval("Deadline")).ToString("yyyy/MM/dd HH:mm") %>
                            </center>
                        </td>
                        <td runat="server" id="ImgLink"></td>
                        <td>
                            <center>
                                <%# Eval("CategoryName") %>
                            </center>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </table>
        <uc1:Pagination runat="server" ID="ucPagination" />
    </div>
</asp:Content>

