using ASP.net_8_Entity_Framework_RESTful_API.Classes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using static DTO.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASP.net_8_Entity_Framework_RESTful_API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly NorthwndContext _context;
        private readonly IConfiguration _configuration;
        private readonly Settings _settings;

        public SettingsController(NorthwndContext context, IConfiguration configuration, Settings settings)
        {
            _context = context;
            _configuration = configuration;
            _settings = settings;
        }

        [HttpGet("printerSettings")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<PrinterConfiguration>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public ActionResult<Response<PrinterConfiguration>> printerSettings()
        {
            try
            {
                if (_settings == null || _settings.printerSettings == null || !_settings.printerSettings.Any())
                {
                    string tError = "Invalid or missing PrinterSettings configuration.";

                    // Serilog.Log.Error(tError);
                    return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorResponse
                        {
                            ErrorCode = StatusCodes.Status404NotFound.ToString(),
                            ErrorMessage = tError
                        });
                }
                else
                {
                    var results = new List<PrinterConfiguration>();
                    results = new List<PrinterConfiguration>
                    (
                        _settings.printerSettings
                            .Select(p => new PrinterConfiguration
                            {
                                Name = p.Name,
                                IP = p.IP,
                                Port = p.Port
                            })
                    );

                    var response = new Response<PrinterConfiguration>
                    {
                        TotalCount = results.Count,
                        Results = results
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                string tError = "Error retrieving PrinterSettings configuration.";

                // Serilog.Log.Error(ex, tError);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    ErrorCode = StatusCodes.Status500InternalServerError.ToString(),
                    ErrorMessage = tError,
                    ErrorDetails =
                    [
                        new ErrorDetail
                {
                    InternalErrorCode = StatusCodes.Status500InternalServerError,
                    Detail = ex.Message
                }
                    ]
                });
            }
        }
    }
}
