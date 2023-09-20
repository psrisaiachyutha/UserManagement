using System.ComponentModel.DataAnnotations;

namespace Common.Configurations
{
    public class ApplicationSettings
    {
        [Required]
        public string SignatureKey { get; set; }
        [Required]
        public string PasswordEncriptionKey { get; set; }
    }
}
