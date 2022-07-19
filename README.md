# Blazor Identity

## What is this?

An exploration of recreating the ASP.NET Core Identity UI functionality in Blazor components.

### NOTE: This is a fork and PR of Damian Edward's project.
[https://github.com/DamianEdwards/BlazorIdentity](https://github.com/DamianEdwards/BlazorIdentity)

Currently supports the following operations in Blazor Server apps:

#### Damian Edwards

- Registering a new user
- Logging in
- Logging out
- Updating profile (telephone number)
- Changing password

#### Mark Nash

- New account confirmation
- Account Lockout
- Return URL (courtesy of article from [Chris Sainty](https://chrissainty.com/working-with-query-strings-in-blazor/))
- Data download
- Delete account
- Password recovery
- Change Email
- 2FA login (TOTP) - partially done. Testing incomplete. Refer [2FA Notes.txt](2FANotes.txt) for explanation of issue encountered.

**NOTE**: Several of the above require configuring a 3rd party provider such as Twillio or SendGrid for full testing and use.

**Also fixed:**
 - A dependency on Microsoft.AspNetCore.Identity that snuck into the BlazorIdentity project.
 - LaunchSettings.json files that got created in BlazorIdentity and BlazorIdentity.Server projects.
 - Cleaned out a <remove> entry in the project file of BlazorIdentity.ServerApp.

<img width="1000" alt="image" src="https://user-images.githubusercontent.com/249088/177449167-a19c3efa-6a24-4e5d-ada4-1ddf617c9643.png">

### Requirements

This solution currently uses a daily .NET 7 SDK `main` build (see exact min-version required in the [`global.json`](global.json)). You can grab such a build from https://github.com/dotnet/installer

### TODO

- Social (external)
- Blazor WebAssembly support (via API). 
