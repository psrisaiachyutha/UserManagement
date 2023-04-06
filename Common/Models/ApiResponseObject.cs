using System.Net;

namespace Common.Models
{
    public class ApiResponseObject<T>
    {        
        public HttpStatusCode StatusCode { get; set; }

        public T Data { get; set; }
    }
}
