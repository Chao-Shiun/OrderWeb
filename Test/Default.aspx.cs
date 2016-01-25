using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

public partial class Test_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString.Replace("Order", "Northwind")))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT distinct a.CustomerID FROM Orders a",conn))
                {
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        DropDownList1.DataTextField = "CustomerID";
                        DropDownList1.DataValueField = "CustomerID";
                        DropDownList1.DataSource = dr;
                        DropDownList1.DataBind();
                        dr.Close();
                        //DropDownList1.Attributes["onchange"] = "LoadAJAX(this.value);";
                    }
                }
                conn.Close();
            }
        }
    }
}