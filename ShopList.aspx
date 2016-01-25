<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ShopList.aspx.cs" Inherits="ShopList" %>

<%@ Register Src="~/UC/Pagination.ascx" TagPrefix="uc1" TagName="Pagination" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="Stylesheet" href="CSS/pagination/pagination.css" />
    <style type="text/css">
        td {
            height: 50px;
        }
        .table>tbody>tr>td, .table>tbody>tr>th, .table>tfoot>tr>td, .table>tfoot>tr>th, .table>thead>tr>td, .table>thead>tr>th
        {
            vertical-align:middle;
        }
    </style>
    <script type="text/javascript" src="<%=ResolveUrl("~/jquery-ui/external/jquery/jquery.js") %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/bootstrap/js/bootstrap.min.js") %>"></script>
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
            </tr>
            <tr>
                <td style="padding: 10px 50px">
                    <center>
                        訂單類型
                        <asp:DropDownList ID="Category" runat="server" CssClass="form-control"></asp:DropDownList>
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
    <div id="ShopList" style="margin: 0 auto;">
        <table class="table table-hover">
            <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound" OnItemCommand="Repeater1_ItemCommand">
                <HeaderTemplate>
                    <tr>
                        <th class="button"></th>
                        <th>
                            <center>店家名稱</center>
                        </th>
                        <th>
                            <center>菜單連結</center>
                        </th>
                        <th>
                            <center>菜單類型</center>
                        </th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <center>
                                <asp:Button ID="AddOrder" runat="server" Text="開單" CssClass="btn btn-default" />
                                <asp:HiddenField ID="ShopID" runat="server" Value='<%# Eval("ShopID") %>' />
                            </center>
                        </td>
                        <td>
                            <center>
                                <asp:Label ID="ShopName" runat="server" Text='<%# HttpUtility.HtmlEncode(Eval("ShopName")) %>'>'></asp:Label></center>
                        </td>
                        <td runat="server" id="ImgLink">
                            <center>
                            </center>
                        </td>
                        <td>
                            <center>
                                <asp:Label ID="MenuType" runat="server" Text='<%# Eval("CategoryName")%>'></asp:Label>
                            </center>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="info">
                        <td>
                            <center>
                                <asp:Button ID="AddOrder" runat="server" Text="開單" CssClass="btn btn-default"/>
                                <asp:HiddenField ID="ShopID" runat="server" Value='<%# Eval("ShopID") %>' />
                            </center>
                        </td>
                        <td>
                            <center>
                                <asp:Label ID="ShopName" runat="server" Text='<%# HttpUtility.HtmlEncode(Eval("ShopName")) %>'>'></asp:Label></center>
                        </td>
                        <td runat="server" id="ImgLink">
                            <center>
                            </center>
                        </td>
                        <td>
                            <center>
                                <asp:Label ID="MenuType" runat="server" Text='<%# Eval("CategoryName")%>'></asp:Label>
                            </center>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </table>
    </div>
    <div>
        <uc1:Pagination runat="server" ID="ucPagination" />
    </div>
</asp:Content>

