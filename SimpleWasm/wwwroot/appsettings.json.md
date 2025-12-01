# appsettings.json

This document describes the structure of the `appsettings.json` file used in the sample application. This file is used to configure various settings for the application.

In order for you to run the sample app, you need to create an `appsettings.json` file in the `wwwroot` folder of the project with the following content:

```json
{
  "blazorade": {
    "id": {
      "metadatauri": "{URI}",
      "clientid": "{string}",
    }
  }
}
```

- `metadatauri`: This is the URI to the metadata endpoint of your identity provider. Replace `{URI}` with the actual URI. Blazorade ID will use this URI to automatically discover the authorization and token endpoints.
- `clientid`: This is the client ID of your application registered with the identity provider. Replace `{string}` with your actual client ID.

> In Microsoft Entra ID, the metadata URI is `https://{tenantname}.microsoftonline.com/{tenantid}/v2.0/.well-known/openid-configuration`. In Microsoft Entra External ID, the metadata URI is `https://{tenantname}.ciamlogin.com/{tenantid}/v2.0/.well-known/openid-configuration`.

## Alternative configuration

If you don't want to use the metadatauri, you can replace that with the following two settings:

- `authorizationendpoint`: The authorization endpoint URI of your identity provider.
- `tokenendpoint`: The token endpoint URI of your identity provider.
