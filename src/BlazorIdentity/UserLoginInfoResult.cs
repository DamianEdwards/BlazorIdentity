namespace BlazorIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UserLoginInfoResult
{
	public string LoginProvider { get; set; }
	public string ProviderDisplayName { get; set; }
	public string ProviderKey { get; set; }	
}
