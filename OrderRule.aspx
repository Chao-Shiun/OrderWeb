<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="OrderRule.aspx.cs" Inherits="OrderRule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <h1>
            訂單規則</h1>
        <ol>
            <li>
                <p>
                    本網站每個人都可以是開單人跟訂購人。</p>
            </li>
            <li>
                <p>
                    每個店家最多只能新增2張菜單，也可以不新增，但請記得在訂單備註中提醒各位訂購人要訂的是什麼。</p>
            </li>
            <li>
                <p>
                    個人七日內的訂單及狀態都會顯示在首頁。</p>
            </li>
            <li>
                <p>
                    每筆訂單都必須設定訂單截止時間，超過這時間就不可訂購也不可取消，但開單人可以取消本次開單。</p>
            </li>
            <li>
                <p>
                    訂單列表只會顯示未取消以及未超過截止期間的有效訂單。</p>
            </li>
            <li>
                <p>
                    如果您使用的是IE瀏覽器，因為目前公司主要瀏覽器為IE8，本網站最低只支援IE8，所以在IE8使用相容性檢視將可能讓網頁顯示不正常。如果要關閉相容性檢視可以參考這篇<a
                        href="http://www.hwai.edu.tw/cc/files/news/53591.2357_IE8.pdf" target="_blank" title="請點我">文章</a>。</p>
            </li>
        </ol>
    </div>
</asp:Content>
