namespace BlazorIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UserLoginInfo
{
	public string LoginProvider { get; set; } = default!;
	public string ProviderDisplayName { get; set; } = default!;
	public string ProviderKey { get; set; } = default!;
}
