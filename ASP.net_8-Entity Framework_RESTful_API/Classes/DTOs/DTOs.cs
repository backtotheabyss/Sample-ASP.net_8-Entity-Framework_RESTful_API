using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ASP.net_8_Entity_Framework_RESTful_API
{
    public class DTOs
    {
        public class ResponseDTO<T>
        {
            [Required]
            public int TotalCount { get; set; }
            [Required]
            public List<T> Results { get; set; } = default!;
        }
        public class GeneralResponseDTO<T>
        {
            [Required]
            public T Response { get; set; } = default!;
        }

        public class ErrorResponseDTO
        {
            [Required]
            public string ErrorCode { get; set; } = "0";
            [Required]
            public string ErrorMessage { get; set; } = string.Empty;
            [Required]
            public List<ErrorDetailDTO> ErrorDetails { get; set; } = new List<ErrorDetailDTO>(); //[];
        }

        public class ErrorDetailDTO
        {
            [Required]
            public int InternalErrorCode { get; set; }
            [Required]
            public string Detail { get; set; } = string.Empty;
        }

        /* Customer DTO */
        public class CustomerResultDTO
        {
            public string CustomerID { get; set; } = string.Empty;
            public string ContactName { get; set; } = string.Empty;
        }

        /* Customer Request DTO */
        public class CustomerRequestDTO
        {
            [Required]
            public string CustomerID { get; set; } = string.Empty;
            [Required]
            public string ContactName { get; set; } = string.Empty;
        }
    }
}
