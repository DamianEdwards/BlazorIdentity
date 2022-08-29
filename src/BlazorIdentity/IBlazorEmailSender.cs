namespace BlazorIdentity;

using System.Threading.Tasks;

public interface IBlazorEmailSender
{
	Task SendEmailAsync(string email, string subject, string htmlMessage);
}