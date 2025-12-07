# Entra ID App Role Admin Sample

This sample application demonstrates how to use Blazorade ID to authenticate users with Microsoft Entra ID and manage application roles. Application roles are used in access tokens to represent features or functionality that users and applications are allowed to use when accessing a resource with an access token.

## Getting Started with the Sample

To run this sample application, you need to create an `appsettings.json` file in the `wwwroot` folder of the project with the following content:

```json
{
  "blazorade": {
	"id": {
	  "metadatauri": "{URI}",
	  "clientid": "{string}",
	  "scope": "{string}"
	}
  }
}
```

- `metadatauri`: This is the URI to the metadata endpoint of your Microsoft Entra ID tenant. Replace `{URI}` with the actual URI. Blazorade ID will use this URI to automatically discover the authorization and token endpoints. The metadata URI is formatted differently depending on whether you are working with Microsoft Entra ID or Microsoft Entra External ID.
	- Entra ID: `https://{tenantname}.microsoftonline.com/{tenantid}/v2.0/.well-known/openid-configuration`.
	- Entra External ID: `https://{tenantname}.ciamlogin.com/{tenantid}/v2.0/.well-known/openid-configuration`.
- `clientid`: This is the client ID of your application registered with Microsoft Entra ID. Replace `{string}` with your actual client ID.
- `scope`: The default scope(s) used when acquiring access tokens in the application. In order to be able to manage application roles, security groups and role assignments, you need to specify the following scopes:
	- `Application.ReadWrite.All`
	- `Directory.Read.All`

