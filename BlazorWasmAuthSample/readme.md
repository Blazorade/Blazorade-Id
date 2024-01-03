# BlazorWasmAuthSample

To run this sample application, you need to create an `appsettings.json` file in the `wwwroot` folder. The file must have the following contents.

## appsettings.json

``` JSON
{
  "blazorade": {
    "id": {
      "metadatauri": "https://login.microsoftonline.com/denomica.com/v2.0/.well-known/openid-configuration",
      "cachemode": "persistent"
    }
  }
}
```

The settings file has the following settings.

- `metadatauri`: The full URL to the OpenID configuration file for your tenant. Default URIs to different directories are listed below. 
- `cachemode`: How to cache tokens in the application. Can be set to one of the following:
    - `session`: Tokens are cached only for the current session and do not survive restarting the application. This is the default value if no value is specified.
    - `persistent`: Tokens are stored persistently, and can be used across multiple sessions as long as the tokens are still valid.

### Metadata URIs

This section describes the default metadata URIs to Open ID configuration documents for various Azure AD and Entra ID types of tenants.

- Microsoft Entra ID (Azure AD): https://login.microsoftonline.com/{TenantId}/v2.0/.well-known/openid-configuration
- Microsoft Entra External ID: 
- Azure AD B2C: 