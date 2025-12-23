# Blazorade ID Core

Blazorade ID Core is the library that contains core functionality and type definitions required across different application types supported by Blazorade ID.

## Getting Started

Get started with Blazorade Id by visiting the Getting Started page on the [Blazorade ID wiki](https://github.com/Blazorade/Blazorade-Id/wiki/Getting-Started). Also be sure to check out the sample applications included in the [Github repository for Blazorade ID](https://github.com/Blazorade/Blazorade-Id).

## Version Highlights

The versions of Blazorade ID Core always match the version numbers of the other Blazorade ID libraries. For details, see the *Used By* tab above.

### v1.0.0-rc.3

- Added `IHttpRequestFactory` service interface to create HTTP requests for resources protected by access tokens managed by Blazorade ID.

### v1.0.0-rc.2

- Changed the default namespace for the `Blazorade.Id.Core` assembly from `Blazorade.Id.Core` to `Blazorade.Id` to align with other Blazorade ID libraries. Moved all defined types accordingly.
- Renamed property `MetadataUri` to `DiscoveryDocumentUri` on the Blazorade.Id.Configuration.AuthorityOptions class to better reflect its purpose.

### v1.0.0-rc.1

Preparing for the first stable version. Even though Blazorade ID is designed to work with any OAuth/OIDC compliant identity provider, the first stable version will focus on Microsoft Entra ID and Microsoft Entra External ID.

Read more about Blazorade ID on the [Blazorade ID Wiki](https://github.com/Blazorade/Blazorade-Id/wiki).

### v1.0.0-beta.x

Working towards the first stable version.

### v1.0.0-alpha.2

- Updated reference to [`System.Text.Json`](https://learn.microsoft.com/dotnet/api/system.text.json) to a non-vulnerable version.

### v1.0.0-alpha.1

- Initial version.