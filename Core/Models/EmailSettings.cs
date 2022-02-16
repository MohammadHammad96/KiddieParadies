namespace KiddieParadies.Core.Models
{
    public class EmailSettings
    {
        public string Sender { get; set; }

        public string MailServer { get; set; }

        public string SenderName { get; set; }

        public string Password { get; set; }

        public int MailPort { get; set; }
    }
}