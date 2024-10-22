using System.Net.Mail;

namespace APIGateway.Utilities
{
    public interface ISendMailSMTP
    {
        Task<int> SendMail(string toMail, string subject, string message);
    }

    public class SendMailSMTP : ISendMailSMTP
    {
        private readonly IConfiguration _config;
        public SendMailSMTP(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> SendMail(string toMail, string subject, string message)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(_config["MailSMPTConfig:host"]);

                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(_config["MailSMPTConfig:fromEmail"], _config["MailSMPTConfig:password"]);
                // smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                MailMessage mail = new MailMessage();
                //Setting From , To and CC
                mail.From = new MailAddress(_config["MailSMPTConfig:fromEmail"], _config["MailSMPTConfig:displayName"]);
                mail.To.Add(new MailAddress(toMail ?? _config["MailSMPTConfig:toEmail"]));
                //mail.CC.Add(new MailAddress("nhanit2012.vn@gmail.com"));
                mail.IsBodyHtml = true;
                mail.Subject = subject;
                mail.Body = message;
                await smtpClient.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return 0;
            }
            return 1;
        }
    }
}
