**Changes in this PR**

1. Removed JS from BlazorIdentity.UI project. Wrote callbacks instead for calling back to components in sample project
2. Removed blind re-directs in BlazorIdentity.UI project. Replace with callbacks to sample project
3. Tabs vs spaces. Have set VS 2022 Preview 4 to use spaces in options.
4. Default to using BlazorStrap alerts in sample project. Removed all alerts from src/BlazorIdentity.UI project. Invoke callbacks to sample project instead
5. Replaced webencoders call and subsequent inclusion of Microsoft.AspNetCore.WebUtilities. Replaced with System.Web.HttpUtility.UrlEncode/UrlDecode
6. Removed Microsoft.AspNetCore.Identity from src/BlazorIdentity.UI project and shimmed a handler for PersonalData into BlazorServerUserManager. DownloadPersonalData now returns a dictionary object to UI. Added JS to sample project as an example way of handling the download. There is most likely a better way to do this that I have not come across.
7. Removed all redirects in src/BlazorIdentity.UI project. Replaced with invoke calls back to sample project.
8. Put overrides before private handler methods.
9. Some general formatting. Adding white space before definitions of components in BlazorIdentity.UI project.
10. Some capitalization fixes for consistency
11. Removed erroneous notes in the readme.