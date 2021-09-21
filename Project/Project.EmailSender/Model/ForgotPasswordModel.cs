using System.ComponentModel.DataAnnotations;

namespace Project.EmailSender.Model
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
