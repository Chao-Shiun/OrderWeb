<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default2.aspx.cs" Inherits="Test_Default2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" href="../bootstrap/css/bootstrap.min.css" />
    <link rel="Stylesheet" href="../bootstrap/css/bootstrap-theme.min.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 800px;">
        <div style="margin: 0 auto;">
            <table class="table table-hover">
                <asp:Repeater ID="Repeater1" runat="server">
                    <HeaderTemplate>
                        <tr>
                            <th>
                                <center>
                                    OrderID
                                </center>
                            </th>
                            <th>
                                <center>
                                    OrderDate
                                </center>
                            </th>
                            <th>
                                <center>
                                    ShipName
                                </center>
                            </th>
                            <th>
                                <center>
                                    ShipAddress
                                </center>
                            </th>
                            <th>
                                <center>
                                    ShipCountry
                                </center>
                            </th>
                            <th>
                                <center>
                                    ShipCity
                                </center>
                            </th>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <center>
                                    <%#Eval("OrderID")%>
                                </center>
                            </td>
                            <td>
                                <center>
                                    <%#Eval("OrderDate")%>
                                </center>
                            </td>
                            <td>
                                <center>
                                    <%#Eval("ShipName")%>
                                </center>
                            </td>
                            <td>
                                <center>
                                    <%#Eval("ShipAddress")%>
                                </center>
                            </td>
                            <td>
                                <center>
                                    <%#Eval("ShipCountry")%>
                                </center>
                            </td>
                            <td>
                                <center>
                                    <%#Eval("ShipCity")%>
                                </center>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
