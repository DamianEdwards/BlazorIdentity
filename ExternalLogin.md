# Blazor Identity - External Login

## Logic Flow

1. Registered external providers are listed on [Login](samples/BlazorIdentity.ServerApp/Pages/Account/Login.razor) page via call to GetExternalAuthenticationSchemesAsync().
2. Each login is shown as a button that performs a POST to /api/account/ExternalLogin.
3. This method retrieves the properties for the external provider and returns a challenge response which is sent to the external provider. This includes the name of the callback method that will received the results of the autentication challenge from the external provider.
4.  The callback method is called by the external provider. This method will handle any errors returned and if none, will attempt to sign in the user.
5. If a local login record already exists, the user will be logged in locally and re-directed to the return Url.
6. If a local login record does not exist, the user will be redirected to [External Login](samples/BlazorIdentity.ServerApp/Pages/Account/ExternalLogin.razor)  and prompted to verify the account, which will add a local record and then login the user.

### The Problem

It is at stage 5 or 6 (depending on execution path) where there is an issue. Both of these code paths call the GetExternalLoginInfoAsync() method from SignInManager. Behind the scenes, this method is adding to the response stream, therefore causing an issue when attempting to navigate to the return url (step 5), or display the confirmation form (step 6).

It is my guess that external cookie handling is getting in the way with the way Blazor handles these things. I think the external cookie handler may need to be modified to handle this correctly. I think this is similar to the way the BlazorServerAwareCookieAuthenticationHandler class is being used for regular login. Unfortunately I have not had the time to figure out how to implement such a thing.

The error message I see in the console log in both of the above cases is a variation on the following:

`OnStarting cannot be set because the response has already started.`

### Conclusion

So, this is where things stand at present. The solution is possibly quite simple. Unfortunately I do not currently have the knowledge of some of the inner workings of GetExternalLoginInfoAsync, ExternalLoginSignInAsync and custom cookie authentication to be able to come up with a solution.

This is definitely something I would like to learn more about. I am not satisfied with leaving this in an unfinished state. Unfortunately, life and my day job are preventing me from dedicating the time to gain the knowledge required to find a solution.