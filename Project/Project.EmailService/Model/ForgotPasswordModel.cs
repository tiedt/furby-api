using System.ComponentModel.DataAnnotations;

namespace Project.EmailService.Model
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
