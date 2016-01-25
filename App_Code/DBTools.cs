using System.Configuration;
using ePro.DBUtility;

/// <summary>
/// DBTools 的摘要描述
/// </summary>
public class DBTools
{
    public DBTools()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    public static string ConnectionString
    {
        get
        {
            switch (ConfigurationManager.AppSettings["ENVIRONMENT"].ToString())
            {
                case "home":
                    return "Data Source=.;Initial Catalog=Order;Integrated Security=True";
                case "company":
                    return "";
                default :
                    return SQLDB.getConnectionString(SQLDB.getEnviorment(ConfigurationManager.AppSettings["ENVIRONMENT"].ToString()),"Order");
            }
        }
    }
}