<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Index.aspx.cs" Inherits="Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="Stylesheet" href="jquery-ui/Nativejquery-ui/jquery-ui.css" />
    <link rel="Stylesheet" href="jquery-ui/Nativejquery-ui/jquery-ui.theme.css" />
    <style type="text/css">
        .table > tbody > tr > td, .table > tbody > tr > th, .table > tfoot > tr > td, .table > tfoot > tr > th, .table > thead > tr > td, .table > thead > tr > th
        {
            vertical-align: middle;
        }
    </style>
    <script type="text/javascript" src="<%=ResolveUrl("~/jquery-ui/external/jquery/jquery.js") %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/jquery-ui/Nativejquery-ui/jquery-ui.js") %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/bootstrap/js/bootstrap.min.js") %>"></script>
    <script type="text/javascript">
        $(function () {
            $("#accordion").accordion({
                event: "click hoverintent"
            });
        });

        $.event.special.hoverintent = {
            setup: function () {
                $(this).bind("mouseover", jQuery.event.special.hoverintent.handler);
            },
            teardown: function () {
                $(this).unbind("mouseover", jQuery.event.special.hoverintent.handler);
            },
            handler: function (event) {
                var currentX, currentY, timeout,
                  args = arguments,
                  target = $(event.target),
                  previousX = event.pageX,
                  previousY = event.pageY;

                function track(event) {
                    currentX = event.pageX;
                    currentY = event.pageY;
                };

                function clear() {
                    target
                      .unbind("mousemove", track)
                      .unbind("mouseout", clear);
                    clearTimeout(timeout);
                }

                function handler() {
                    var prop,
                      orig = event;

                    if ((Math.abs(previousX - currentX) +
                        Math.abs(previousY - currentY)) < 7) {
                        clear();

                        event = $.Event("hoverintent");
                        for (prop in orig) {
                            if (!(prop in event)) {
                                event[prop] = orig[prop];
                            }
                        }
                        delete event.originalEvent;

                        target.trigger(event);
                    } else {
                        previousX = currentX;
                        previousY = currentY;
                        timeout = setTimeout(handler, 100);
                    }
                }

                timeout = setTimeout(handler, 100);
                target.bind({
                    mousemove: track,
                    mouseout: clear
                });
            }
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="accordion">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <h3>
                    [<%# Eval("Category") %>]
                    <%# Eval("Title") %></h3>
                <div>
                    <p>
                        <%#Eval("Context") %>
                    </p>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <hr />
    <div id="OrderList">
        <h2>
            <asp:Label ID="OrderMessage" runat="server" Text="" CssClass="form-control"></asp:Label></h2>
        <table class="table table-hover">
            <asp:Repeater ID="Repeater2" runat="server" OnItemDataBound="Repeater2_ItemDataBound">
                <HeaderTemplate>
                    <tr>
                        <th>
                        </th>
                        <th>
                            <center>
                                品名</center>
                        </th>
                        <th>
                            <center>
                                數量</center>
                        </th>
                        <th>
                            <center>
                                總金額</center>
                        </th>
                        <th>
                            <center>
                                訂購日期</center>
                        </th>
                        <th>
                            <center>
                                訂單狀態</center>
                        </th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <div class="dropdown">
                                <button runat="server" id="Modifly" class="btn btn-default dropdown-toggle" type="button"
                                    data-toggle="dropdown">
                                    訂單異動 <span class="caret"></span>
                                </button>
                                <ul class="dropdown-menu">
                                    <li class="disabled">
                                        <asp:LinkButton ID="EditOrder" runat="server" Enabled="false">修改訂單</asp:LinkButton></li>
                                    <li class="divider"></li>
                                    <li>
                                        <asp:LinkButton ID="CancelOrder" runat="server" CommandName='<%#Eval("OrderID")%>'
                                            CommandArgument='<%#Eval("OrderDetailID")%>' OnClick="CancelOrder_Click" OnClientClick="return confirm('確定要取消該筆訂單?');">取消訂單</asp:LinkButton></li>
                                </ul>
                            </div>
                        </td>
                        <td>
                            <center>
                                <%# HttpUtility.HtmlEncode(Eval("ItemName")) %>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%# Eval("Quantity") %>
                            </center>
                        </td>
                        <td align="right">
                            <asp:Label ID="Total" runat="server" Text='<%#Eval("Total","{0:N0}") %>'></asp:Label>
                        </td>
                        <td>
                            <center>
                                <asp:Label ID="CreateDateTime" runat="server" Text='<%#Eval("CreateDateTime","{0:yyyy/MM/dd HH:mm}") %>'></asp:Label>
                            </center>
                        </td>
                        <td>
                            <center>
                                <center>
                                    <asp:Label ID="OrderStatus" runat="server" Text='<%# Eval("OrderStatus") %>'></asp:Label>
                                </center>
                            </center>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="info">
                        <td>
                            <div class="dropdown">
                                <button runat="server" id="Modifly" class="btn btn-default dropdown-toggle" type="button"
                                    data-toggle="dropdown">
                                    訂單異動 <span class="caret"></span>
                                </button>
                                <ul class="dropdown-menu">
                                    <li class="disabled">
                                        <asp:LinkButton ID="EditOrder" runat="server" Enabled="false">修改訂單</asp:LinkButton></li>
                                    </li>
                                    <li class="divider"></li>
                                    <li>
                                        <asp:LinkButton ID="CancelOrder" runat="server" CommandName='<%#Eval("OrderID")%>'
                                            CommandArgument='<%#Eval("OrderDetailID")%>' OnClick="CancelOrder_Click" OnClientClick="return confirm('確定要取消該筆訂單?');">取消訂單</asp:LinkButton></li>
                                </ul>
                            </div>
                        </td>
                        <td>
                            <center>
                                <%# HttpUtility.HtmlEncode(Eval("ItemName")) %>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%# Eval("Quantity")%>
                            </center>
                        </td>
                        <td align="right">
                            <asp:Label ID="Total" runat="server" Text='<%#Eval("Total","{0:N0}") %>'></asp:Label>
                        </td>
                        <td>
                            <center>
                                <asp:Label ID="CreateDateTime" runat="server" Text='<%#Eval("CreateDateTime","{0:yyyy/MM/dd HH:mm}") %>'></asp:Label>
                            </center>
                        </td>
                        <td>
                            <center>
                                <center>
                                    <asp:Label ID="OrderStatus" runat="server" Text='<%#Eval("OrderStatus") %>'></asp:Label>
                                </center>
                            </center>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </table>
    </div>
</asp:Content>
