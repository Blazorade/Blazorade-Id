using Blazorade.Id.Components.Pages;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Uses Blazor's <see cref="NavigationManager"/> to provide redirect URIs.
    /// </summary>
    public class BlazorRedirectUriProvider : IRedirectUriProvider
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BlazorRedirectUriProvider"/> class.
        /// </summary>
        public BlazorRedirectUriProvider(NavigationManager navMan)
        {
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
        }

        private readonly NavigationManager NavMan;

        /// <inheritdoc/>
        public Uri GetRedirectUri()
        {
            var uri = new Uri(new Uri(this.NavMan.BaseUri), "_content/Blazorade.Id/oauth-callback.html");
            return uri;
        }
    }
}
