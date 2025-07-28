using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ASP.net_8_Entity_Framework_RESTful_API
{
    [Route("api/products")]
    [ApiController]
    public class productsController : ControllerBase
    {
        [HttpGet]
        [Route("productsSorting/{sortField}/{sortCriteria}")]
        //public ActionResult<List<string>> customersRetrieveTop(int value)
        public ActionResult<string> productsSorting(string sortField, string sortCriteria)
        {
            SqlCommand sqlCommand;
            SqlDataReader dsData = null;

            List<Dictionary<string, object>> listProducts = new List<Dictionary<string, object>>();

            ADONet objADONet = new ADONet();
            objADONet.connectionSQLServer = objADONet.connectionOpen(objADONet.connectionSQLServer, 2);

            sqlCommand = new SqlCommand("select c.CategoryName, c.Description, p.ProductID, p.ProductName, p.UnitPrice, p.Discontinued from products p inner join categories c on c.CategoryID = p.CategoryID order by p." + sortField + " " + sortCriteria, objADONet.connectionSQLServer);
            dsData = sqlCommand.ExecuteReader();

            if (dsData.HasRows)
                while (dsData.Read())
                {
                    /* product - item */
                    /* var listItem = new Dictionary<string, object> { };
                    listItem.Add("ProductID", dsData["ProductID"].ToString().Trim());
                    listItem.Add("ProductName", dsData["ProductName"].ToString().Trim());
                    
                    /* product - item  */
                    var productItem = new Dictionary<string, object>
                    {
                        ["CategoryName"] = dsData["CategoryName"].ToString().Trim(),
                        ["Description"] = dsData["Description"].ToString().Trim(),
                        ["ProductID"] = dsData["ProductID"].ToString().Trim(),
                        ["ProductName"] = dsData["ProductName"].ToString().Trim(),
                        ["UnitPrice"] = dsData["UnitPrice"].ToString().Trim(),
                        ["Discontinued"] = dsData["Discontinued"].ToString().Trim()
                    };

                    /* product - row */
                    listProducts.Add(productItem);
                }

            dsData.Close();
            dsData = null;

            objADONet.connectionSQLServer.Close();
            objADONet.connectionSQLServer = null;
            objADONet = null;

            string json = JsonConvert.SerializeObject(listProducts);

            return Ok(json);
        }
    }
}
