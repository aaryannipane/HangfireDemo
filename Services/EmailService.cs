namespace HangfireDemo.Services
{
    public class EmailService
    {
        public void SendWelcomeEmail(string email)
        {
            Console.WriteLine($"Welcome {email}");
        }
    }
}
