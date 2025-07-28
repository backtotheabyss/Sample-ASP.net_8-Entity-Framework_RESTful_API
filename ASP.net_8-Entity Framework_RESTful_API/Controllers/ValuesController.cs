using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ASP.net_8_Entity_Framework_RESTful_API
{
    [Route("api/values")]   //[controller]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        
        public ActionResult<String[]> Get()
        {
            string[] x = new string[] { };
            String [] vecValues = new String [2] { "value1", "value2" };
            return (vecValues); // new string[] { "value1", "value2" };
        }
      
        // GET api/values/obtenerValor/5
        [HttpGet]
        [Route("obtenerValor/{id}")]
        public ActionResult<string> obtenerValor (int id)
        {
            var jsonDataDummy = new { customerid = id, customername = "Anna" };
            return Content(JsonConvert.SerializeObject(jsonDataDummy), "application/json"); //"value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
