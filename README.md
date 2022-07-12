# Blazor Identity

## What is this?

An exploration of recreating the ASP.NET Core Identity UI functionality in Blazor components.

#### As of 7/10/22 this is a copy of Damian Edward's project.
[https://github.com/DamianEdwards/BlazorIdentity](https://github.com/DamianEdwards/BlazorIdentity)

Currently supports the following operations in Blazor Server apps:

- Registering a new user
- Logging in
- Logging out
- Updating profile (telephone number)
- Changing password

<img width="1000" alt="image" src="https://user-images.githubusercontent.com/249088/177449167-a19c3efa-6a24-4e5d-ada4-1ddf617c9643.png">

### Requirements

This solution currently uses a daily .NET 7 SDK `main` build (see exact min-version required in the [`global.json`](global.json)). You can grab such a build from https://github.com/dotnet/installer

### TODO

- New account confirmation (Code complete - testing required)
- Account lockout
- 2FA login (TOTP)
- Password recovery
- Social (external) login
- Blazor WebAssembly support (via API)
- Data download (Code 90% complete)
- Delete account
