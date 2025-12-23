# Blazorade ID

Blazorade ID offers authentication and user identification services to all types of Blazor applications. Blazorade ID aims to unify the programming model for authentication across all Blazor application types, as far as possible.

## Getting Started

Get started with Blazorade Id by visiting the Getting Started page on the [Blazorade ID wiki](https://github.com/Blazorade/Blazorade-Id/wiki/Getting-Started). Also be sure to check out the sample applications included in the [Github repository for Blazorade ID](https://github.com/Blazorade/Blazorade-Id).

## Version Highlights

### v1.0.0-rc.3

- Added `IHttpRequestFactory` service interface to create HTTP requests for resources protected by access tokens managed by Blazorade ID.

### v1.0.0-rc.2

- Improved refresh token handling in token store implementations.
- Implemented log out functionality through the ISignOutService.
- Implemented a default sign-out service for Blazor applications.
- Renamed property store and token store implementations to better reflect what storage they use.

### v1.0.0-rc.1

Preparing for the first stable version. Even though Blazorade ID is designed to work with any OAuth/OIDC compliant identity provider, the first stable version will focus on Microsoft Entra ID and Microsoft Entra External ID.

Read more about Blazorade ID on the [Blazorade ID Wiki](https://github.com/Blazorade/Blazorade-Id/wiki).

### v1.0.0-beta.x

Working towards the first stable version.

### v1.0.0-alpha.2

- Updated reference to [`Blazorade.Id.Core`](https://www.nuget.org/packages/Blazorade.Id.Core).

### v1.0.0-alpha.1

Authentication and token acquisition works in both Blazor WebAssembly and Blazor Server applications.