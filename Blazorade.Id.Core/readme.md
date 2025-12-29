# Blazorade ID Core

Blazorade ID Core is the library that contains core functionality and type definitions required across different application types supported by Blazorade ID.

## Getting Started

Get started with Blazorade Id by visiting the Getting Started page on the [Blazorade ID wiki](https://github.com/Blazorade/Blazorade-Id/wiki/GettingStarted). Also be sure to check out the sample applications included in the [Github repository for Blazorade ID](https://github.com/Blazorade/Blazorade-Id).

## Version Highlights

### v1.2.0

- Added constants for common OAuth and OIDC authorization errors that can be fixed with user interaction.
- Added `EnableSilentAuthorizationCodeFlow` to `AuthorityOptions` to enable silent authorization code flow. This setting is `false` by default.

### v1.1.0

- Improved error handling when processing callbacks from the Identity Provider.
- Improved token handling to prevent tokens from being persisted when switching users.

### v1.0.0

Initial stable release of Blazorade ID Core.

### v1.0.0-rc.6

- Added `CancellationToken` parameters to several asynchronous methods in service interface definitions.
- Added a separate `IRefreshTokenStore` service interface for separately managing refresh tokens. Changed service implementations accoordingly.

### v1.0.0-rc.4

- Replaced `ISignOutService` with `IAuthenticationService` that combines sign-in and sign-out functionality into a single service interface.
- Added a default `AuthenticationService` implementation.

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