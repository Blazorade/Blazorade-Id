# Blazorade ID
Authentication library for Blazor applications that support OAuth 2.0 and Open ID protocols.

## Primary Design Goals

The primary design goal for Blazorade ID is to provide an authentication library for all Blazor platforms and provide the same programming model on all platforms. This includes
- Blazor Server
- Blazor WebAssembly
- Blazor Hybrid

This means that a Blazor component built for one of these platforms works on the other platforms. This is done by abstracting all different aspects of these platforms into service intefaces, which are then implemented for the different platforms.

Blazorade ID is different from other authentication libraries, for instance [MSAL](https://learn.microsoft.com/entra/identity-platform/msal-overview), in the way authentication is invoked. For instance MSAL focuses on protecting entire views or pages, and requires authentication before that page can be viewed.

Blazorade ID focuses more on protecting actions. This means that authentication and authorization in the form of access tokens takes place when an access token with particular permissions and scopes is needed. Identity an access tokens are cached so that they can be returned quicker.

> This library is currently in early development phases. The main functionality works on Blazor Server and Blazor WebAssembly, but is still too early for a beta release.
