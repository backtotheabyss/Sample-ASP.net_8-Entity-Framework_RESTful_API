using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sql;

using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using static Models.Customer;
using static Models.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASP.net_8_Entity_Framework_RESTful_API
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly NorthwndContext _context;
        private readonly IConfiguration _configuration;

        public CustomersController (NorthwndContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("customer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<Customer>))]
        [ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<Response<Customer>>> customer([FromQuery, Required] string Id)
        {
            Response<Customer> listCustomers = new Response<Customer>();

            try
            {
                List<Customer> customers = await _context.Customers
                    .Where(c => c.CustomerId == Id)                    
                    .ToListAsync();

                if (customers != null && customers.Count > 0)
                {
                    /* customers */
                    if (listCustomers.Results == null)
                        listCustomers.Results = new List<Customer>();

                    listCustomers = new Response<Customer>
                    {
                        Results = customers,
                        TotalCount = customers.Count
                    };

                    return Ok(listCustomers);
                }
                else
                    return NotFound(new ErrorResponse
                    {
                        ErrorCode = StatusCodes.Status404NotFound.ToString(),
                        ErrorMessage = "No customer have been found."
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

        [HttpGet]
        [Route("customers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<Customer>))]
        [ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<Response<Customer>>> customers([FromQuery, Required] int rows)
        {
            Response<Customer> listCustomers = new Response<Customer>();

            try
            {
                var customers = await _context.Customers
                    .Take(rows)
                    .ToListAsync();

                if (customers != null && customers.Count > 0)
                {
                    /* customers */
                    if (listCustomers.Results == null)
                        listCustomers.Results = new List<Customer>();

                    listCustomers = new Response<Customer>
                    {
                        Results = customers,
                        TotalCount = customers.Count
                    };

                    return Ok(listCustomers);
                }
                else
                    return NotFound(new ErrorResponse
                    {
                        ErrorCode = StatusCodes.Status404NotFound.ToString(),
                        ErrorMessage = "No customers have been found."
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

        [HttpGet]
        [Route("customersNonEF")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<Customer>)),
        // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Customer>)),
        ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<Response<Customer>>> customersNonEF([FromQuery, Required] int rows)
        // public async Task<ActionResult<List<Customer>>> customers([FromQuery] int value)
        {
            SqlCommand sqlCommand;
            SqlDataReader dsData = null;
            string tError = string.Empty;

            Response<Customer> listCustomers = new Response<Customer>();
            // List<Customer> listCustomers = new List<Customer>();

            ADONet objADONet = new ADONet(_configuration);

            try
            {
                objADONet.connectionSQLServer = objADONet.connectionOpen(objADONet.connectionSQLServer, 2);

                sqlCommand = new SqlCommand("select TOP " + rows.ToString() + " CustomerId, ContactName, CompanyName from Customers", objADONet.connectionSQLServer);
                dsData = sqlCommand.ExecuteReader();

                if (dsData.HasRows)
                {
                    while (dsData.Read())
                    {
                        /* customer */
                        Customer customer = new Customer
                        {
                            CustomerId = dsData["CustomerId"] != DBNull.Value ? Convert.ToString(dsData["CustomerId"])!.Trim() : "",
                            ContactName = dsData["ContactName"] != DBNull.Value ? Convert.ToString(dsData["ContactName"])!.Trim() : "",
                            CompanyName = dsData["CompanyName"] != DBNull.Value ? Convert.ToString(dsData["CompanyName"])!.Trim() : "",
                        };

                        /* customers */
                        // listCustomers.Add(customer);

                        if (listCustomers.Results == null)
                            listCustomers.Results = new List<Customer>();

                        listCustomers.Results.Add(customer);
                    }
                }
                else
                {
                    dsData.Close();
                    dsData = null;
                    tError = "No customers have been found.";

                    return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorResponse
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
                    new ErrorResponse
                    {
                        ErrorCode = StatusCodes.Status500InternalServerError.ToString(),
                        ErrorMessage = tError,
                        ErrorDetails = new List<ErrorDetail>
                        {
                            new ErrorDetail
                            {
                                InternalErrorCode = 500,
                                Detail = e.Message
                            }
                        }
                    });
            }
        }

        [HttpPut]
        [Route("customerEdit")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse<string>)),
        // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Customer>)),
        ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<Response<Customer>>> customerEdit([FromBody] CustomerRequest customerPayload)
        {
            try
            {                
                Customer customer = await _context.Customers.FindAsync(customerPayload.CustomerId);
                if (customer == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ErrorResponse
                    {
                        ErrorCode = StatusCodes.Status404NotFound.ToString(),
                        ErrorMessage = customerPayload.CustomerId + " doesn't exists."
                    });
                }
                else
                {
                    customer.ContactName = customerPayload.ContactName;
                    customer.CompanyName = customerPayload.CompanyName;
                        
                    await _context.SaveChangesAsync();
                }

                return Ok(new GeneralResponse<string>
                {
                    Response = customerPayload.ContactName + " has been edited succesfuly."
                });
                
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse
                    {
                        ErrorCode = StatusCodes.Status500InternalServerError.ToString(),
                        ErrorMessage = "Internal Server Error",
                        ErrorDetails = new List<ErrorDetail>
                        {
                            new ErrorDetail
                            {
                                InternalErrorCode = 500,
                                Detail = e.Message
                            }
                        }
                    });
            }
        }

        [HttpDelete]
        [Route("customerDelete")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse<string>)),
        // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Customer>)),
        ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<Response<Customer>>> customerDelete(string Id)
        {
            try
            {                
                Customer customer = await _context.Customers.FindAsync(Id);
                if (customer == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ErrorResponse
                    {
                        ErrorCode = StatusCodes.Status404NotFound.ToString(),
                        ErrorMessage = Id + " doesn't exists."
                    });
                }
                else
                {
                    _context.Customers.Remove(customer);
                    await _context.SaveChangesAsync();
                }

                return Ok(new GeneralResponse<string>
                {
                    Response = Id + " has been deleted succesfuly."
                });                
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse
                    {
                        ErrorCode = StatusCodes.Status500InternalServerError.ToString(),
                        ErrorMessage = "Internal Server Error",
                        ErrorDetails = new List<ErrorDetail>
                        {
                            new ErrorDetail
                            {
                                InternalErrorCode = 500,
                                Detail = e.Message
                            }
                        }
                    });
            }
        }

        [HttpPost]
        [Route("customerAdd")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse<string>)),
        // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Customer>)),
        ProducesResponseType(StatusCodes.Status302Found, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse)),
        ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<Response<Customer>>> customerAdd([FromBody] CustomerRequest customerPayload)
        {            
            try
            {                
                Customer customer = await _context.Customers.FindAsync(customerPayload.CustomerId);
                if (customer != null)
                {
                    return StatusCode(StatusCodes.Status302Found, new ErrorResponse
                    {
                        ErrorCode = StatusCodes.Status302Found.ToString(),
                        ErrorMessage = customerPayload.CustomerId + " already exists."
                    });
                }
                else
                {
                    customer = new Customer
                    {
                        CustomerId = customerPayload.CustomerId,
                        ContactName = customerPayload.ContactName,
                        CompanyName = customerPayload.CompanyName
                    };

                    _context.Customers.Add(customer);                        
                    await _context.SaveChangesAsync();
                }

                return Ok(new GeneralResponse<string>
                {
                    Response = customerPayload.ContactName + " has been added succesfuly."
                });                
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse
                    {
                        ErrorCode = StatusCodes.Status500InternalServerError.ToString(),
                        ErrorMessage = "Internal Server Error",
                        ErrorDetails = new List<ErrorDetail>
                        {
                            new ErrorDetail
                            {
                                InternalErrorCode = 500,
                                Detail = e.Message
                            }
                        }
                    });
            }
        }
    }
}
