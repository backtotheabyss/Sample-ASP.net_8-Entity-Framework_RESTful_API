using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using static Models.Models;
using static Models.Product;
using System.Linq.Dynamic.Core;

namespace ASP.net_8_Entity_Framework_RESTful_API
{
    [Route("api/products")]
    [ApiController]
    public class productsController : ControllerBase
    {
        private readonly NorthwndContext _context;
        public productsController(NorthwndContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("productsEF")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<Product>))]
        [ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<Response<Product>>> productsEF([FromQuery, Required] int value)
        {
            Response<Product> listProducts = new Response<Product>();

            try
            {
                var products = await _context.Products    
                    .Take(value)
                    .ToListAsync();

                if (products != null && products.Count > 0)
                {
                    /* products */
                    if (listProducts.Results == null)
                        listProducts.Results = new List<Product>();

                    listProducts = new Response<Product>
                    {
                        Results = products,
                        TotalCount = products.Count
                    };

                    return Ok(listProducts);
                }
                else
                    return NotFound(new ErrorResponse
                    {
                        ErrorCode = StatusCodes.Status404NotFound.ToString(),
                        ErrorMessage = "No products have been found."
                    });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    ErrorCode = StatusCodes.Status500InternalServerError.ToString(),
                    ErrorMessage = "Internal server error: " + ex.Message
                });
            }
        }

        //[HttpGet]
        //[Route("products")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<Product>)),        
        //ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponse)),
        //ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse)),
        //ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse)),
        //ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        //public async Task<ActionResult<Response<Product>>> products([FromQuery, Required] int value)
        //{
        //    SqlCommand sqlCommand;
        //    SqlDataReader dsData = null;
        //    string tError = string.Empty;

        //    ADONet objADONet = new ADONet();
        //    objADONet.connectionSQLServer = objADONet.connectionOpen(objADONet.connectionSQLServer, 2);

        //    sqlCommand = new SqlCommand("select TOP " + value.ToString() + " p.Id, p.CategoryId, p.ProductName, p.UnitPrice, p.Discontinued from products p", objADONet.connectionSQLServer);
        //    dsData = sqlCommand.ExecuteReader();

        //    Response<Product> listProducts = new Response<Product> ();

        //    if (dsData.HasRows)
        //    {
        //        while (dsData.Read())
        //        {
        //            /* product  */
        //            Product product = new Product
        //            {
        //                // CategoryName = dsData["CategoryName"] != DBNull.Value ? Convert.ToString(dsData["CategoryName"])!.Trim() : string.Empty,
        //                CategoryId = dsData["CategoryId"] != DBNull.Value ? Convert.ToInt32(dsData["CategoryId"]) : 0,
        //                Id = dsData["Id"] != DBNull.Value ? Convert.ToInt32(dsData["Id"]) : 0,
        //                ProductName = dsData["ProductName"] != DBNull.Value ? Convert.ToString(dsData["ProductName"])!.Trim() : string.Empty,
        //                UnitPrice = dsData["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(dsData["UnitPrice"]) : 0,
        //                Discontinued = dsData["Discontinued"] != DBNull.Value ? Convert.ToBoolean(dsData["Discontinued"]) : false
        //            };

        //            /* products */
        //            if (listProducts.Results == null)
        //                listProducts.Results = new List<Product>();

        //            listProducts.Results.Add(product);
        //        }
        //    }
        //    else
        //    {
        //        tError = "No products have been found.";
        //        ErrorResponse errorResponse = new ErrorResponse
        //        {
        //            ErrorCode = StatusCodes.Status404NotFound.ToString(),
        //            ErrorMessage = tError
        //        };

        //        return StatusCode(StatusCodes.Status404NotFound, errorResponse);
        //    }

        //    if (listProducts != null && listProducts.Results.Count > 0)
        //        listProducts.TotalCount = listProducts.Results.Count;

        //    dsData.Close();
        //    dsData = null;

        //    objADONet.connectionSQLServer.Close();
        //    objADONet.connectionSQLServer = null;
        //    objADONet = null;

        //    return Ok(listProducts);            
        //}

        //[HttpGet]
        //[Route("productsSorted")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<Product>)),        
        //ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponse)),
        //ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse)),
        //ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse)),
        //ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        ////public ActionResult<List<string>> customersRetrieveTop(int value)
        //public async Task<ActionResult<string>> productsSorted([FromQuery, Required] string sortField, [FromQuery, Required] string sortCriteria)
        //{
        //    SqlCommand sqlCommand;
        //    SqlDataReader dsData = null;
        //    string tError = string.Empty;

        //    ADONet objADONet = new ADONet();
        //    objADONet.connectionSQLServer = objADONet.connectionOpen(objADONet.connectionSQLServer, 2);

        //    sqlCommand = new SqlCommand("select p.CategoryId, p.Id, p.ProductName, p.UnitPrice, p.Discontinued from products p order by p." + sortField + " " + sortCriteria, objADONet.connectionSQLServer);
        //    dsData = sqlCommand.ExecuteReader();

        //    Response<Product> listProducts = new Response<Product>();

        //    if (dsData.HasRows)
        //    {
        //        while (dsData.Read())
        //        {
        //            /* product  */
        //            Product product = new Product
        //            {
        //                // CategoryName = dsData["CategoryName"] != DBNull.Value ? Convert.ToString(dsData["CategoryName"])!.Trim() : string.Empty,
        //                CategoryId = dsData["CategoryId"] != DBNull.Value ? Convert.ToInt32(dsData["CategoryId"]) : 0,
        //                Id = dsData["Id"] != DBNull.Value ? Convert.ToInt32(dsData["Id"]) : 0,
        //                ProductName = dsData["ProductName"] != DBNull.Value ? Convert.ToString(dsData["ProductName"])!.Trim() : string.Empty,
        //                UnitPrice = dsData["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(dsData["UnitPrice"]) : 0,
        //                Discontinued = dsData["Discontinued"] != DBNull.Value ? Convert.ToBoolean(dsData["Discontinued"]) : false
        //            };

        //            /* products */
        //            if (listProducts.Results == null)
        //                listProducts.Results = new List<Product>();

        //            listProducts.Results.Add(product);
        //        }
        //    }
        //    else
        //    {
        //        tError = "No products have been found.";
        //        ErrorResponse errorResponse = new ErrorResponse
        //        {
        //            ErrorCode = StatusCodes.Status404NotFound.ToString(),
        //            ErrorMessage = tError
        //        };

        //        return StatusCode(StatusCodes.Status404NotFound, errorResponse);
        //    }

        //    if (listProducts != null && listProducts.Results.Count > 0)
        //        listProducts.TotalCount = listProducts.Results.Count;

        //    dsData.Close();
        //    dsData = null;

        //    objADONet.connectionSQLServer.Close();
        //    objADONet.connectionSQLServer = null;
        //    objADONet = null;

        //    return Ok(listProducts);
        //}

        [HttpGet]
        [Route("productsSortedEF")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<Product>)),
        ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        //public ActionResult<List<string>> customersRetrieveTop(int value)
        public async Task<ActionResult<string>> productsSortedEF([FromQuery, Required] string sortField, [FromQuery, Required] string sortCriteria)
        {
            string tError = string.Empty;            

            // Query EF Core
            var products = await _context.Products                
                .OrderBy($"{sortField} {sortCriteria}")
                .Select(p => new Product
                {
                    CategoryId = p.CategoryId,
                    ProductId = p.ProductId,
                    ProductName = p.ProductName ?? string.Empty,
                    UnitPrice = p.UnitPrice,
                    Discontinued = p.Discontinued
                })
                .ToListAsync();

            Response<Product> listProducts = new Response<Product>();

            if (products != null && products.Count > 0)
            {
                /* products */
                if (listProducts.Results == null)
                    listProducts.Results = new List<Product>();

                listProducts = new Response<Product>
                {
                    Results = products,         
                    TotalCount = products.Count
                };

                return Ok(listProducts);
            }
            else
            {
                tError = "No products have been found.";
                ErrorResponse errorResponse = new ErrorResponse
                {
                    ErrorCode = StatusCodes.Status404NotFound.ToString(),
                    ErrorMessage = tError
                };

                return StatusCode(StatusCodes.Status404NotFound, errorResponse);
            }
        }
    }
}
