namespace BlazorIdentity.Server;

using System.Threading.Tasks;

internal class BlazorServerEmailSender : IBlazorEmailSender
{

	public Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		return Task.CompletedTask;
	}
}
