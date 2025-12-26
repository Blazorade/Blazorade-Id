using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Components.Pages
{
    /// <summary>
    /// The OAuth callback page provided by Blazorade ID.
    /// </summary>
    /// <remarks>
    /// When you configure the <c>Blazorade.Id</c> assembly as an additional assembly on the
    /// <see cref="Microsoft.AspNetCore.Components.Routing.Router"/> component in your application,
    /// then this page is available at <c>/blazorade-id/oauth-callback</c> in your application.
    /// </remarks>
    [Route(RoutePath)]
    partial class OAuthCallback
    {
        /// <summary>
        /// The route path for the page.
        /// </summary>
        public const string RoutePath = "/blazorade-id/oauth-callback";
    }
}
