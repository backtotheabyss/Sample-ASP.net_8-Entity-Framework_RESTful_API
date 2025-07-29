using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using static ASP.net_8_Entity_Framework_RESTful_API.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASP.net_8_Entity_Framework_RESTful_API
{
    [Route("api/products")]
    [ApiController]
    public class productsController : ControllerBase
    {
        [HttpGet]
        [Route("products")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<ProductResultDTO>)),        
        ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponseDTO)),
        ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO)),
        ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO)),
        ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult<ResponseDTO<ProductResultDTO>>> products([FromQuery, Required] int value)
        {
            SqlCommand sqlCommand;
            SqlDataReader dsData = null;
            string tError = string.Empty;

            ADONet objADONet = new ADONet();
            objADONet.connectionSQLServer = objADONet.connectionOpen(objADONet.connectionSQLServer, 2);

            sqlCommand = new SqlCommand("select TOP " + value.ToString() + " c.CategoryID, c.CategoryName, p.ProductID, p.ProductName, p.UnitPrice, p.Discontinued from products p inner join categories c on c.CategoryID = p.CategoryID", objADONet.connectionSQLServer);
            dsData = sqlCommand.ExecuteReader();

            ResponseDTO<ProductResultDTO> listProducts = new ResponseDTO<ProductResultDTO> ();

            if (dsData.HasRows)
            {
                while (dsData.Read())
                {
                    /* product  */
                    ProductResultDTO product = new ProductResultDTO
                    {
                        CategoryName = dsData["CategoryName"] != DBNull.Value ? Convert.ToString(dsData["CategoryName"])!.Trim() : string.Empty,
                        CategoryID = dsData["CategoryID"] != DBNull.Value ? Convert.ToInt32(dsData["CategoryID"]) : 0,
                        ProductID = dsData["ProductID"] != DBNull.Value ? Convert.ToInt32(dsData["ProductID"]) : 0,
                        ProductName = dsData["ProductName"] != DBNull.Value ? Convert.ToString(dsData["ProductName"])!.Trim() : string.Empty,
                        UnitPrice = dsData["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(dsData["UnitPrice"]) : 0,
                        Discontinued = dsData["Discontinued"] != DBNull.Value ? Convert.ToBoolean(dsData["Discontinued"]) : false
                    };

                    /* products */
                    if (listProducts.Results == null)
                        listProducts.Results = new List<ProductResultDTO>();

                    listProducts.Results.Add(product);
                }
            }
            else
            {
                tError = "No products have been found.";
                ErrorResponseDTO errorResponse = new ErrorResponseDTO
                {
                    ErrorCode = StatusCodes.Status404NotFound.ToString(),
                    ErrorMessage = tError
                };

                return StatusCode(StatusCodes.Status404NotFound, errorResponse);
            }

            if (listProducts != null && listProducts.Results.Count > 0)
                listProducts.TotalCount = listProducts.Results.Count;

            dsData.Close();
            dsData = null;

            objADONet.connectionSQLServer.Close();
            objADONet.connectionSQLServer = null;
            objADONet = null;

            return Ok(listProducts);            
        }

        [HttpGet]
        [Route("productsSorted")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<ProductResultDTO>)),        
        ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponseDTO)),
        ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO)),
        ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO)),
        ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseDTO))]
        //public ActionResult<List<string>> customersRetrieveTop(int value)
        public ActionResult<string> productsSorted([FromQuery, Required] string sortField, [FromQuery, Required] string sortCriteria)
        {
            SqlCommand sqlCommand;
            SqlDataReader dsData = null;
            string tError = string.Empty;

            ADONet objADONet = new ADONet();
            objADONet.connectionSQLServer = objADONet.connectionOpen(objADONet.connectionSQLServer, 2);

            sqlCommand = new SqlCommand("select c.CategoryID, c.CategoryName, p.ProductID, p.ProductName, p.UnitPrice, p.Discontinued from products p inner join categories c on c.CategoryID = p.CategoryID order by p." + sortField + " " + sortCriteria, objADONet.connectionSQLServer);
            dsData = sqlCommand.ExecuteReader();

            ResponseDTO<ProductResultDTO> listProducts = new ResponseDTO<ProductResultDTO>();

            if (dsData.HasRows)
            {
                while (dsData.Read())
                {
                    /* product  */
                    ProductResultDTO product = new ProductResultDTO
                    {
                        CategoryName = dsData["CategoryName"] != DBNull.Value ? Convert.ToString(dsData["CategoryName"])!.Trim() : string.Empty,
                        CategoryID = dsData["CategoryID"] != DBNull.Value ? Convert.ToInt32(dsData["CategoryID"]) : 0,
                        ProductID = dsData["ProductID"] != DBNull.Value ? Convert.ToInt32(dsData["ProductID"]) : 0,
                        ProductName = dsData["ProductName"] != DBNull.Value ? Convert.ToString(dsData["ProductName"])!.Trim() : string.Empty,
                        UnitPrice = dsData["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(dsData["UnitPrice"]) : 0,
                        Discontinued = dsData["Discontinued"] != DBNull.Value ? Convert.ToBoolean(dsData["Discontinued"]) : false
                    };

                    /* products */
                    if (listProducts.Results == null)
                        listProducts.Results = new List<ProductResultDTO>();

                    listProducts.Results.Add(product);
                }
            }
            else
            {
                tError = "No products have been found.";
                ErrorResponseDTO errorResponse = new ErrorResponseDTO
                {
                    ErrorCode = StatusCodes.Status404NotFound.ToString(),
                    ErrorMessage = tError
                };

                return StatusCode(StatusCodes.Status404NotFound, errorResponse);
            }

            if (listProducts != null && listProducts.Results.Count > 0)
                listProducts.TotalCount = listProducts.Results.Count;

            dsData.Close();
            dsData = null;

            objADONet.connectionSQLServer.Close();
            objADONet.connectionSQLServer = null;
            objADONet = null;

            return Ok(listProducts);
        }
    }
}
