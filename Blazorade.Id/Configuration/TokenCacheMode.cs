using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Configuration
{
    /// <summary>
    /// Defines different storages for caching tokens in.
    /// </summary>
    public enum TokenCacheMode
    {
        /// <summary>
        /// Tokens are cached in session storage. This means that tokens will not survive closing the browser and opening a new one.
        /// </summary>
        Session,
        /// <summary>
        /// Tokens are cached in the browser's local storage, meaning that they will be available to the next browser instance.
        /// </summary>
        Persistent
    }
}
