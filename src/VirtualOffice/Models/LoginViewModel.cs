using System.ComponentModel.DataAnnotations;

namespace VirtualOffice.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(100_000, ErrorMessage = "Your icon is too large.")]
        public string Icon { get; set; }

        public string Code { get; set; }
    }
}
