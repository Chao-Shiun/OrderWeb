<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="OrderManagement.aspx.cs" Inherits="OrderManagement" %>

<%@ PreviousPageType VirtualPath="~/MyOrderList.aspx" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        input[type="checkbox"]
        {
            width: 15px; /*Desired width*/
            height: 15px; /*Desired height*/
        }
    </style>
    <script type="text/javascript" src="<%=ResolveUrl("~/jquery-ui/external/jquery/jquery.js") %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/bootstrap/js/bootstrap.min.js") %>"></script>
    <script type="text/javascript">
        function printScreen(printId) {
            //var removebtn = document.getElementById('OrderButton');
            //var removebtn = $('#OrderButton')[0]; //document.getElementsByTagName('th')[0];

            var printObj = document.getElementById(printId);
            //var th = document.getElementsByTagName('th')[0];
            //th.parentNode.removeChild('th');

            var printPage = window.open("", "printPage", "");
            printPage.document.write("<HTML><head></head><BODY onload='window.print();window.close()'>");
            printPage.document.write(printObj.innerHTML);
            printPage.document.close("</BODY></HTML>");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="Chart" style="margin: 0 auto;">
        <asp:Chart ID="Chart1" runat="server" BackColor="192, 255, 192" BackGradientStyle="Center"
            Width="900px">
            <Series>
                <asp:Series Name="Series1" CustomProperties="DrawingStyle=Cylinder" IsValueShownAsLabel="True"
                    LegendText="數量" XValueMember="品名" YValueMembers="數量" Font="Consolas, 14.25pt, style=Bold">
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="ChartArea1">
                    <AxisY Title="數量" TitleFont="Microsoft Sans Serif, 12pt">
                    </AxisY>
                    <AxisX IntervalAutoMode="VariableCount" IsLabelAutoFit="False" Title="品名" TitleFont="Microsoft Sans Serif, 12pt">
                        <LabelStyle IsStaggered="True" TruncatedLabels="True" />
                    </AxisX>
                </asp:ChartArea>
            </ChartAreas>
            <Titles>
                <asp:Title Font="Microsoft Sans Serif, 15.75pt, style=Bold" Name="Title1" Text="訂單品項合計">
                </asp:Title>
            </Titles>
            <BorderSkin BackColor="Transparent" SkinStyle="Sunken" />
        </asp:Chart>
    </div>
    <div id="AllOrderDetails" style="margin: 0 auto;">
        <table class="table table-hover">
            <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                <HeaderTemplate>
                    <tr>
                        <th id="OrderButton" style="width: 150px;">
                            <center>
                                <div class="dropup">
                                    <button id="Management" class="btn btn-default dropdown-toggle" type="button" data-toggle="dropdown">
                                        訂單管理 <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li>
                                            <asp:LinkButton ID="SelectAll" runat="server" OnClick="SelectAll_Click">全選</asp:LinkButton>
                                        </li>
                                        <li>
                                            <asp:LinkButton ID="Update" runat="server" OnClick="UpdateOrder_Click" OnClientClick="return confirm('確定更新已付款狀態?');">更新已付款訂單</asp:LinkButton></li>
                                        <li>
                                            <asp:LinkButton ID="Print" runat="server" OnClientClick="printScreen('AllOrderDetails');return false;">列印</asp:LinkButton>
                                        </li>
                                        <li class="divider"></li>
                                        <li>
                                            <asp:LinkButton ID="Cancel" runat="server" OnClick="CancelOrder_Click" OnClientClick="return confirm('確定取消本次訂單?');">取消本次訂單</asp:LinkButton></li>
                                    </ul>
                                </div>
                            </center>
                        </th>
                        <th>
                            <center>
                                訂購人
                            </center>
                        </th>
                        <th>
                            <center>
                                品名
                            </center>
                        </th>
                        <th>
                            <center>
                                數量
                            </center>
                        </th>
                        <th>
                            <center>
                                總金額
                            </center>
                        </th>
                        <th>
                            <center>
                                訂單狀態
                            </center>
                        </th>
                        <th>
                            訂單備註
                        </th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <center>
                                <asp:CheckBox ID="IsBill" runat="server" />
                                <asp:HiddenField ID="OrderDetailID" runat="server" Value='<%#Eval("OrderDetailID") %>' />
                            </center>
                        </td>
                        <td>
                            <center>
                                <%#Eval("DepartmentUser")%>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%#Eval("ItemName") %>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%#Eval("Quantity") %>
                            </center>
                        </td>
                        <td align="right">
                            <asp:Label ID="Total" runat="server" Text='<%#Eval("Total","{0:N0}") %>'></asp:Label>
                        </td>
                        <td>
                            <center>
                                <asp:Label ID="OrderStatus" runat="server" Text='<%#Eval("OrderStatus") %>'></asp:Label>
                            </center>
                        </td>
                        <td>
                            <%#Eval("Remark") %>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="info">
                        <td>
                            <center>
                                <asp:CheckBox ID="IsBill" runat="server" />
                                <asp:HiddenField ID="OrderDetailID" runat="server" Value='<%#Eval("OrderDetailID") %>' />
                            </center>
                        </td>
                        <td>
                            <center>
                                <%#Eval("DepartmentUser")%>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%#Eval("ItemName") %>
                            </center>
                        </td>
                        <td>
                            <center>
                                <%#Eval("Quantity") %>
                            </center>
                        </td>
                        <td align="right">
                            <asp:Label ID="Total" runat="server" Text='<%#Eval("Total","{0:N0}") %>'></asp:Label>
                        </td>
                        <td>
                            <center>
                                <asp:Label ID="OrderStatus" runat="server" Text='<%#Eval("OrderStatus") %>'></asp:Label>
                            </center>
                        </td>
                        <td>
                            <%#Eval("Remark") %>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    <tr>
                        <td>
                            <center>
                                <span style="font-size: Large; font-weight: bold;">訂單總金額 </span>
                            </center>
                        </td>
                        <td colspan="3">
                        </td>
                        <td align="right">
                            <asp:Label ID="Sum" runat="server" Text="" Font-Bold="true" Font-Size="Large"></asp:Label>
                        </td>
                        <td colspan="2">
                        </td>
                    </tr>
                </FooterTemplate>
            </asp:Repeater>
        </table>
    </div>
</asp:Content>
