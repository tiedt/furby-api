using System.Threading.Tasks;

namespace Project.EmailSender
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
        Task SendEmailAsync(Message message);
    }
}
