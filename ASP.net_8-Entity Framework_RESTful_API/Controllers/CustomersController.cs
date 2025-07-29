using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using static ASP.net_8_Entity_Framework_RESTful_API.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASP.net_8_Entity_Framework_RESTful_API
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {        
        [HttpGet]
        [Route("customers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CustomerResultDTO>)),
        // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CustomerResultDTO>)),
        ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponseDTO)),
        ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO)),
        ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO)),
        ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult<ResponseDTO<CustomerResultDTO>>> customers([FromQuery] int value)        
        // public async Task<ActionResult<List<CustomerResultDTO>>> customers([FromQuery] int value)
        {
            SqlCommand sqlCommand;
            SqlDataReader dsData = null;
            string tError = string.Empty;

            ResponseDTO<CustomerResultDTO> listCustomers = new ResponseDTO<CustomerResultDTO>();
            // List<CustomerResultDTO> listCustomers = new List<CustomerResultDTO>();

            ADONet objADONet = new ADONet();

            try
            {
                objADONet.connectionSQLServer = objADONet.connectionOpen(objADONet.connectionSQLServer, 2);

                sqlCommand = new SqlCommand("select TOP " + value.ToString() + " CustomerID, ContactName from Customers", objADONet.connectionSQLServer);
                dsData = sqlCommand.ExecuteReader();

                if (dsData.HasRows)
                {
                    while (dsData.Read())
                    {
                        /* customer */
                        CustomerResultDTO customer = new CustomerResultDTO
                        {
                            CustomerID = dsData["CustomerID"] != DBNull.Value ? Convert.ToString(dsData["CustomerID"])!.Trim() : "",
                            ContactName = dsData["ContactName"] != DBNull.Value ? Convert.ToString(dsData["ContactName"])!.Trim() : "",
                        };

                        /* customers */
                        // listCustomers.Add(customer);

                        if (listCustomers.Results == null)
                            listCustomers.Results = new List<CustomerResultDTO>();

                        listCustomers.Results.Add(customer);
                    }
                }
                else
                {
                    dsData.Close();
                    dsData = null;
                    tError = "No customers have been found.";

                    return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorResponseDTO
                        {
                            ErrorCode = StatusCodes.Status404NotFound.ToString(),
                            ErrorMessage = tError
                        });
                }

                dsData.Close();
                dsData = null;

                if (listCustomers != null && listCustomers.Results.Count > 0)
                    listCustomers.TotalCount = listCustomers.Results.Count;

                objADONet.connectionSQLServer.Close();
                objADONet.connectionSQLServer = null;
                objADONet = null;

                return Ok(listCustomers);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponseDTO
                    {
                        ErrorCode = StatusCodes.Status500InternalServerError.ToString(),
                        ErrorMessage = tError,
                        ErrorDetails = new List<ErrorDetailDTO>
                        {
                            new ErrorDetailDTO
                            {
                                InternalErrorCode = 500,
                                Detail = e.Message
                            }
                        }
                    });
            }            
        }

        [HttpPost]
        [Route("customers")]        
        public ActionResult<string> customers([FromBody]object jsonData)
        {
            SqlCommand sqlCommand, sqlCommandInsert;
            SqlDataReader dsData;
            string tCustomerID, tContactName;

            //if (string.IsNullOrEmpty(jsonData))
            //{
            //    return BadRequest("Invalid JSON data.");
            //}

            ADONet objADONet = new ADONet();
            objADONet.connectionSQLServer = objADONet.connectionOpen(objADONet.connectionSQLServer, 2);

            // Deserialize the JSON string to a specific data type (e.g., List<Dictionary<string, object>>)
            try
            {
                string jsonString = jsonData.ToString(); // Cast the object to a string
                List<Dictionary<string, object>> jsonDataAux = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonString);

                // Process the data as needed
                foreach (var item in jsonDataAux)
                {
                    tCustomerID = item["CustomerID"].ToString();
                    tContactName = item["ContactName"].ToString();

                    sqlCommand = new SqlCommand("select CustomerID from Customers where CustomerID = '" + tCustomerID + "'", objADONet.connectionSQLServer);
                    dsData = sqlCommand.ExecuteReader();
                    if (!dsData.HasRows)
                    {
                        dsData.Close();
                        sqlCommandInsert = new SqlCommand("insert into Customers (CustomerID, ContactName, CompanyName) values ('" + tCustomerID + "','" + tContactName + "', '')",
                            objADONet.connectionSQLServer);                        
                    }
                    else
                    {
                        dsData.Close();
                        sqlCommandInsert = new SqlCommand("update Customers set ContactName = '" + tContactName + "' where CustomerID = '" + tCustomerID + "'",
                                objADONet.connectionSQLServer);
                        
                    }
                    sqlCommandInsert.ExecuteNonQuery();
                }

                objADONet.connectionSQLServer.Close();
                objADONet.connectionSQLServer = null;
                objADONet = null;

                return Ok("Customers inserted/updated succesfuly.");
            }
            catch (JsonException)
            {
                return Ok("Invalid JSON format.");
            }
        }

        [HttpPut]
        [Route("customer")]
        public ActionResult<string> customer([FromBody] object jsonData)
        {
            SqlCommand sqlCommand;
            string tCustomerID, tContactName;

            ADONet objADONet = new ADONet();
            objADONet.connectionSQLServer = objADONet.connectionOpen(objADONet.connectionSQLServer, 2);

            try
            {
                string jsonString = jsonData.ToString(); // Cast the object to a string
                List<Dictionary<string, object>> jsonDataAux = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonString);

                try
                {
                    tCustomerID = jsonDataAux[0]["CustomerID"].ToString();
                    tContactName = jsonDataAux[0]["ContactName"].ToString();

                    sqlCommand = new SqlCommand("insert into Customers (CustomerID, ContactName, CompanyName) values ('" + tCustomerID + "','" + tContactName + "', '')",
                        objADONet.connectionSQLServer);
                    sqlCommand.ExecuteNonQuery();

                    objADONet.connectionSQLServer.Close();
                    objADONet.connectionSQLServer = null;
                    objADONet = null;

                    return (Ok("Single customer inserted succesfuly."));
                }
                catch (Exception Ex)
                {
                    objADONet.connectionSQLServer.Close();
                    objADONet.connectionSQLServer = null;
                    objADONet = null;

                    return (Ok("Customer already exists."));
                }
            }
            catch (JsonException)
            {
                return Ok("Invalid JSON format.");
            }
        }

        [HttpDelete]
        [Route("customer")]
        public ActionResult<string> customerDelete(string idCustomer)
        {
            SqlCommand sqlCommand, sqlCommandDelete;
            SqlDataReader dsData = null;

            ADONet objADONet = new ADONet();
            objADONet.connectionSQLServer = objADONet.connectionOpen(objADONet.connectionSQLServer, 2);

            sqlCommand = new SqlCommand("select CustomerID from Customers where CustomerID = '" + idCustomer + "'", objADONet.connectionSQLServer);
            dsData = sqlCommand.ExecuteReader();
            if (dsData.HasRows)
            {
                dsData.Close();
                sqlCommandDelete = new SqlCommand("delete from Customers where CustomerID = '" + idCustomer + "'", objADONet.connectionSQLServer);
                sqlCommandDelete.ExecuteNonQuery();

                return (Ok("Customer deleted succesfully."));
            }
            else
                return (Ok("Customer ID couldn't be found."));

            objADONet.connectionSQLServer.Close();
            objADONet.connectionSQLServer = null;
            objADONet = null;
        }
    }
}
