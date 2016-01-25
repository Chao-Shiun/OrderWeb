<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="ToOrder.aspx.cs" Inherits="ToOrder" EnableEventValidation="false" %>

<%@ PreviousPageType VirtualPath="~/OrderList.aspx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="<%=ResolveUrl("~/jquery-ui/external/jquery/jquery.js") %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/bootstrap/js/bootstrap.min.js") %>"></script>
    <style type="text/css">
        td
        {
            padding: 10px 10px;
        }
        .table > tbody > tr > td, .table > tbody > tr > th, .table > tfoot > tr > td, .table > tfoot > tr > th, .table > thead > tr > td, .table > thead > tr > th
        {
            vertical-align: middle;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div style="height: 129px">
        <table class="table table-striped">
            <tr>
                <td>
                    <center>
                        <asp:DropDownList ID="FoodItems" runat="server" AutoPostBack="True" CssClass="form-control"
                            OnSelectedIndexChanged="FoodItems_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:TextBox ID="AddItem" runat="server" MaxLength="20" Visible="false" CssClass="form-control"></asp:TextBox>
                    </center>
                </td>
                <td>
                    <center>
                        數量<asp:DropDownList ID="Quantity" runat="server" CssClass="form-control" AutoPostBack="True"
                            OnSelectedIndexChanged="Quantity_SelectedIndexChanged">
                        </asp:DropDownList>
                    </center>
                </td>
                <td>
                    <center>
                        單價
                        <asp:Label ID="Price" runat="server" Text=""></asp:Label>
                        <asp:TextBox ID="AddPrice" runat="server" Visible="false" CssClass="form-control"
                            AutoPostBack="True" OnTextChanged="AddPrice_TextChanged" Width="200px"></asp:TextBox>
                    </center>
                </td>
                <td>
                    <center>
                        合計：<asp:Label ID="Total" runat="server" Text=""></asp:Label></center>
                </td>
            </tr>
            <tr>
                <td colspan="3" class="auto-style1">
                    訂單註記(任何冷、熱、加飯…etc，請寫在這)
                    <asp:TextBox ID="Remark" runat="server" Width="739px" Height="37px" TextMode="MultiLine"
                        MaxLength="20" CssClass="form-control"></asp:TextBox>
                </td>
                <td class="auto-style1">
                    <center>
                        <asp:Button ID="AddOrder" runat="server" Text="確認訂購" CssClass="btn btn-primary" OnClick="AddOrder_Click"
                            OnClientClick="return confirm('確認品項跟數量無誤?');" />
                    </center>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Label ID="OrderRemark" runat="server" Text='' CssClass="label label-info"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <div style="margin-top: 50px;">
        <asp:Chart ID="Chart1" runat="server" BackColor="255, 255, 192" BackGradientStyle="HorizontalCenter"
            Width="900px">
            <Series>
                <asp:Series Name="Series1" CustomProperties="DrawingStyle=Cylinder" IsValueShownAsLabel="True"
                    XValueMember="品名" YValueMembers="數量" Font="Consolas, 14.25pt, style=Bold">
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="ChartArea1">
                    <AxisY Title="數量" TitleFont="Microsoft Sans Serif, 12pt">
                    </AxisY>
                    <AxisX IntervalAutoMode="VariableCount" IsLabelAutoFit="False" Title="品名" TitleFont="Microsoft Sans Serif, 12pt">
                        <LabelStyle IsStaggered="True" />
                    </AxisX>
                </asp:ChartArea>
            </ChartAreas>
            <Titles>
                <asp:Title Font="Microsoft Sans Serif, 15.75pt, style=Bold" Name="Title1" Text="已訂購品項合計">
                </asp:Title>
            </Titles>
            <BorderSkin SkinStyle="Sunken" />
        </asp:Chart>
    </div>
    <div class="bg-info" style="padding: 10px;line-height: 24px;">
        Hi，Every one。關於這裡的設計構思我想看看各位的意見。如果我們每個人每次點同一店家都要key相同的東西，那麼我們為什麼不讓第一個人key完後，後面的人就不用key呢？這樣的設計還有個好處，能夠避免品項的內容每次都不一樣，也減少開單人維護訂單的難度。但這樣的方式還是有缺點的，開放給每個人來建立品項，這勢必面臨了「要如何統一品項名稱？」的問題，否則如果每個人對於品名的認定不同，最後還是可能造成不必要的品名重複，而且各種店家菜單的差異也不同。
    </div>
    <div class="bg-info" style="padding: 10px;line-height: 24px;">
        所以在這裡我想到比較折衷的方式就是只新增菜單上有的品名，例如：菜單上有個牛肉麵大碗120小碗100，如果你想吃小碗只要新增牛肉麵(小)這個品名就行，但如果是牛肉麵有個加麵或便當加飯加十元或其他選項要求，就把他寫到「訂單註記」。例如：我的奶茶要大杯無糖，大杯50元那就新增奶茶(大)這品項，無糖請寫在「訂單註記」，這方式是想避免訂單品項過度膨脹，因為一個品項可能有冷、熱、大、小千奇百怪的排列組合，每種組合就key一個最後有可能會造成開單人的困擾，所以在此想看看各位對這種設計方式有無意見，或是您有更好的想法也歡迎跟我討論。
    </div>
</asp:Content>
