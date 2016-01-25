using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI.HtmlControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string CurrentPage = Path.GetFileName(Request.PhysicalPath).Split(new char[] { '.' })[0];
        HtmlGenericControl CurrentLi = Page.Master.FindControl(CurrentPage) as HtmlGenericControl;
        if (CurrentLi != null)
            CurrentLi.Attributes["class"] = "active";
    }
}
