using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace ASP.net_8_Entity_Framework_RESTful_API
{
    [Route("api/orders")]
    [ApiController]
    public class ordersController : ControllerBase
    {
        [HttpGet]
        [Route("ordersbyCustomer/{customerID}/{startingPage}/{rowsperPage}")]
        //public ActionResult<List<string>> customersRetrieveTop(int value)
        public ActionResult<string> ordersbyCustomer(string customerID, int startingPage, int rowsperPage)
        {
            SqlCommand sqlCommand;
            SqlDataReader dsData = null;
            int iRet = 0;
            int irows = 0;
            int irowsIndex = 0;

            List<Dictionary<string, object>> listorders = new List<Dictionary<string, object>>();

            ADONet objADONet = new ADONet();
            objADONet.connectionSQLServer = objADONet.connectionOpen(objADONet.connectionSQLServer, 2);

            sqlCommand = new SqlCommand("select c.ContactName, c.CompanyName, c.Country, o.OrderID, o.OrderDate, o.ShippedDate from orders o inner join customers c on c.CustomerID = o.CustomerID where o.CustomerID = '" + customerID + "' order by o.OrderDate", objADONet.connectionSQLServer);
            dsData = sqlCommand.ExecuteReader();

            if (dsData.HasRows)
            {
                iRet = 0;                

                if (startingPage > 1 && rowsperPage !=-1)
                {
                    while (irows < ((startingPage * rowsperPage) - rowsperPage))
                    {
                        dsData.Read();
                        irows++;
                    }
                    irowsIndex = irows;
                }

                while (dsData.Read() && ((startingPage > 1 && irows < (irowsIndex + rowsperPage)) || (startingPage==1 && (irows < rowsperPage) || rowsperPage == -1 ))
                    )
                {
                    /* order - item */
                    /* var listItem = new Dictionary<string, object> { };
                    listItem.Add("orderID", dsData["orderID"].ToString().Trim());
                    listItem.Add("orderName", dsData["orderName"].ToString().Trim());
                    
                    /* order - item  */
                    var orderItem = new Dictionary<string, object>
                    {
                        ["ContactName"] = dsData["ContactName"].ToString().Trim() + "",
                        ["CompanyName"] = dsData["CompanyName"].ToString().Trim() + "",
                        ["Country"] = dsData["Country"].ToString().Trim() + "",
                        ["OrderID"] = dsData["OrderID"].ToString().Trim() + "",
                        ["OrderDate"] = dsData["OrderDate"].ToString().Trim() + "",
                        ["ShippedDate"] = dsData["ShippedDate"].ToString().Trim() + ""
                    };

                    /* order - row */
                    listorders.Add(orderItem);

                    /* order - next */
                    irows++;
                }                
            }
            else
                iRet = 1;

            dsData.Close();
            dsData = null;

            objADONet.connectionSQLServer.Close();
            objADONet.connectionSQLServer = null;
            objADONet = null;

            switch (iRet)
            {
                default:
                    {
                        string json = JsonConvert.SerializeObject(listorders);                        
                        return Ok(json);
                        break;
                    }

                case 1:
                    {
                        return Ok("No results.");
                        break;
                    }
            }            
        }
    }
}
