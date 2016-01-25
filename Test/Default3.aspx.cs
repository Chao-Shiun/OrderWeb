using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Test_Default3 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        string blockjs;
        //blockjs = @"alert('" + Message + "');";
        if (Request.Browser.Type.Equals("IE8") || Request.Browser.Type.Equals("IE7"))
            blockjs = @"alert('這是IE7或是IE8');";
        else
            //blockjs = @"swal('新增成功');location.href='ShopList.aspx';";
            /*blockjs = @"swal({title: ""Auto close alert!"",text: ""I will close in 2 seconds."",timer: 2000,   showConfirmButton: false },function ()
                            {
                                location.href='../ShopList.aspx';
                            });";*/
            blockjs = @"swal({title: ""新增成功""},function (){location.href='../ShopList.aspx';})";
        if (ScriptManager.GetCurrent(this.Page) == null)
            Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "buttonStartup", blockjs, true);
        else
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "buttonStartupBySM", blockjs, true);
        //Response.Write(@"<script>swal('新增成功', '新增店家成功', 'success');location.href='ShopList.aspx';</script>");
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        Response.Write("~~~~~~~~~~");
    }
}