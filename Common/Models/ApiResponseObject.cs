using System.Runtime.Serialization;

namespace Common.Models
{
    public class ApiResponseObject<T>
    {
        [DataMember()]
        public bool? Success { get; set; }


        [DataMember()]
        public string Message { get; set; }


        [DataMember()]
        public T Data { get; set; }

    }
}
