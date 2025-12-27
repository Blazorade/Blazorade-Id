using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Model
{
    /// <summary>
    /// Defines different types for web storage.
    /// </summary>
    public enum WebStoreType
    {
        /// <summary>
        /// Specifies the browser's local storage.
        /// </summary>
        LocalStorage,
        /// <summary>
        /// Specifies the browser's session storage.
        /// </summary>
        SessionStorage
    }
}
