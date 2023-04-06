using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.ExtensionMethods
{
    /// <summary>
    /// Class for creating additional functionality for the Object 
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// This function will convert the object into serializable string
        /// </summary>
        /// <param name="input"></param>
        /// <returns>returns the serialized object as string</returns>
        public static string JsonSerialize(this object input)
        {
            return JsonSerializer.Serialize(input, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
    }
}
