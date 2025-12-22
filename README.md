# Blazorade ID
Authentication library for Blazor applications that support OAuth 2.0 and Open ID protocols.

## Primary Design Goals

The primary design goal for Blazorade ID is to provide an authentication library for all Blazor platforms and provide the same programming model on all platforms. This includes
- Blazor Server
- Blazor WebAssembly
- .NET MAUI Blazor Hybrid

This means that a Blazor component built for one of these platforms works on the other platforms. This is done by abstracting all different aspects of these platforms into service intefaces, which are then implemented for the different platforms.

For more information, plase visit the [Blazorade ID Wiki](https://github.com/Blazorade/Blazorade-Id/wiki).