using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

public partial class Test_Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string CustomerID = Request["CustomerID"];

            using (SqlConnection conn = new SqlConnection(DBTools.ConnectionString.Replace("Order", "Northwind")))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT a.OrderID,a.OrderDate,a.ShipName,a.ShipAddress,a.ShipCountry,a.ShipCity FROM Orders a where a.CustomerID='" + CustomerID + "'", conn))
                {
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        Repeater1.DataSource = dr;
                        Repeater1.DataBind();
                        dr.Close();
                    }
                }
                conn.Close();
            }
        }
    }
}